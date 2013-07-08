using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using SpecBind.Pages;

namespace ContosoUniversity.AcceptanceTests.Pages
{
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
		/// Gets or sets the students link button.
		/// </summary>
		/// <value>The students link button.</value>
		[ElementLocator(Id = "studentsLink")]
		public HtmlHyperlink Students { get; set; }
	}
}