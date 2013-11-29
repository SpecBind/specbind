// <copyright file="ErrorCheckSteps.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

using TechTalk.SpecFlow;

namespace SpecBind.CodedUI.IntegrationTests.Steps
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.ActionPipeline;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    using Steps = TechTalk.SpecFlow.Steps;

    /// <summary>
    /// A set of steps used to ensure errors actually occur
    /// </summary>
    [Binding]
    public class ErrorCheckSteps : Steps
    {
        private readonly CommonPageSteps commonPageSteps;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCheckSteps" /> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="pageMapper">The page mapper.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="tokenManager">The token manager.</param>
        /// <param name="actionPipelineService">The action pipeline service.</param>
        public ErrorCheckSteps(IBrowser browser, IPageMapper pageMapper, IScenarioContextHelper scenarioContext, ITokenManager tokenManager, IActionPipelineService actionPipelineService)
        {
            this.commonPageSteps = new CommonPageSteps(browser, pageMapper, scenarioContext, tokenManager, actionPipelineService);
        }

        /// <summary>
        /// A when step indicating that invalid data should be entered.
        /// </summary>
        /// <param name="data">The data.</param>
        [When("I enter invalid data")]
        public void WhenIEnterInvalidData(Table data)
        {
            try
            {
                this.commonPageSteps.WhenIEnterDataInFieldsStep(data);
            }
            catch (ElementExecuteException)
            {
                return;
            }

            throw new AssertFailedException("Step should have thrown a ElementExecuteException due to invalid data");
        }
    }
}
