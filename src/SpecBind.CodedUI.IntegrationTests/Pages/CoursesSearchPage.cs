// <copyright file="CoursesSearchPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.CodedUI.IntegrationTests.Pages
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

    using SpecBind.Pages;

    /// <summary>
    /// The courses page model
    /// </summary>
    [PageNavigation("/Course")]
    [PageAlias("Courses")]
    public class CoursesSearchPage : HtmlDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoursesSearchPage" /> class by using the provided parent control.
        /// </summary>
        /// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
        public CoursesSearchPage(UITestControl parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Gets or sets the create new link button.
        /// </summary>
        /// <value>The create new link button.</value>
        [ElementLocator(Id = "createNewCourse")]
        public HtmlHyperlink CreateNew { get; set; }
    }
}