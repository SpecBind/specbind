// <copyright file="WaitForElementActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for a wait for element action
    /// </summary>
    [TestClass]
    public class WaitForElementActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var buttonClickAction = new WaitForElementAction();

            Assert.AreEqual("WaitForElementAction", buttonClickAction.Name);
        }

        /// <summary>
        /// Tests the action execute with a field on the page that doesn't exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestExecuteFieldDoesNotExist()
        {
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            IPropertyData expectedPropertyData = null;
            locator.Setup(p => p.TryGetElement("doesnotexist", out expectedPropertyData)).Returns(false);
            locator.Setup(p => p.GetElement("doesnotexist")).Throws(new ElementExecuteException("Cannot find item"));

            var buttonClickAction = new WaitForElementAction
            {
                ElementLocator = locator.Object
            };

            var context = new WaitForElementAction.WaitForElementContext("doesnotexist", WaitConditions.Exists, TimeSpan.FromMilliseconds(1));

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => buttonClickAction.Execute(context), e => locator.VerifyAll());
        }

        /// <summary>
        /// Tests the element execute success.
        /// </summary>
        [TestMethod]
        public void TestExecuteSuccess()
        {
            var timeout = TimeSpan.FromMilliseconds(1);
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.WaitForElementCondition(WaitConditions.Exists, It.IsAny<TimeSpan>())).Returns(true);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            var expectedPropertyData = propData.Object;
            locator.Setup(p => p.TryGetElement("myproperty", out expectedPropertyData)).Returns(true);

            var waitForElementAction = new WaitForElementAction
            {
                ElementLocator = locator.Object
            };

            var context = new WaitForElementAction.WaitForElementContext("myproperty", WaitConditions.Exists, timeout);
            var result = waitForElementAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the element execute success.
        /// </summary>
        [TestMethod]
        public void TestExecuteFailureIfResultIsFalse()
        {
            var timeout = TimeSpan.FromSeconds(1);
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.WaitForElementCondition(WaitConditions.Exists, It.IsAny<TimeSpan>())).Returns(false);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            var expectedPropertyData = propData.Object;
            locator.Setup(p => p.TryGetElement("myproperty", out expectedPropertyData)).Returns(true);

            var waitForElementAction = new WaitForElementAction
            {
                ElementLocator = locator.Object
            };

            var context = new WaitForElementAction.WaitForElementContext("myproperty", WaitConditions.Exists, timeout);
            var result = waitForElementAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("Could not perform action 'BecomesExistent' before timeout: 00:00:01", result.Exception.Message);

            locator.VerifyAll();
            propData.VerifyAll();
        }
    }
}