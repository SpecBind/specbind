// <copyright file="WaitForElementAction.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Actions
{
    using System;

    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// An action that waits for an element to be in a certain condition.
    /// </summary>
    public class WaitForElementAction : ContextActionBase<WaitForElementAction.WaitForElementContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForElementAction"/> class.
        /// </summary>
        public WaitForElementAction()
            : base(typeof(WaitForElementAction).Name)
        {
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The action result.</returns>
        protected override ActionResult Execute(WaitForElementContext context)
        {
            var property = this.ElementLocator.GetElement(context.PropertyName);

            var result = property.WaitForElementCondition(context.Condition, context.Timeout);

            return result
                       ? ActionResult.Successful()
                       : ActionResult.Failure(
                           new ElementExecuteException(
                             "Could not perform action '{0}' before timeout: {1}",
                             context.Condition,
                             context.Timeout));
        }

        /// <summary>
        /// The action context for the <see cref="WaitForElementAction"/>.
        /// </summary>
        public class WaitForElementContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WaitForElementContext" /> class.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="condition">The condition.</param>
            /// <param name="timeout">The timeout for waiting.</param>
            public WaitForElementContext(string propertyName, WaitConditions condition, TimeSpan? timeout)
                : base(propertyName)
            {
                this.Condition = condition;
                this.Timeout = timeout;
            }

            /// <summary>
            /// Gets the condition.
            /// </summary>
            /// <value>The condition.</value>
            public WaitConditions Condition { get; private set; }

            /// <summary>
            /// Gets the timeout.
            /// </summary>
            /// <value>The timeout.</value>
            public TimeSpan? Timeout { get; private set; }
        }
    }
}