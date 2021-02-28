// <copyright file="AboutPage.cs" company="">
//     Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.IntegrationTests.Pages
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    using SpecBind.Pages;

    /// <summary>
    /// The about page
    /// </summary>
    [PageNavigation("/Home/About")]
    public class AboutPage
    {
        /// <summary>
        /// Gets or sets the enrollment table.
        /// </summary>
        /// <value>The enrollment table.</value>
        [ElementLocator(Id = "studentTable")]
        [FindsBy(How = How.Id, Using = "studentTable")]
        public IWebElement EnrollmentTable { get; set; }
    }
}