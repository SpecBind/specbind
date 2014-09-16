// <copyright file="TokenSteps.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind
{
    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;

    using TechTalk.SpecFlow;

    /// <summary>
    /// Steps that relate to the manipulation of tokens.
    /// </summary>
    [Binding]
    public class TokenSteps : PageStepBase
    {
        // Step definition text constants
        private const string SetTokenFromFieldRegex = @"I set token (.+) with the value of (.+)";
        private const string ValidateTokenValueRegex = @"I ensure token (.+) matches rule (.+) with value (.+)";

        private readonly IActionPipelineService actionPipelineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenSteps"/> class.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="actionPipelineService">The action pipeline service.</param>
        public TokenSteps(IScenarioContextHelper scenarioContext, IActionPipelineService actionPipelineService)
            : base(scenarioContext)
        {
            this.actionPipelineService = actionPipelineService;
        }

        /// <summary>
        /// Sets the token specified from the given property value.
        /// </summary>
        /// <param name="tokenName">Name of the token.</param>
        /// <param name="propertyName">Name of the property.</param>
        [Given(SetTokenFromFieldRegex)]
        [When(SetTokenFromFieldRegex)]
        [Then(SetTokenFromFieldRegex)]
        public void SetTokenFromFieldStep(string tokenName, string propertyName)
        {
            var page = this.GetPageFromContext();

            var context = new SetTokenFromValueAction.TokenFieldContext(propertyName.ToLookupKey(), tokenName);

            this.actionPipelineService
                .PerformAction<SetTokenFromValueAction>(page, context)
                .CheckResult();
        }

        /// <summary>
        /// A step that validates a given token value.
        /// </summary>
        /// <param name="tokenName">Name of the token.</param>
        /// <param name="rule">The rule.</param>
        /// <param name="value">The value.</param>
        [Given(ValidateTokenValueRegex)]
        [When(ValidateTokenValueRegex)]
        [Then(ValidateTokenValueRegex)]
        public void ValidateTokenValueStep(string tokenName, string rule, string value)
        {
            var context = new ValidateTokenAction.ValidateTokenActionContext(tokenName, rule, value);

            this.actionPipelineService
                .PerformAction<ValidateTokenAction>(null, context)
                .CheckResult();
        }
    }
}