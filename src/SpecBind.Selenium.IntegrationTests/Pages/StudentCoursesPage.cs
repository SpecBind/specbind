// <copyright file="StudentCoursesPage.cs">
//   Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.IntegrationTests.Pages
{
    using OpenQA.Selenium;

    using SpecBind.Pages;

    /// <summary>
    /// The student courses page
    /// </summary>
    [PageNavigation("/Student/Courses")]
    public class StudentCoursesPage
    {
        /// <summary>
        /// Gets or sets the student courses.
        /// </summary>
        /// <value>
        /// The student courses.
        /// </value>
		[ElementLocator(Id = "studentList")]
        public IElementList<IWebElement, StudentCourseItem> StudentCourses { get; set; }

        /// <summary>
        /// A class that represents a student's course
        /// </summary>
        /// <seealso cref="SpecBind.Selenium.WebElement" />
		[ElementLocator(Name = "studentCourseItem")]
        public class StudentCourseItem : WebElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StudentCourseItem"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public StudentCourseItem(ISearchContext parent) : base(parent)
            {
            }

            /// <summary>
            /// Gets or sets the full name of the student.
            /// </summary>
            /// <value>
            /// The full name of the student.
            /// </value>
			[ElementLocator(Name = "studentFullName")]
            public IWebElement StudentFullName { get; set; }

            /// <summary>
            /// Gets or sets the courses.
            /// </summary>
            /// <value>
            /// The courses.
            /// </value>
			[ElementLocator(Name = "courseList")]
            public IElementList<IWebElement, CourseItem> Courses { get; set; }
        }

        /// <summary>
        /// Represents an item in the course.
        /// </summary>
        /// <seealso cref="SpecBind.Selenium.WebElement" />
		[ElementLocator(Name = "courseItem")]
        public class CourseItem : WebElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CourseItem"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public CourseItem(ISearchContext parent) : base(parent)
            {
            }

            /// <summary>
            /// Gets or sets the course title.
            /// </summary>
            /// <value>
            /// The course title.
            /// </value>
			[ElementLocator(Name = "courseTitle")]
            public IWebElement CourseTitle { get; set; }
        }
    }
}