﻿// <copyright file="HomePage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.CodedUI.IntegrationTests.Pages
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

    using SpecBind.Pages;

    /// <summary>
	/// The main home page model
	/// </summary>
	[PageNavigation("/")]
    public class HomePage : HtmlDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomePage" /> class by using the provided parent control.
        /// </summary>
        /// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
        public HomePage(UITestControl parent) : base(parent)
        {
        }

        /// <summary>
        /// Gets or sets the about link button.
        /// </summary>
        /// <value>The about link button.</value>
        [ElementLocator(Id = "aboutLink")]
        public HtmlHyperlink About { get; set; }

        /// <summary>
        /// Gets or sets the courses link button.
        /// </summary>
        /// <value>The courses link button.</value>
        [ElementLocator(Id = "coursesLink")]
        [VirtualProperty(Name = "CoursesLink", Attribute = "Href")]
        public HtmlHyperlink Courses { get; set; }

        /// <summary>
        /// Gets or sets the departments link button.
        /// </summary>
        /// <value>The departments link button.</value>
        [ElementLocator(Id = "departmentsLink")]
        public HtmlHyperlink Departments { get; set; }

        /// <summary>
        /// Gets or sets the instructors link button.
        /// </summary>
        /// <value>The instructors link button.</value>
        [ElementLocator(Id = "instructorsLink")]
        public HtmlHyperlink Instructors { get; set; }

        /// <summary>
        /// Gets or sets the login link button.
        /// </summary>
        /// <value>The login link button.</value>
        [ElementLocator(Id = "loginLink")]
        public HtmlHyperlink LogOn { get; set; }

        /// <summary>
        /// Gets or sets the new information link button.
        /// </summary>
        /// <value>The new information link button.</value>
        [ElementLocator(Id = "newInfoLink")]
        public HtmlHyperlink NewInformation { get; set; }

        /// <summary>
        /// Gets or sets the students link button.
        /// </summary>
        /// <value>The students link button.</value>
        [ElementLocator(Id = "studentsLink")]
        public HtmlHyperlink Students { get; set; }
    }
}