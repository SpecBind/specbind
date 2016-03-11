// <copyright file="ValidateItemActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Pages;
    using SpecBind.Tests.Validation;
    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for a validate item action
    /// </summary>
    [TestClass]
    public class ValidateItemActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var buttonClickAction = new ValidateItemAction();

            Assert.AreEqual("ValidateItemAction", buttonClickAction.Name);
        }

        /// <summary>
        /// Tests the execute method with a property that does not exist.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenFieldDoesNotExistReturnsFailure()
        {
            IPropertyData propertyData;
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.TryGetProperty("doesnotexist", out propertyData)).Returns(false);

            var validateItemAction = new ValidateItemAction
                                        {
                                            ElementLocator = locator.Object
                                        };

            var table = new ValidationTable();
            table.AddValidation("doesnotexist", "My Data", "equals");
            table.Process();

            var context = new ValidateItemAction.ValidateItemContext(table);

            var result = validateItemAction.Execute(context);

            Assert.AreEqual(false, result.Success);

            Assert.IsNotNull(result.Exception);
            StringAssert.Contains(result.Exception.Message, "doesnotexist");

            locator.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method with a property that does not exist but that's the comparer.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenFieldDoesNotExistAndIsDoesNotExistComparerReturnsSuccess()
        {
            IPropertyData propertyData;
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.TryGetProperty("pdoesnotexist", out propertyData)).Returns(false);

            var validateItemAction = new ValidateItemAction
            {
                ElementLocator = locator.Object
            };

            var table = new ValidationTable();
            table.AddValidation("pdoesnotexist", "My Data", "doesnotexist");
            table.Process(new DoesNotExistComparer());

            var context = new ValidateItemAction.ValidateItemContext(table);

            var result = validateItemAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method with a property that exists and is valid returns a successful result.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenFieldExistsAndIsValidReturnsSuccess()
        {
            var table = new ValidationTable();
            table.AddValidation("name", "My Data", "equals");
            table.Process();

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            string actualValue;
            propData.Setup(p => p.ValidateItem(table.Validations.First(), out actualValue)).Returns(true);

            // ReSharper disable once RedundantAssignment
            var propertyData = propData.Object;
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.TryGetProperty("name", out propertyData)).Returns(true);

            var validateItemAction = new ValidateItemAction
            {
                ElementLocator = locator.Object
            };

            var context = new ValidateItemAction.ValidateItemContext(table);

            var result = validateItemAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method with a property that exists and is valid returns a failure result.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenFieldExistsButIsNotValidReturnsFailure()
        {
            var table = new ValidationTable();
            table.AddValidation("name", "equals", "wrong");
            table.Process();

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            string actualValue;
            propData.Setup(p => p.ValidateItem(table.Validations.First(), out actualValue)).Returns(false);

            // ReSharper disable once RedundantAssignment
            var propertyData = propData.Object;
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.TryGetProperty("name", out propertyData)).Returns(true);

            var validateItemAction = new ValidateItemAction
            {
                ElementLocator = locator.Object
            };

            var context = new ValidateItemAction.ValidateItemContext(table);

            var result = validateItemAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.IsNotNull(result.Exception);
            StringAssert.Contains(result.Exception.Message, "wrong");

            locator.VerifyAll();
        }
    }
}