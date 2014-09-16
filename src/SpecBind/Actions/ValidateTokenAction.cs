// <copyright file="ValidateTokenAction.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
    /// An action that validates a token value.
    /// </summary>
    public class ValidateTokenAction : ContextActionBase<ValidateTokenAction.ValidateTokenActionContext>
    {
        private readonly ITokenManager tokenManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateTokenAction" /> class.
        /// </summary>
        /// <param name="tokenManager">The token manager.</param>
        public ValidateTokenAction(ITokenManager tokenManager)
            : base(typeof(ValidateTokenAction).Name)
        {
            this.tokenManager = tokenManager;
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(ValidateTokenActionContext context)
        {
            return ValidateTableHelpers.PerformValidation(context.ValidationTable.Validations, ValidateToken);
        }

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="validation">The validation.</param>
        /// <param name="itemResult">The validation item result.</param>
        /// <returns><c>true</c> if the validation is successful, <c>false</c> otherwise.</returns>
        private bool ValidateToken(ItemValidation validation, ValidationItemResult itemResult)
        {
            var tokenValue = this.tokenManager.GetTokenByKey(validation.FieldName);
            var successful = validation.Compare(null, tokenValue);
            itemResult.NoteValidationResult(validation, successful, tokenValue);

            return successful;
        }

        /// <summary>
        /// The token validation context.
        /// </summary>
        public class ValidateTokenActionContext : ActionContext, IValidationTable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValidateTokenActionContext" /> class.
            /// </summary>
            /// <param name="tokenName">Name of the token.</param>
            /// <param name="comparisonType">Type of the comparison.</param>
            /// <param name="comparisonValue">The comparison value.</param>
            public ValidateTokenActionContext(string tokenName, string comparisonType, string comparisonValue)
                : base(null)
            {
                var table = new ValidationTable();
                table.AddValidation(tokenName, comparisonType, comparisonValue);

                this.ValidationTable = table;
            }

            /// <summary>
            /// Gets the validation table.
            /// </summary>
            /// <value>The validation table.</value>
            public ValidationTable ValidationTable { get; private set; }
        }
    }
}