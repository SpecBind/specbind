// <copyright file="DismissDialogActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.Actions;
    using SpecBind.BrowserSupport;

    /// <summary>
    /// A test fixture for dismissing a dialog action.
    /// </summary>
    [TestClass]
    public class DismissDialogActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var buttonClickAction = new DismissDialogAction(null);

            Assert.AreEqual("DismissDialogAction", buttonClickAction.Name);
        }


        /// <summary>
        /// Tests the dismiss alert when an invalid option is selected returns a failure.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertWhenInvalidOptionIsSelectedReturnsAFailure()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            
            var buttonClickAction = new DismissDialogAction(browser.Object);

            var context = new DismissDialogAction.DismissDialogContext("foo");
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.IsNotNull(result.Exception);
            StringAssert.Contains(result.Exception.Message, "Could not translate 'foo' into a known dialog action. Available Actions:");

            browser.VerifyAll();
        }

        /// <summary>
        /// dismiss alert when the OK button is selected.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertWhenOkButtonIsChoosenCallsBrowserAction()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(p => p.DismissAlert(AlertBoxAction.Ok, null));

            var buttonClickAction = new DismissDialogAction(browser.Object);

            var context = new DismissDialogAction.DismissDialogContext("ok");
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            browser.VerifyAll();
        }

        /// <summary>
        /// dismiss alert when the OK button is selected.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertWhenOkButtonWithWhitespaceIsChoosenCallsBrowserAction()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(p => p.DismissAlert(AlertBoxAction.Ok, null));

            var buttonClickAction = new DismissDialogAction(browser.Object);

            var context = new DismissDialogAction.DismissDialogContext("  ok  ");
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            browser.VerifyAll();
        }

        /// <summary>
        /// dismiss alert when the text is null, to make sure it's translated into an empty string and the OK button is selected.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertWhenTextIsEnteredButNullAndOkButtonIsChoosenCallsBrowserAction()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(p => p.DismissAlert(AlertBoxAction.Ok, string.Empty));

            var buttonClickAction = new DismissDialogAction(browser.Object);

            var context = new DismissDialogAction.DismissDialogContext("ok", null);
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            browser.VerifyAll();
        }
    }
}