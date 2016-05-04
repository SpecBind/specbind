// <copyright file="WaitingStepsFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;
	using SpecBind.BrowserSupport;
	using BoDi;

    /// <summary>
    /// A test fixture for the <see cref="WaitingSteps"/> step class.
    /// </summary>
    [TestClass]
    public class WaitingStepsFixture
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Tests the default wait property for a 30 second timeout.
        /// </summary>
        [TestMethod]
        public void TestGetDefaultWaitReturnsThirtySeconds()
        {
            var result = WaitingSteps.DefaultWait;

            Assert.AreEqual(TimeSpan.FromSeconds(30), result);
        }

        /// <summary>
        /// Tests the wait for control exists step.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlExistsStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.Exists && c.Timeout == Timeout)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitToSeeElementWithTimeout(10, "My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control exists step with a timeout of 0.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlExistsStepInvalidTimeout()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.Exists && c.Timeout == null)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitToSeeElementWithTimeout(0, "My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control exists step with no timeout specified.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlExistsNoTimeoutStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.Exists && c.Timeout == null)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitToSeeElement("My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control not exists step.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlNotExistsStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.NotExists && c.Timeout == Timeout)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitToNotSeeElementWithTimeout(10, "My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control not exists step with no timeout specified.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlNotExistsNoTimeoutStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.NotExists && c.Timeout == null)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitToNotSeeElement("My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control still exists step.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlStillExistsStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.RemainsExistent && c.Timeout == Timeout)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitToStillSeeElementWithTimeout(10, "My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control not exists step with no timeout specified.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlStillExistsNoTimeoutStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.RemainsExistent && c.Timeout == null)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitToStillSeeElement("My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control remains non-existent step.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlRemainsNonExistentStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.RemainsNonExistent && c.Timeout == Timeout)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitToStillNotSeeElementWithTimeout(10, "My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control remains non-existent step with no timeout specified.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlRemainsNonExistentNoTimeoutStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.RemainsNonExistent && c.Timeout == null)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitToStillNotSeeElement("My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control enabled step.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlEnabledStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.Enabled && c.Timeout == Timeout)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitForElementEnabledWithTimeout(10, "My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control enabled step with no timeout specified.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlEnabledNoTimeoutStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.Enabled && c.Timeout == null)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitForElementEnabled("My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control remains enabled step.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlRemainsEnabledStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.RemainsEnabled && c.Timeout == Timeout)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitForElementStillEnabledWithTimeout(10, "My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control remains enabled step with no timeout specified.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlRemainsEnabledNoTimeoutStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.RemainsEnabled && c.Timeout == null)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitForElementStillEnabled("My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control not enabled step.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlNotEnabledStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.NotEnabled && c.Timeout == Timeout)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitForElementNotEnabledWithTimeout(10, "My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control not enabled step with no timeout specified.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlNotEnabledNoTimeoutStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.NotEnabled && c.Timeout == null)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitForElementNotEnabled("My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control remains disabled step.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlRemainsDisabledStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.RemainsDisabled && c.Timeout == Timeout)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitForElementStillNotEnabledWithTimeout(10, "My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control remains disabled step with no timeout specified.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlRemainsDisabledNoTimeoutStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.RemainsDisabled && c.Timeout == null)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitForElementStillNotEnabled("My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control not moving step.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlNotMovingStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.NotMoving && c.Timeout == Timeout)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitForElementNotMovingWithTimeout(10, "My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for control not moving step with no timeout specified.
        /// </summary>
        [TestMethod]
        public void TestWaitForControlNotMovingNoTimeoutStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForElementAction>(
                testPage.Object,
                It.Is<WaitForElementAction.WaitForElementContext>(c => c.PropertyName == "myfield" && c.Condition == WaitConditions.NotMoving && c.Timeout == null)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitForElementNotMoving("My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for list items step with no timeout specified.
        /// </summary>
        [TestMethod]
        public void TestWaitForListItemsNoTimeoutStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForListItemsAction>(
                testPage.Object,
                It.Is<WaitForListItemsAction.WaitForListItemsContext>(c => c.PropertyName == "myfield" && c.Timeout == null)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitForListElementToContainItems("My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the wait for list items step with a timeout specified.
        /// </summary>
        [TestMethod]
        public void TestWaitForListItemsWithTimeoutStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForListItemsAction>(
                testPage.Object,
                It.Is<WaitForListItemsAction.WaitForListItemsContext>(c => c.PropertyName == "myfield" && c.Timeout == TimeSpan.FromSeconds(3))))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitForListElementToContainItemsWithTimeout(3, "My Field");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the GivenIWaitForTheViewToBeActive with a successful result.
        /// </summary>
        [TestMethod]
        public void TestGivenIWaitForViewToBeActiveStep()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

            var testPage = new Mock<IPage>(MockBehavior.Strict);
            testPage.Setup(t => t.WaitForPageToBeActive());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(testPage.Object);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

            steps.WaitForTheViewToBeActive();

            testPage.VerifyAll();
            scenarioContext.VerifyAll();
        }

		/// <summary>
		/// Tests the IWaitForAngularAjaxCallsToComplete with a successful result, when nothing is pending.
		/// </summary>
		[TestMethod]
		public void TestIWaitForAngularAjaxCallsToCompleteStepWithNothingPending()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(s => s.IsClosed).Returns(false);
			browser.Setup(s => s.IsDisposed).Returns(false);
			browser.Setup(s => s.Url).Returns("http://www.specbind.org");
			browser.Setup(s => s.ExecuteScript(It.IsAny<string>())).Returns("0");
			WebDriverSupport.Browser = browser.Object;

			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

			steps.WaitForAngular();

			browser.VerifyAll();
		}

		/// <summary>
		/// Tests the IWaitForAngularAjaxCallsToComplete with a successful result, when something is pending.
		/// </summary>
		[TestMethod]
		public void TestIWaitForAngularAjaxCallsToCompleteStepWithSomethingPending()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(s => s.IsClosed).Returns(false);
			browser.Setup(s => s.IsDisposed).Returns(false);
			browser.Setup(s => s.Url).Returns("http://www.specbind.org");
			browser.SetupSequence(s => s.ExecuteScript(It.IsAny<string>()))
				.Returns("1")
				.Returns("0");
			WebDriverSupport.Browser = browser.Object;

			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

			steps.WaitForAngular();

			browser.VerifyAll();
		}

		/// <summary>
		/// Tests the IWaitForjQueryAjaxCallsToComplete with a successful result, when nothing is pending.
		/// </summary>
		[TestMethod]
		public void TestIWaitForjQueryAjaxCallsToCompleteStepWithNothingPending()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(s => s.IsClosed).Returns(false);
			browser.Setup(s => s.IsDisposed).Returns(false);
			browser.Setup(s => s.Url).Returns("http://www.specbind.org");
			browser.Setup(s => s.ExecuteScript(It.IsAny<string>())).Returns("0");
			WebDriverSupport.Browser = browser.Object;

			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

			steps.WaitForjQuery();

			browser.VerifyAll();
		}

		/// <summary>
		/// Tests the IWaitForjQueryAjaxCallsToComplete with a successful result, when something is pending.
		/// </summary>
		[TestMethod]
		public void TestIWaitForjQueryAjaxCallsToCompleteStepWithSomethingPending()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(s => s.IsClosed).Returns(false);
			browser.Setup(s => s.IsDisposed).Returns(false);
			browser.Setup(s => s.Url).Returns("http://www.specbind.org");
			browser.SetupSequence(s => s.ExecuteScript(It.IsAny<string>()))
				.Returns("1")
				.Returns("0");
			WebDriverSupport.Browser = browser.Object;

			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);

			steps.WaitForjQuery();

			browser.VerifyAll();
		}
    }
}