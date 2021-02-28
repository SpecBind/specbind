// <copyright file="InstructorSearchPage.cs" company="">
//     Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.IntegrationTests.Pages
{
    using SpecBind.Pages;

    /// <summary>
    /// The instructors page.
    /// </summary>
    [PageNavigation("/Instructor")]
    public class InstructorSearchPage
    {
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        [ElementLocator(Id = "resultsGrid")]
        public SeleniumTableDriver ResultsGrid { get; set; }
    }
}