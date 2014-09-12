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

    /// <summary>
    /// A test fixture for the <see cref="WaitingSteps"/> step class.
    /// </summary>
    [TestClass]
    public class WaitingStepsFixture
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

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
    }
}