// <copyright file="CommonPageStepsFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using System.Linq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using SpecBind.Tests.Support;

    using TechTalk.SpecFlow;

    /// <summary>
    ///     A test fixture for common page steps.
    /// </summary>
    [TestClass]
    public class CommonPageStepsFixture
    {
        /// <summary>
        /// Tests the GivenNavigateToPageStep with a successful result.
        /// </summary>
        [TestMethod]
        public void TestGivenNavigateToPageStep()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

            var testPage = new Mock<IPage>();
            

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.GoToPage(typeof(TestBase), null)).Returns(testPage.Object);


            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("mypage")).Returns(typeof(TestBase));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.SetValue(It.IsAny<IPage>(), PageStepBase.CurrentPageKey));

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            steps.GivenNavigateToPageStep("mypage");

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();
            
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the GivenNavigateToPageStep with a successful result.
        /// </summary>
        [TestMethod]
        public void TestGivenNavigateToPageStepWithArguments()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

            var testPage = new Mock<IPage>();
            

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.GoToPage(typeof(TestBase), It.Is<IDictionary<string, string>>(d => d.Count == 2))).Returns(testPage.Object);


            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("mypage")).Returns(typeof(TestBase));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.SetValue(It.IsAny<IPage>(), PageStepBase.CurrentPageKey));

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            var table = new Table("Id", "Part");
            table.AddRow("1", "A");

            steps.GivenNavigateToPageWithArgumentsStep("mypage", table);

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();
            
        }

        /// <summary>
        /// Tests the GivenNavigateToPageStep with the page type not being found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(PageNavigationException))]
        public void TestGivenNavigateToPageStepTypeNotFound()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("mypage")).Returns((Type)null);

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            try
            {
                steps.GivenNavigateToPageStep("mypage");
            }
            catch (PageNavigationException ex)
            {
                StringAssert.Contains(ex.Message, "mypage");

                browser.VerifyAll();
                pageMapper.VerifyAll();
                scenarioContext.VerifyAll();
                

                throw;
            }
        }

        /// <summary>
        /// Tests the GivenEnsureOnPageStep with a successful result.
        /// </summary>
        [TestMethod]
        public void TestGivenEnsureOnPageStep()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

            var testPage = new Mock<IPage>();
            

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Page(typeof(TestBase))).Returns(testPage.Object);
            browser.Setup(b => b.EnsureOnPage(testPage.Object));


            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("mypage")).Returns(typeof(TestBase));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.SetValue(It.IsAny<IPage>(), PageStepBase.CurrentPageKey));

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            steps.GivenEnsureOnPageStep("mypage");

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();
            
        }

        /// <summary>
        /// Tests the GivenEnsureOnPageStep with the page type not being found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(PageNavigationException))]
        public void TestGivenEnsureOnPageStepTypeNotFound()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("mypage")).Returns((Type)null);

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            try
            {
                steps.GivenEnsureOnPageStep("mypage");
            }
            catch (PageNavigationException ex)
            {
                StringAssert.Contains(ex.Message, "mypage");

                browser.VerifyAll();
                pageMapper.VerifyAll();
                scenarioContext.VerifyAll();
                

                throw;
            }
        }

        /// <summary>
        /// Tests the GivenEnsureOnPageStep with the page not existing found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(PageNavigationException))]
        public void TestGivenEnsureOnPageStepPageNotFound()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

            
            var testPage = new Mock<IPage>();

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Page(typeof(TestBase))).Returns(testPage.Object);
            browser.Setup(b => b.EnsureOnPage(testPage.Object)).Throws(new PageNavigationException("Page Not found"));


            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("mypage")).Returns(typeof(TestBase));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            try
            {
                steps.GivenEnsureOnPageStep("mypage");
            }
            catch (PageNavigationException)
            {
                browser.VerifyAll();
                pageMapper.VerifyAll();
                scenarioContext.VerifyAll();
                

                throw;
            }
        }

        /// <summary>
        /// Tests the WhenIChooseALinkStep method with a successful result.
        /// </summary>
        [TestMethod]
        public void TestWhenIChooseALinkStep()
        {
            
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<ButtonClickAction>(testPage.Object, It.Is<ActionContext>(c => c.PropertyName == "mylink")))
                           .Returns(ActionResult.Successful());

            var browser = new Mock<IBrowser>(MockBehavior.Strict);


            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            steps.WhenIChooseALinkStep("my link");

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();
            
        }

        /// <summary>
        /// Tests the WhenIChooseALinkStep method when a step has not been set.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(PageNavigationException))]
        public void TestWhenIChooseALinkStepContextNotSet()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns((IPage)null);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            ExceptionHelper.SetupForException<PageNavigationException>(
                () => steps.WhenIChooseALinkStep("my link"),
                e =>
                {
                    browser.VerifyAll();
                    pageMapper.VerifyAll();
                    scenarioContext.VerifyAll();
                    
                });
        }

        /// <summary>
        /// Tests the WhenIEnterDataInFieldsStep method with a null table.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestWhenIEnterDataInFieldsStepNullTable()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.WhenIEnterDataInFieldsStep(null),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    browser.VerifyAll();
                    pageMapper.VerifyAll();
                    scenarioContext.VerifyAll();
                    
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

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            var table = new Table("Value");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.WhenIEnterDataInFieldsStep(table),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    browser.VerifyAll();
                    pageMapper.VerifyAll();
                    scenarioContext.VerifyAll();
                    
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

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.WhenIEnterDataInFieldsStep(table),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    browser.VerifyAll();
                    pageMapper.VerifyAll();
                    scenarioContext.VerifyAll();
                    
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

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Value");
            table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "My Field" },
								 { "Value", "myvalue" }
				             });

            steps.WhenIEnterDataInFieldsStep(table);

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();
            
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

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

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

                browser.VerifyAll();
                pageMapper.VerifyAll();
                scenarioContext.VerifyAll();
                

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

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.ThenISeeStep(null),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    browser.VerifyAll();
                    pageMapper.VerifyAll();
                    scenarioContext.VerifyAll();
                    
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

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            var table = new Table("Value");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.ThenISeeStep(table),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    browser.VerifyAll();
                    pageMapper.VerifyAll();
                    scenarioContext.VerifyAll();
                    
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

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.ThenISeeStep(table),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    browser.VerifyAll();
                    pageMapper.VerifyAll();
                    scenarioContext.VerifyAll();
                    
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

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Value");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.ThenISeeStep(table),
                e =>
                {
                    StringAssert.Contains(e.Message, "A table must be specified for this step");

                    browser.VerifyAll();
                    pageMapper.VerifyAll();
                    scenarioContext.VerifyAll();
                    
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

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Rule", "Value");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => steps.ThenISeeStep(table),
                e =>
                {
                    Assert.AreEqual(e.Message, "The validation table must contain at least one validation row.");

                    browser.VerifyAll();
                    pageMapper.VerifyAll();
                    scenarioContext.VerifyAll();

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

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Rule", "Value");
            table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "myfield" },
								 { "Rule", "equals" },
								 { "Value", "myvalue" }
				             });

            steps.ThenISeeStep(table);

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();
            
        }

        /// <summary>
        /// Tests the ThenISeeStep method multiple comparisons in the table.
        /// </summary>
        [TestMethod]
        public void TestThenISeeStepMultipleComparisons()
        {
            var testPage = new Mock<IPage>();
            
            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<ValidateItemAction>(testPage.Object,
                It.Is<ValidateItemAction.ValidateItemContext>(c =>
                    c.ValidationTable.Validations.Any(v => v.RawFieldName == "myfield" && v.RawComparisonValue == "myvalue" && v.RawComparisonType == "equals") &&
                    c.ValidationTable.Validations.Any(v => v.RawFieldName == "myotherfield" && v.RawComparisonValue == "somevalue" && v.RawComparisonType == "equals"))))
                           .Returns(ActionResult.Successful());

            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

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

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();
            
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

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);


            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

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
                    browser.VerifyAll();
                    pageMapper.VerifyAll();
                    scenarioContext.VerifyAll();
                    
                });
        }

        /// <summary>
        /// Runs the step list with no table rows.
        /// </summary>
        [TestMethod]
        public void TestThenISeeStepListNoRows()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Rule", "Value");

            steps.ThenISeeListStep("myfield", "contains", table);

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();
            
        }

        /// <summary>
        /// Tests the GivenEnsureOnDialogStep method for happy path.
        /// </summary>
        [TestMethod]
        public void TestGivenEnsureOnDialogStep()
        {
            var page = new Mock<IPage>();
            var listItem = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<GetElementAsPageAction>(
                page.Object, It.Is<ActionContext>(a => a.PropertyName == "myproperty")))
                           .Returns(ActionResult.Successful(listItem.Object));

            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);


            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(page.Object);
            scenarioContext.Setup(s => s.SetValue(listItem.Object, PageStepBase.CurrentPageKey));

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            steps.GivenEnsureOnDialogStep("my property");

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();
            
        }

        /// <summary>
        /// Tests the SetTokenFromFieldStep method pulls the value from the field and sets the value.
        /// </summary>
        [TestMethod]
        public void TestSetTokenFromFieldStepSetsCurrentValue()
        {
            var page = new Mock<IPage>();

            

            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(
                p => p.PerformAction<SetTokenFromValueAction>(
                    page.Object,
                    It.Is<SetTokenFromValueAction.TokenFieldContext>(c => c.TokenName == "MyToken" && c.PropertyName == "somefield")))
                    .Returns(ActionResult.Successful("The Field Value"));

            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(page.Object);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            steps.SetTokenFromFieldStep("MyToken", "SomeField");

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();
            
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

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            
            
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new CommonPageSteps(browser.Object, pageMapper.Object, scenarioContext.Object, pipelineService.Object);

            var table = new Table("Field", "Rule", "Value");
            table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "myfield" },
								 { "Rule", "equals" },
								 { "Value", "myvalue" }
				             });

            steps.ThenISeeListStep("myfield", rule, table);

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();
            
        }
    }
}