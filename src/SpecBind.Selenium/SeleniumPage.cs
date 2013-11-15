// <copyright file="SeleniumPage.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Reflection;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using OpenQA.Selenium.Support.UI;

    using SpecBind.Pages;

    /// <summary>
    /// An implementation of <see cref="IPage"/> for the Selenium driver.
    /// </summary>
    public class SeleniumPage : PageBase<object, IWebElement>
    {
        private readonly SeleniumPageBuilder builder;
        private readonly object nativePage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumPage"/> class.
        /// </summary>
        /// <param name="nativePage">The native page.</param>
        public SeleniumPage(object nativePage) : base(nativePage.GetType())
        {
            this.nativePage = nativePage;

            this.builder = new SeleniumPageBuilder();
        }

        /// <summary>
        /// Gets the native page.
        /// </summary>
        /// <typeparam name="TPage">The type of the t page.</typeparam>
        /// <returns>The native page object.</returns>
        public override TPage GetNativePage<TPage>()
        {
            return (TPage)this.nativePage;
        }

        /// <summary>
        /// Checks to see if the element is enabled.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element is enabled, <c>false</c> otherwise.</returns>
        public override bool ElementEnabledCheck(IWebElement element)
        {
            return CheckElementState(e => e.Displayed && e.Enabled, element);
        }

        /// <summary>
        /// Checks to see if the element exists.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element exists, <c>false</c> otherwise.</returns>
        public override bool ElementExistsCheck(IWebElement element)
        {
            return CheckElementState(e => e.Displayed, element);
        }

        /// <summary>
        /// Gets the element text.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The text of the element.</returns>
        public override string GetElementText(IWebElement element)
        {
            var tagName = element.TagName.ToLowerInvariant().Trim();
            switch (tagName)
            {
                case "select":
                    var selectElement = new SelectElement(element);
                    return selectElement.SelectedOption.Text;
                case "input":
                    // Special case for a checkbox control
                    if (string.Equals("checkbox", element.GetAttribute("type"), StringComparison.OrdinalIgnoreCase))
                    {
                        return element.Selected.ToString();
                    }

                    return element.GetAttribute("value");
                default:
                    return element.Text;
            }
        }

        /// <summary>
        /// Gets the page from element.
        /// </summary>
        /// <param name="element">The parent element.</param>
        /// <returns>The child page as a scope.</returns>
        public override IPage GetPageFromElement(IWebElement element)
        {
            return new SeleniumPage(element);
        }

        /// <summary>
        /// Clicks the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element is clicked, <c>false</c> otherwise.</returns>
        public override bool ClickElement(IWebElement element)
        {
            if (!element.Selected)
            {
                element.Click();
            }

            return true;
        }

        /// <summary>
        /// Gets the page fill method.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>The action to fill the page.</returns>
        public override Action<IWebElement, string> GetPageFillMethod(Type propertyType)
        {
            return FillPage;
        }
        
        /// <summary>
        /// Checks to see if the property type is supported.
        /// </summary>
        /// <param name="type">The type being checked.</param>
        /// <returns><c>true</c> if the type is supported, <c>false</c> otherwise.</returns>
        protected override bool SupportedPropertyType(Type type)
        {
            return type.IsInterface;
        }

        /// <summary>
        /// Checks to see if the current type matches the base type of the system to not reflect base properties.
        /// </summary>
        /// <param name="propertyInfo">Type of the page.</param>
        /// <returns><c>true</c> if the type is the base class, otherwise <c>false</c>.</returns>
        protected override bool TypeIsNotBaseClass(PropertyInfo propertyInfo)
        {
            return true;
        }

        /// <summary>
        /// Checks the state of the element.
        /// </summary>
        /// <param name="checkFunc">The check function.</param>
        /// <param name="element">The element.</param>
        /// <returns>The result of the check.</returns>
        private static bool CheckElementState(Func<IWebElement, bool> checkFunc, IWebElement element)
        {
            try
            {
                return checkFunc(element);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (ElementNotVisibleException)
            {
                return false;
            }
        }

        /// <summary>
        /// Fills the page.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="data">The data.</param>
        private static void FillPage(IWebElement element, string data)
        {
            var tagName = element.TagName.ToLowerInvariant().Trim();
            switch (tagName)
            {
                case "select":
                    var selectElement = new SelectElement(element);
                    if (selectElement.IsMultiple)
                    {
                        selectElement.DeselectAll();    
                    }
                    
                    selectElement.SelectByText(data);
                    break;
                case "input":
                    // Special case for a checkbox control
                    if (string.Equals("checkbox", element.GetAttribute("type"), StringComparison.OrdinalIgnoreCase))
                    {
                        bool checkValue;
                        if (bool.TryParse(data, out checkValue) && element.Selected != checkValue)
                        {
                            element.Click();
                        }
                        return;
                    }
                    goto default;
                default:
                    element.SendKeys(data);
                    break;
            }
        }
    }
}