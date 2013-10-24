// <copyright file="CreateCoursePage.cs">
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
    [PageNavigation("/Course/Create")]
    public class CreateCoursePage : HtmlDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlDocument" /> class by using the provided parent control.
        /// </summary>
        /// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
        public CreateCoursePage(UITestControl parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Gets or sets the department combo box.
        /// </summary>
        /// <value>The the department combo box.</value>
        [ElementLocator(Id = "DepartmentID")]
        public HtmlComboBox Department { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [ElementLocator(Id = "Description")]
        public HtmlTextArea Description { get; set; }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>The number.</value>
        [ElementLocator(Id = "CourseID")]
        public HtmlEdit Number { get; set; }

        /// <summary>
        /// Gets or sets the is popular.
        /// </summary>
        /// <value>The is popular.</value>
        [ElementLocator(Id = "IsPopular")]
        public HtmlCheckBox Popular { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [ElementLocator(Id = "Title")]
        public HtmlEdit CourseTitle { get; set; }

        /// <summary>
        /// Gets or sets the fake property used for testing.
        /// </summary>
        /// <value>The fake property used for testing.</value>
        [ElementLocator(Id = "foo")]
        public HtmlEdit Foo { get; set; }
    }
}