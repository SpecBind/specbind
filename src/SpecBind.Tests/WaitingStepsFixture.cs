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
        /// Tests the regex to time span conversion.
        /// </summary>
        [TestMethod]
        public void TestRegexToTimeSpanConversion()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);
            var result = steps.TransformWaitTimeToTimeout(10);

            Assert.AreEqual(TimeSpan.FromSeconds(10), result);

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the empty regex to time span conversion.
        /// </summary>
        [TestMethod]
        public void TestEmptyRegexToTimeSpanConversion()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new WaitingSteps(pipelineService.Object, scenarioContext.Object);
            var result = steps.TransformWaitTimeToTimeoutWhenEmpty();

            Assert.AreEqual(null, result);

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
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

            steps.WaitToSeeElement("My Field", Timeout);

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

            steps.WaitToNotSeeElement("My Field", Timeout);

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

            steps.WaitForElementEnabled("My Field", Timeout);

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

            steps.WaitForElementNotEnabled("My Field", Timeout);

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