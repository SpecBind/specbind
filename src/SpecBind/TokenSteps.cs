// <copyright file="TokenSteps.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind
{
    using System.Collections.Generic;
    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

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
        private readonly ITokenManager tokenManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenSteps" /> class.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="actionPipelineService">The action pipeline service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="tokenManager">The token manager.</param>
        public TokenSteps(
            IScenarioContextHelper scenarioContext,
            IActionPipelineService actionPipelineService,
            ILogger logger,
            ITokenManager tokenManager)
            : base(scenarioContext, logger)
        {
            this.actionPipelineService = actionPipelineService;
            this.tokenManager = tokenManager;
        }

        /// <summary>
        /// Transforms the specified table to an enumerable collection of tokens.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>An enumerable collection of tokens.</returns>
        [StepArgumentTransformation]
        public IEnumerable<Token> Transform(Table table)
        {
            return table.CreateSet<Token>();
        }

        /// <summary>
        /// Given I set the following tokens.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        [Given("I set the following tokens")]
        public void SetTheFollowingTokens(IEnumerable<Token> tokens)
        {
            foreach (Token token in tokens)
            {
                this.tokenManager.SetToken(token.Name, token.Value);
            }
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

        /// <summary>
        /// Token
        /// </summary>
        public class Token
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>
            /// The value.
            /// </value>
            public string Value { get; set; }
        }
    }
}