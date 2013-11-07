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

    using SpecBind.Pages;

    /// <summary>
    /// A page builder class that follows Selenium rules for page building.
    /// </summary>
    public class SeleniumPageBuilder : PageBuilderBase<IWebDriver, object, IWebElement>
    {
        /// <summary>
        /// Creates the page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <returns>The created page class.</returns>
        public Func<IWebDriver, Action<object>, object> CreatePage(Type pageType)
        {
            return this.CreateElementInternal(pageType);
        }

        /// <summary>
        /// Assigns the element attributes.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="attribute">The attribute.</param>
        protected override void AssignElementAttributes(IWebElement control, ElementLocatorAttribute attribute)
        {
            var proxy = control as WebElement;
            if (proxy == null)
            {
                return;
            }

            // Convert any locator property to "find by" classes
            var locators = new List<By>(3);
            SetProperty(locators, attribute, a => By.Id(a.Id), a => a.Id != null);
            SetProperty(locators, attribute, a => By.Name(a.Name), a => a.Name != null);
            SetProperty(locators, attribute, a => By.TagName(a.TagName), a => a.TagName != null);
            SetProperty(locators, attribute, a => By.ClassName(a.Class), a => a.Class != null);
            SetProperty(locators, attribute, a => By.LinkText(a.Text), a => a.Text != null);

            // Also try to parse the native attributes
            var nativeAttributes = control.GetType().GetCustomAttributes(typeof(FindsByAttribute), true)
                                                    .OfType<FindsByAttribute>()
                                                    .ToList();
            if (nativeAttributes.Count > 0)
            {
                foreach (var locator in nativeAttributes
                                            .Where(a => a.Using != null)
                                            .OrderBy(n => n.Priority)
                                            .Select(CreateNativeLocator)
                                            .Where(l => l != null && !locators.Any(c => Equals(c, l))))
                {
                    locators.Add(locator);
                }
            }

            proxy.UpdateLocators(locators);
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
        /// Gets the constructor.
        /// </summary>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="parentArgument">The parent argument.</param>
        /// <param name="parentArgumentType">Type of the parent argument.</param>
        /// <param name="rootLocator">The root locator if different from the parent.</param>
        /// <returns>The constructor information that matches.</returns>
        protected override Tuple<ConstructorInfo, IEnumerable<Expression>> GetConstructor(Type itemType, Expression parentArgument, Type parentArgumentType, Expression rootLocator)
        {
            foreach (var constructorInfo in itemType.GetConstructors(BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var paramters = constructorInfo.GetParameters();
                if (paramters.Length != 1)
                {
                    continue;
                }

                var firstParameter = paramters.First();
                if (typeof(IWebDriver).IsAssignableFrom(firstParameter.ParameterType) ||
                    typeof(ISearchContext).IsAssignableFrom(firstParameter.ParameterType))
                {
                    // Need a build page or context here see if the parent matches
                    var parentArg = (rootLocator != null && !typeof(ISearchContext).IsAssignableFrom(parentArgumentType))
                                        ? rootLocator
                                        : parentArgument;

                    return new Tuple<ConstructorInfo, IEnumerable<Expression>>(constructorInfo, new[] { Expression.Convert(parentArg, firstParameter.ParameterType) });
                }
            }

            var emptyConstructor = itemType.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 0);
            return emptyConstructor != null
                       ? new Tuple<ConstructorInfo, IEnumerable<Expression>>(emptyConstructor, new List<Expression>(0))
                       : null;
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
        /// Creates the native locator attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>The created locator.</returns>
        private static By CreateNativeLocator(FindsByAttribute attribute)
        {
            var how = attribute.How;
            var usingValue = attribute.Using;
            switch (how)
            {
                case How.Id:
                    return By.Id(usingValue);
                case How.Name:
                    return By.Name(usingValue);
                case How.TagName:
                    return By.TagName(usingValue);
                case How.ClassName:
                    return By.ClassName(usingValue);
                case How.CssSelector:
                    return By.CssSelector(usingValue);
                case How.LinkText:
                    return By.LinkText(usingValue);
                case How.PartialLinkText:
                    return By.PartialLinkText(usingValue);
                case How.XPath:
                    return By.XPath(usingValue);
                case How.Custom:
                    if (attribute.CustomFinderType == null ||
                        !attribute.CustomFinderType.IsSubclassOf(typeof(By)))
                    {
                        return null;
                    }

                    var constructor = attribute.CustomFinderType.GetConstructor(new[] { typeof(string) });
                    return constructor != null ? constructor.Invoke(new object[] { usingValue }) as By : null;
                default:
                    return null;
            }
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