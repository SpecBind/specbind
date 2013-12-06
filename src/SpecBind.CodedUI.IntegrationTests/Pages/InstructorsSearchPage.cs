// <copyright file="InstructorsSearchPage.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI.IntegrationTests.Pages
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

    using SpecBind.Pages;

    /// <summary>
    /// The instructors page.
    /// </summary>
    [PageNavigation("/Instructor")]
    public class InstructorsSearchPage : HtmlDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlDocument" /> class by using the provided parent control.
        /// </summary>
        /// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
        public InstructorsSearchPage(UITestControl parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        [ElementLocator(Id = "resultsGrid")]
        public CodedUITableDriver ResultsGrid { get; set; }
    }
}