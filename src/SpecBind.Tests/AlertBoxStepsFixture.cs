// <copyright file="AlertBoxStepsFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A set of unit tests for the <see cref="AlertBoxSteps"/>.
    /// </summary>
    [TestClass]
    public class AlertBoxStepsFixture
    {
        /// <summary>
        /// Tests that the see alert and select button calls pipeline action correctly.
        /// </summary>
        [TestMethod]
        public void TestSeeAlertAndSelectButtonCallsPipelineAction()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<DismissDialogAction>(
                testPage.Object,
                It.Is<DismissDialogAction.DismissDialogContext>(c => c.ButtonName == "Ok" && !c.IsTextEntered)))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new AlertBoxSteps(pipelineService.Object, scenarioContext.Object);

            steps.SeeAlertAndSelectButton("Ok");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests that the see alert and select button calls pipeline action correctly.
        /// </summary>
        [TestMethod]
        public void TestSeeAlertEnterTextAndSelectButtonCallsPipelineAction()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<DismissDialogAction>(
                testPage.Object,
                It.Is<DismissDialogAction.DismissDialogContext>(c => c.ButtonName == "Ok" && c.IsTextEntered && c.Text == "Hello!")))
                .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);

            var steps = new AlertBoxSteps(pipelineService.Object, scenarioContext.Object);

            steps.SeeAlertEnterTextAndSelectButton("Hello!", "Ok");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
        }
    }
}