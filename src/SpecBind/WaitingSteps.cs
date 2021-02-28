// <copyright file="WaitingSteps.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind
{
    using System;
    using System.Threading;
    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
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
        private const string WaitToStillSeeElementRegex = @"I wait to still see (.+)";
        private const string WaitToStillSeeElementWithTimeoutRegex = @"I wait for (\d+) seconds? to still see (.+)";
        private const string WaitToNotSeeElementRegex = @"I wait to not see (.+)";
        private const string WaitToNotSeeElementWithTimeoutRegex = @"I wait for (\d+) seconds? to not see (.+)";
        private const string WaitToStillNotSeeElementRegex = @"I wait to still not see (.+)";
        private const string WaitToStillNotSeeElementWithTimeoutRegex = @"I wait for (\d+) seconds? to still not see (.+)";
        private const string WaitForElementEnabledRegex = @"I wait for (.+) to become enabled";
        private const string WaitForElementEnabledWithTimeoutRegex = @"I wait (\d+) seconds? for (.+) to become enabled";
        private const string WaitForElementStillEnabledRegex = @"I wait for (.+) to remain enabled";
        private const string WaitForElementStillEnabledWithTimeoutRegex = @"I wait (\d+) seconds? for (.+) to remain enabled";
        private const string WaitForElementNotEnabledRegex = @"I wait for (.+) to become disabled";
        private const string WaitForElementNotEnabledWithTimeoutRegex = @"I wait (\d+) seconds? for (.+) to become disabled";
        private const string WaitForElementStillNotEnabledRegex = @"I wait for (.+) to remain disabled";
        private const string WaitForElementStillNotEnabledWithTimeoutRegex = @"I wait (\d+) seconds? for (.+) to remain disabled";
        private const string WaitForElementNotMovingRegex = @"I wait for (.+) to stop moving";
        private const string WaitForElementNotMovingWithTimeoutRegex = @"I wait (\d+) seconds? for (.+) to stop moving";
        private const string WaitForListElementToContainItemsRegex = @"I wait for (.+) to contain items";
        private const string WaitForListElementToContainItemsWithTimeoutRegex = @"I wait (\d+) seconds? for (.+) to contain items";
        private const string WaitForActiveViewRegex = @"I wait for the view to become active";
        private const string WaitForAngularRegex = @"I wait for (?i)angular ajax(?-i) calls to complete";
        private const string WaitForjQueryRegex = @"I wait for (?i)jquery ajax(?-i) calls to complete";

        // The following Regex items are for the given "past tense" form
        private const string GivenWaitToSeeElementRegex = @"I waited to see (.+)";
        private const string GivenWaitToSeeElementWithTimeoutRegex = @"I waited for (\d+) seconds? to see (.+)";
        private const string GivenWaitToNotSeeElementRegex = @"I waited to not see (.+)";
        private const string GivenWaitToNotSeeElementWithTimeoutRegex = @"I waited for (\d+) seconds? to not see (.+)";
        private const string GivenWaitForElementEnabledRegex = @"I waited for (.+) to become enabled";
        private const string GivenWaitForElementEnabledWithTimeoutRegex = @"I waited (\d+) seconds? for (.+) to become enabled";
        private const string GivenWaitForElementNotEnabledRegex = @"I waited for (.+) to become disabled";
        private const string GivenWaitForElementNotEnabledWithTimeoutRegex = @"I waited (\d+) seconds? for (.+) to become disabled";
        private const string GivenWaitForElementNotMovingRegex = @"I waited for (.+) to stop moving";
        private const string GivenWaitForElementNotMovingWithTimeoutRegex = @"I waited (\d+) seconds? for (.+) to stop moving";
        private const string GivenWaitForListElementToContainItemsRegex = @"I waited for (.+) to contain items";
        private const string GivenWaitForListElementToContainItemsWithTimeoutRegex = @"I waited (\d+) seconds? for (.+) to contain items";
        private const string GivenWaitForActiveViewRegex = @"I waited for the view to become active";
        private const string GivenWaitForAngularRegex = @"I waited for (?i)angular ajax(?-i) calls to complete";
        private const string GivenWaitForjQueryRegex = @"I waited for (?i)jquery ajax(?-i) calls to complete";

        private readonly IActionPipelineService actionPipelineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitingSteps" /> class.
        /// </summary>
        /// <param name="actionPipelineService">The action pipeline service.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="logger">The logger.</param>
        public WaitingSteps(
            IActionPipelineService actionPipelineService,
            IScenarioContextHelper scenarioContext,
            ILogger logger)
            : base(scenarioContext, logger)
        {
            this.actionPipelineService = actionPipelineService;
        }

        /// <summary>
        /// Gets the default timeout to wait, if none is specified.
        /// </summary>
        /// <value>
        /// The default wait, 30 seconds.
        /// </value>
        public static TimeSpan DefaultWait
        {
            get
            {
                return TimeSpan.FromSeconds(30);
            }
        }

        /// <summary>
        /// A step that waits for an element to match the specified criteria.
        /// </summary>
        /// <param name="criteriaTable">The criteria table.</param>
        [Given("I waited to see")]
        [When("I wait to see")]
        [Then("I wait to see")]
        public void WaitToSeeElement(Table criteriaTable)
        {
            var page = this.GetPageFromContext();
            var validationTable = criteriaTable.ToValidationTable();

            var context = new WaitForElementsAction.WaitForElementsContext(page, validationTable, null);
            this.actionPipelineService.PerformAction<WaitForElementsAction>(page, context).CheckResult();
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
            this.CallPipelineAction(propertyName, WaitConditions.BecomesExistent, null);
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
            this.CallPipelineAction(propertyName, WaitConditions.BecomesExistent, GetTimeSpan(timeout));
        }

        /// <summary>
        /// A step that waits to still see an element.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [When(WaitToStillSeeElementRegex)]
        [Then(WaitToStillSeeElementRegex)]
        public void WaitToStillSeeElement(string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.RemainsExistent, null);
        }

        /// <summary>
        /// A step that waits to still see an element.
        /// </summary>
        /// <param name="timeout">The timeout for waiting.</param>
        /// <param name="propertyName">Name of the property.</param>
        [When(WaitToStillSeeElementWithTimeoutRegex)]
        [Then(WaitToStillSeeElementWithTimeoutRegex)]
        public void WaitToStillSeeElementWithTimeout(int timeout, string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.RemainsExistent, GetTimeSpan(timeout));
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
            this.CallPipelineAction(propertyName, WaitConditions.BecomesNonExistent, null);
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
            this.CallPipelineAction(propertyName, WaitConditions.BecomesNonExistent, GetTimeSpan(timeout));
        }

        /// <summary>
        /// A step that waits to still not see an element.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [When(WaitToStillNotSeeElementRegex)]
        [Then(WaitToStillNotSeeElementRegex)]
        public void WaitToStillNotSeeElement(string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.RemainsNonExistent, null);
        }

        /// <summary>
        /// A step that waits to still not see an element.
        /// </summary>
        /// <param name="timeout">The timeout for waiting.</param>
        /// <param name="propertyName">Name of the property.</param>
        [When(WaitToStillNotSeeElementWithTimeoutRegex)]
        [Then(WaitToStillNotSeeElementWithTimeoutRegex)]
        public void WaitToStillNotSeeElementWithTimeout(int timeout, string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.RemainsNonExistent, GetTimeSpan(timeout));
        }

        /// <summary>
        /// A step that waits for an element to become enabled.
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
        /// A step that waits for an element to become enabled.
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
        /// A step that waits for an element to remain enabled.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [When(WaitForElementStillEnabledRegex)]
        [Then(WaitForElementStillEnabledRegex)]
        public void WaitForElementStillEnabled(string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.RemainsEnabled, null);
        }

        /// <summary>
        /// A step that waits for an element to remain enabled.
        /// </summary>
        /// <param name="timeout">The timeout for waiting.</param>
        /// <param name="propertyName">Name of the property.</param>
        [When(WaitForElementStillEnabledWithTimeoutRegex)]
        [Then(WaitForElementStillEnabledWithTimeoutRegex)]
        public void WaitForElementStillEnabledWithTimeout(int timeout, string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.RemainsEnabled, GetTimeSpan(timeout));
        }

        /// <summary>
        /// A step that waits for an element to become disabled.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitForElementNotEnabledRegex)]
        [When(WaitForElementNotEnabledRegex)]
        [Then(WaitForElementNotEnabledRegex)]
        public void WaitForElementNotEnabled(string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.BecomesDisabled, null);
        }

        /// <summary>
        /// A step that waits for an element to become disabled.
        /// </summary>
        /// <param name="timeout">The timeout for waiting.</param>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitForElementNotEnabledWithTimeoutRegex)]
        [When(WaitForElementNotEnabledWithTimeoutRegex)]
        [Then(WaitForElementNotEnabledWithTimeoutRegex)]
        public void WaitForElementNotEnabledWithTimeout(int timeout, string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.BecomesDisabled, GetTimeSpan(timeout));
        }

        /// <summary>
        /// A step that waits for an element to remain disabled.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [When(WaitForElementStillNotEnabledRegex)]
        [Then(WaitForElementStillNotEnabledRegex)]
        public void WaitForElementStillNotEnabled(string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.RemainsDisabled, null);
        }

        /// <summary>
        /// A step that waits for an element to remain disabled.
        /// </summary>
        /// <param name="timeout">The timeout for waiting.</param>
        /// <param name="propertyName">Name of the property.</param>
        [When(WaitForElementStillNotEnabledWithTimeoutRegex)]
        [Then(WaitForElementStillNotEnabledWithTimeoutRegex)]
        public void WaitForElementStillNotEnabledWithTimeout(int timeout, string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.RemainsDisabled, GetTimeSpan(timeout));
        }

        /// <summary>
        /// A step that waits for an element to stop moving.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitForElementNotMovingRegex)]
        [When(WaitForElementNotMovingRegex)]
        [Then(WaitForElementNotMovingRegex)]
        public void WaitForElementNotMoving(string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.NotMoving, null);
        }

        /// <summary>
        /// A step that waits for an element to stop moving.
        /// </summary>
        /// <param name="timeout">The timeout for waiting.</param>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitForElementNotMovingWithTimeoutRegex)]
        [When(WaitForElementNotMovingWithTimeoutRegex)]
        [Then(WaitForElementNotMovingWithTimeoutRegex)]
        public void WaitForElementNotMovingWithTimeout(int timeout, string propertyName)
        {
            this.CallPipelineAction(propertyName, WaitConditions.NotMoving, GetTimeSpan(timeout));
        }

        /// <summary>
        /// A step that waits for a list element to contain children.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitForListElementToContainItemsRegex)]
        [When(WaitForListElementToContainItemsRegex)]
        [Then(WaitForListElementToContainItemsRegex)]
        public void WaitForListElementToContainItems(string propertyName)
        {
            this.WaitForListElementToContainItemsWithTimeout(0, propertyName);
        }

        /// <summary>
        /// A step that waits for a list element to contain children.
        /// </summary>
        /// <param name="timeout">The timeout for waiting.</param>
        /// <param name="propertyName">Name of the property.</param>
        [Given(GivenWaitForListElementToContainItemsWithTimeoutRegex)]
        [When(WaitForListElementToContainItemsWithTimeoutRegex)]
        [Then(WaitForListElementToContainItemsWithTimeoutRegex)]
        public void WaitForListElementToContainItemsWithTimeout(int timeout, string propertyName)
        {
            var page = this.GetPageFromContext();

            var context = new WaitForListItemsAction.WaitForListItemsContext(propertyName.ToLookupKey(), GetTimeSpan(timeout));
            this.actionPipelineService.PerformAction<WaitForListItemsAction>(page, context).CheckResult();
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
        /// A step that waits for any pending Angular AJAX calls to complete.
        /// </summary>
        [Given(GivenWaitForAngularRegex)]
        [When(WaitForAngularRegex)]
        [Then(WaitForAngularRegex)]
        public void WaitForAngular()
        {
            WebDriverSupport.WaitForAngular();
        }

        /// <summary>
        /// A step that waits for any pending jQuery AJAX calls to complete.
        /// </summary>
        [Given(GivenWaitForjQueryRegex)]
        [When(WaitForjQueryRegex)]
        [Then(WaitForjQueryRegex)]
        public void WaitForjQuery()
        {
            WebDriverSupport.WaitForjQuery(expectedJQueryDefined: true);
        }

        /// <summary>
        /// Give I waited for x second(s).
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        [Given(@"I waited for (.*) seconds?")]
        [When(@"I wait for (.*) seconds?")]
        public void GivenIWaitedForSeconds(int seconds)
        {
            int totalMilliseconds = (int)TimeSpan.FromSeconds(seconds).TotalMilliseconds;

            Thread.Sleep(totalMilliseconds);
        }

        /// <summary>
        /// Given I kept refreshing the page for up to x second(s) until the title contains "title".
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <param name="title">The title.</param>
        [Given(@"I kept refreshing the page for up to (.*) seconds? until the title contains ""(.*)""")]
        [When(@"I keep refreshing the page for up to (.*) seconds? until the title contains ""(.*)""")]
        public void GivenIKeepRefreshingThePageForUpToSecondsUntilTheTitleContain(int seconds, string title)
        {
            var page = this.GetPageFromContext();

            var context = new WaitForPageTitleAction.WaitForPageTitleContext(title, GetTimeSpan(seconds));
            this.actionPipelineService.PerformAction<WaitForPageTitleAction>(page, context).CheckResult();
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