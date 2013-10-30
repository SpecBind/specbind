// <copyright file="PageBase.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	/// <summary>
	/// A base class for pages.
	/// </summary>
	/// <typeparam name="TPageBase">The base type of any given page setup.</typeparam>
	/// <typeparam name="TElement">The type of the basic element on a page.</typeparam>
	public abstract class PageBase<TPageBase, TElement> : IPageElementHandler<TElement>
		where TPageBase : class 
		where TElement : class
	{
		#region Fields

		private readonly Dictionary<string, PropertyData<TElement>> properties;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PageBase{TPageBase, TElement}" /> class.
		/// </summary>
		/// <param name="pageType">Type of the page.</param>
		protected PageBase(Type pageType)
		{
			this.PageType = pageType;
			this.properties = new Dictionary<string, PropertyData<TElement>>(StringComparer.InvariantCultureIgnoreCase);
			this.GetProperties();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the type of the page.
		/// </summary>
		/// <value>
		/// The type of the page.
		/// </value>
		public virtual Type PageType { get; private set; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Gets the native page object.
		/// </summary>
		/// <typeparam name="TPage">The type of the page.</typeparam>
		/// <returns>
		/// The native page object.
		/// </returns>
		public abstract TPage GetNativePage<TPage>() where TPage : class;

		/// <summary>
		/// Gets the property names.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <returns>
		/// A list of matching properties.
		/// </returns>
		public IEnumerable<string> GetPropertyNames(Func<IPropertyData, bool> filter)
		{
			return this.properties.Values.Where(filter).Select(p => p.Name);
		}

		/// <summary>
		/// Highlights this instance.
		/// </summary>
		public virtual void Highlight()
		{
		}

		/// <summary>
		/// Tries the get the element.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <param name="propertyData">
		/// The property data.
		/// </param>
		/// <returns>
		/// <c>true</c> if the element exists; otherwise <c>false</c>.
		/// </returns>
		public bool TryGetElement(string key, out IPropertyData propertyData)
		{
			PropertyData<TElement> item;
			if (this.properties.TryGetValue(key, out item) && item.IsElement)
			{
				propertyData = item;
				return true;
			}

			propertyData = null;
			return false;
		}

		/// <summary>
		/// Tries the get the element.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <param name="propertyData">
		/// The property data.
		/// </param>
		/// <returns>
		/// <c>true</c> if the element exists; otherwise <c>false</c>.
		/// </returns>
		public bool TryGetProperty(string key, out IPropertyData propertyData)
		{
			PropertyData<TElement> item;
			if (this.properties.TryGetValue(key, out item))
			{
				propertyData = item;
				return true;
			}

			propertyData = null;
			return false;
		}

		/// <summary>
		/// Waits for the page to become active based on some user content.
		/// </summary>
		public void WaitForPageToBeActive()
		{
			var nativePage = this.GetNativePage<IActiveCheck>();
			if (nativePage == null)
			{
				return;
			}

			nativePage.WaitForActive();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Elements the enabled check.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns><c>true</c> if the element is enabled.</returns>
		public abstract bool ElementEnabledCheck(TElement element);

		/// <summary>
		/// Gets the element exists check function.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>
		/// True if the element exists; otherwise false.
		/// </returns>
		public abstract bool ElementExistsCheck(TElement element);

		/// <summary>
		/// Gets the element text.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>The element's text.</returns>
		public abstract string GetElementText(TElement element);

		/// <summary>
		/// Gets the page from element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>The page element.</returns>
		public abstract IPage GetPageFromElement(TElement element);

		/// <summary>
		/// Gets the click element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>
		/// True if the click is successful.
		/// </returns>
		public abstract bool ClickElement(TElement element);

		/// <summary>
		/// Gets the page fill method.
		/// </summary>
		/// <param name="propertyType">Type of the property.</param>
		/// <returns>
		/// The function used to fill the data.
		/// </returns>
		public abstract Action<TElement, string> GetPageFillMethod(Type propertyType);

        /// <summary>
        /// Highlights the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
	    public virtual void Highlight(TElement element)
	    {
	    }

	    /// <summary>
		/// Checks to see if the current type matches the base type of the system to not reflect base properties.
		/// </summary>
		/// <param name="propertyInfo">Type of the page.</param>
		/// <returns><c>true</c> if the type is the base class, otherwise <c>false</c>.</returns>
		protected virtual bool TypeIsNotBaseClass(PropertyInfo propertyInfo)
		{
			var type = propertyInfo.DeclaringType;
			return typeof(TPageBase) != type && typeof(TPageBase).IsAssignableFrom(type);
		}

        /// <summary>
        /// Checks to see if the property type is supported.
        /// </summary>
        /// <param name="type">The type being checked.</param>
        /// <returns><c>true</c> if the type is supported, <c>false</c> otherwise.</returns>
	    protected virtual bool SupportedPropertyType(Type type)
        {
            return type.IsClass;
        }

		/// <summary>
		/// Adds the element property.
		/// </summary>
		/// <param name="pageType">Type of the page.</param>
		/// <param name="propertyInfo">The property info.</param>
		/// <param name="propertyData">The property data.</param>
		private static void AddElementProperty(Type pageType, PropertyInfo propertyInfo, PropertyData<TElement> propertyData)
		{
			var nativePageFunc = new Func<IPage, TPageBase>(p => p.GetNativePage<TPageBase>());
			var pageArgument = Expression.Parameter(typeof(IPage), "page");
			var actionFunc = Expression.Parameter(typeof(Func<TElement, bool>), "actionFunc");

			var nativePageVariable = Expression.Variable(typeof(TPageBase), "nativePageType");
			var propertyVariable = Expression.Variable(pageType, "pageItem");

			var methodCall = Expression.Block(
				new[] { nativePageVariable, propertyVariable },
				Expression.Assign(nativePageVariable, Expression.Call(nativePageFunc.GetMethodInfo(), pageArgument)),
				Expression.Assign(propertyVariable, Expression.Convert(nativePageVariable, pageType)),
				Expression.Invoke(actionFunc, Expression.Property(propertyVariable, propertyInfo)));

			var expression =
				Expression.Lambda<Func<IPage, Func<TElement, bool>, bool>>(methodCall, pageArgument, actionFunc).Compile();

			propertyData.ElementAction = expression;
		}

		/// <summary>
		/// Adds the element property.
		/// </summary>
		/// <param name="pageType">Type of the page.</param>
		/// <param name="propertyInfo">The property info.</param>
		/// <param name="propertyData">The property data.</param>
		private static void AddProperty(Type pageType, PropertyInfo propertyInfo, PropertyData<TElement> propertyData)
		{
			var nativePageFunc = new Func<IPage, TPageBase>(p => p.GetNativePage<TPageBase>());
			var pageArgument = Expression.Parameter(typeof(IPage), "page");
			var actionFunc = Expression.Parameter(typeof(Func<object, bool>), "actionFunc");

			var nativePageVariable = Expression.Variable(typeof(TPageBase), "nativePageType");
			var propertyVariable = Expression.Variable(pageType, "pageItem");

			var methodCall = Expression.Block(
				new[] { nativePageVariable, propertyVariable },
				Expression.Assign(nativePageVariable, Expression.Call(nativePageFunc.GetMethodInfo(), pageArgument)),
				Expression.Assign(propertyVariable, Expression.Convert(nativePageVariable, pageType)), 
				Expression.Invoke(actionFunc, Expression.Property(propertyVariable, propertyInfo)));

			var expression =
				Expression.Lambda<Func<IPage, Func<object, bool>, bool>>(methodCall, pageArgument, actionFunc).Compile();

			propertyData.Action = expression;
		}

		/// <summary>
		/// Gets the page element.
		/// </summary>
		/// <returns>The page element.</returns>
		private PropertyData<TElement> GetPageElement()
		{
			//Note the word 'page' as a property representing the element
			return new PropertyData<TElement>(this)
			{
				Name = "Page",
				IsElement = true,
				IsList = false,
				PropertyType = this.PageType,
				ElementAction = (page, func) => func(this.GetNativePage<TElement>())
			};
		}

		/// <summary>
		/// Gets the properties.
		/// </summary>
		private void GetProperties()
		{
			const BindingFlags Flags = BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public;

			var pageType = this.PageType;

			var element = this.GetPageElement();
			this.properties.Add(element.Name, element);

			foreach (var propertyInfo in pageType.GetProperties(Flags).Where(
                                                p => p.CanRead && (this.SupportedPropertyType(p.PropertyType) || p.PropertyType.IsElementListType()) && this.TypeIsNotBaseClass(p)))
			{
				var propertyData = new PropertyData<TElement>(this) { Name = propertyInfo.Name, PropertyType = propertyInfo.PropertyType };

				if (typeof(TElement).IsAssignableFrom(propertyInfo.PropertyType))
				{
					propertyData.IsElement = true;
					AddElementProperty(pageType, propertyInfo, propertyData);
				}
				else
				{
					propertyData.IsList = propertyInfo.PropertyType.IsElementListType();
					AddProperty(pageType, propertyInfo, propertyData);
				}

				this.properties.Add(propertyData.Name, propertyData);
			}
		}

		#endregion
	}
}