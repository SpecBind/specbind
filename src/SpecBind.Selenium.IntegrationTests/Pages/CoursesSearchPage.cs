// <copyright file="CoursesSearchPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.IntegrationTests.Pages
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    using SpecBind.Pages;

    /// <summary>
    /// The courses page model
    /// </summary>
    [PageNavigation("/Course")]
    [PageAlias("Courses")]
    public class CoursesSearchPage
    {
        /// <summary>
        /// Gets or sets the create new link button.
        /// </summary>
        /// <value>The create new link button.</value>
        [ElementLocator(Id = "createNewCourse")]
        [FindsBy(How = How.Id, Using = "createNewCourse")]
        public IWebElement CreateNew { get; set; }
    }
}