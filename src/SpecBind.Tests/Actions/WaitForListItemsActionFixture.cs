// <copyright file="WaitForListItemsActionFixture.cs">
//    Copyright © 2014 Dan Piessens  All rights reserved.
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
    /// A test fixture for a wait for list items action
    /// </summary>
    [TestClass]
    public class WaitForListItemsActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var buttonClickAction = new WaitForListItemsAction(null);

            Assert.AreEqual("WaitForListItemsAction", buttonClickAction.Name);
        }

        /// <summary>
        /// Tests the action execute with an element that does not exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestExecuteWhenElementIsNotFoundThrowsAnException()
        {
            var logger = new Mock<ILogger>();

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetProperty("doesnotexist")).Throws(new ElementExecuteException("Cannot find item"));

            var action = new WaitForListItemsAction(logger.Object) { ElementLocator = locator.Object };
            var context = new WaitForListItemsAction.WaitForListItemsContext("doesnotexist", null);

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => action.Execute(context),
                e => locator.VerifyAll());
        }

        /// <summary>
        /// Tests the action execute with an element that does not exist.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenElementIsNotAListReturnsAFailure()
        {
            var logger = new Mock<ILogger>();
            var property = new Mock<IPropertyData>(MockBehavior.Strict);
            property.SetupGet(p => p.IsList).Returns(false);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetProperty("notalist")).Returns(property.Object);

            var action = new WaitForListItemsAction(logger.Object) { ElementLocator = locator.Object };
            var context = new WaitForListItemsAction.WaitForListItemsContext("notalist", null);

            var result = action.Execute(context);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Property 'notalist' is not a list and cannot be used in this wait.", result.Exception.Message);

            locator.VerifyAll();
            property.VerifyAll();
        }

        /// <summary>
        /// Tests the action execute with a list that contains an item.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPageIsFoundAndUrlMatchesReturnsSuccess()
        {
            var logger = new Mock<ILogger>();
            var listItem = new Mock<IPage>();

            var property = new Mock<IPropertyData>(MockBehavior.Strict);
            property.SetupGet(p => p.IsList).Returns(true);
            property.Setup(p => p.GetItemAtIndex(0)).Returns(listItem.Object);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetProperty("SampleList")).Returns(property.Object);

            var action = new WaitForListItemsAction(logger.Object) { ElementLocator = locator.Object };
            var context = new WaitForListItemsAction.WaitForListItemsContext("SampleList", TimeSpan.FromSeconds(3));

            var result = action.Execute(context);

            Assert.AreEqual(true, result.Success);
            
            locator.VerifyAll();
            property.VerifyAll();
        }

        /// <summary>
        /// Tests the action execute with a page exists and matches the url after an initial failure.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPageIsFoundAndUrlMatchesAfterInitialFailureReturnsSuccess()
        {
            var logger = new Mock<ILogger>(MockBehavior.Strict);
            logger.Setup(l => l.Debug("List did not contain any elements, waiting..."));

            var property = new Mock<IPropertyData>(MockBehavior.Strict);
            property.SetupGet(p => p.IsList).Returns(true);

            var pageItem = new Mock<IPage>(MockBehavior.Strict);

            var count = 0;
            property.Setup(p => p.GetItemAtIndex(0))
                    .Callback(() => count++)
                    .Returns(() => (count == 1) ? null : pageItem.Object);


            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetProperty("SampleList")).Returns(property.Object);

            var action = new WaitForListItemsAction(logger.Object) { ElementLocator = locator.Object };
            var context = new WaitForListItemsAction.WaitForListItemsContext("SampleList", TimeSpan.FromSeconds(3));

            var result = action.Execute(context);

            Assert.AreEqual(true, result.Success);

            property.Verify(b => b.GetItemAtIndex(0), Times.Exactly(2));
            property.VerifyAll();
            locator.VerifyAll();
            logger.VerifyAll();
            pageItem.VerifyAll();
        }

        /// <summary>
        /// Tests the action execute with a page exists but never matches the URL returns a failure.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPageIsFoundAndUrlDoesntMatchAfterTimeoutReturnsFailure()
        {
            var logger = new Mock<ILogger>(MockBehavior.Strict);
            logger.Setup(l => l.Debug("List did not contain any elements, waiting..."));

            var property = new Mock<IPropertyData>(MockBehavior.Strict);
            property.SetupGet(p => p.IsList).Returns(true);
            property.Setup(p => p.GetItemAtIndex(0)).Returns((IPage)null);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetProperty("SampleList")).Returns(property.Object);

            var action = new WaitForListItemsAction(logger.Object) { ElementLocator = locator.Object };
            var context = new WaitForListItemsAction.WaitForListItemsContext("SampleList", TimeSpan.FromSeconds(1));

            var result = action.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.IsNotNull(result.Exception);
            Assert.AreEqual("List 'SampleList' did not contain elements after 00:00:01", result.Exception.Message);

            property.VerifyAll();
            locator.VerifyAll();
            logger.VerifyAll();
        }
    }
}