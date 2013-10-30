// <copyright file="LogOnPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.IntegrationTests.Pages
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    using SpecBind.Pages;

    /// <summary>
    /// The main home page model
    /// </summary>
    [PageNavigation("/Account/LogOn")]
    public class LogOnPage
    {
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [ElementLocator(Id = "Password", Type = "PASSWORD")]
        [FindsBy(How = How.Id, Using = "Password")]
        public IWebElement Password { get; set; }
    }
}