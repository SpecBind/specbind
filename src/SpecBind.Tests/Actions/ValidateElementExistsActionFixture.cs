// <copyright file="ValidateElementExistsActionFixture.cs">
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
    public class ValidateElementExistsActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var buttonClickAction = new ValidateElementExistsAction();

            Assert.AreEqual("ValidateElementExistsAction", buttonClickAction.Name);
        }

        /// <summary>
        /// Tests the check items exists with a field on the page that doesn't exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestExecuteFieldDoesNotExist()
        {
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("doesnotexist")).Throws(new ElementExecuteException("Cannot find item"));

            var buttonClickAction = new ValidateElementExistsAction
                                        {
                                            ElementLocator = locator.Object
                                        };

            var context = new ValidationCheckContext("doesnotexist", true);

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => buttonClickAction.Execute(context), e => locator.VerifyAll());
        }

        /// <summary>
        /// Tests the execute with an element that is enabled and should be enabled.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenElementShouldBeEnabledAndIsReturnsSuccess()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.CheckElementExists()).Returns(true);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var buttonClickAction = new ValidateElementExistsAction
                                        {
                                            ElementLocator = locator.Object
                                        };

            var context = new ValidationCheckContext("myproperty", true);
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute with an element that is not enabled and should be enabled.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenElementShouldBeEnabledAndIsNotEnabledReturnsFailure()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.Name).Returns("MyProperty");
            propData.Setup(p => p.CheckElementExists()).Returns(false);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var buttonClickAction = new ValidateElementExistsAction
                                        {
                                            ElementLocator = locator.Object
                                        };

            var context = new ValidationCheckContext("myproperty", true);
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("Element 'MyProperty' does not exist on the page and should exist.", result.Exception.Message);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute with an element that is not enabled and should be enabled.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenElementShouldNoeBeEnabledAndIsnabledReturnsFailure()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.Name).Returns("MyProperty");
            propData.Setup(p => p.CheckElementExists()).Returns(true);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var buttonClickAction = new ValidateElementExistsAction
                                        {
                                            ElementLocator = locator.Object
                                        };

            var context = new ValidationCheckContext("myproperty", false);
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("Element 'MyProperty' exists on the page and should not exist.", result.Exception.Message);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute with an element that is not enabled and should not be enabled.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenElementShouldBeNotEnabledAndIsNotEnabledReturnsSuccess()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.CheckElementExists()).Returns(false);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var buttonClickAction = new ValidateElementExistsAction
                                        {
                                            ElementLocator = locator.Object
                                        };

            var context = new ValidationCheckContext("myproperty", false);
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
        }
    }
}