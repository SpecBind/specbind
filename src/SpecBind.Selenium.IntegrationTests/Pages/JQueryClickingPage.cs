// <copyright file="JQueryClickingPage.cs" company="">
//     Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium.IntegrationTests.Pages
{
    using OpenQA.Selenium;

    using SpecBind.Pages;

    /// <summary>
    /// A page for JQuery tabs testing.
    /// </summary>
    [PageNavigation("http://jqueryui.com/resources/demos/tabs/default.html", IsAbsoluteUrl = true)]
    public class JQueryClickingPage
    {
        /// <summary>
        /// Gets or sets the second tab.
        /// </summary>
        /// <value>The second tab.</value>
        [ElementLocator(CssSelector = "div#tabs li:nth-child(2) > a")]
        public IWebElement SecondTab { get; set; }

        /// <summary>
        /// Gets or sets the active tab.
        /// </summary>
        /// <value>The active tab.</value>
        [ElementLocator(CssSelector = "li.ui-state-active > a")]
        public IWebElement ActiveTab { get; set; }
    }
}