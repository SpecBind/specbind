// <copyright file="GetElementAsPageActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for a button click action
    /// </summary>
    [TestClass]
    public class GetElementAsPageActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var buttonClickAction = new GetElementAsPageAction();

            Assert.AreEqual("GetElementAsPageAction", buttonClickAction.Name);
        }

        /// <summary>
        /// Tests the get element as page with a field on the page that doesn't exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestGetElementAsPageFieldDoesNotExist()
        {
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("doesnotexist")).Throws(new ElementExecuteException("Cannot find item"));

            var buttonClickAction = new GetElementAsPageAction
            {
                ElementLocator = locator.Object
            };

            var context = new ActionContext("doesnotexist");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => buttonClickAction.Execute(context), e => locator.VerifyAll());
        }

        /// <summary>
        /// Tests the get element as page with an element that exists is a list returns a failure.
        /// </summary>
        [TestMethod]
        public void TestGetElementAsPageWhenPageIsAListReturnsAFailure()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.IsList).Returns(true);
            propData.SetupGet(p => p.Name).Returns("MyProperty");

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var buttonClickAction = new GetElementAsPageAction
            {
                ElementLocator = locator.Object
            };

            var context = new ActionContext("myproperty");
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(false, result.Success);

            Assert.IsNotNull(result.Exception);
            StringAssert.Contains(result.Exception.Message, "MyProperty");

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the get element as page with an element that exists but returns null returns a failure.
        /// </summary>
        [TestMethod]
        public void TestGetElementAsPageWhenPageIsNullReturnsAFailure()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.IsList).Returns(false);
            propData.SetupGet(p => p.Name).Returns("MyProperty");
            propData.Setup(p => p.GetItemAsPage()).Returns((IPage)null);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var buttonClickAction = new GetElementAsPageAction
            {
                ElementLocator = locator.Object
            };

            var context = new ActionContext("myproperty");
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(false, result.Success);

            Assert.IsNotNull(result.Exception);
            Assert.AreEqual("Could not retrieve a page from property 'MyProperty'", result.Exception.Message);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the get element as page with an element that exists is returned.
        /// </summary>
        [TestMethod]
        public void TestGetElementAsPageSuccess()
        {
            var page = new Mock<IPage>(MockBehavior.Strict);

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.IsList).Returns(false);
            propData.Setup(p => p.GetItemAsPage()).Returns(page.Object);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var buttonClickAction = new GetElementAsPageAction
            {
                ElementLocator = locator.Object
            };

            var context = new ActionContext("myproperty");
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(true, result.Success);
            Assert.AreSame(page.Object, result.Result);

            locator.VerifyAll();
            propData.VerifyAll();
        }
    }
}