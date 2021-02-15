// <copyright file="TempWebElement.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using OpenQA.Selenium;

    /// <summary>
    /// Temp Web Element.
    /// </summary>
    /// <seealso cref="SpecBind.Selenium.WebElement" />
    public class TempWebElement : WebElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TempWebElement"/> class.
        /// </summary>
        /// <param name="searchContext">The driver used to search for elements.</param>
        public TempWebElement(ISearchContext searchContext)
            : base(searchContext)
        {
            this.Cache = false;
        }
    }
}