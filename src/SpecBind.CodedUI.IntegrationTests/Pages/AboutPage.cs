// <copyright file="AboutPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.CodedUI.IntegrationTests.Pages
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

    using SpecBind.Pages;

    /// <summary>
    /// The about page
    /// </summary>
    [PageNavigation("/Home/About")]
    public class AboutPage : HtmlDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutPage" /> class by using the provided parent control.
        /// </summary>
        /// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
        public AboutPage(UITestControl parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Gets or sets the enrollment table.
        /// </summary>
        /// <value>The enrollment table.</value>
        [ElementLocator(Id = "studentTable")]
        public HtmlTable EnrollmentTable { get; set; }
    }
}