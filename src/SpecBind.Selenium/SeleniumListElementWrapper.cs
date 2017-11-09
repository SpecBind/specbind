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
        private readonly Lazy<IUriHelper> uriHelper;
        private readonly Lazy<Func<ISearchContext, IBrowser, Lazy<IUriHelper>, Action<object>, object>> builderFunc;
        private readonly Lazy<By> locator;

        private ReadOnlyCollection<IWebElement> itemCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumListElementWrapper{TElement, TChildElement}" /> class.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="browser">The browser.</param>
        /// <param name="uriHelper">The URI helper.</param>
        public SeleniumListElementWrapper(TElement parentElement, IBrowser browser, Lazy<IUriHelper> uriHelper)
            : base(parentElement, browser)
        {
            this.uriHelper = uriHelper;
            this.builderFunc = new Lazy<Func<ISearchContext, IBrowser, Lazy<IUriHelper>, Action<object>, object>>(() => this.CreateBuilderFunction(uriHelper));
            this.locator = new Lazy<By>(this.GetElementLocator);
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
            if (this.locator.Value != null)
            {
                if (this.itemCollection == null)
                {
                    this.itemCollection = this.BuildItemCollection(parentElement);
                }

                var element = (index > 0 && index <= this.itemCollection.Count) ? this.itemCollection[index - 1] : null;
                if (element != null)
                {
                    return this.CreateChildElement(browser, parentElement, element);
                }
            }

            return null;
        }

        /// <summary>
        /// Builds the item collection from the parent.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <returns>The created collection.</returns>
        protected virtual ReadOnlyCollection<IWebElement> BuildItemCollection(TElement parentElement)
        {
            return parentElement.FindElements(this.locator.Value);
        }

        /// <summary>
        /// Creates the builder function.
        /// </summary>
        /// <param name="uriHelper">The URI helper.</param>
        /// <returns>The created builder function.</returns>
        protected Func<ISearchContext, IBrowser, Lazy<IUriHelper>, Action<object>, object> CreateBuilderFunction(Lazy<IUriHelper> uriHelper)
        {
            var builder = new SeleniumPageBuilder(uriHelper);
            return builder.CreatePage(typeof(TChildElement));
        }

        /// <summary>
        /// Creates the child element.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="element">The element.</param>
        /// <returns>The created child element.</returns>
        protected virtual TChildElement CreateChildElement(IBrowser browser, TElement parentElement, IWebElement element)
        {
            var childElement = (TChildElement)this.builderFunc.Value(element, browser, this.uriHelper, null);

            var webElement = childElement as WebElement;
            if (webElement != null)
            {
                webElement.CloneNativeElement(element);
            }

            return childElement;
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
        protected virtual By GetElementLocator()
        {
            ElementLocatorAttribute attribute;

            return typeof(TChildElement).TryGetAttribute(out attribute)
                       ? LocatorBuilder.GetElementLocators(attribute).FirstOrDefault()
                       : null;
        }
     }
}