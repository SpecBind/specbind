// <copyright file="PageNavigationSteps.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    using TechTalk.SpecFlow;

    /// <summary>
    /// A set of common step bindings that drive the underlying fixtures.
    /// </summary>
    [Binding]
    public class PageNavigationSteps : PageStepBase
    {
        // Step regex values - in constants because they are shared.
        private const string EnsureOnPageStepRegex = @"I am on the (.+) (?:page|window)";
        private const string EnsureOnDialogStepRegex = @"I am on the (.+) dialog";
        private const string NavigateToPageStepRegex = @"I navigate to the (.+) page";
        private const string NavigateToPageWithParamsStepRegex = @"I navigate to the (.+) page with parameters";
        private const string WaitForPageStepRegex = @"I wait for the (.+) (?:page|window)";
        private const string WaitForPageWithTimeoutStepRegex = @"I wait (\d+) seconds? for the (.+) (?:page|window)";

        // The following Regex items are for the given "past tense" form
        private const string GivenEnsureOnPageStepRegex = @"I was on the (.+) (?:page|window)";
        private const string GivenEnsureOnDialogStepRegex = @"I was on the (.+) dialog";
        private const string GivenNavigateToPageStepRegex = @"I navigated to the (.+) page";
        private const string GivenNavigateToPageWithParamsStepRegex = @"I navigated to the (.+) page with parameters";
        private const string GivenWaitForPageStepRegex = @"I waited for the (.+) (?:page|window)";
        private const string GivenWaitForPageWithTimeoutStepRegex = @"I waited (\d+) seconds? for the (.+) (?:page|window)";

        private readonly IActionPipelineService actionPipelineService;
        private readonly ITokenManager tokenManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageNavigationSteps" /> class.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="actionPipelineService">The action pipeline service.</param>
        /// <param name="tokenManager">The token manager.</param>
        /// <param name="logger">The logger.</param>
        public PageNavigationSteps(
            IScenarioContextHelper scenarioContext,
            IActionPipelineService actionPipelineService,
            ITokenManager tokenManager,
            ILogger logger)
            : base(scenarioContext, logger)
        {
            this.actionPipelineService = actionPipelineService;
            this.tokenManager = tokenManager;
        }

        /// <summary>
        /// A Given step for ensuring the browser is on the page with the specified name.
        /// </summary>
        /// <param name="pageName">The page name.</param>
        [Given(GivenEnsureOnPageStepRegex)]
        [When(EnsureOnPageStepRegex)]
        [Then(EnsureOnPageStepRegex)]
        public void GivenEnsureOnPageStep(string pageName)
        {
            var context = new PageNavigationAction.PageNavigationActionContext(pageName, PageNavigationAction.PageAction.EnsureOnPage);
            var page = this.actionPipelineService.PerformAction<PageNavigationAction>(null, context)
                                                 .CheckResult<IPage>();

            this.UpdatePageContext(page);
        }

        /// <summary>
        /// Given I was on the "property name" section.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Given(@"I was on the (.+) section")]
        [When(@"I am on the (.+) section")]
        public void GivenIWasOnTheSection(string propertyName)
        {
            var page = this.GetPageFromContext();

            var context = new ActionContext(propertyName.ToLookupKey());
            var item = this.actionPipelineService.PerformAction<GetElementAsPageAction>(page, context)
                                                 .CheckResult<IPage>();

            this.UpdatePageContext(item);
        }

        /// <summary>
        /// Given I was on the "property name" context.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Given(@"I was on the (.+) context")]
        [When(@"I am on the (.+) context")]
        [Then(@"I am on the (.+) context")]
        public void GivenIWasOnTheContext(string propertyName)
        {
            var page = this.GetPageFromContext();

            var context = new GetElementAsContextInPageAction.GetElementAsContextInPageActionContext(page, propertyName.ToLookupKey());
            var item = this.actionPipelineService.PerformAction<GetElementAsContextInPageAction>(page, context)
                                                 .CheckResult<IPage>();

            this.UpdatePageContext(item);
        }

        /// <summary>
        /// A Given step for ensuring the browser is on the dialog which is a sub-element of the page.
        /// </summary>
        /// <param name="propertyName">Name of the property that represents the dialog.</param>
        [Given(GivenEnsureOnDialogStepRegex)]
        [When(EnsureOnDialogStepRegex)]
        [Then(EnsureOnDialogStepRegex)]
        public void GivenEnsureOnDialogStep(string propertyName)
        {
            var currentPage = this.GetPageFromContext();

            var context = new WaitForActionBase.WaitForActionBaseContext(propertyName.ToLookupKey(), null);
            var page = this.actionPipelineService
                .PerformAction<DialogNavigationAction>(currentPage, context)
                .CheckResult<IPage>();

            this.UpdatePageContext(page);
        }

        /// <summary>
        /// A Given step for navigating to a page with the specified name.
        /// </summary>
        /// <param name="pageName">The page name.</param>
        [Given(GivenNavigateToPageStepRegex)]
        [When(NavigateToPageStepRegex)]
        [Then(NavigateToPageStepRegex)]
        public void GivenNavigateToPageStep(string pageName)
        {
            this.GivenNavigateToPageWithArgumentsStep(pageName, null);
        }

        /// <summary>
        /// Given I navigated back.
        /// </summary>
        [Given("I navigated back")]
        public void GivenNavigatedBackStep()
        {
            var context = new PageNavigationAction.PageNavigationActionContext(PageNavigationAction.PageAction.NavigateBack);
            this.actionPipelineService.PerformAction<PageNavigationAction>(null, context);
        }

        /// <summary>
        /// A Given step for navigating to a page with the specified name and url parameters.
        /// </summary>
        /// <param name="pageName">The page name.</param>
        /// <param name="pageArguments">The page arguments.</param>
        [Given(GivenNavigateToPageWithParamsStepRegex)]
        [When(NavigateToPageWithParamsStepRegex)]
        [Then(NavigateToPageWithParamsStepRegex)]
        public void GivenNavigateToPageWithArgumentsStep(string pageName, Table pageArguments)
        {
            Dictionary<string, string> args = null;
            if (pageArguments != null && pageArguments.RowCount > 0)
            {
                args = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                var row = pageArguments.Rows[0];
                foreach (var header in pageArguments.Header.Where(h => !args.ContainsKey(h)))
                {
                    var value = this.tokenManager.GetToken(row[header]);
                    args.Add(header, value);
                }
            }

            var context = new PageNavigationAction.PageNavigationActionContext(pageName, PageNavigationAction.PageAction.NavigateToPage, args);
            var page = this.actionPipelineService.PerformAction<PageNavigationAction>(null, context)
                                                 .CheckResult<IPage>();

            this.UpdatePageContext(page);
        }

        /// <summary>
        /// A step that waits for a page to become visible.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        [Given(GivenWaitForPageStepRegex)]
        [When(WaitForPageStepRegex)]
        [Then(WaitForPageStepRegex)]
        public void WaitForPageStep(string pageName)
        {
            this.CallWaitForPageAction(pageName, null);
        }

        /// <summary>
        /// A step to maximize the current window.
        /// </summary>
        [Given("I maximized the current window")]
        public void MaximizeWindowStep()
        {
            this.actionPipelineService.PerformAction<MaximizeWindowAction>(null, null);
        }

        /// <summary>
        /// A step that waits for a page to become visible with a timeout.
        /// </summary>
        /// <param name="seconds">The seconds to wait for the page to appear.</param>
        /// <param name="pageName">Name of the page.</param>
        [Given(GivenWaitForPageWithTimeoutStepRegex)]
        [When(WaitForPageWithTimeoutStepRegex)]
        [Then(WaitForPageWithTimeoutStepRegex)]
        public void WaitForPageStepWithTimeout(int seconds, string pageName)
        {
            var timeout = seconds > 0 ? TimeSpan.FromSeconds(seconds) : (TimeSpan?)null;
            this.CallWaitForPageAction(pageName, timeout);
        }

        /// <summary>
        /// When I switched to the %page% page.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        [Given(@"I switched to the (.+) page")]
        [When(@"I switch to the (.+) page")]
        public void WhenISwitchedToThePage(string pageName)
        {
            var context = new WaitForActionBase.WaitForActionBaseContext(pageName, null);
            IPage page = this.actionPipelineService.PerformAction<SwitchWindowAction>(null, context)
                .CheckResult<IPage>();

            this.UpdatePageContext(page);
        }

        /// <summary>
        /// Then the query string has the following parameters.
        /// </summary>
        /// <param name="table">The table.</param>
        [Then(@"the query string has the following parameters")]
        public void ThenTheQueryStringHasTheFollowingParameters(Table table)
        {
            Dictionary<string, string> pageParameters = table.Rows.ToDictionary(r => r[0], r => r[1]);

            var context = new ValidatePageParametersAction.ValidatePageParametersActionContext(
                ValidatePageParametersAction.PageParameterValidationAction.Contains,
                pageParameters);
            var result = this.actionPipelineService
                .PerformAction<ValidatePageParametersAction>(null, context);

            result.CheckResult();
        }

        /// <summary>
        /// Then the query string doesn't have the following parameters.
        /// </summary>
        /// <param name="table">The table.</param>
        [Then(@"the query string doesn't have the following parameters")]
        public void ThenTheQueryStringDoesntHaveTheFollowingParameters(Table table)
        {
            Dictionary<string, string> pageParameters = table.Rows.ToDictionary(r => r[0], r => (string)null);

            var context = new ValidatePageParametersAction.ValidatePageParametersActionContext(
                ValidatePageParametersAction.PageParameterValidationAction.DoesNotContain,
                pageParameters);
            var result = this.actionPipelineService
                .PerformAction<ValidatePageParametersAction>(null, context);

            result.CheckResult();
        }

        /// <summary>
        /// Calls the wait for page action.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="timeout">The timeout.</param>
        private void CallWaitForPageAction(string pageName, TimeSpan? timeout)
        {
            var context = new WaitForActionBase.WaitForActionBaseContext(pageName, timeout);

            IPage page = null;
            try
            {
                page = this.actionPipelineService.PerformAction<WaitForPageAction>(null, context).CheckResult<IPage>();
            }
            finally
            {
                this.UpdatePageContext(page);
            }
        }
    }
}