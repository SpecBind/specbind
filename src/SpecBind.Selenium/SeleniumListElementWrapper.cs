// <copyright file="SeleniumListElementWrapper.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using OpenQA.Selenium;

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
        private readonly Func<ISearchContext, Action<object>, object> builderFunc;
        private readonly By locator;

        private ReadOnlyCollection<IWebElement> itemCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumListElementWrapper{TElement, TChildElement}"/> class.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        public SeleniumListElementWrapper(TElement parentElement)
            : base(parentElement)
        {
            var builder = new SeleniumPageBuilder();
            this.builderFunc = builder.CreatePage(typeof(TChildElement));
            this.locator = GetElementLocator();
        }

        /// <summary>
        /// Creates the element.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="index">The index.</param>
        /// <returns>The created child element.</returns>
        protected override TChildElement CreateElement(TElement parentElement, int index)
        {
            if (this.locator != null)
            {
                if (this.itemCollection == null)
                {
                    this.itemCollection = parentElement.FindElements(this.locator);
                }

                var element = this.itemCollection.ElementAt(index);
                if (element != null)
                {
                    return (TChildElement)this.builderFunc(element, null);
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
            return true;
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