// <copyright file="PageNavigationStepsFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    using TechTalk.SpecFlow;

    /// <summary>
    ///     A test fixture for common page steps.
    /// </summary>
    [TestClass]
    public class PageNavigationStepsFixture
    {
        private readonly Mock<ILogger> logger = new Mock<ILogger>();

        /// <summary>
        /// Tests the GivenNavigateToPageStep with a successful result.
        /// </summary>
        [TestMethod]
        public void TestGivenNavigateToPageStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<PageNavigationAction>(
                null,
                It.Is<PageNavigationAction.PageNavigationActionContext>(c => c.PropertyName == "mypage" && c.PageAction == PageNavigationAction.PageAction.NavigateToPage && c.PageArguments == null)))
                .Returns(ActionResult.Successful(testPage.Object));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.SetCurrentPage(testPage.Object));

            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

            var steps = new PageNavigationSteps(scenarioContext.Object, pipelineService.Object, tokenManager.Object, this.logger.Object);

            steps.GivenNavigateToPageStep("mypage");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
            testPage.VerifyAll();
        }

        /// <summary>
        /// Tests the GivenNavigateToPageStep with a successful result.
        /// </summary>
        [TestMethod]
        public void TestGivenNavigateToPageStepWithArguments()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<PageNavigationAction>(
                null,
                It.Is<PageNavigationAction.PageNavigationActionContext>(c => c.PropertyName == "mypage" &&
                    c.PageAction == PageNavigationAction.PageAction.NavigateToPage &&
                    c.PageArguments != null && c.PageArguments.Count == 2)))
                .Returns(ActionResult.Successful(testPage.Object));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.SetCurrentPage(It.IsAny<IPage>()));

            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
            tokenManager.Setup(t => t.GetToken(It.IsAny<string>())).Returns<string>(s => s);

            var steps = new PageNavigationSteps(scenarioContext.Object, pipelineService.Object, tokenManager.Object, this.logger.Object);

            var table = new Table("Id", "Part");
            table.AddRow("1", "A");

            steps.GivenNavigateToPageWithArgumentsStep("mypage", table);

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the GivenEnsureOnPageStep with a successful result.
        /// </summary>
        [TestMethod]
        public void TestGivenEnsureOnPageStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<PageNavigationAction>(
                null,
                It.Is<PageNavigationAction.PageNavigationActionContext>(c => c.PropertyName == "mypage" && c.PageAction == PageNavigationAction.PageAction.EnsureOnPage && c.PageArguments == null)))
                .Returns(ActionResult.Successful(testPage.Object));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.SetCurrentPage(testPage.Object));

            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

            var steps = new PageNavigationSteps(scenarioContext.Object, pipelineService.Object, tokenManager.Object, this.logger.Object);

            steps.GivenEnsureOnPageStep("mypage");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
            testPage.VerifyAll();
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
            pipelineService.Setup(p => p.PerformAction<DialogNavigationAction>(
                page.Object, It.Is<WaitForActionBase.WaitForActionBaseContext>(a => a.PropertyName == "myproperty")))
                           .Returns(ActionResult.Successful(listItem.Object));


            var browser = new Mock<IBrowser>(MockBehavior.Strict);


            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(page.Object);
            scenarioContext.Setup(s => s.SetCurrentPage(listItem.Object));

            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

            var steps = new PageNavigationSteps(scenarioContext.Object, pipelineService.Object, tokenManager.Object, this.logger.Object);

            steps.GivenEnsureOnDialogStep("my property");

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();

        }

        /// <summary>
        /// Tests the WaitForPageStep with a successful result.
        /// </summary>
        [TestMethod]
        public void TestWaitForPageStepWhenSuccessful()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForPageAction>(
                null,
                It.Is<WaitForPageAction.WaitForActionBaseContext>(c => c.PropertyName == "mypage" && c.Timeout == null)))
                .Returns(ActionResult.Successful(testPage.Object));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.SetCurrentPage(testPage.Object));

            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

            var steps = new PageNavigationSteps(scenarioContext.Object, pipelineService.Object, tokenManager.Object, this.logger.Object);

            steps.WaitForPageStep("mypage");

            scenarioContext.VerifyAll();
            testPage.VerifyAll();
        }

        /// <summary>
        /// Tests the WaitForPageStep with a failure result, makes sure to clear the context then throw the error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(PageNavigationException))]
        public void TestWaitForPageStepWhenFailsClearsPageContext()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForPageAction>(
                null,
                It.Is<WaitForPageAction.WaitForActionBaseContext>(c => c.PropertyName == "mypage" && c.Timeout == null)))
                .Returns(ActionResult.Failure(new PageNavigationException("Navigation Failed")));

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.SetCurrentPage(null));

            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

            var steps = new PageNavigationSteps(scenarioContext.Object, pipelineService.Object, tokenManager.Object, this.logger.Object);

            ExceptionHelper.SetupForException<PageNavigationException>(() => steps.WaitForPageStep("mypage"),
                e =>
                    {
                        browser.VerifyAll();
                        pageMapper.VerifyAll();
                        scenarioContext.VerifyAll();
                        testPage.VerifyAll();
                    });
        }

        /// <summary>
        /// Tests the WaitForPageStep with a timeout specified and a successful result.
        /// </summary>
        [TestMethod]
        public void TestWaitForPageStepWithTimeoutWhenSuccessful()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForPageAction>(
                null,
                It.Is<WaitForPageAction.WaitForActionBaseContext>(c => c.PropertyName == "mypage" && c.Timeout == TimeSpan.FromSeconds(10))))
                .Returns(ActionResult.Successful(testPage.Object));

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.SetCurrentPage(testPage.Object));

            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

            var steps = new PageNavigationSteps(scenarioContext.Object, pipelineService.Object, tokenManager.Object, this.logger.Object);

            steps.WaitForPageStepWithTimeout(10, "mypage");

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();
            testPage.VerifyAll();
        }

        /// <summary>
        /// Tests the WaitForPageStep with a timeout of zero specified and a successful result.
        /// </summary>
        [TestMethod]
        public void TestWaitForPageStepWithTimeoutWhenTimeoutIsZeroSetsTimeoutToNull()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForPageAction>(
                null,
                It.Is<WaitForPageAction.WaitForActionBaseContext>(c => c.PropertyName == "mypage" && c.Timeout == null)))
                .Returns(ActionResult.Successful(testPage.Object));

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.SetCurrentPage(testPage.Object));

            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

            var steps = new PageNavigationSteps(scenarioContext.Object, pipelineService.Object, tokenManager.Object, this.logger.Object);

            steps.WaitForPageStepWithTimeout(0, "mypage");

            browser.VerifyAll();
            pageMapper.VerifyAll();
            scenarioContext.VerifyAll();
            testPage.VerifyAll();
        }
    }
}