// <copyright file="WaitingSteps.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind
{
    using System;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;

    using TechTalk.SpecFlow;

    /// <summary>
    /// A set of step definitions that allow the user to wait for a given condition.
    /// </summary>
    [Binding]
    public class WaitingSteps : PageStepBase
    {
        // Step regex values - in constants because they are shared.
        private const string WaitToSeeElementRegex = @"I wait to see (.+)";
        private const string WaitToSeeElementWithTimeoutRegex = @"I wait for (\d+) seconds? to see (.+)";
        private const string WaitToNotSeeElementRegex = @"I wait to not see (.+)";
        private const string WaitToNotSeeElementWithTimeoutRegex = @"I wait for (\d+) seconds? to not see (.+)";
        private const string WaitForElementEnabledRegex = @"I wait for (.+) to become enabled";
        private const string WaitForElementEnabledWithTimeoutRegex = @"I wait (\d+) seconds? for (.+) to become enabled";
        private const string WaitForElementNotEnabledRegex = @"I wait for (.+) to become disabled";
        private const string WaitForElementNotEnabledWithTimeoutRegex = @"I wait (\d+) seconds? for (.+) to become disabled";
        private const string WaitForActiveViewRegex = @"I wait for the view to become active";

        // The following Regex items are for the given "past tense" form
        private const string GivenWaitToSeeElementRegex = @"I waited to see (.+)";
        private const string GivenWaitToSeeElementWithTimeoutRegex = @"I waited for (\d+) seconds? to see (.+)";
        private const string GivenWaitToNotSeeElementRegex = @"I waited to not see (.+)";
        private const string GivenWaitToNotSeeElementWithTimeoutRegex = @"I waited for (\d+) seconds? to not see (.+)";
        private const string GivenWaitForElementEnabledRegex = @"I waited for (.+) to become enabled";
        private const string GivenWaitForElementEnabledWithTimeoutRegex = @"I waited (\d+) seconds? for (.+) to become enabled";
        private const string GivenWaitForElementNotEnabledRegex = @"I waited for (.+) to become disabled";
        private const string GivenWaitForElementNotEnabledWithTimeoutRegex = @"I waited (\d+) seconds? for (.+) to become disabled";
        private const string GivenWaitForActiveViewRegex = @"I waited for the view to become active";

        private readonly IActionPipelineService actionPipelineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertBoxSteps"/> class.
        /// </summary>
        /// <param name="actionPipelineService">The action pipeline service.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        public WaitingSteps(IActionPipelineService actionPipelineService, IScenarioContextHelper scenarioContext)
            : base(scenarioContext)
        {
            this.actionPipelineService = actionPipelineService;
        }

        /// <summary>
        /// A step that waits to see an element.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitToSeeElementRegex)]
        [When(WaitToSeeElementRegex)]
        [Then(WaitToSeeElementRegex)]
        public void WaitToSeeElement(string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.Exists, null);
        }

        /// <summary>
        /// A step that waits to see an element.
        /// </summary>
        /// <param name="timeout">The timeout for waiting.</param>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitToSeeElementWithTimeoutRegex)]
        [When(WaitToSeeElementWithTimeoutRegex)]
        [Then(WaitToSeeElementWithTimeoutRegex)]
        public void WaitToSeeElementWithTimeout(int timeout, string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.Exists, GetTimeSpan(timeout));
        }

        /// <summary>
        /// A step that waits to not see an element.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitToNotSeeElementRegex)]
        [When(WaitToNotSeeElementRegex)]
        [Then(WaitToNotSeeElementRegex)]
        public void WaitToNotSeeElement(string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.NotExists, null);
        }

        /// <summary>
        /// A step that waits to not see an element.
        /// </summary>
        /// <param name="timeout">The timeout for waiting.</param>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitToNotSeeElementWithTimeoutRegex)]
        [When(WaitToNotSeeElementWithTimeoutRegex)]
        [Then(WaitToNotSeeElementWithTimeoutRegex)]
        public void WaitToNotSeeElementWithTimeout(int timeout, string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.NotExists, GetTimeSpan(timeout));
        }

        /// <summary>
        /// A step that waits for an element to be enabled.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitForElementEnabledRegex)]
        [When(WaitForElementEnabledRegex)]
        [Then(WaitForElementEnabledRegex)]
        public void WaitForElementEnabled(string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.Enabled, null);
        }

        /// <summary>
        /// A step that waits for an element to be enabled.
        /// </summary>
        /// <param name="timeout">The timeout for waiting.</param>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitForElementEnabledWithTimeoutRegex)]
        [When(WaitForElementEnabledWithTimeoutRegex)]
        [Then(WaitForElementEnabledWithTimeoutRegex)]
        public void WaitForElementEnabledWithTimeout(int timeout, string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.Enabled, GetTimeSpan(timeout));
        }

        /// <summary>
        /// A step that waits for an element to be not enabled.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitForElementNotEnabledRegex)]
        [When(WaitForElementNotEnabledRegex)]
        [Then(WaitForElementNotEnabledRegex)]
        public void WaitForElementNotEnabled(string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.NotEnabled, null);
        }

        /// <summary>
        /// A step that waits for an element to be not enabled.
        /// </summary>
        /// <param name="timeout">The timeout for waiting.</param>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitForElementNotEnabledWithTimeoutRegex)]
        [When(WaitForElementNotEnabledWithTimeoutRegex)]
        [Then(WaitForElementNotEnabledWithTimeoutRegex)]
        public void WaitForElementNotEnabledWithTimeout(int timeout, string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.NotEnabled, GetTimeSpan(timeout));
        }

        /// <summary>
        /// I wait for the view to be active step.
        /// </summary>
        [Given(GivenWaitForActiveViewRegex)]
        [When(WaitForActiveViewRegex)]
        public void WaitForTheViewToBeActive()
        {
            var page = this.GetPageFromContext();
            page.WaitForPageToBeActive();
        }

        /// <summary>
        /// Gets the time span from the seconds value.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns>The parsed time span.</returns>
        private static TimeSpan? GetTimeSpan(int seconds)
        {
            return seconds > 0 ? TimeSpan.FromSeconds(seconds) : (TimeSpan?)null;
        }

        /// <summary>
        /// Calls the pipeline action.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="expectedCondition">The expected condition.</param>
        /// <param name="timeout">The timeout for waiting.</param>
        private void CallPipelineAction(string propertyName, WaitConditions expectedCondition, TimeSpan? timeout)
        {
            var page = this.GetPageFromContext();

            var context = new WaitForElementAction.WaitForElementContext(propertyName.ToLookupKey(), expectedCondition, timeout);
            this.actionPipelineService.PerformAction<WaitForElementAction>(page, context).CheckResult();
        }
    }
}