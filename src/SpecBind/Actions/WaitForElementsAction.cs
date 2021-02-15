// <copyright file="WaitForElementsAction.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System;

    using SpecBind.ActionPipeline;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
    /// An action that waits for elements to match the specified criteria.
    /// </summary>
    public class WaitForElementsAction : ValidateActionBase<WaitForElementsAction.WaitForElementsContext>
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForElementsAction" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public WaitForElementsAction(ILogger logger)
            : base(typeof(WaitForElementsAction).Name)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(WaitForElementsContext actionContext)
        {
            // Setup timeout items
            var timeout = actionContext.Timeout.GetValueOrDefault(DefaultTimeout);
            var waitInterval = TimeSpan.FromMilliseconds(200);
            var waiter = new Waiter(timeout, waitInterval);
            ActionResult result = null;

            try
            {
                waiter.WaitFor(() =>
                {
                    result = ValidateTableHelpers.PerformValidation(actionContext.ValidationTable.Validations, this.ValidateProperty);
                    if (!result.Success)
                    {
                        this.logger.Info(result.Exception.ToString());
                    }

                    return result;
                });

                return ActionResult.Successful();
            }
            catch (TimeoutException)
            {
                return ActionResult.Failure(new ElementExecuteException($"Value comparison(s) failed after {timeout}.", result?.Exception));
            }
        }

        /// <summary>
        /// An action context for the action.
        /// </summary>
        public class WaitForElementsContext : ActionContext, IValidationTable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WaitForElementsContext" /> class.
            /// </summary>
            /// <param name="page">The page.</param>
            /// <param name="validations">The validations.</param>
            /// <param name="timeout">The timeout.</param>
            public WaitForElementsContext(IPage page, ValidationTable validations, TimeSpan? timeout)
                : base(null)
            {
                this.Page = page;
                this.ValidationTable = validations;
                this.Timeout = timeout;
            }

            /// <summary>
            /// Gets the page.
            /// </summary>
            /// <value>The page.</value>
            public IPage Page { get; private set; }

            /// <summary>
            /// Gets the validation table.
            /// </summary>
            /// <value>The validation table.</value>
            public ValidationTable ValidationTable { get; private set; }

            /// <summary>
            /// Gets the timeout.
            /// </summary>
            /// <value>The timeout.</value>
            public TimeSpan? Timeout { get; private set; }
        }
    }
}