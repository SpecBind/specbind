// <copyright file="SeleniumListElementWrapper.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium
{
    using System;

    using OpenQA.Selenium;

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
        private Func<IWebDriver, Action<object>, object> builderFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumListElementWrapper{TElement, TChildElement}"/> class.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        public SeleniumListElementWrapper(TElement parentElement)
            : base(parentElement)
        {
            var builder = new SeleniumPageBuilder();
            this.builderFunc = builder.CreatePage(typeof(TChildElement));

            this.ValidateElementExists = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the element exists.
        /// </summary>
        /// <value>
        /// <c>true</c> if the class should validate element exists; otherwise, <c>false</c>.
        /// </value>
        public bool ValidateElementExists { get; set; }

        /// <summary>
        /// Creates the element.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="index">The index.</param>
        /// <returns>The created child element.</returns>
        protected override TChildElement CreateElement(TElement parentElement, int index)
        {
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
            return !ValidateElementExists;
        }
     }
}