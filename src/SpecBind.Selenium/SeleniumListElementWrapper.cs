// <copyright file="SeleniumListElementWrapper.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using OpenQA.Selenium;

    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A list element wrapper collection for Selenium elements.
    /// </summary>
    /// <typeparam name="TElement">The type of the parent element.</typeparam>
    /// <typeparam name="TChildElement">The type of the child element.</typeparam>
    public class SeleniumListElementWrapper<TElement, TChildElement> : ListElementWrapper<TElement, TChildElement>
        where TElement : IWebElement 
        where TChildElement : class
    {
        private readonly Func<ISearchContext, IBrowser, Action<object>, object> builderFunc;
        private readonly By locator;

        private ReadOnlyCollection<IWebElement> itemCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumListElementWrapper{TElement, TChildElement}" /> class.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="browser">The browser.</param>
        public SeleniumListElementWrapper(TElement parentElement, IBrowser browser)
            : base(parentElement, browser)
        {
            var builder = new SeleniumPageBuilder();
            this.builderFunc = builder.CreatePage(typeof(TChildElement));
            this.locator = GetElementLocator();
        }

        /// <summary>
        /// Creates the element.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="index">The index.</param>
        /// <returns>The created child element.</returns>
        protected override TChildElement CreateElement(IBrowser browser, TElement parentElement, int index)
        {
            if (this.locator != null)
            {
                if (this.itemCollection == null)
                {
                    this.itemCollection = parentElement.FindElements(this.locator);
                }

                var element = (index > 0 && index <= this.itemCollection.Count) ? this.itemCollection[index - 1] : null;
                if (element != null)
                {
                    var childElement = (TChildElement)this.builderFunc(parentElement, browser, null);

                    var webElement = childElement as WebElement;
                    if (webElement != null)
                    {
                        webElement.CloneNativeElement(element);
                    }

                    return childElement;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks to see if the element exists.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="expectedIndex">The expected index.</param>
        /// <returns><c>true</c> if the element exists, <c>false</c> otherwise.</returns>
        protected override bool ElementExists(TChildElement element, int expectedIndex)
        {
            var webElement = element as IWebElement;
            return webElement == null || webElement.Displayed;
        }

        /// <summary>
        /// Gets the element locator.
        /// </summary>
        /// <returns>The most important locator attribute.</returns>
        private static By GetElementLocator()
        {
            ElementLocatorAttribute attribute;

            return typeof(TChildElement).TryGetAttribute(out attribute)
                       ? SeleniumPageBuilder.GetElementLocators(attribute).FirstOrDefault()
                       : null;
        }
     }
}