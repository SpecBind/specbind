// <copyright file="ValidationTablePreActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for the ValidationTablePreAction.
    /// </summary>
    [TestClass]
    public class ValidationTablePreActionFixture
    {
        /// <summary>
        /// Tests the action when context is not a table, ensures it exits.
        /// </summary>
        [TestMethod]
        public void TestActionWhenContextIsNotATableExits()
        {
            var action = new Mock<IAction>(MockBehavior.Strict);
            var actionRepository = new Mock<IActionRepository>(MockBehavior.Strict);
            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

            var preAction = new ValidationTablePreAction(actionRepository.Object, tokenManager.Object);

            var context = new ActionContext("myproperty");

            preAction.PerformPreAction(action.Object, context);

            action.VerifyAll();
            actionRepository.VerifyAll();
            tokenManager.VerifyAll();
        }

        /// <summary>
        /// Tests the action when context is a table, processes the values.
        /// </summary>
        [TestMethod]
        public void TestActionWhenContextIsATableProcessesActions()
        {
            var action = new Mock<IAction>(MockBehavior.Strict);
            var items = new List<IValidationComparer> { new StartsWithComparer() }.AsReadOnly();

            var actionRepository = new Mock<IActionRepository>(MockBehavior.Strict);
            actionRepository.Setup(a => a.GetComparisonTypes()).Returns(items);

            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
            tokenManager.Setup(t => t.GetToken("foo")).Returns("foo");

            var preAction = new ValidationTablePreAction(actionRepository.Object, tokenManager.Object);

            var table = new ValidationTable();
            table.AddValidation("My Field", "starts with", "foo");

            var context = new TableContext(table);
            preAction.PerformPreAction(action.Object, context);

            var validation = table.Validations.First();
            Assert.AreEqual("myfield", validation.FieldName);
            Assert.AreEqual("foo", validation.ComparisonValue);
            Assert.IsNotNull(validation.Comparer);
            Assert.IsInstanceOfType(validation.Comparer, typeof(StartsWithComparer));

            action.VerifyAll();
            actionRepository.VerifyAll();
            tokenManager.VerifyAll();
        }

        /// <summary>
        /// Tests the action when context is a table throws an exception if the rule is invalid.
        /// </summary>
        [TestMethod]
        public void TestActionWhenContextIsATableButHasBadRuleProcessesActions()
        {
            var action = new Mock<IAction>(MockBehavior.Strict);
            var items = new List<IValidationComparer> { new StartsWithComparer(), new EqualsComparer() }.AsReadOnly();

            var actionRepository = new Mock<IActionRepository>(MockBehavior.Strict);
            actionRepository.Setup(a => a.GetComparisonTypes()).Returns(items);

            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

            var preAction = new ValidationTablePreAction(actionRepository.Object, tokenManager.Object);

            var table = new ValidationTable();
            table.AddValidation("My Field", "bad rule", "foo");

            var context = new TableContext(table);

            try
            {
                preAction.PerformPreAction(action.Object, context);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ElementExecuteException));
                Assert.AreEqual("Vaidation Rule could not be found for rule name: bad rule", e.Message);
            }

            action.VerifyAll();
            actionRepository.VerifyAll();
            tokenManager.VerifyAll();
        }

        /// <summary>
        /// a test context
        /// </summary>
        private class TableContext : ActionContext, IValidationTable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TableContext"/> class.
            /// </summary>
            /// <param name="table">The validation table.</param>
            public TableContext(ValidationTable table)
                : base(null)
            {
                this.ValidationTable = table;
            }

            /// <summary>
            /// Gets the validation table.
            /// </summary>
            /// <value>The validation table.</value>
            public ValidationTable ValidationTable { get; private set; }
        }
    }
}