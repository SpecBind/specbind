// <copyright file="StudentsSearchPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.CodedUI.IntegrationTests.Pages
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

    using SpecBind.Pages;

    /// <summary>
	/// The students search page model
	/// </summary>
	[PageNavigation("/Student")]
	public class StudentsSearchPage : HtmlDocument
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StudentsSearchPage" /> class by using the provided parent control.
		/// </summary>
		/// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
		public StudentsSearchPage(UITestControl parent) : base(parent)
		{
		}

		/// <summary>
		/// Gets or sets the find by name search box.
		/// </summary>
		/// <value>The the find by name search box.</value>
		[ElementLocator(Id = "SearchString")]
		public HtmlEdit FindByName { get; set; }

		/// <summary>
		/// Gets or sets the Search button.
		/// </summary>
		/// <value>The Search button.</value>
		[ElementLocator(Id = "searchButton", Type = "submit")]
		public HtmlInputButton Search { get; set; }

		/// <summary>
		/// Gets or sets the results grid.
		/// </summary>
		/// <value>The results grid.</value>
		[ElementLocator(Id = "resultsGrid")]
		public IElementList<HtmlTable, PersonTableRow> ResultsGrid { get; set; }

		/// <summary>
		/// A nested class to represent the result row
		/// </summary>
		[ElementLocator(Id = "resultRow")]
		public class PersonTableRow : HtmlRow
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="PersonTableRow" /> class.
			/// </summary>
			/// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
			public PersonTableRow(UITestControl parent) : base(parent)
			{
			}

			/// <summary>
			/// Gets or sets the first name cell.
			/// </summary>
			/// <value>The first name cell.</value>
			[ElementLocator(Id = "firstName")]
			public HtmlCell FirstName { get; set; }

			/// <summary>
			/// Gets or sets the last name cell.
			/// </summary>
			/// <value>The last name cell.</value>
			[ElementLocator(Id = "lastName")]
			public HtmlCell LastName { get; set; }

			/// <summary>
			/// Gets or sets the enrollment date cell.
			/// </summary>
			/// <value>
			/// The enrollment date cell.
			/// </value>
			[ElementLocator(Id = "enrollmentDate")]
			public HtmlCell EnrollmentDate { get; set; }
		}
	}
}