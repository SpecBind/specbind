// <copyright file="JQueryClickingPage.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
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
        [ElementLocator(CssSelector = "div#tabs li:nth-child(2) > a")]
        public IWebElement SecondTab { get; set; }

        [ElementLocator(CssSelector = "li.ui-state-active > a")]
        public IWebElement ActiveTab { get; set; }
    }
}