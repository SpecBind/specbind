// <copyright file="ValidationTableExtensionsFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Tests.Helpers
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Helpers;
    using SpecBind.Pages;

    using TechTalk.SpecFlow;

    /// <summary>
    ///     A test fixture for the ValidationTableExtensions methods.
    /// </summary>
    [TestClass]
    public class ValidationTableExtensionsFixture
    {
        private const string InvalidColumnErrorMessage =
            @"A table must be specified for this step with the columns 'Field', 'Rule' and 'Value'";

        /// <summary>
        /// Tests to validation table that when field column is missing it throws an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestToValidationTableWhenFieldColumnIsMissingThrowsException()
        {
            var table = new Table("Rule", "Value");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => table.ToValidationTable(),
                ex => Assert.AreEqual(InvalidColumnErrorMessage, ex.Message));
        }

        /// <summary>
        /// Tests to validation table that when rule column is missing it throws an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestToValidationTableWhenRuleColumnIsMissingThrowsException()
        {
            var table = new Table("Field");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => table.ToValidationTable(),
                ex => Assert.AreEqual(InvalidColumnErrorMessage, ex.Message));
        }

        /// <summary>
        /// Tests to validation table that when value column is missing it throws an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestToValidationTableWhenValueColumnIsMissingThrowsException()
        {
            var table = new Table("Field", "Rule");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => table.ToValidationTable(),
                ex => Assert.AreEqual(InvalidColumnErrorMessage, ex.Message));
        }

        /// <summary>
        /// Tests to validation table that the columns exist but the table is empty, returns an empty collection.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestToValidationTableWhenTableIsEmptyThrowsException()
        {
            var table = new Table("Field", "Rule", "Value");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => table.ToValidationTable(),
                ex => Assert.AreEqual("The validation table must contain at least one validation row.", ex.Message));
        }

        /// <summary>
        /// Tests to validation table that the columns exist but the table is empty, returns an empty collection.
        /// </summary>
        [TestMethod]
        public void TestToValidationTableWhenTableIsPopulatedReturnsValidationTable()
        {
            var table = new Table("Field", "Rule", "Value");
            table.AddRow("My Column", "equals", "Foo");

            var validationTable = table.ToValidationTable();

            Assert.IsNotNull(validationTable);
            Assert.AreEqual(1, validationTable.ValidationCount);

            var item = validationTable.Validations.First();
            Assert.AreEqual("My Column", item.RawFieldName);
            Assert.AreEqual("Foo", item.RawComparisonValue);
            Assert.AreEqual("equals", item.RawComparisonType);
        }
    }
}