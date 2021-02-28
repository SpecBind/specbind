// <copyright file="GoogleHomePage.cs" company="">
//     Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium.IntegrationTests.Pages
{
    using OpenQA.Selenium;

    using SpecBind.Pages;

    /// <summary>
    /// The page model for Google
    /// </summary>
    [PageNavigation("http://www.google.com", IsAbsoluteUrl = true)]
    public class GoogleHomePage
    {
        /// <summary>
        /// Gets or sets the search box.
        /// </summary>
        /// <value>The search box.</value>
        [ElementLocator(Name = "q")]
        public IWebElement SearchBox { get; set; }
    }
}