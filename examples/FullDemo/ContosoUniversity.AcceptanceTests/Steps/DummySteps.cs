using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using TechTalk.SpecFlow;

namespace ContosoUniversity.AcceptanceTests.Steps
{
	/// <summary>
	/// A dummy binding class to import the CodedUI assembly, temporary until this is fixed.
	/// </summary>
	[Binding]
	public class DummySteps
	{
		/// <summary>
		/// Whens the I do nothing.
		/// </summary>
		[When("I do nothing")]
		public void WhenIDoNothing()
		{
			SpecBind.CodedUI.PageBuilder.CreateElement<HtmlDocument, HtmlDocument>(null);
		}
	}
}