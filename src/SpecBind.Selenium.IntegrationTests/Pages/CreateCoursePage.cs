// <copyright file="CreateCoursePage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.IntegrationTests.Pages
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    using SpecBind.Pages;

    /// <summary>
    /// The courses page model
    /// </summary>
    [PageNavigation("/Course/Create")]
    public class CreateCoursePage
    {
        /// <summary>
        /// Gets or sets the dash title.
        /// </summary>
        /// <value>The dash title.</value>
        public string DashTitle
        {
            get
            {
                return string.Format("{0}-{1}", this.Number.Text.Trim(), this.CourseTitle.Text.Trim());
            }

            set
            {
                var split = value.Split(new[] { '-' }, 2);
                if (split.Length == 2)
                {
                    this.Number.SendKeys(split[0]);
                    this.CourseTitle.SendKeys(split[1]);
                }
            }
        }

        /// <summary>
        /// Gets or sets the department combo box.
        /// </summary>
        /// <value>The the department combo box.</value>
        [ElementLocator(Id = "DepartmentID")]
        [FindsBy(How = How.Id, Using = "DepartmentID")]
        public IWebElement Department { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [ElementLocator(Id = "Description")]
        [FindsBy(How = How.Id, Using = "Description")]
        public IWebElement Description { get; set; }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>The number.</value>
        [ElementLocator(Id = "CourseID")]
        [FindsBy(How = How.Id, Using = "CourseID")]
        public IWebElement Number { get; set; }

        /// <summary>
        /// Gets or sets the is popular.
        /// </summary>
        /// <value>The is popular.</value>
        [ElementLocator(Id = "IsPopular")]
        [FindsBy(How = How.Id, Using = "IsPopular")]
        public IWebElement Popular { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [ElementLocator(Id = "Title")]
        [FindsBy(How = How.Id, Using = "Title")]
        public IWebElement CourseTitle { get; set; }

        /// <summary>
        /// Gets or sets the fake property used for testing.
        /// </summary>
        /// <value>The fake property used for testing.</value>
        [ElementLocator(Id = "foo")]
        [FindsBy(How = How.Id, Using = "foo")]
        public IWebElement Foo { get; set; }
    }
}