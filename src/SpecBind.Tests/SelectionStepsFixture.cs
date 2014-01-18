// <copyright file="SelectionStepsFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    using TechTalk.SpecFlow;

    /// <summary>
    /// A test fixture for a steps involving selection.
    /// </summary>
    [TestClass]
    public class SelectionStepsFixture
    {
        /// <summary>
        /// Tests the GivenEnsureOnListItemStep method for common path.
        /// </summary>
        [TestMethod]
        public void TestGivenEnsureOnListItemStep()
        {
            var page = new Mock<IPage>();

            var listItem = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<GetListItemByIndexAction>(
                page.Object, It.Is<GetListItemByIndexAction.ListItemByIndexContext>(c => c.PropertyName == "myproperty" && c.ItemNumber == 2)))
                           .Returns(ActionResult.Successful(listItem.Object));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(page.Object);
            scenarioContext.Setup(s => s.SetValue(listItem.Object, PageStepBase.CurrentPageKey));

            var steps = new SelectionSteps(pipelineService.Object, scenarioContext.Object);

            steps.GivenEnsureOnListItemStep("my property", 2);

            listItem.VerifyAll();
            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the GoToListItemWithCriteriaStep method for expected path.
        /// </summary>
        [TestMethod]
        public void TestGoToListItemWithCriteriaStep()
        {
            var page = new Mock<IPage>();

            var listItem = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<GetListItemByCriteriaAction>(
                page.Object, 
                It.Is<GetListItemByCriteriaAction.ListItemByCriteriaContext>(
                    c => c.PropertyName == "myproperty" && c.ValidationTable.ValidationCount == 1)))
                           .Returns(ActionResult.Successful(listItem.Object));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetValue<IPage>(PageStepBase.CurrentPageKey)).Returns(page.Object);
            scenarioContext.Setup(s => s.SetValue(listItem.Object, PageStepBase.CurrentPageKey));

            var table = new Table("Field", "Rule", "Value");
            table.AddRow("item1", "equals", "foo");

            var steps = new SelectionSteps(pipelineService.Object, scenarioContext.Object);
            steps.GoToListItemWithCriteriaStep("my property", table);

            listItem.VerifyAll();
            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }
    }
}