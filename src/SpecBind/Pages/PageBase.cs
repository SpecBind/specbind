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

	using SpecBind.Actions;
	using SpecBind.PropertyHandlers;

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

		private readonly Dictionary<string, PropertyDataBase<TElement>> properties;
        private readonly TPageBase page;

		#endregion

		#region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PageBase{TPageBase, TElement}" /> class.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="page">The page.</param>
		protected PageBase(Type pageType, TPageBase page)
		{
		    this.page = page;
			this.PageType = pageType;
			this.properties = new Dictionary<string, PropertyDataBase<TElement>>(StringComparer.InvariantCultureIgnoreCase);
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
        public virtual TPage GetNativePage<TPage>() where TPage : class
        {
            return this.page as TPage;
        }

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
			PropertyDataBase<TElement> item;
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
			PropertyDataBase<TElement> item;
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
        /// Gets the element not-exists check function.
        /// Unlike ELementExistsCheck, this doesn't let the web driver wait first for the element to exist.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// True if the element doesn't exist; otherwise false.
        /// </returns>
        public abstract bool ElementNotExistsCheck(TElement element);

        /// <summary>
        /// Gets the element attribute value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>The attribute value.</returns>
        public abstract string GetElementAttributeValue(TElement element, string attributeName);

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
        /// Waits for element condition to be met.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="waitCondition">The wait condition.</param>
        /// <param name="timeout">The time to wait before failing.</param>
        /// <returns><c>true</c> if the condition is met, <c>false</c> otherwise.</returns>
        public abstract bool WaitForElement(TElement element, WaitConditions waitCondition, TimeSpan? timeout);

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
        /// <returns>The created element locator function.</returns>
        private static Func<IPage, Func<TElement, bool>, bool> AddElementProperty(Type pageType, PropertyInfo propertyInfo)
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

			return Expression.Lambda<Func<IPage, Func<TElement, bool>, bool>>(methodCall, pageArgument, actionFunc).Compile();
		}

        /// <summary>
        /// Adds the element property.
        /// </summary>
        /// <param name="elementDescription">The element description.</param>
        /// <returns>The value function.</returns>
        private static Func<IPage, Func<TElement, bool>, bool> AddValueProperty(ElementDescription elementDescription)
        {
            var pageArgument = Expression.Parameter(typeof(IPage), "page");
            var actionFunc = Expression.Parameter(typeof(Func<TElement, bool>), "actionFunc");
            
            return
                Expression.Lambda<Func<IPage, Func<TElement, bool>, bool>>(
                                    Expression.Invoke(actionFunc, Expression.Convert(Expression.Constant(elementDescription.Value, elementDescription.PropertyType), typeof(TElement))),
                                    pageArgument, 
                                    actionFunc)
                          .Compile();
        }

        /// <summary>
        /// Adds the element property.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>A tuple containing the get and set expressions.</returns>
        private static Tuple<Func<IPage, Func<object, bool>, bool>, Action<IPage, object>> AddProperty(Type pageType, PropertyInfo propertyInfo)
		{
			var nativePageFunc = new Func<IPage, TPageBase>(p => p.GetNativePage<TPageBase>());
			var pageArgument = Expression.Parameter(typeof(IPage), "page");
			var actionFunc = Expression.Parameter(typeof(Func<object, bool>), "actionFunc");

			var nativePageVariable = Expression.Variable(typeof(TPageBase), "nativePageType");
			var propertyVariable = Expression.Variable(pageType, "pageItem");

			var getMethodCall = Expression.Block(
				new[] { nativePageVariable, propertyVariable },
				Expression.Assign(nativePageVariable, Expression.Call(nativePageFunc.GetMethodInfo(), pageArgument)),
				Expression.Assign(propertyVariable, Expression.Convert(nativePageVariable, pageType)), 
				Expression.Invoke(actionFunc, Expression.Property(propertyVariable, propertyInfo)));

			var getExpression = Expression.Lambda<Func<IPage, Func<object, bool>, bool>>(getMethodCall, pageArgument, actionFunc).Compile();

			Action<IPage, object> setExpression = null;
		    if (propertyInfo.CanWrite && propertyInfo.GetSetMethod() != null)
		    {
		        var setValue = Expression.Variable(typeof(object));
		        var setMethodCall = Expression.Block(
		            new[] { nativePageVariable, propertyVariable },
		            Expression.Assign(nativePageVariable, Expression.Call(nativePageFunc.GetMethodInfo(), pageArgument)),
		            Expression.Assign(propertyVariable, Expression.Convert(nativePageVariable, pageType)),
		            Expression.Assign(
		                Expression.Property(propertyVariable, propertyInfo),
		                Expression.Convert(setValue, propertyInfo.PropertyType)));

		        setExpression = Expression.Lambda<Action<IPage, object>>(setMethodCall, pageArgument, setValue).Compile();
		    }

            return new Tuple<Func<IPage, Func<object, bool>, bool>, Action<IPage, object>>(getExpression, setExpression);
		}

		/// <summary>
		/// Gets the page element.
		/// </summary>
		/// <returns>The page element.</returns>
		private ElementPropertyData<TElement> GetPageElement()
		{
			//Note the word 'page' as a property representing the element
			return new ElementPropertyData<TElement>(this, "Page", this.PageType, (p, func) => func(this.GetNativePage<TElement>()));
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

            var locatorElement = this.GetNativePage<IElementProvider>();
		    if (locatorElement != null)
		    {
		        foreach (var property in locatorElement.GetElements())
		        {
                    var propertyData = new ElementPropertyData<TElement>(
                                            this, property.PropertyName, property.PropertyType, AddValueProperty(property));

		            this.properties.Add(propertyData.Name, propertyData);
		        }

		        return;
		    }

			foreach (var propertyInfo in pageType.GetProperties(Flags).Where(
                                                p => p.CanRead && (this.SupportedPropertyType(p.PropertyType) || p.PropertyType.IsElementListType()) && this.TypeIsNotBaseClass(p)))
			{
			    PropertyDataBase<TElement> propertyData;

				if (typeof(TElement).IsAssignableFrom(propertyInfo.PropertyType))
				{
				    var elementHandler = AddElementProperty(pageType, propertyInfo);
                    var elementPropertyData = new ElementPropertyData<TElement>(this, propertyInfo.Name, propertyInfo.PropertyType, elementHandler);
                    
                    // Check for any alias attributes and attempt to build additional properties
                    this.CheckForVirtualProperties(propertyInfo, elementHandler);
				    propertyData = elementPropertyData;
				}
                else if (propertyInfo.PropertyType.IsElementListType())
				{
                    var expressions = AddProperty(pageType, propertyInfo);
                    propertyData = new ListPropertyData<TElement>(this, propertyInfo.Name, propertyInfo.PropertyType, expressions.Item1);
			    }
                else
                {
                    var expressions = AddProperty(pageType, propertyInfo);
                    propertyData = new PagePropertyData<TElement>(
                        this,
                        propertyInfo.Name,
                        propertyInfo.PropertyType,
                        expressions.Item1,
                        expressions.Item2);
                }

				this.properties.Add(propertyData.Name, propertyData);
			}
		}

        /// <summary>
        /// Checks for virtual properties and creates the structure for it.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="elementHandler">The property data.</param>
        private void CheckForVirtualProperties(MemberInfo propertyInfo, Func<IPage, Func<TElement, bool>, bool> elementHandler)
        {
            foreach (var customAttribute in propertyInfo.GetCustomAttributes<VirtualPropertyAttribute>())
            {
                var propertyName = customAttribute.Name.Trim();
                if (!this.properties.ContainsKey(propertyName))
                {
                    var softProperty = new VirtualPropertyData<TElement>(this, propertyName, elementHandler, customAttribute.Attribute);
                    this.properties.Add(propertyName, softProperty);
                }
            }
        }

        #endregion
	}
}