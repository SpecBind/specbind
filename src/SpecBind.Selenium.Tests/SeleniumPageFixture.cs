// <copyright file="SeleniumPageFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Drawing;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using OpenQA.Selenium;

    using SpecBind.Actions;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the <see cref="SeleniumPage"/> class.
    /// </summary>
    [TestClass]
    public class SeleniumPageFixture
    {
        /// <summary>
        /// Tests the get native page method.
        /// </summary>
        [TestMethod]
        public void TestGetNativePage()
        {
            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.GetNativePage<NativePage>();
            Assert.AreSame(nativePage, result);
        }

        /// <summary>
        /// Tests the get native page type property.
        /// </summary>
        [TestMethod]
        public void TestGetPageType()
        {
            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.PageType;
            Assert.AreEqual(typeof(NativePage), result);
        }

        /// <summary>
        /// Tests the obtaining of elements.
        /// </summary>
        [TestMethod]
        public void TestTryGetCorrectElement()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);

            var nativePage = new NativePage { MyControl = element.Object };
            var page = new SeleniumPage(nativePage, null);


            IPropertyData propertyData;
            var result = page.TryGetElement("MyControl", out propertyData);

            Assert.IsNotNull(nativePage.MyControl);
            Assert.AreEqual(true, result);
            Assert.IsNotNull(propertyData);
        }

        /// <summary>
        /// Tests the get a nested page from an element within the page.
        /// </summary>
        [TestMethod]
        public void TestGetPageFromElement()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.GetPageFromElement(element.Object);

            Assert.IsNotNull(result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the element enabled check returns true when the element is displayed.
        /// </summary>
        [TestMethod]
        public void TestElementExistsCheckWhenDisplayedReturnsTrue()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Displayed).Returns(true);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.ElementExistsCheck(element.Object);

            Assert.AreEqual(true, result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the element enabled check returns false when the element is not displayed.
        /// </summary>
        [TestMethod]
        public void TestElementExistsCheckWhenNotDisplayedReturnsFalse()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Displayed).Returns(false);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.ElementExistsCheck(element.Object);

            Assert.AreEqual(false, result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the element enabled check returns false when a NoSuchElementException is thrown.
        /// </summary>
        [TestMethod]
        public void TestElementExistsCheckWhenNoSuchElementExceptionReturnsFalse()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Displayed).Throws<NoSuchElementException>();

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.ElementExistsCheck(element.Object);

            Assert.AreEqual(false, result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the element enabled check returns false when a ElementNotVisibleException is thrown.
        /// </summary>
        [TestMethod]
        public void TestElementExistsCheckWhenElementNotVisibleExceptionReturnsFalse()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Displayed).Throws<ElementNotVisibleException>();

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.ElementExistsCheck(element.Object);

            Assert.AreEqual(false, result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the element enabled check returns true when the element is displayed and enabled.
        /// </summary>
        [TestMethod]
        public void TestElementEnabledCheckWhenDisplayedAndEnabledReturnsTrue()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Displayed).Returns(true);
            element.SetupGet(e => e.Enabled).Returns(true);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.ElementEnabledCheck(element.Object);

            Assert.AreEqual(true, result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the element enabled check returns false when the element is not displayed.
        /// </summary>
        [TestMethod]
        public void TestElementEnabledCheckWhenNotDisplayedReturnsFalse()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Displayed).Returns(false);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.ElementEnabledCheck(element.Object);

            Assert.AreEqual(false, result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the highlight method draws a border around the element.
        /// </summary>
        [TestMethod]
        public void TestHighlightElementExecutesJavascript()
        {
            const string DefaultStyle = "color: blue;";
            const string JavaScript = "arguments[0].setAttribute(arguments[1], arguments[2])";

            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.Setup(e => e.GetAttribute("style")).Returns(DefaultStyle);

            var item = element.Object;
            var webDriver = new Mock<IWebDriver>(MockBehavior.Strict);
            webDriver.As<IJavaScriptExecutor>().Setup(j => j.ExecuteScript(JavaScript, item, "style", "border: 5px solid red;")).Returns((object)null);
            webDriver.As<IJavaScriptExecutor>().Setup(j => j.ExecuteScript(JavaScript, item, "style", DefaultStyle)).Returns((object)null);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, webDriver.Object);

            page.Highlight(item);
            
            element.VerifyAll();
            webDriver.VerifyAll();
        }

        /// <summary>
        /// Tests the highlight method does not throw an exception if the driver isn't supported.
        /// </summary>
        [TestMethod]
        public void TestHighlightElementWhenDriverNotSupportedDoesNotThowException()
        {
            const string DefaultStyle = "color: blue;";

            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.Setup(e => e.GetAttribute("style")).Returns(DefaultStyle);

            var item = element.Object;
            var webDriver = new Mock<IWebDriver>(MockBehavior.Strict);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, webDriver.Object);

            page.Highlight(item);

            element.VerifyAll();
            webDriver.VerifyAll();
        }

        /// <summary>
        /// Tests the element enabled check returns false when the element is displayed but not enabled.
        /// </summary>
        [TestMethod]
        public void TestElementEnabledCheckWhenDisplayedButNotEnabledReturnsFalse()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Displayed).Returns(true);
            element.SetupGet(e => e.Enabled).Returns(false);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.ElementEnabledCheck(element.Object);

            Assert.AreEqual(false, result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the element enabled check returns false when a NoSuchElementException is thrown.
        /// </summary>
        [TestMethod]
        public void TestElementEnabledCheckWhenNoSuchElementExceptionReturnsFalse()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Displayed).Throws<NoSuchElementException>();

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.ElementEnabledCheck(element.Object);

            Assert.AreEqual(false, result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the element enabled check returns false when a ElementNotVisibleException is thrown.
        /// </summary>
        [TestMethod]
        public void TestElementEnabledCheckWhenElementNotVisibleExceptionReturnsFalse()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Displayed).Throws<ElementNotVisibleException>();

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.ElementEnabledCheck(element.Object);

            Assert.AreEqual(false, result);
            element.VerifyAll();
        }


        /// <summary>
        /// Tests the element enabled check when stale element reference returns false.
        /// </summary>
        [TestMethod]
        public void TestElementEnabledCheckWhenStaleElementReferenceReturnsFalse()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Displayed).Throws<StaleElementReferenceException>();

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.ElementEnabledCheck(element.Object);

            Assert.IsFalse(result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the get element text method when the control is a standard control.
        /// </summary>
        [TestMethod]
        public void TestGetElementTextWhenControlIsNotSpecialReturnsTheElementText()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.TagName).Returns("DIV");
            element.SetupGet(e => e.Text).Returns("Normal Text");

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.GetElementText(element.Object);

            Assert.AreEqual("Normal Text", result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the get element text method when the control is a select (combo box) control.
        /// </summary>
        [TestMethod]
        public void TestGetElementTextWhenControlASelectControlReturnsTheSelectedItemText()
        {
            var option = new Mock<IWebElement>(MockBehavior.Strict);
            option.SetupGet(o => o.Selected).Returns(true);
            option.SetupGet(o => o.Text).Returns("Selected Item");

            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.TagName).Returns("select");

            element.Setup(e => e.GetAttribute("multiple")).Returns("false");

            element.Setup(e => e.FindElements(By.TagName("option")))
                   .Returns(new ReadOnlyCollection<IWebElement>(new[] { option.Object }));

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.GetElementText(element.Object);

            Assert.AreEqual("Selected Item", result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the get element text method when the control is a standard input control.
        /// </summary>
        [TestMethod]
        public void TestGetElementTextWhenControlIsStandardInputControl()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.TagName).Returns("input ");
            element.Setup(e => e.GetAttribute("type")).Returns("text");
            element.Setup(e => e.GetAttribute("value")).Returns("Input Text");

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.GetElementText(element.Object);

            Assert.AreEqual("Input Text", result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the get element text method when the control is a text area control.
        /// </summary>
        [TestMethod]
        public void TestGetElementTextWhenControlIsTextAreaControl()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.TagName).Returns("textarea");
            element.Setup(e => e.GetAttribute("type")).Returns("textarea");
            element.Setup(e => e.GetAttribute("value")).Returns("Input Text Area");

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.GetElementText(element.Object);

            Assert.AreEqual("Input Text Area", result);
            element.VerifyAll();
        }


        /// <summary>
        /// Tests the get element text method when the control is a checkbox input control.
        /// </summary>
        [TestMethod]
        public void TestGetElementTextWhenControlIsCheckBoxInputControl()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.TagName).Returns("input ");
            element.Setup(e => e.GetAttribute("type")).Returns("checkBox");
            element.SetupGet(e => e.Selected).Returns(true);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.GetElementText(element.Object);

            Assert.AreEqual("True", result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the click element
        /// </summary>
        [TestMethod]
        public void TestGetClickElement()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            this.SetupClick(element);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.ClickElement(element.Object);

            Assert.AreEqual(true, result);
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the get clear method.
        /// </summary>
        [TestMethod]
        public void TestGetClearMethod()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.Setup(e => e.Clear());

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var clearMethod = page.GetClearMethod(null);
            clearMethod(element.Object);

            element.VerifyAll();
        }

        /// <summary>
        /// Tests the get page fill method for a standard control.
        /// </summary>
        [TestMethod]
        public void TestGetFillMethodForStandardControl()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.TagName).Returns("div");
            element.Setup(e => e.SendKeys("Some Text"));

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var fillMethod = page.GetPageFillMethod(null);
            fillMethod(element.Object, "Some Text");

            element.VerifyAll();
        }

        /// <summary>
        /// Tests the get page fill method for a standard input control.
        /// </summary>
        [TestMethod]
        public void TestGetFillMethodForStandardInputControl()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.TagName).Returns("input");
            element.Setup(e => e.GetAttribute("type")).Returns("text");
            element.Setup(e => e.SendKeys("Some Text"));

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var fillMethod = page.GetPageFillMethod(null);
            fillMethod(element.Object, "Some Text");

            element.VerifyAll();
        }

        /// <summary>
        /// Tests the get page fill method for a checkbox control.
        /// </summary>
        [TestMethod]
        public void TestGetFillMethodForCheckboxControl()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.TagName).Returns("input");
            element.Setup(e => e.GetAttribute("type")).Returns("checkbox");
            element.SetupGet(e => e.Selected).Returns(false);
            this.SetupClick(element);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var fillMethod = page.GetPageFillMethod(null);
            fillMethod(element.Object, "true");

            element.VerifyAll();
        }

        /// <summary>
        /// Tests the get page fill method for a radio button control.
        /// </summary>
        [TestMethod]
        public void TestGetFillMethodForRadioButtonControl()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.TagName).Returns("input");
            element.Setup(e => e.GetAttribute("type")).Returns("radio");
            this.SetupClick(element);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var fillMethod = page.GetPageFillMethod(null);
            fillMethod(element.Object, "true");

            element.Verify(e => e.Click(), Times.Exactly(2));
            element.VerifyAll();
        }

        /// <summary>
        /// Tests the get fill method when the control is a select (combo box) control.
        /// </summary>
        [TestMethod]
        public void TestGetFillMethodForSingleSelectControl()
        {
            var option = new Mock<IWebElement>(MockBehavior.Strict);
            option.SetupGet(o => o.Selected).Returns(false);
            this.SetupClick(option);

            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.TagName).Returns("select");

            element.Setup(e => e.GetAttribute("multiple")).Returns("false");

            element.Setup(e => e.FindElements(By.XPath(".//option[normalize-space(.) = \"Selected Item\"]")))
                   .Returns(new ReadOnlyCollection<IWebElement>(new[] { option.Object }));

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var fillMethod = page.GetPageFillMethod(null);
            fillMethod(element.Object, "Selected Item");

            element.VerifyAll();
        }

        /// <summary>
        /// Tests the get fill method when the control is a select (combo box) control.
        /// </summary>
        [TestMethod]
        public void TestGetFillMethodForMultipleSelectControl()
        {
            var option = new Mock<IWebElement>(MockBehavior.Strict);
            option.SetupGet(o => o.Selected).Returns(false);
            this.SetupClick(option);

            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.TagName).Returns("select");

            element.Setup(e => e.GetAttribute("multiple")).Returns("true");

            element.Setup(e => e.FindElements(By.TagName("option")))
                   .Returns(new ReadOnlyCollection<IWebElement>(new[] { option.Object }));

            element.Setup(e => e.FindElements(By.XPath(".//option[normalize-space(.) = \"Selected Item\"]")))
                   .Returns(new ReadOnlyCollection<IWebElement>(new[] { option.Object }));

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var fillMethod = page.GetPageFillMethod(null);
            fillMethod(element.Object, "Selected Item");

            element.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control exists.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlExists()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Displayed).Returns(true);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.WaitForElement(element.Object, WaitConditions.Exists, null);

            Assert.IsTrue(result);

            element.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control to not exist.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlExpectExistButIsNot()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Displayed).Returns(false);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.WaitForElement(element.Object, WaitConditions.Exists, null);

            Assert.IsFalse(result);

            element.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control to not exist.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlNotExists()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Displayed).Returns(false);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);
            this.SetupToWaitForElement(page);

            var result = page.WaitForElement(element.Object, WaitConditions.NotExists, null);

            Assert.IsTrue(result);

            element.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control enabled.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlEnabled()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Enabled).Returns(true);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.WaitForElement(element.Object, WaitConditions.Enabled, null);

            Assert.IsTrue(result);

            element.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control to not exist.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlNotEnabled()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.SetupGet(e => e.Enabled).Returns(false);

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.WaitForElement(element.Object, WaitConditions.NotEnabled, null);

            Assert.IsTrue(result);

            element.VerifyAll();
        }

        /// <summary>
        /// Tests the get element attribute value.
        /// </summary>
        [TestMethod]
        public void TestGetElementAttributeValueWithValidAttribute()
        {
            var element = new Mock<IWebElement>(MockBehavior.Strict);
            element.Setup(e => e.GetAttribute("href")).Returns("http://myurl.com/page");

            var nativePage = new NativePage();
            var page = new SeleniumPage(nativePage, null);

            var result = page.GetElementAttributeValue(element.Object, "href");

            Assert.AreEqual("http://myurl.com/page", result);

            element.VerifyAll();
        }

        /// <summary>
        /// Sets up the click handler.
        /// </summary>
        /// <param name="element">The element.</param>
        protected void SetupClick(Mock<IWebElement> element)
        {
            element.SetupGet(e => e.Displayed).Returns(true);
            element.SetupGet(e => e.Location).Returns(new Point(100, 100)); // Initial element position
            element.SetupGet(e => e.Location).Returns(new Point(105, 105)); // Element has moved
            element.SetupGet(e => e.Location).Returns(new Point(105, 105)); // Element has stopped moving
            element.SetupGet(e => e.Enabled).Returns(true);
            element.Setup(e => e.Click());
        }

        /// <summary>
        /// Sets up the wait for element.
        /// </summary>
        /// <param name="seleniumPage">The selenium page.</param>
        protected void SetupToWaitForElement(SeleniumPage seleniumPage)
        {
            seleniumPage.ExecuteWithElementLocateTimeout = (TimeSpan t, Action work) => work();
            seleniumPage.EvaluateWithElementLocateTimeout = (TimeSpan t, Func<bool> work) => { return work(); };
        }

        #region Test Class - Native Page
        /// <summary>
        /// A test class for the native page.
        /// </summary>
        private class NativePage
        {
            /// <summary>
            /// Gets or sets my test control.
            /// </summary>
            /// <value>My control.</value>
            [ElementLocator(Id = "myControl")]
            public IWebElement MyControl { get; set; }
        }

        #endregion
    }
}