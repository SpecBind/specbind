// <copyright file="ValidateComboBoxActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for a button click action
    /// </summary>
    [TestClass]
    public class ValidateComboBoxActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var comboBoxAction = new ValidateComboBoxAction();

            Assert.AreEqual("ValidateComboBoxAction", comboBoxAction.Name);
        }

        /// <summary>
        ///     Tests the fill field with a field on the page that doesn't exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestExecuteWhenFieldDoesNotExistThrowsAnException()
        {
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("doesnotexist")).Throws(new ElementExecuteException("Cannot find item"));

            var comboBoxAction = new ValidateComboBoxAction
            {
                                            ElementLocator = locator.Object
                                        };

            var context = new ValidateComboBoxAction.ValidateComboBoxContext("doesnotexist", ComboComparisonType.Contains, new List<ComboBoxItem>(), true, true);
            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => comboBoxAction.Execute(context), e => locator.VerifyAll());
        }

        /// <summary>
        /// Tests the execute method when the property is not a list returns a failure result.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPropertyIsNotAComboBoxReturnsFailureResult()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.Name).Returns("MyProperty");
            propData.Setup(p => p.GetComboBoxItems()).Returns((List<ComboBoxItem>)null);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var comboBoxAction = new ValidateComboBoxAction
            {
                                            ElementLocator = locator.Object
                                        };

            var context = new ValidateComboBoxAction.ValidateComboBoxContext("myproperty", ComboComparisonType.Contains, new List<ComboBoxItem>(), true, true);
            var result = comboBoxAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("Property 'MyProperty' was found but is not a combo box element.", result.Exception.Message);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method when the property returns a validation failure returns an error.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPropertyValidationReturnsContainsErrorsReturnsFailureResult()
        {

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.Name).Returns("MyProperty");
            propData.Setup(p => p.GetComboBoxItems()).Returns(new List<ComboBoxItem>());

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var comboBoxAction = new ValidateComboBoxAction
            {
                                            ElementLocator = locator.Object
                                        };

            var expectedItems = new List<ComboBoxItem> { new ComboBoxItem { Text = "Item 1", Value = "1" } };
            var context = new ValidateComboBoxAction.ValidateComboBoxContext("myproperty", ComboComparisonType.Contains, expectedItems, true, true);
            var result = comboBoxAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("Combo box validation of field 'MyProperty' failed. Expected items that do not exist: Item 1", result.Exception.Message);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method when the property returns a validation failure returns an error.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPropertyValidationReturnsDoesNotContainErrorsReturnsFailureResult()
        {
            var items = new List<ComboBoxItem> { new ComboBoxItem { Text = "Item 1", Value = "1" } };
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.Name).Returns("MyProperty");
            propData.Setup(p => p.GetComboBoxItems()).Returns(items);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var comboBoxAction = new ValidateComboBoxAction
            {
                ElementLocator = locator.Object
            };

            var context = new ValidateComboBoxAction.ValidateComboBoxContext("myproperty", ComboComparisonType.DoesNotContain, items, true, true);
            var result = comboBoxAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("Combo box validation of field 'MyProperty' failed. Expected items that exists but should not have: Item 1", result.Exception.Message);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method when the property returns a validation failure returns an error.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPropertyValidationReturnsExcactMatchWithExtraItemsReturnsFailureResult()
        {
            var items = new List<ComboBoxItem> { new ComboBoxItem { Text = "Item 1", Value = "1" }, new ComboBoxItem { Text = "Item 2", Value = "2" }, };
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.Name).Returns("MyProperty");
            propData.Setup(p => p.GetComboBoxItems()).Returns(items);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var comboBoxAction = new ValidateComboBoxAction
            {
                ElementLocator = locator.Object
            };

            var expectedItems = new List<ComboBoxItem> { new ComboBoxItem { Text = "Item 1", Value = "1" } };
            var context = new ValidateComboBoxAction.ValidateComboBoxContext("myproperty", ComboComparisonType.ContainsExactly, expectedItems, true, true);
            var result = comboBoxAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("Combo box exact match validation of field 'MyProperty' failed. Expected Items: Item 1; Actual Items: Item 1,Item 2", result.Exception.Message);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method when the property returns no validation failures returns an successful result.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPropertyValidationReturnsSuccessReturnsASuccess()
        {
            var mockItem = new ComboBoxItem { Text = "Item 1", Value = "1" };

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.GetComboBoxItems()).Returns(new List<ComboBoxItem> { mockItem });

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var comboBoxAction = new ValidateComboBoxAction
            {
                ElementLocator = locator.Object
            };

            var context = new ValidateComboBoxAction.ValidateComboBoxContext("myproperty", ComboComparisonType.Contains, new List<ComboBoxItem> { mockItem }, true, true);
            var result = comboBoxAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method when the property returns no validation failures returns an successful result.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPropertyValidationIgnoresValuesReturnsASuccess()
        {
            var mockItem = new ComboBoxItem { Text = "Item 1", Value = "0" };

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.GetComboBoxItems()).Returns(new List<ComboBoxItem> { mockItem });

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var comboBoxAction = new ValidateComboBoxAction
            {
                ElementLocator = locator.Object
            };

            var mockExpectedItem = new ComboBoxItem { Text = "Item 1", Value = "1" };
            var context = new ValidateComboBoxAction.ValidateComboBoxContext("myproperty", ComboComparisonType.Contains, new List<ComboBoxItem> { mockExpectedItem }, true, false);
            var result = comboBoxAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method when the property returns no validation failures returns an successful result.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPropertyValidationIgnoresTextReturnsASuccess()
        {
            var mockItem = new ComboBoxItem { Text = "Item", Value = "1" };

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.GetComboBoxItems()).Returns(new List<ComboBoxItem> { mockItem });

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var comboBoxAction = new ValidateComboBoxAction
            {
                ElementLocator = locator.Object
            };

            var mockExpectedItem = new ComboBoxItem { Text = "Item 1", Value = "1" };
            var context = new ValidateComboBoxAction.ValidateComboBoxContext("myproperty", ComboComparisonType.Contains, new List<ComboBoxItem> { mockExpectedItem }, false, true);
            var result = comboBoxAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
        }
    }
}