// <copyright file="GetListItemByCriteriaActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for an action that gets an item by some criteria
    /// </summary>
    [TestClass]
    public class GetListItemByCriteriaActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var buttonClickAction = new GetListItemByCriteriaAction();

            Assert.AreEqual("GetListItemByCriteriaAction", buttonClickAction.Name);
        }

        /// <summary>
        /// Tests the locator when the list does not exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestGetElementAsPageFieldDoesNotExist()
        {
            var locater = new Mock<IElementLocator>(MockBehavior.Strict);
            locater.Setup(p => p.GetProperty("doesnotexist")).Throws(new ElementExecuteException("Cannot find item"));

            var buttonClickAction = new GetListItemByCriteriaAction
            {
                ElementLocator = locater.Object
            };

            var context = new GetListItemByCriteriaAction.ListItemByCriteriaContext("doesnotexist", new ValidationTable());

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => buttonClickAction.Execute(context), e => locater.VerifyAll());
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

            var buttonClickAction = new GetListItemByCriteriaAction
            {
                ElementLocator = locator.Object
            };

            var context = new GetListItemByCriteriaAction.ListItemByCriteriaContext("myproperty", new ValidationTable());
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("Property 'MyProperty' was found but is not a list element.", result.Exception.Message);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method when the property cannot find a matching element returns an error.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPropertyValidationReturnsErrorsReturnsFailureResult()
        {
            var table = new ValidationTable();
            table.AddValidation("name", "Hello", "equals");

            var itemResult = new ValidationItemResult();
            itemResult.NoteValidationResult(table.Validations.First(), false, "World");

            var validationResult = new ValidationResult(table.Validations) { IsValid = false, ItemCount = 1 };
            validationResult.CheckedItems.Add(itemResult);

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.IsList).Returns(true);
            propData.SetupGet(p => p.Name).Returns("MyProperty");
            propData.Setup(p => p.FindItemInList(It.IsAny<ICollection<ItemValidation>>()))
                    .Returns(new Tuple<IPage, ValidationResult>(null, validationResult));

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetProperty("myproperty")).Returns(propData.Object);

            var buttonClickAction = new GetListItemByCriteriaAction
            {
                ElementLocator = locator.Object
            };

            var context = new GetListItemByCriteriaAction.ListItemByCriteriaContext("myproperty", table);
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.IsNotNull(result.Exception);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method when the property finds a matching element returns success and the result.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPropertyContainsMatchingItemReturnsSuccessful()
        {
            var table = new ValidationTable();
            table.AddValidation("name", "Hello", "equals");

            var page = new Mock<IPage>();

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.IsList).Returns(true);
            propData.Setup(p => p.FindItemInList(It.IsAny<ICollection<ItemValidation>>()))
                    .Returns(new Tuple<IPage, ValidationResult>(page.Object, null));

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetProperty("myproperty")).Returns(propData.Object);

            var buttonClickAction = new GetListItemByCriteriaAction
            {
                ElementLocator = locator.Object
            };

            var context = new GetListItemByCriteriaAction.ListItemByCriteriaContext("myproperty", table);
            var result = buttonClickAction.Execute(context);

            Assert.AreEqual(true, result.Success);
            Assert.AreSame(page.Object, result.Result);

            locator.VerifyAll();
            propData.VerifyAll();
        }
    }
}