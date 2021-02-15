// <copyright file="StudentDetailPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.CodedUI.IntegrationTests.Pages
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

    using SpecBind.Pages;

    /// <summary>
    /// The student detail page
    /// </summary>
    [PageNavigation("/Student/Details/[0-9]+", UrlTemplate = "/Student/Details/{Id}")]
    public class StudentDetailPage : HtmlDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StudentDetailPage" /> class by using the provided parent control.
        /// </summary>
        /// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
        public StudentDetailPage(UITestControl parent) : base(parent)
        {
        }

        /// <summary>
        /// Gets or sets the details link.
        /// </summary>
        /// <value>The details link.</value>
        [ElementLocator(Id = "detailsLink")]
        public HtmlHyperlink Details { get; set; }

        /// <summary>
        /// Gets or sets the first name cell.
        /// </summary>
        /// <value>The first name cell.</value>
        [ElementLocator(Id = "firstName")]
        public HtmlDiv FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name cell.
        /// </summary>
        /// <value>The last name cell.</value>
        [ElementLocator(Id = "lastName")]
        public HtmlDiv LastName { get; set; }
    }
}