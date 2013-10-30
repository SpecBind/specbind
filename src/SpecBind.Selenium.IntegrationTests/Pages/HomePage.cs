// // <copyright file="HomePage.cs">
// //    Copyright © 2013 Dan Piessens.  All rights reserved.
// // </copyright>
namespace SpecBind.Selenium.IntegrationTests.Pages
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    using SpecBind.Pages;

    /// <summary>
    /// The main home page model
    /// </summary>
    [PageNavigation("/")]
    public class HomePage
    {
        /// <summary>
        /// Gets or sets the about link button.
        /// </summary>
        /// <value>The about link button.</value>
        [ElementLocator(Id = "aboutLink")]
        [FindsBy(How = How.Id, Using = "aboutLink")]
        public IWebElement About { get; set; }

        /// <summary>
        /// Gets or sets the courses link button.
        /// </summary>
        /// <value>The courses link button.</value>
        [ElementLocator(Id = "coursesLink")]
        [FindsBy(How = How.Id, Using = "coursesLink")]
        public IWebElement Courses { get; set; }

        /// <summary>
        /// Gets or sets the login link button.
        /// </summary>
        /// <value>The login link button.</value>
        [ElementLocator(Id = "loginLink")]
        [FindsBy(How = How.Id, Using = "loginLink")]
        public IWebElement LogOn { get; set; }

        /// <summary>
        /// Gets or sets the students link button.
        /// </summary>
        /// <value>The students link button.</value>
        [ElementLocator(Id = "studentsLink")]
        [FindsBy(How = How.Id, Using = "studentsLink")]
        public IWebElement Students { get; set; }
    }
}