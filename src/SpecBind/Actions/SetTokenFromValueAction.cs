// <copyright file="SetTokenFromValueAction.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.Helpers;

    /// <summary>
    /// An action that gets the content of an item.
    /// </summary>
    internal class SetTokenFromValueAction : ContextActionBase<SetTokenFromValueAction.TokenFieldContext>
    {
        private readonly ITokenManager tokenManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTokenFromValueAction" /> class.
        /// </summary>
        /// <param name="tokenManager">The token manager.</param>
        public SetTokenFromValueAction(ITokenManager tokenManager)
            : base(typeof(SetTokenFromValueAction).Name)
        {
            this.tokenManager = tokenManager;
        }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(TokenFieldContext actionContext)
        {
            var propertyData = this.ElementLocator.GetElement(actionContext.PropertyName);

            var value = propertyData.GetCurrentValue();

            this.tokenManager.SetToken(actionContext.TokenName, value);

            return ActionResult.Successful(value);
        }

        /// <summary>
        /// An extended context that holds the token name.
        /// </summary>
        public class TokenFieldContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TokenFieldContext"/> class.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="tokenName">Name of the token.</param>
            public TokenFieldContext(string propertyName, string tokenName)
                : base(propertyName)
            {
                this.TokenName = tokenName;
            }

            /// <summary>
            /// Gets the name of the token.
            /// </summary>
            /// <value>The name of the token.</value>
            public string TokenName { get; private set; }
        }
    }
}