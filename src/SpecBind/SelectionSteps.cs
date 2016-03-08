// <copyright file="SelectionSteps.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind
{
    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    using TechTalk.SpecFlow;

    /// <summary>
    /// A set of step bindings for selecting an item.
    /// </summary>
    [Binding]
    public class SelectionSteps : PageStepBase
    {
        // Step regex values - in constants because they are shared.
        private const string ChooseALinkStepRegex = @"I choose (.+)";
        private const string EnsureOnListItemRegex = @"I am on list (.+) item ([0-9]+)";
        private const string GoToListItemWithCriteriaRegex = @"I am on (.+) list item matching criteria";

        // The following Regex items are for the given "past tense" form
        private const string GivenChooseALinkStepRegex = @"I chose (.+)";
        private const string GivenEnsureOnListItemRegex = @"I was on list (.+) item ([0-9]+)";
        private const string GivenGoToListItemWithCriteriaRegex = @"I was on (.+) list item matching criteria";

        private readonly IActionPipelineService actionPipelineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageStepBase" /> class.
        /// </summary>
        /// <param name="actionPipelineService">The action pipeline service.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        public SelectionSteps(IActionPipelineService actionPipelineService, IScenarioContextHelper scenarioContext)
            : base(scenarioContext)
        {
            this.actionPipelineService = actionPipelineService;
        }

        /// <summary>
        /// A When step indicating a link click should occur.
        /// </summary>
        /// <param name="linkName">Name of the link.</param>
        [Given(GivenChooseALinkStepRegex)]
        [When(ChooseALinkStepRegex)]
        public void WhenIChooseALinkStep(string linkName)
        {
            var page = this.GetPageFromContext();

            var context = new ActionContext(linkName.ToLookupKey());

            this.actionPipelineService
                    .PerformAction<ButtonClickAction>(page, context)
                    .CheckResult();
        }

        /// <summary>
        /// A Given step for ensuring the browser is on the list item with the specified name and index.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        /// <param name="itemNumber">The item number.</param>
        [Given(GivenEnsureOnListItemRegex)]
        [When(EnsureOnListItemRegex)]
        [Then(EnsureOnListItemRegex)]
        public void GivenEnsureOnListItemStep(string listName, int itemNumber)
        {
            var page = this.GetPageFromContext();

            var context = new GetListItemByIndexAction.ListItemByIndexContext(listName.ToLookupKey(), itemNumber);

            var item = this.actionPipelineService.PerformAction<GetListItemByIndexAction>(page, context)
                                                 .CheckResult<IPage>();

            this.UpdatePageContext(item);
        }

        /// <summary>
        /// A step for ensuring the browser is on the list item with the specified name and criteria.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        /// <param name="criteriaTable">The criteria table.</param>
        [Given(GivenGoToListItemWithCriteriaRegex)]
        [When(GoToListItemWithCriteriaRegex)]
        [Then(GoToListItemWithCriteriaRegex)]
        public void GoToListItemWithCriteriaStep(string listName, Table criteriaTable)
        {
            var page = this.GetPageFromContext();
            var validationTable = criteriaTable.ToValidationTable();

            var context = new GetListItemByCriteriaAction.ListItemByCriteriaContext(listName.ToLookupKey(), validationTable);

            var item = this.actionPipelineService.PerformAction<GetListItemByCriteriaAction>(page, context)
                                                 .CheckResult<IPage>();

            this.UpdatePageContext(item);
        }
    }
}