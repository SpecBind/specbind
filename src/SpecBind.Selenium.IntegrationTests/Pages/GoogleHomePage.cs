// <copyright file="GoogleHomePage.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium.IntegrationTests.Pages
{
    using OpenQA.Selenium;

    using SpecBind.Pages;

    [PageNavigation("http://www.google.com", IsAbsoluteUrl = true)]
    public class GoogleHomePage
    {
        [ElementLocator(Name = "q")]
        public IWebElement SearchBox { get; set; }    
    }
}