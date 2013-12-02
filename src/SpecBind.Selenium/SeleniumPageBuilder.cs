// <copyright file="SeleniumPageBuilder.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    using SpecBind.BrowserSupport;
    using SpecBind.Pages;

    /// <summary>
    /// A page builder class that follows Selenium rules for page building.
    /// </summary>
    public class SeleniumPageBuilder : PageBuilderBase<ISearchContext, object, IWebElement>
    {
        /// <summary>
        /// Gets a value indicating whether to allow an empty constructor for a page object.
        /// </summary>
        /// <value><c>true</c> if an empty constructor should be allowed; otherwise, <c>false</c>.</value>
        protected override bool AllowEmptyConstructor
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Creates the page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <returns>The created page class.</returns>
        public Func<ISearchContext, IBrowser, Action<object>, object> CreatePage(Type pageType)
        {
            return this.CreateElementInternal(pageType);
        }

        /// <summary>
        /// Gets the element locators.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>The list of locators to use.</returns>
        internal static List<By> GetElementLocators(ElementLocatorAttribute attribute)
        {
            var locators = new List<By>(3);
            SetProperty(locators, attribute, a => By.Id(a.Id), a => a.Id != null);
            SetProperty(locators, attribute, a => By.Name(a.Name), a => a.Name != null);
            SetProperty(locators, attribute, a => By.TagName(a.TagName), a => a.TagName != null);
            SetProperty(locators, attribute, a => By.ClassName(a.Class), a => a.Class != null);
            SetProperty(locators, attribute, a => By.LinkText(a.Text), a => a.Text != null);

            // Setup the alt tag with whatever specifies it.
            SetProperty(locators, attribute, a => GetXPath(a.NormalizedTagName, "alt", a.Alt), a => a.Alt != null && a.TagName != null);
            
            // URL for Image and Hyperlink
            SetProperty(locators, attribute, a => GetXPath("img", "src", a.Url), a => a.Url != null && a.NormalizedTagName == "img");
            SetProperty(locators, attribute, a => GetXPath("a", "href", a.Url), a => a.Url != null && a.NormalizedTagName == "a");
            SetProperty(locators, attribute, a => GetXPath("area", "href", a.Url), a => a.Url != null && a.NormalizedTagName == "area");

            // Value attribute
            SetProperty(locators, attribute, a => GetXPath(a.NormalizedTagName, "value", a.Value), a => a.Value != null);

            // Title attribute
            SetProperty(locators, attribute, a => GetXPath(a.NormalizedTagName, "title", a.Title), a => a.Title != null);

            // Title attribute
            SetProperty(locators, attribute, a => GetXPath(a.NormalizedTagName, "type", a.Type), a => a.Type != null);

            // Index attribute
            SetProperty(locators, attribute, a => By.XPath(string.Format("//{0}[{1}]", a.NormalizedTagName, a.Index - 1)), a => a.Index > 0 && a.TagName != null);

            return locators;
        }

        /// <summary>
        /// Assigns the element attributes.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="nativeAttributes">The native attributes.</param>
        protected override void AssignElementAttributes(IWebElement control, ElementLocatorAttribute attribute, object[] nativeAttributes)
        {
            var proxy = control as WebElement;
            if (proxy == null)
            {
                return;
            }

            // Convert any locator property to "find by" classes
            var locators = attribute != null ? GetElementLocators(attribute) : new List<By>();

            // Also try to parse the native attributes
            var nativeItems = nativeAttributes != null ?  nativeAttributes.OfType<FindsByAttribute>().ToList() : null;

            if (nativeItems != null && nativeItems.Count > 0)
            {
                var localLocators = locators;
                locators.AddRange(nativeItems.Where(a => a.Using != null)
                                             .OrderBy(n => n.Priority)
                                             .Select(NativeAttributeBuilder.GetLocator)
                                             .Where(l => l != null && !localLocators.Any(c => Equals(c, l))));
            }

            locators = locators.Count > 1 ? new List<By> { new ByChained(locators.ToArray()) } : locators;
            proxy.UpdateLocators(locators);
        }

        /// <summary>
        /// Gets the custom attributes.
        /// </summary>
        /// <param name="propertyInfo">Type of the item.</param>
        /// <returns>A collection of custom attributes.</returns>
        protected override object[] GetCustomAttributes(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes(typeof(FindsByAttribute), true);
        }

        /// <summary>
        /// Gets the type of the property proxy.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>The created type.</returns>
        protected override Type GetPropertyProxyType(Type propertyType)
        {
            if (typeof(IWebElement) == propertyType)
            {
                return typeof(WebElement);
            }

            return propertyType;
        }

        /// <summary>
        /// Gets the constructor parameter for the given type.
        /// </summary>
        /// <param name="parameterType">Type of the parameter to fill.</param>
        /// <param name="parentArgument">The parent argument.</param>
        /// <param name="rootLocator">The root locator argument if different from the parent.</param>
        /// <returns>The constructor information that matches.</returns>
        protected override Expression FillConstructorParameter(Type parameterType, ExpressionData parentArgument, ExpressionData rootLocator)
        {
            var parentArg = (rootLocator != null && !typeof(ISearchContext).IsAssignableFrom(parentArgument.Type))
                                        ? rootLocator.Expression
                                        : parentArgument.Expression;

            // If it's a web driver use that first.
            if (typeof(IWebDriver).IsAssignableFrom(parameterType) && rootLocator != null)
            {
                return Expression.Convert(rootLocator.Expression, parameterType);
            }
            
            if (typeof(ISearchContext).IsAssignableFrom(parameterType))
            {
                // Use a search context second
                return Expression.Convert(parentArg, parameterType);
            }

            return null;
        }

        /// <summary>
        /// Gets the type of the element collection.
        /// </summary>
        /// <returns>The collection type.</returns>
        protected override Type GetElementCollectionType()
        {
            return typeof(SeleniumListElementWrapper<,>);
        }

        /// <summary>
        /// Gets the locator based on XPath syntax.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>The locator object.</returns>
        private static By GetXPath(string tagName, string propertyName, string value)
        {
            return By.XPath(
                string.Format("//{0}[@{1}='{2}']", tagName, propertyName.ToLowerInvariant(), value));
        }

        /// <summary>
        /// Sets the property of the locator by the filter.
        /// </summary>
        /// <typeparam name="T">The type of the element being set.</typeparam>
        /// <param name="locators">The locator collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="setterFunc">The setter function.</param>
        /// <param name="filterFunc">The filter function.</param>
        private static void SetProperty<T>(ICollection<By> locators, T item, Func<T, By> setterFunc, Func<T, bool> filterFunc = null)
        {
            if (filterFunc == null || filterFunc(item))
            {
                locators.Add(setterFunc(item));
            }
        }
    }
}