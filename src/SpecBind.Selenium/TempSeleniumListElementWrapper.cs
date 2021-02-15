// <copyright file="TempSeleniumListElementWrapper.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Selenium
{
    using OpenQA.Selenium;

    using SpecBind.BrowserSupport;

    /// <summary>
    /// A list element wrapper collection for temporary Selenium elements.
    /// </summary>
    /// <typeparam name="TElement">The type of the parent element.</typeparam>
    /// <typeparam name="TChildElement">The type of the child element.</typeparam>
    public class TempSeleniumListElementWrapper<TElement, TChildElement> : SeleniumListElementWrapper<TElement, TChildElement>
        where TElement : IWebElement
        where TChildElement : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TempSeleniumListElementWrapper{TElement, TChildElement}" /> class.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="browser">The browser.</param>
        public TempSeleniumListElementWrapper(TElement parentElement, IBrowser browser)
            : base(parentElement, browser)
        {
            this.Cache = false;
        }
    }
}