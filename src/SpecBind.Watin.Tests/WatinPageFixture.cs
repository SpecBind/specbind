// <copyright file="WatinPageFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Watin.Tests
{
	using System.Collections.Specialized;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using SpecBind.Watin;

	using WatiN.Core;
	using WatiN.Core.Native;

	/// <summary>
	///     A unit test fixture for the <see cref="WatinPage" /> class.
	/// </summary>
	[TestClass]
	public class WatinPageFixture
	{
		/// <summary>
		///    Tests the TestGetNativePage method.
		/// </summary>
		[TestMethod]
		public void TestGetNativePage()
		{
			var page = new TestPage();

			var watinPage = new WatinPage(page);
			var result = watinPage.GetNativePage<TestPage>();

			Assert.IsNotNull(result);
		}

		/// <summary>
		///    Tests the ElementExistsCheck method where the element does not exist.
		/// </summary>
		[TestMethod]
		public void TestElementExistsCheckElementDoesNotExist()
		{
			var domContainer = new Mock<DomContainer>(MockBehavior.Loose);

			var elementFinder = new Mock<INativeElement>(MockBehavior.Strict);
			elementFinder.SetupGet(e => e.TagName).Returns("div");
			elementFinder.Setup(e => e.IsElementReferenceStillValid()).Returns(false);

			var page = new TestPage { DisplayArea = new Div(domContainer.Object, elementFinder.Object) };

			var watinPage = new WatinPage(page);

			var result = watinPage.ElementExistsCheck(page.DisplayArea);

			Assert.IsFalse(result);
		}

		/// <summary>
		///    Tests the ElementExistsCheck method where the element does exist.
		/// </summary>
		[TestMethod]
		public void TestElementExistsCheckElementExists()
		{
			var domContainer = new Mock<DomContainer>(MockBehavior.Loose);

			var elementFinder = new Mock<INativeElement>(MockBehavior.Strict);
			elementFinder.SetupGet(e => e.TagName).Returns("div");
			elementFinder.Setup(e => e.IsElementReferenceStillValid()).Returns(true);

			var page = new TestPage { DisplayArea = new Div(domContainer.Object, elementFinder.Object) };

			var watinPage = new WatinPage(page);

			var result = watinPage.ElementExistsCheck(page.DisplayArea);

			Assert.IsTrue(result);
		}

		/// <summary>
		///    Tests the ElementEnabledCheck method where the element is enabled.
		/// </summary>
		[TestMethod]
		public void TestElementEnabledCheckElementIsEnabled()
		{
			var domContainer = new Mock<DomContainer>(MockBehavior.Loose);

			var elementFinder = new Mock<INativeElement>(MockBehavior.Strict);
			elementFinder.SetupGet(e => e.TagName).Returns("div");
			elementFinder.Setup(e => e.IsElementReferenceStillValid()).Returns(true);
			elementFinder.Setup(e => e.GetAttributeValue("disabled")).Returns("false");

			var page = new TestPage { DisplayArea = new Div(domContainer.Object, elementFinder.Object) };

			var watinPage = new WatinPage(page);

			var result = watinPage.ElementEnabledCheck(page.DisplayArea);

			Assert.IsTrue(result);
		}

		/// <summary>
		///     Tests the ClickElement method with an element that exists and can be clicked.
		/// </summary>
		[TestMethod]
		public void TestClickElement()
		{
			var domContainer = new Mock<DomContainer>(MockBehavior.Loose);

			var elementFinder = new Mock<INativeElement>(MockBehavior.Strict);
			elementFinder.SetupGet(e => e.TagName).Returns("div");
			elementFinder.Setup(e => e.IsElementReferenceStillValid()).Returns(true);
			elementFinder.Setup(e => e.GetAttributeValue("disabled")).Returns("false");
			elementFinder.Setup(e => e.FireEvent("onclick", null));

			var page = new TestPage { DisplayArea = new Div(domContainer.Object, elementFinder.Object) };

			var watinPage = new WatinPage(page);

			var result = watinPage.ClickElement(page.DisplayArea);

			Assert.IsTrue(result);

			domContainer.VerifyAll();
			elementFinder.VerifyAll();
		}

		/// <summary>
		///     Tests the GetElementText method.
		/// </summary>
		[TestMethod]
		public void TestGetElementText()
		{
			var domContainer = new Mock<DomContainer>(MockBehavior.Loose);

			var elementFinder = new Mock<INativeElement>(MockBehavior.Strict);
			elementFinder.SetupGet(e => e.TagName).Returns("input");
			elementFinder.Setup(e => e.GetAttributeValue("type")).Returns("text");
			elementFinder.Setup(e => e.GetAttributeValue("value")).Returns("Hello");

			var page = new TestPage { Name = new TextField(domContainer.Object, elementFinder.Object) };

			var watinPage = new WatinPage(page);

			var result = watinPage.GetElementText(page.Name);

			Assert.AreEqual("Hello", result);

			elementFinder.VerifyAll();
			domContainer.VerifyAll();
		}

		/// <summary>
		///     Tests the GetPageFillMethod with an unsupported type.
		/// </summary>
		[TestMethod]
		public void TestGetPageFillMethodUnsupportedType()
		{
			var page = new TestPage();
			var watinPage = new WatinPage(page);

			var fillMethod = watinPage.GetPageFillMethod(typeof(Div));

			Assert.IsNull(fillMethod);
		}

		/// <summary>
		///     Tests the GetPageFillMethod with a textbox.
		/// </summary>
		[TestMethod]
		public void TestGetPageFillMethodTextbox()
		{
			var domContainer = new Mock<DomContainer>(MockBehavior.Loose);
			domContainer.Setup(d => d.WaitForComplete(30));

			var elementFinder = new Mock<INativeElement>(MockBehavior.Strict);
			elementFinder.SetupGet(e => e.TagName).Returns("input");
			elementFinder.Setup(e => e.GetAttributeValue("type")).Returns("text");

			elementFinder.Setup(e => e.IsElementReferenceStillValid()).Returns(true);
			elementFinder.Setup(e => e.GetAttributeValue("disabled")).Returns("false");
			elementFinder.Setup(e => e.GetAttributeValue("readOnly")).Returns("false");
			elementFinder.Setup(e => e.SetFocus());
			elementFinder.Setup(e => e.FireEvent("onFocus", null));
			elementFinder.Setup(e => e.Select());
			elementFinder.Setup(e => e.SetAttributeValue("value", string.Empty));
			elementFinder.Setup(e => e.GetAttributeValue("maxLength")).Returns(string.Empty);
			elementFinder.Setup(e => e.FireEvent("onKeyDown", It.IsAny<NameValueCollection>()));
			elementFinder.Setup(e => e.FireEvent("onKeyPress", It.IsAny<NameValueCollection>()));
			elementFinder.Setup(e => e.FireEvent("onKeyUp", It.IsAny<NameValueCollection>()));
			elementFinder.Setup(e => e.FireEvent("onChange", null));
			elementFinder.Setup(e => e.FireEvent("onBlur", null));

			var page = new TestPage { Name = new TextField(domContainer.Object, elementFinder.Object) };

			var watinPage = new WatinPage(page);

			var fillMethod = watinPage.GetPageFillMethod(typeof(TextField));

			fillMethod(page.Name, "Hello");

			elementFinder.VerifyAll();
			domContainer.VerifyAll();
		}

		/// <summary>
		///     Tests the GetPageFillMethod with a checkbox.
		/// </summary>
		[TestMethod]
		public void TestGetPageFillMethodCheckbox()
		{
			var domContainer = new Mock<DomContainer>(MockBehavior.Loose);
			domContainer.Setup(d => d.WaitForComplete(30));

			var elementFinder = new Mock<INativeElement>(MockBehavior.Strict);
			elementFinder.SetupGet(e => e.TagName).Returns("input");
			elementFinder.Setup(e => e.GetAttributeValue("type")).Returns("checkbox");

			elementFinder.Setup(e => e.IsElementReferenceStillValid()).Returns(true);
			elementFinder.Setup(e => e.GetAttributeValue("disabled")).Returns("false");
			elementFinder.Setup(e => e.GetAttributeValue("checked")).Returns("false");
			elementFinder.Setup(e => e.FireEvent("onClick", null));

			var page = new TestPage { IsPerson = new CheckBox(domContainer.Object, elementFinder.Object) };

			var watinPage = new WatinPage(page);

			var fillMethod = watinPage.GetPageFillMethod(typeof(CheckBox));

			fillMethod(page.IsPerson, "true");

			elementFinder.VerifyAll();
			domContainer.VerifyAll();
		}

		/// <summary>
		///     Tests the GetPageFillMethod with a radio button.
		/// </summary>
		[TestMethod]
		public void TestGetPageFillMethodRadioButton()
		{
			var domContainer = new Mock<DomContainer>(MockBehavior.Loose);
			domContainer.Setup(d => d.WaitForComplete(30));

			var elementFinder = new Mock<INativeElement>(MockBehavior.Strict);
			elementFinder.SetupGet(e => e.TagName).Returns("input");
			elementFinder.Setup(e => e.GetAttributeValue("type")).Returns("radio");

			elementFinder.Setup(e => e.IsElementReferenceStillValid()).Returns(true);
			elementFinder.Setup(e => e.GetAttributeValue("disabled")).Returns("false");
			elementFinder.Setup(e => e.GetAttributeValue("checked")).Returns("false");
			elementFinder.Setup(e => e.FireEvent("onClick", null));


			var page = new TestPage { Option1 = new RadioButton(domContainer.Object, elementFinder.Object) };

			var watinPage = new WatinPage(page);

			var fillMethod = watinPage.GetPageFillMethod(typeof(RadioButton));

			fillMethod(page.Option1, "true");

			elementFinder.VerifyAll();
			domContainer.VerifyAll();
		}

		/// <summary>
		///     Tests the GetPageFillMethod with a select list.
		/// </summary>
		[TestMethod]
		public void TestGetPageFillMethodSelectList()
		{
			var page = new TestPage();

			var watinPage = new WatinPage(page);

			var fillMethod = watinPage.GetPageFillMethod(typeof(SelectList));

			//Simply check it's not null, select is too complicate to validate actions.
			Assert.IsNotNull(fillMethod);
		}
	}
}