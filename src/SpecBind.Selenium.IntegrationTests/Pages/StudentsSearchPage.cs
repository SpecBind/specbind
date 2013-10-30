// <copyright file="StudentsSearchPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.IntegrationTests.Pages
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    using SpecBind.Pages;

    /// <summary>
    /// The students search page model
    /// </summary>
    [PageNavigation("/Student")]
    public class StudentsSearchPage
    {
        /// <summary>
        /// Gets or sets the find by name search box.
        /// </summary>
        /// <value>The the find by name search box.</value>
        [ElementLocator(Id = "SearchString")]
        [FindsBy(How = How.Id, Using = "SearchString")]
        public IWebElement FindByName { get; set; }

        /// <summary>
        /// Gets or sets the Search button.
        /// </summary>
        /// <value>The Search button.</value>
        [ElementLocator(Id = "searchButton", Type = "submit")]
        [FindsBy(How = How.Id, Using = "searchButton")]
        public IWebElement Search { get; set; }

        /// <summary>
        /// Gets or sets the results grid.
        /// </summary>
        /// <value>The results grid.</value>
        [ElementLocator(Id = "resultsGrid")]
        [FindsBy(How = How.Id, Using = "resultsGrid")]
        public IElementList<IWebElement, PersonTableRow> ResultsGrid { get; set; }

        /// <summary>
        /// A nested class to represent the result row
        /// </summary>
        [ElementLocator(Id = "resultRow")]
        public class PersonTableRow
        {
            /// <summary>
            /// Gets or sets the first name cell.
            /// </summary>
            /// <value>The first name cell.</value>
            [ElementLocator(Id = "firstName")]
            [FindsBy(How = How.Id, Using = "firstName")]
            public IWebElement FirstName { get; set; }

            /// <summary>
            /// Gets or sets the last name cell.
            /// </summary>
            /// <value>The last name cell.</value>
            [ElementLocator(Id = "lastName")]
            [FindsBy(How = How.Id, Using = "lastName")]
            public IWebElement LastName { get; set; }

            /// <summary>
            /// Gets or sets the enrollment date cell.
            /// </summary>
            /// <value>
            /// The enrollment date cell.
            /// </value>
            [ElementLocator(Id = "enrollmentDate")]
            [FindsBy(How = How.Id, Using = "enrollmentDate")]
            public IWebElement EnrollmentDate { get; set; }
        }
    }
}