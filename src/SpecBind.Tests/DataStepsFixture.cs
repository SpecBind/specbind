﻿// <copyright file="DataStepsFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    using TechTalk.SpecFlow;

    /// <summary>
    /// A test fixture for the data steps class.
    /// </summary>
    [TestClass]
    public class DataStepsFixture
    {
        /// <summary>
        /// Tests the WhenIEnterDataInFieldsStep method with a null table.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestWhenIEnterDataInFieldsStepNullTable()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.WhenIEnterDataInFieldsStep(null),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    scenarioContext.VerifyAll();
                    pipelineService.VerifyAll();
                });
        }

        /// <summary>
        /// Tests the WhenIEnterDataInFieldsStep method with a table that has no field column.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestWhenIEnterDataInFieldsStepMissingFieldColumn()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Value");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.WhenIEnterDataInFieldsStep(table),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    scenarioContext.VerifyAll();
                    pipelineService.VerifyAll();
                });
        }

        /// <summary>
        /// Tests the WhenIEnterDataInFieldsStep method with a table that has no field column.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestWhenIEnterDataInFieldsStepMissingValueColumn()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.WhenIEnterDataInFieldsStep(table),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    scenarioContext.VerifyAll();
                    pipelineService.VerifyAll();
                });
        }

        /// <summary>
        /// Tests the WhenIEnterDataInFieldsStep method with a successful result.
        /// </summary>
        [TestMethod]
        public void TestWhenIEnterDataInFieldsStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(
                p => p.PerformAction<EnterDataAction>(testPage.Object, It.Is<EnterDataAction.EnterDataContext>(c => c.PropertyName == "myfield" && c.Data == "myvalue")))
                            .Returns(ActionResult.Successful());


            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Value");
            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Field", "My Field" },
                                 { "Value", "myvalue" }
                             });

            steps.WhenIEnterDataInFieldsStep(table);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the WhenIEnterDataInFieldsStep method with a successful result.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestWhenIEnterDataInFieldsStepMultipleEntriesWithFailure()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(
                p => p.PerformAction<EnterDataAction>(testPage.Object, It.Is<EnterDataAction.EnterDataContext>(c => c.PropertyName == "myfield" && c.Data == "myvalue")))
                            .Returns(ActionResult.Successful());
            pipelineService.Setup(
                p => p.PerformAction<EnterDataAction>(testPage.Object, It.Is<EnterDataAction.EnterDataContext>(c => c.PropertyName == "mysecondfield" && c.Data == "something")))
                            .Returns(ActionResult.Failure(new ElementExecuteException("Could Not Find Field: mysecondfield")));


            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Value");
            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Field", "My Second Field" },
                                 { "Value", "something" }
                             });

            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Field", "My Field" },
                                 { "Value", "myvalue" }
                             });

            try
            {
                steps.WhenIEnterDataInFieldsStep(table);
            }
            catch (ElementExecuteException ex)
            {
                StringAssert.Contains(ex.Message, "Could Not Find Field: mysecondfield");

                scenarioContext.VerifyAll();
                pipelineService.VerifyAll();
                throw;
            }
        }

        /// <summary>
        /// Tests the WhenIEnterDataInFieldsStep method with a successful result.
        /// </summary>
        [TestMethod]
        public void TestWhenIEnterDataInFieldStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(
                    p => p.PerformAction<EnterDataAction>(testPage.Object, It.Is<EnterDataAction.EnterDataContext>(c => c.PropertyName == "myfield" && c.Data == "myvalue")))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            steps.WhenIEnterDataInFieldStep("myvalue", "My Field");

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the WhenIEnterDataInFieldsStep method with a failed result.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestWhenIEnterDataInFieldStepWithFailure()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(
                    p => p.PerformAction<EnterDataAction>(testPage.Object, It.Is<EnterDataAction.EnterDataContext>(c => c.PropertyName == "myfield" && c.Data == "myvalue")))
                .Returns(ActionResult.Failure(new ElementExecuteException("Could Not Find Field: myfield")));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            try
            {
                steps.WhenIEnterDataInFieldStep("myvalue", "My Field");
            }
            catch (ElementExecuteException ex)
            {
                StringAssert.Contains(ex.Message, "Could Not Find Field: myfield");

                scenarioContext.VerifyAll();
                pipelineService.VerifyAll();
                throw;
            }
        }

        /// <summary>
        /// Tests the WhenIClearDataInFieldsStep method with a null table.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestWhenIClearDataInFieldsStepNullTable()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.WhenIClearDataInFieldsStep(null),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    scenarioContext.VerifyAll();
                    pipelineService.VerifyAll();
                });
        }

        /// <summary>
        /// Tests the WhenIClearDataInFieldsStep method with a table that has no field column.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestWhenIClearDataInFieldsStepMissingFieldColumn()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Notes");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.WhenIClearDataInFieldsStep(table),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    scenarioContext.VerifyAll();
                    pipelineService.VerifyAll();
                });
        }

        /// <summary>
        /// Tests the WhenIClearDataInFieldsStep method with a successful result.
        /// </summary>
        [TestMethod]
        public void TestWhenIClearDataInFieldsStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(
                p => p.PerformAction<ClearDataAction>(testPage.Object, It.Is<ClearDataAction.ClearDataContext>(c => c.PropertyName == "myfield")))
                            .Returns(ActionResult.Successful());


            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field");
            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Field", "My Field" }
                             });

            steps.WhenIClearDataInFieldsStep(table);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the WhenIClearDataInFieldsStep method with a successful result.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestWhenIClearDataInFieldsStepMultipleEntriesWithFailure()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(
                p => p.PerformAction<ClearDataAction>(testPage.Object, It.Is<ClearDataAction.ClearDataContext>(c => c.PropertyName == "myfield")))
                            .Returns(ActionResult.Successful());
            pipelineService.Setup(
                p => p.PerformAction<ClearDataAction>(testPage.Object, It.Is<ClearDataAction.ClearDataContext>(c => c.PropertyName == "mysecondfield")))
                            .Returns(ActionResult.Failure(new ElementExecuteException("Could Not Find Field: mysecondfield")));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field");
            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Field", "My Second Field" }
                             });

            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Field", "My Field" }
                             });

            try
            {
                steps.WhenIClearDataInFieldsStep(table);
            }
            catch (ElementExecuteException ex)
            {
                StringAssert.Contains(ex.Message, "Could Not Find Field: mysecondfield");

                scenarioContext.VerifyAll();
                pipelineService.VerifyAll();
                throw;
            }
        }

        /// <summary>
        /// Tests the ThenISeeStep method with a null table.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestThenISeeStepNullTable()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.ThenISeeStep(null),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    scenarioContext.VerifyAll();
                    pipelineService.VerifyAll();
                });
        }

        /// <summary>
        /// Tests the ThenISeeStep method with a table that has no field column.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestThenISeeStepMissingFieldColumn()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Value");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.ThenISeeStep(table),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    scenarioContext.VerifyAll();
                    pipelineService.VerifyAll();
                });
        }

        /// <summary>
        /// Tests the ThenISeeStep method with a table that has no field column.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestThenISeeStepMissingValueColumn()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.ThenISeeStep(table),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    scenarioContext.VerifyAll();
                    pipelineService.VerifyAll();
                });
        }

        /// <summary>
        /// Tests the ThenISeeStep method with a table that has no rule column.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestThenISeeStepMissingRuleColumn()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Value");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.ThenISeeStep(table),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    scenarioContext.VerifyAll();
                    pipelineService.VerifyAll();
                });
        }

        /// <summary>
        /// Tests the ThenISeeStep method with headers but no rows just exists.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestThenISeeStepEmptyTableRunsCorrectly()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Rule", "Value");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.ThenISeeStep(table),
                e =>
                {
                    Assert.AreEqual(e.Message, "The validation table must contain at least one validation row.");

                    scenarioContext.VerifyAll();
                    pipelineService.VerifyAll();
                });
        }

        /// <summary>
        /// Tests the ThenISeeStep method with a successful result.
        /// </summary>
        [TestMethod]
        public void TestThenISeeStepEqualsComparison()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<ValidateItemAction>(testPage.Object,
                It.Is<ValidateItemAction.ValidateItemContext>(
                c => c.ValidationTable.Validations.All(v => v.RawFieldName == "myfield" && v.RawComparisonValue == "myvalue" && v.RawComparisonType == "equals"))))
                           .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Rule", "Value");
            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Field", "myfield" },
                                 { "Rule", "equals" },
                                 { "Value", "myvalue" }
                             });

            steps.ThenISeeStep(table);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();

        }

        /// <summary>
        /// Tests the ThenISeeStep method multiple comparisons in the table.
        /// </summary>
        [TestMethod]
        public void TestThenISeeStepMultipleComparisons()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<ValidateItemAction>(testPage.Object,
                It.Is<ValidateItemAction.ValidateItemContext>(c =>
                    c.ValidationTable.Validations.Any(v => v.RawFieldName == "myfield" && v.RawComparisonValue == "myvalue" && v.RawComparisonType == "equals") &&
                    c.ValidationTable.Validations.Any(v => v.RawFieldName == "myotherfield" && v.RawComparisonValue == "somevalue" && v.RawComparisonType == "equals"))))
                           .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Rule", "Value");
            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Field", "myfield" },
                                 { "Rule", "equals" },
                                 { "Value", "myvalue" }
                             });

            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Field", "myotherfield" },
                                 { "Rule", "equals" },
                                 { "Value", "somevalue" }
                             });

            steps.ThenISeeStep(table);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();

        }

        /// <summary>
        /// Tests the ThenISeeStep method with a not enabled comparison.
        /// </summary>
        [TestMethod]
        public void TestThenISeeStepListRuleScenarios()
        {
            RunStepListScenario("equals", ComparisonType.Equals);
            RunStepListScenario("exists", ComparisonType.Contains);
            RunStepListScenario("contains", ComparisonType.Contains);
            RunStepListScenario("does not contain", ComparisonType.DoesNotContain);
            RunStepListScenario("does not exist", ComparisonType.DoesNotContain);
            RunStepListScenario("starts with", ComparisonType.StartsWith);
            RunStepListScenario("ends with", ComparisonType.EndsWith);
        }

        /// <summary>
        /// Runs the step list with an invalid rule type.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestThenISeeStepListInvalidRuleType()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Rule", "Value");
            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Field", "myfield" },
                                 { "Rule", "equals" },
                                 { "Value", "myvalue" }
                             });

            ExceptionHelper.SetupForException<InvalidOperationException>(
                () => steps.ThenISeeListStep("myfield", "invalid", table),
                ex =>
                {
                    scenarioContext.VerifyAll();
                    pipelineService.VerifyAll();
                });
        }

        /// <summary>
        /// Runs the step list with no table rows.
        /// </summary>
        [TestMethod]
        public void TestThenISeeStepListNoRows()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Rule", "Value");

            steps.ThenISeeListStep("myfield", "contains", table);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the row count step with expected parameters.
        /// </summary>
        [TestMethod]
        public void TestThenISeeAListRowCountStepWithExactlyComparison()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<ValidateListRowCountAction>(testPage.Object,
                 It.Is<ValidateListRowCountAction.ValidateListRowCountContext>(
                        c => c.PropertyName == "myfield" && c.CompareType == NumericComparisonType.Equals && c.RowCount == 1)))
                            .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            steps.ThenISeeAListRowCountStep("myfield", "exactly", 1);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the row count step with at most comparison parameter.
        /// </summary>
        [TestMethod]
        public void TestThenISeeAListRowCountStepWithAtMostEvaluation()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<ValidateListRowCountAction>(testPage.Object,
                 It.Is<ValidateListRowCountAction.ValidateListRowCountContext>(
                        c => c.PropertyName == "myfield" && c.CompareType == NumericComparisonType.LessThanEquals && c.RowCount == 1)))
                            .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            steps.ThenISeeAListRowCountStep("myfield", "at most", 1);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the row count step with at least comparison parameter.
        /// </summary>
        [TestMethod]
        public void TestThenISeeAListRowCountStepWithAtLeastEvaluation()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<ValidateListRowCountAction>(testPage.Object,
                 It.Is<ValidateListRowCountAction.ValidateListRowCountContext>(
                        c => c.PropertyName == "myfield" && c.CompareType == NumericComparisonType.GreaterThanEquals && c.RowCount == 1)))
                            .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            steps.ThenISeeAListRowCountStep("myfield", "at least", 1);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Runs the step list scenario.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        private static void RunStepListScenario(string rule, ComparisonType comparisonType)
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<ValidateListAction>(
                testPage.Object,
                It.Is<ValidateListAction.ValidateListContext>(c => c.PropertyName == "myfield" && c.CompareType == comparisonType)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Rule", "Value");
            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Field", "myfield" },
                                 { "Rule", "equals" },
                                 { "Value", "myvalue" }
                             });

            steps.ThenISeeListStep("myfield", rule, table);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the combo box validate step with expected parameters.
        /// </summary>
        [TestMethod]
        public void TestThenISeeComboBoxContainsStepHasNoTable()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            steps.ThenISeeComboBoxContainsStep("myfield", "contains", null);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the combo box validate with expected parameters.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestThenISeeComboBoxContainsStepHasInvalidTable()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Item");
            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Item", "Something Cool" }
                             });

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.ThenISeeComboBoxContainsStep("myfield", "contains", table),
                ex =>
                {
                    Assert.IsTrue(ex.Message.StartsWith("A table must be specified for this step"));

                    scenarioContext.VerifyAll();
                    pipelineService.VerifyAll();
                });
        }

        /// <summary>
        /// Tests the that the combo box contains the list of items.
        /// </summary>
        [TestMethod]
        public void TestThenISeeComboBoxContainsStepContainsItems()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<ValidateComboBoxAction>(
                testPage.Object,
                It.Is<ValidateComboBoxAction.ValidateComboBoxContext>(
                    c => c.PropertyName == "myfield" && c.ComparisonType == ComboComparisonType.Contains && c.Items.Count == 1)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Text");
            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Text", "Something Cool" }
                             });

            steps.ThenISeeComboBoxContainsStep("myfield", "contains", table);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the that the combo box does not contain the list of items.
        /// </summary>
        [TestMethod]
        public void TestThenISeeComboBoxContainsStepDoesNotContainItems()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<ValidateComboBoxAction>(
                testPage.Object,
                It.Is<ValidateComboBoxAction.ValidateComboBoxContext>(
                    c => c.PropertyName == "myfield" && c.ComparisonType == ComboComparisonType.DoesNotContain && c.Items.Count == 1)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Value");
            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Value", "1" }
                             });

            steps.ThenISeeComboBoxContainsStep("myfield", "does not contain", table);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the that the combo box does contains exactly the list of items.
        /// </summary>
        [TestMethod]
        public void TestThenISeeComboBoxContainsStepContainsExactlyItems()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<ValidateComboBoxAction>(
                testPage.Object,
                It.Is<ValidateComboBoxAction.ValidateComboBoxContext>(
                    c => c.PropertyName == "myfield" && c.ComparisonType == ComboComparisonType.ContainsExactly && c.Items.Count == 1)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            var table = new Table("Text");
            table.AddRow(new Dictionary<string, string>
                             {
                                 { "Text", "Something Cool" }
                             });

            steps.ThenISeeComboBoxContainsStep("myfield", "contains exactly", table);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }
    }
}