// <copyright file="PageBuilder.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	using Microsoft.VisualStudio.TestTools.UITesting;
	using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

	using SpecBind.Helpers;
	using SpecBind.Pages;

	/// <summary>
	/// A class that constructs pages for the framework.
	/// </summary>
	public static class PageBuilder
	{
		private static readonly MethodInfo AssignMethodInfo = new Action<HtmlControl, ElementLocatorAttribute>(AssignElementAttributes).GetMethodInfo();

		/// <summary>
		/// Creates the page.
		/// </summary>
		/// <typeparam name="TParent">The type of the parent.</typeparam>
		/// <typeparam name="TOutput">The type of the output.</typeparam>
		/// <param name="elementType">Type of the page.</param>
		/// <returns>
		/// The page builder function.
		/// </returns>
		/// <exception cref="System.InvalidOperationException">Thrown if the constructor is invalid.</exception>
		public static Func<TParent, Action<TOutput>, TOutput> CreateElement<TParent, TOutput>(Type elementType)
			where TParent : UITestControl
			where TOutput : HtmlControl
		{
			var expression = CreateNewItemExpression<TParent, TOutput>(elementType);
			return expression.Compile();
		}

		/// <summary>
		/// Creates the frame locator method to help load that from a property.
		/// </summary>
		/// <typeparam name="TParent">The type of the parent.</typeparam>
		/// <typeparam name="TOutput">The type of the output.</typeparam>
		/// <param name="frameType">Type of the class that will provide the frame.</param>
		/// <param name="property">The property on the class that should be accessed to provide the frame.</param>
		/// <returns>The function used to create the document.</returns>
		public static Func<TParent, TOutput> CreateFrameLocator<TParent, TOutput>(Type frameType, PropertyInfo property)
			where TParent : UITestControl
			where TOutput : HtmlControl
		{
			var createExpression = CreateNewItemExpression<TParent, HtmlControl>(frameType);

			var parentArgument = Expression.Parameter(typeof(TParent), "parent");
			var docVariable = Expression.Variable(frameType);

			var expressions = new List<Expression>
				                  {
					                  Expression.Assign(docVariable, Expression.Convert(Expression.Invoke(createExpression, parentArgument, Expression.Constant(null, typeof(Action<HtmlControl>))), frameType)),
									  Expression.Convert(Expression.Property(docVariable, property), typeof(TOutput))
				                  };

			var methodCall = Expression.Block(new[] { docVariable }, expressions);

			return Expression.Lambda<Func<TParent, TOutput>>(methodCall, parentArgument).Compile();
		}

		/// <summary>
		/// Assigns the element attributes.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <param name="attribute">The attribute.</param>
		private static void AssignElementAttributes(HtmlControl control, ElementLocatorAttribute attribute)
		{
			SetProperty(control.SearchProperties, HtmlControl.PropertyNames.Id, attribute.Id);
			SetProperty(control.SearchProperties, UITestControl.PropertyNames.Name, attribute.Name);
			SetProperty(control.SearchProperties, HtmlControl.PropertyNames.TagName, attribute.TagName);
			SetProperty(control.SearchProperties, HtmlControl.PropertyNames.Type, attribute.Type);

			SetProperty(control.FilterProperties, HtmlControl.PropertyNames.Title, attribute.Title);
			SetProperty(control.FilterProperties, UITestControl.PropertyNames.ClassName, attribute.Class);
			SetProperty(control.FilterProperties, HtmlControl.PropertyNames.ValueAttribute, attribute.Value);

			SetProperty(() => attribute.Index > -1, control.FilterProperties, HtmlControl.PropertyNames.TagInstance, attribute.Index.ToString(CultureInfo.InvariantCulture));

			SetProperty(() => (control is HtmlImage), control.FilterProperties, HtmlImage.PropertyNames.Alt, attribute.Alt);
			SetProperty(() => (control is HtmlImage), control.FilterProperties, HtmlImage.PropertyNames.Src, attribute.Url);

			SetProperty(() => (control is HtmlHyperlink), control.FilterProperties, HtmlHyperlink.PropertyNames.Alt, attribute.Alt);
			SetProperty(() => (control is HtmlHyperlink), control.FilterProperties, HtmlHyperlink.PropertyNames.Href, attribute.Url);

			SetProperty(() => (control is HtmlAreaHyperlink), control.FilterProperties, HtmlAreaHyperlink.PropertyNames.Alt, attribute.Alt);
			SetProperty(() => (control is HtmlAreaHyperlink), control.FilterProperties, HtmlAreaHyperlink.PropertyNames.Href, attribute.Url);

			SetTextLocator(control, attribute.Text);
		}

		/// <summary>
		/// Assigns the attributes for the document based on its metadata.
		/// </summary>
		/// <typeparam name="TOutput">The type of the output.</typeparam>
		/// <param name="control">The control.</param>
		/// <param name="customAction">The custom action.</param>
		private static void AssignPageAttributes<TOutput>(TOutput control, Action<TOutput> customAction)
			where TOutput : HtmlControl
		{
			if (control is HtmlDocument)
			{
				control.SearchProperties[HtmlControl.PropertyNames.Id] = null;
				SetProperty(control.SearchProperties, HtmlDocument.PropertyNames.RedirectingPage, "False");
				SetProperty(control.SearchProperties, HtmlDocument.PropertyNames.FrameDocument, "False");
			}

			if (customAction != null)
			{
				customAction.Invoke(control);
			}

			var controlType = control.GetType();
			PageNavigationAttribute attribute;
			if (controlType.TryGetAttribute(out attribute))
			{
				SetProperty(control.FilterProperties, HtmlDocument.PropertyNames.AbsolutePath, attribute.Url);
				SetProperty(control.FilterProperties, HtmlDocument.PropertyNames.PageUrl, UriHelper.GetQualifiedPageUri(attribute.Url).ToString());
			}

			ElementLocatorAttribute locatorAttribute;
			if (controlType.TryGetAttribute(out locatorAttribute))
			{
				AssignElementAttributes(control, locatorAttribute);
			}
		}

		/// <summary>
		/// Creates the new item expression that creates the object and initial mapping.
		/// </summary>
		/// <typeparam name="TParent">The type of the parent.</typeparam>
		/// <typeparam name="TOutput">The type of the output.</typeparam>
		/// <param name="elementType">Type of the element.</param>
		/// <returns>The initial creation lambda expression.</returns>
		/// <exception cref="System.InvalidOperationException">Thrown if the constructor is invalid.</exception>
		private static Expression<Func<TParent, Action<TOutput>, TOutput>> CreateNewItemExpression<TParent, TOutput>(Type elementType)
			where TParent : UITestControl
			where TOutput : HtmlControl
		{
			var constructor = GetConstructor(elementType);
			if (constructor == null)
			{
				throw new InvalidOperationException(
					string.Format("Constructor on type '{0}' must have a sigle argument of type UITestControl.", elementType.Name));
			}

			var parentArgument = Expression.Parameter(typeof(TParent), "parent");
			var actionArgument = Expression.Parameter(typeof(Action<TOutput>), "action");
			var docVariable = Expression.Variable(elementType);

			//Spin though properties and make an initializer for anything we can set that has an attribute
			var pageMethodInfo = new Action<TOutput, Action<TOutput>>(AssignPageAttributes).GetMethodInfo();
			var expressions = new List<Expression>
				                  {
					                  Expression.Assign(docVariable, Expression.New(constructor, parentArgument)),
					                  Expression.Call(
						                  pageMethodInfo,
						                  Expression.Convert(docVariable, typeof(TOutput)),
						                  actionArgument)
				                  };

			MapObjectProperties(expressions, elementType, docVariable);
			expressions.Add(docVariable);

			var methodCall = Expression.Block(new[] { docVariable }, expressions);
			return Expression.Lambda<Func<TParent, Action<TOutput>, TOutput>>(methodCall, parentArgument, actionArgument);
		}

		/// <summary>
		/// Gets the constructor.
		/// </summary>
		/// <param name="itemType">Type of the item.</param>
		/// <returns>The constructor information that matches.</returns>
		private static ConstructorInfo GetConstructor(Type itemType)
		{
			return itemType.GetConstructors()
				.FirstOrDefault(c => c.GetParameters().Length == 1 && typeof(UITestControl).IsAssignableFrom(c.GetParameters().First().ParameterType));
		}

		/// <summary>
		/// Maps the object properties for the given object.
		/// </summary>
		/// <param name="expressions">The parent expression list.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="parentVariable">The parent variable.</param>
		private static void MapObjectProperties(ICollection<Expression> expressions, Type objectType, Expression parentVariable)
		{
			// ReSharper disable LoopCanBeConvertedToQuery
			foreach (var propertyInfo in objectType.GetProperties().Where(p =>
				p.SetMethod != null && p.CanWrite && (typeof(HtmlControl).IsAssignableFrom(p.PropertyType) || p.PropertyType.IsElementListType())))
			// ReSharper restore LoopCanBeConvertedToQuery
			{
				var propertyType = propertyInfo.PropertyType;
				var attribute = propertyInfo.GetCustomAttributes(typeof(ElementLocatorAttribute), false).FirstOrDefault();
				if (attribute == null)
				{
					continue;
				}

				//Final Properties
				var itemVariable = Expression.Variable(propertyType);
				var variableList = new List<ParameterExpression> { itemVariable };
				var propertyExpressions = new List<Expression>();

				//Special case for lists
				if (propertyType.IsElementListType())
				{
					var concreteType = typeof(CodedUIListElementWrapper<,>).MakeGenericType(propertyType.GetGenericArguments());
					var concreteTypeConstructor = concreteType.GetConstructors().First();

					var parentListType = propertyType.GetGenericArguments().First();
					var parentListVariable = Expression.Variable(parentListType, "collectionParent");
					variableList.Add(parentListVariable);
					
					propertyExpressions.AddRange(CreateHtmlObject(parentVariable, parentListVariable, parentListType, propertyInfo.Name, attribute));
					propertyExpressions.Add(Expression.Assign(itemVariable, Expression.New(concreteTypeConstructor, parentListVariable)));
				}
				else
				{
					//Normal path starts here
					//New up property and then check if for inner properties.
					propertyExpressions.AddRange(CreateHtmlObject(parentVariable, itemVariable, propertyType, propertyInfo.Name, attribute));

					MapObjectProperties(propertyExpressions, propertyType, itemVariable);
				}

				propertyExpressions.Add(Expression.Assign(Expression.Property(parentVariable, propertyInfo), itemVariable));
				expressions.Add(Expression.Block(variableList, propertyExpressions));
			}
		}

		/// <summary>
		/// Creates the HTML object.
		/// </summary>
		/// <param name="parentVariable">The parent variable.</param>
		/// <param name="itemVariable">The item variable.</param>
		/// <param name="propertyType">Type of the property.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="attribute">The attribute.</param>
		/// <returns>
		/// The expressions needed to create the list
		/// </returns>
		private static IEnumerable<Expression> CreateHtmlObject(Expression parentVariable, Expression itemVariable, Type propertyType, string propertyName, object attribute)
		{
			var propConstructor = GetConstructor(propertyType);
			if (propConstructor == null)
			{
				throw new InvalidOperationException(string.Format(
					"Property '{0}' of type '{1}' has an empty constructor. Elements need to inherit the base constructor that accepts a UITestControl parameter.",
					propertyName,
					propertyType.Name));
			}

			return new[]
				       {
					       (Expression)Expression.Assign(itemVariable, Expression.New(propConstructor, Expression.Convert(parentVariable, typeof(UITestControl)))),
						   Expression.Call(AssignMethodInfo, Expression.Convert(itemVariable, typeof(HtmlControl)), Expression.Constant(attribute))
				       };
		}

		/// <summary>
		/// Sets the text locator.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <param name="textLocator">The text locator.</param>
		private static void SetTextLocator(UITestControl control, string textLocator)
		{
			if (string.IsNullOrWhiteSpace(textLocator))
			{
				return;
			}

			string locator;
			if (control is HtmlButton)
			{
				locator = HtmlButton.PropertyNames.DisplayText;
			}
			else if (control is HtmlEdit)
			{
				locator = HtmlEdit.PropertyNames.Text;
			}
			else
			{
				locator = HtmlControl.PropertyNames.InnerText;
			}

			SetProperty(control.FilterProperties, locator, textLocator);
		}

		/// <summary>
		/// Sets the property.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		private static void SetProperty(PropertyExpressionCollection collection, string key, string value)
		{
			SetProperty(null, collection, key, value);
		}

		/// <summary>
		/// Sets the property.
		/// </summary>
		/// <param name="conditionFunc">The condition function.</param>
		/// <param name="collection">The collection.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		private static void SetProperty(Func<bool> conditionFunc, PropertyExpressionCollection collection, string key, string value)
		{
			if (!string.IsNullOrWhiteSpace(value) && (conditionFunc == null || conditionFunc()))
			{
				collection[key] = value;
			}
		}
	}
}