// <copyright file="TokenStepsFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the token steps class.
    /// </summary>
    [TestClass]
    public class TokenStepsFixture
    {
        /// <summary>
        /// Tests the SetTokenFromFieldStep method pulls the value from the field and sets the value.
        /// </summary>
        [TestMethod]
        public void TestSetTokenFromFieldStepSetsCurrentValue()
        {
            var page = new Mock<IPage>();
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(
                p => p.PerformAction<SetTokenFromValueAction>(
                    page.Object,
                    It.Is<SetTokenFromValueAction.TokenFieldContext>(c => c.TokenName == "MyToken" && c.PropertyName == "somefield")))
                    .Returns(ActionResult.Successful("The Field Value"));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(page.Object);

            var steps = new TokenSteps(scenarioContext.Object, pipelineService.Object);

            steps.SetTokenFromFieldStep("MyToken", "SomeField");

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }
    }
}