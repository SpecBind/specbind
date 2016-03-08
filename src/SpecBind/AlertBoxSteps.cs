// <copyright file="AlertBoxSteps.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind
{
    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;

    using TechTalk.SpecFlow;

    /// <summary>
    /// A set of SpecFlow steps that assist with browser dialog manipulation.
    /// </summary>
    [Binding]
    public class AlertBoxSteps : PageStepBase
    {
        // Step regex values - in constants because they are shared.
        private const string SeeAlertAndSelectButtonRegex = @"I see an alert box and select (.+)";
        private const string SeeAlertEnterTextAndSelectButtonRegex = @"I see an alert box, enter ""(.+)"" and select (.+)";

        // The following Regex items are for the given "past tense" form
        private const string GivenSeeAlertAndSelectButtonRegex = @"I saw an alert box and selected (.+)";
        private const string GivenSeeAlertEnterTextAndSelectButtonRegex = @"I saw an alert box, entered ""(.+)"" and selected (.+)";

        private readonly IActionPipelineService actionPipelineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertBoxSteps"/> class.
        /// </summary>
        /// <param name="actionPipelineService">The action pipeline service.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        public AlertBoxSteps(IActionPipelineService actionPipelineService, IScenarioContextHelper scenarioContext)
            : base(scenarioContext)
        {
            this.actionPipelineService = actionPipelineService;
        }

        /// <summary>
        /// A step for seeing a dialog box and selecting an action.
        /// </summary>
        /// <param name="buttonName">Name of the button.</param>
        [Given(GivenSeeAlertAndSelectButtonRegex)]
        [When(SeeAlertAndSelectButtonRegex)]
        public void SeeAlertAndSelectButton(string buttonName)
        {
            var context = new DismissDialogAction.DismissDialogContext(buttonName);
            this.CallDialogAction(context);
        }

        /// <summary>
        /// A step for seeing a dialog box, entering some text and selecting an action.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="buttonName">Name of the button.</param>
        [Given(GivenSeeAlertEnterTextAndSelectButtonRegex)]
        [When(SeeAlertEnterTextAndSelectButtonRegex)]
        public void SeeAlertEnterTextAndSelectButton(string text, string buttonName)
        {
            var context = new DismissDialogAction.DismissDialogContext(buttonName, text);
            this.CallDialogAction(context);
        }

        /// <summary>
        /// Calls the dialog action in the pipeline service.
        /// </summary>
        /// <param name="context">The context.</param>
        private void CallDialogAction(ActionContext context)
        {
            var page = this.GetPageFromContext();
            this.actionPipelineService.PerformAction<DismissDialogAction>(page, context).CheckResult();
        }
    }
}