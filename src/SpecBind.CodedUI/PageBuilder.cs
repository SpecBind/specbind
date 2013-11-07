// <copyright file="PageBuilder.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI
{
	using System;
	using System.Globalization;
	using System.Reflection;

	using Microsoft.VisualStudio.TestTools.UITesting;
	using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

	using SpecBind.Helpers;
	using SpecBind.Pages;

    /// <summary>
    /// A class that constructs pages for the framework.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent control.</typeparam>
    /// <typeparam name="TOutput">The type of the output control.</typeparam>
    public class PageBuilder<TParent, TOutput> : PageBuilderBase<UITestControl, TOutput, HtmlControl>
        where TParent : UITestControl
        where TOutput : HtmlControl
	{
        /// <summary>
        /// Creates the page.
        /// </summary>
        /// <param name="elementType">Type of the page.</param>
        /// <returns>The page builder function.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the constructor is invalid.</exception>
        public static Func<TParent, Action<HtmlControl>, TOutput> CreateElement(Type elementType)
		{
		    var builder = new PageBuilder<TParent, TOutput>();
		    return builder.CreateElementInternal(elementType);
		}

        /// <summary>
        /// Creates the frame locator method to help load that from a property.
        /// </summary>
        /// <param name="frameType">Type of the class that will provide the frame.</param>
        /// <param name="property">The property on the class that should be accessed to provide the frame.</param>
        /// <returns>The function used to create the document.</returns>
        public static Func<TParent, TOutput> CreateFrameLocator(Type frameType, PropertyInfo property)
        {
            var builder = new PageBuilder<TParent, TOutput>();
            return builder.CreateFrameLocatorInternal(frameType, property);
        }

        /// <summary>
        /// Assigns the element attributes.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="attribute">The attribute.</param>
        protected override void AssignElementAttributes(HtmlControl control, ElementLocatorAttribute attribute)
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
        /// Checks to see if the control is the same type as the base class and performs the appropriate actions.
        /// </summary>
        /// <param name="control">The control.</param>
        protected override void CheckPageIsBaseClass(TOutput control)
        {
            if (!(control is HtmlDocument))
            {
                return;
            }

            control.SearchProperties[HtmlControl.PropertyNames.Id] = null;
            SetProperty(control.SearchProperties, HtmlDocument.PropertyNames.RedirectingPage, "False");
            SetProperty(control.SearchProperties, HtmlDocument.PropertyNames.FrameDocument, "False");
        }

        /// <summary>
        /// Assigns the page element attributes.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="locatorAttribute">The locator attribute.</param>
        protected override void AssignPageElementAttributes(TOutput control, ElementLocatorAttribute locatorAttribute)
        {
            this.AssignElementAttributes(control, locatorAttribute);
        }

        /// <summary>
        /// Gets the type of the element collection.
        /// </summary>
        /// <returns>The element collection concrete type.</returns>
        protected override Type GetElementCollectionType()
        {
            return typeof(CodedUIListElementWrapper<,>);
        }

        /// <summary>
        /// Checks to see if the control is the same type as the base class and performs the appropriate actions.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="attribute">The page navigation attribute.</param>
        protected override void SetPageNavigationAttribute(TOutput control, PageNavigationAttribute attribute)
        {
            SetProperty(control.FilterProperties, HtmlDocument.PropertyNames.AbsolutePath, attribute.Url);
            SetProperty(control.FilterProperties, HtmlDocument.PropertyNames.PageUrl, UriHelper.GetQualifiedPageUri(attribute.Url).ToString());
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