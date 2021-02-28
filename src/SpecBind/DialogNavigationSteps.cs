// <copyright file="DialogNavigationSteps.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind
{
    using System;
    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Dialog Navigation Steps.
    /// </summary>
    /// <seealso cref="SpecBind.PageStepBase" />
    [Binding]
    public class DialogNavigationSteps : PageStepBase
    {
        private readonly IActionPipelineService actionPipelineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogNavigationSteps" /> class.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="actionPipelineService">The action pipeline service.</param>
        /// <param name="logger">The logger.</param>
        public DialogNavigationSteps(
            IScenarioContextHelper scenarioContext,
            IActionPipelineService actionPipelineService,
            ILogger logger)
            : base(scenarioContext, logger)
        {
            this.actionPipelineService = actionPipelineService;
        }

        /// <summary>
        /// Given I waited for the other dialog.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Given(@"I waited for the (.+) dialog")]
        [When(@"I wait for the (.+) dialog")]
        public void WaitForDialogStep(string propertyName)
        {
            var currentPage = this.GetPageFromContext();

            var context = new WaitForActionBase.WaitForActionBaseContext(propertyName.ToLookupKey(), null);
            var page = this.actionPipelineService
                .PerformAction<DialogNavigationAction>(currentPage, context)
                .CheckResult<IPage>();

            this.UpdatePageContext(page);
        }

        /// <summary>
        /// Given I waited 30 seconds for the Test dialog.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <param name="propertyName">Name of the property.</param>
        [Given(@"I waited (\d+) seconds? for the (.+) dialog")]
        public void WaitForDialogWithTimeoutStep(int seconds, string propertyName)
        {
            var currentPage = this.GetPageFromContext();

            var timeout = seconds > 0 ? TimeSpan.FromSeconds(seconds) : (TimeSpan?)null;

            var context = new WaitForActionBase.WaitForActionBaseContext(propertyName.ToLookupKey(), timeout);
            var page = this.actionPipelineService
                .PerformAction<WaitForDialogAction>(currentPage, context)
                .CheckResult<IPage>();

            this.UpdatePageContext(page);
        }

        /// <summary>
        /// Given I waited for the "dialog" dialog to close.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Given(@"I waited for the (.+) dialog to close")]
        [When(@"I wait for the (.+) dialog to close")]
        [Then(@"the (.+) dialog is closed")]
        public void GivenIWaitedForTheDialogToClose(string propertyName)
        {
            var currentPage = this.GetPageFromContext();

            var context = new WaitForActionBase.WaitForActionBaseContext(propertyName.ToLookupKey(), null)
            {
                Page = currentPage
            };

            var page = this.actionPipelineService
                .PerformAction<DialogCloseAction>(currentPage, context)
                .CheckResult<IPage>();

            this.UpdatePageContext(page);
        }
    }
}
