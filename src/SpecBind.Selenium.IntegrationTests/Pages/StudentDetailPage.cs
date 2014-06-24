// <copyright file="StudentDetailPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.IntegrationTests.Pages
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    using SpecBind.Pages;

    /// <summary>
    /// The student detail page
    /// </summary>
    [PageNavigation("/Student/Details/[0-9]+", UrlTemplate = "/Student/Details/{Id}")]
    public class StudentDetailPage
    {
        /// <summary>
        /// Gets or sets the details link.
        /// </summary>
        /// <value>The details link.</value>
        [ElementLocator(Id = "detailsLink")]
        [FindsBy(How = How.Id, Using = "detailsLink")]
        public IWebElement Details { get; set; }

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
        /// Gets the full name based on other fields as a string.
        /// </summary>
        /// <value>The full name.</value>
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName.Text, this.LastName.Text);
            }
        }
    }
}