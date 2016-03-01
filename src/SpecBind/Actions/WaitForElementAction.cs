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
        /// Initializes the <see cref="WaitForElementAction"/> class.
        /// </summary>
        static WaitForElementAction()
        {
            DefaultTimeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForElementAction"/> class.
        /// </summary>
        public WaitForElementAction()
            : base(typeof(WaitForElementAction).Name)
        {
        }

        /// <summary>
        /// Gets or sets the default timeout to wait, if none is specified.
        /// </summary>
        /// <value>
        /// The default timeout, 30 seconds.
        /// </value>
        public static TimeSpan DefaultTimeout { get; set; }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The action result.</returns>
        protected override ActionResult Execute(WaitForElementContext context)
        {
            var timeout = context.Timeout.GetValueOrDefault(DefaultTimeout);

            var waitStartTime = DateTime.Now;
            var property = this.GetElement(context.PropertyName, timeout);
            var remainingTimeout = timeout - (DateTime.Now - waitStartTime);

            var result = property.WaitForElementCondition(context.Condition, remainingTimeout);

            return result
                       ? ActionResult.Successful()
                       : ActionResult.Failure(
                           new ElementExecuteException(
                             "Could not perform action '{0}' before timeout: {1}",
                             context.Condition,
                             timeout));
        }

        /// <summary>
        /// Gets an element within a specified timeout.
        /// </summary>
        /// <param name="propertyName">The element to locate.</param>
        /// <param name="timeout">The duration to keep trying.</param>
        /// <returns>The located element.</returns>
        protected IPropertyData GetElement(string propertyName, TimeSpan? timeout)
        {
            timeout = timeout.GetValueOrDefault(DefaultTimeout);

            var getStartTime = DateTime.Now;

            do
            {
                IPropertyData element;
                if (this.ElementLocator.TryGetElement(propertyName, out element))
                {
                    return element;
                }

                System.Threading.Thread.Sleep(200);
            }
            while (DateTime.Now - getStartTime < timeout);

            // This will throw the appropriate exception if still not found
            return this.ElementLocator.GetElement(propertyName);
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