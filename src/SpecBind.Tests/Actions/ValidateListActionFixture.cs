// <copyright file="ValidateListActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Pages;
    using SpecBind.Tests.Validation;
    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for a button click action
    /// </summary>
    [TestClass]
    public class ValidateListActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var buttonClickAction = new ValidateListAction();

            Assert.AreEqual("ValidateListAction", buttonClickAction.Name);
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

            var buttonClickAction = new ValidateListAction
                                        {
                                            ElementLocator = locator.Object
                                        };

            var context = new ValidateListAction.ValidateListContext("doesnotexist", ComparisonType.Equals, new ValidationTable());
            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => buttonClickAction.Execute(context), e => locator.VerifyAll());
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

            var buttonClickAction = new ValidateListAction
                                        {
                                            ElementLocator = locator.Object
                                        };

            var context = new ValidateListAction.ValidateListContext("myproperty", ComparisonType.Equals, new ValidationTable());
            var result = buttonClickAction.Execute(context);

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
            var table = new ValidationTable();
            table.AddValidation("name", "Hello", "equals");
            table.Process();

            var itemResult = new ValidationItemResult();
            itemResult.NoteValidationResult(table.Validations.First(), false, "World");

            var validationResult = new ValidationResult(table.Validations) { IsValid = false, ItemCount = 1 };
            validationResult.CheckedItems.Add(itemResult);

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.IsList).Returns(true);
            propData.SetupGet(p => p.Name).Returns("MyProperty");
            propData.Setup(p => p.ValidateList(ComparisonType.Equals, It.Is<ICollection<ItemValidation>>(c => c.Count == 1))).Returns(validationResult);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetProperty("myproperty")).Returns(propData.Object);

            var buttonClickAction = new ValidateListAction
            {
                ElementLocator = locator.Object
            };

            var context = new ValidateListAction.ValidateListContext("myproperty", ComparisonType.Equals, table);
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.IsNotNull(result.Exception);
            
            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method when the property returns no validation failures returns an successful result.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPropertyValidationReturnsSuccessReturnsASuccess()
        {
            var table = new ValidationTable();
            table.AddValidation("name", "Hello", "equals");
            table.Process();

            var itemResult = new ValidationItemResult();
            itemResult.NoteValidationResult(table.Validations.First(), true, "World");

            var validationResult = new ValidationResult(table.Validations) { IsValid = true, ItemCount = 1 };
            validationResult.CheckedItems.Add(itemResult);

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.IsList).Returns(true);
            propData.Setup(p => p.ValidateList(ComparisonType.Equals, It.Is<ICollection<ItemValidation>>(c => c.Count == 1))).Returns(validationResult);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetProperty("myproperty")).Returns(propData.Object);

            var buttonClickAction = new ValidateListAction
            {
                ElementLocator = locator.Object
            };

            var context = new ValidateListAction.ValidateListContext("myproperty", ComparisonType.Equals, table);
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(true, result.Success);
            
            locator.VerifyAll();
            propData.VerifyAll();
        }
    }
}