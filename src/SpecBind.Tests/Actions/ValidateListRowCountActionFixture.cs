// <copyright file="ValidateListRowCountActionFixture.cs">
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
    /// A test fixture for a button click action
    /// </summary>
    [TestClass]
    public class ValidateListRowCountActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var rowCountAction = new ValidateListRowCountAction();

            Assert.AreEqual("ValidateListRowCountAction", rowCountAction.Name);
        }

        /// <summary>
        ///     Tests the fill field with a field on the page that doesn't exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestExecuteWhenFieldDoesNotExistThrowsAnException()
        {
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetProperty("doesnotexist")).Throws(new ElementExecuteException("Cannot find item"));

            var rowCountAction = new ValidateListRowCountAction
                                        {
                                            ElementLocator = locator.Object
                                        };

            var context = new ValidateListRowCountAction.ValidateListRowCountContext("doesnotexist", NumericComparisonType.Equals, 1);
            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => rowCountAction.Execute(context), e => locator.VerifyAll());
        }

        /// <summary>
        /// Tests the execute method when the property is not a list returns a failure result.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPropertyIsNotAListReturnsFailureResult()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.IsList).Returns(false);
            propData.SetupGet(p => p.Name).Returns("MyProperty");

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetProperty("myproperty")).Returns(propData.Object);

            var rowCountAction = new ValidateListRowCountAction
                                        {
                                            ElementLocator = locator.Object
                                        };

            var context = new ValidateListRowCountAction.ValidateListRowCountContext("myproperty", NumericComparisonType.Equals, 1);
            var result = rowCountAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("Property 'MyProperty' was found but is not a list element.", result.Exception.Message);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method when the property returns a validation failure returns an error.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPropertyValidationReturnsErrorsReturnsFailureResult()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.IsList).Returns(true);
            propData.SetupGet(p => p.Name).Returns("MyProperty");
            propData.Setup(p => p.ValidateListRowCount(NumericComparisonType.Equals, 1))
                    .Returns(new Tuple<bool, int>(false, 2));

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetProperty("myproperty")).Returns(propData.Object);

            var rowCountAction = new ValidateListRowCountAction
                                        {
                                            ElementLocator = locator.Object
                                        };

            var context = new ValidateListRowCountAction.ValidateListRowCountContext("myproperty", NumericComparisonType.Equals, 1);
            var result = rowCountAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("List count validation of field 'MyProperty' failed. Expected Items: 1, Actual Items: 2", result.Exception.Message);
            
            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method when the property returns no validation failures returns an successful result.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPropertyValidationReturnsSuccessReturnsASuccess()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.IsList).Returns(true);
            propData.Setup(p => p.ValidateListRowCount(NumericComparisonType.Equals, 1))
                    .Returns(new Tuple<bool, int>(true, 1));

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetProperty("myproperty")).Returns(propData.Object);

            var rowCountAction = new ValidateListRowCountAction
                                        {
                                            ElementLocator = locator.Object
                                        };

            var context = new ValidateListRowCountAction.ValidateListRowCountContext("myproperty", NumericComparisonType.Equals, 1);
            var result = rowCountAction.Execute(context);

            Assert.AreEqual(true, result.Success);
            
            locator.VerifyAll();
            propData.VerifyAll();
        }
    }
}