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
		[ElementLocator(Id = "studentList")]
		public IElementList<IWebElement, StudentCourseItem> StudentCourses { get; set; }

		[ElementLocator(Name = "studentCourseItem")]
		public class StudentCourseItem : WebElement
		{
			public StudentCourseItem(ISearchContext parent):base(parent)
			{
			}

			[ElementLocator(Name = "studentFullName")]
			public IWebElement StudentFullName { get; set; }

			[ElementLocator(Name = "courseList")]
			public IElementList<IWebElement, CourseItem> Courses { get; set; }
		}

		[ElementLocator(Name = "courseItem")]
		public class CourseItem : WebElement
		{
			public CourseItem(ISearchContext parent):base(parent)
			{
			}

			[ElementLocator(Name = "courseTitle")]
			public IWebElement CourseTitle { get; set; }
		}
	}
}