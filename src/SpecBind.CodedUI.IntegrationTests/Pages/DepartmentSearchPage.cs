// <copyright file="DepartmentSearchPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.CodedUI.IntegrationTests.Pages
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

    using SpecBind.Pages;

    /// <summary>
    /// The department search page model
    /// </summary>
    [PageNavigation("/Department")]
    public class DepartmentSearchPage : HtmlDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentSearchPage" /> class by using the provided parent control.
        /// </summary>
        /// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
        public DepartmentSearchPage(UITestControl parent) : base(parent)
        {
        }

        /// <summary>
        /// Gets or sets the results grid.
        /// </summary>
        /// <value>The results grid.</value>
        [ElementLocator(Id = "departmentList")]
        public IElementList<HtmlDiv, DepartmentItem> ResultsGrid { get; set; }

        /// <summary>
        /// A nested class to represent the result row
        /// </summary>
        [ElementLocator(Id = "departmentItem")]
        public class DepartmentItem : HtmlDiv
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DepartmentItem" /> class.
            /// </summary>
            /// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
            public DepartmentItem(UITestControl parent) : base(parent)
            {
            }

            /// <summary>
            /// Gets or sets the administrator div.
            /// </summary>
            /// <value>The administrator div.</value>
            [ElementLocator(Id = "administrator")]
            public HtmlDiv Administrator { get; set; }

            /// <summary>
            /// Gets or sets the department name div.
            /// </summary>
            /// <value>The department name div.</value>
            [ElementLocator(Id = "name")]
            public HtmlDiv DepartmentName { get; set; }

            /// <summary>
            /// Gets or sets the budget div.
            /// </summary>
            /// <value>The budget div.</value>
            [ElementLocator(Id = "budget")]
            public HtmlDiv Budget { get; set; }

            /// <summary>
            /// Gets or sets the start date cell.
            /// </summary>
            /// <value>
            /// The start date cell.
            /// </value>
            [ElementLocator(Id = "startDate")]
            public HtmlDiv StartDate { get; set; }
        }
    }
}