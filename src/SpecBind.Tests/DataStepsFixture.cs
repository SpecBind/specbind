// <copyright file="DataStepsFixture.cs">
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
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

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
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

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
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

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
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

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
            RunStepListScenario("contains", ComparisonType.Contains);
            RunStepListScenario("does not contain", ComparisonType.DoesNotContain);
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
        public void TestThenISeeAListRowCountStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<ValidateListRowCountAction>(testPage.Object,
                 It.Is<ValidateListRowCountAction.ValidateListRowCountContext>(
                        c => c.PropertyName == "myfield" && c.CompareType == NumericComparisonType.Equals && c.RowCount == 1)))
                            .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new DataSteps(scenarioContext.Object, pipelineService.Object);

            steps.ThenISeeAListRowCountStep("myfield", 1);

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
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

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
    }
}