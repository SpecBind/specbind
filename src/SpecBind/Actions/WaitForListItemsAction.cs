// <copyright file="WaitForListItemsAction.cs">
//    Copyright © 2014 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System;

    using SpecBind.ActionPipeline;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// An action that waits for a list element to contain items.
    /// </summary>
    public class WaitForListItemsAction : ContextActionBase<WaitForListItemsAction.WaitForListItemsContext>
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForListItemsAction" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public WaitForListItemsAction(ILogger logger)
            : base(typeof(WaitForListItemsAction).Name)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(WaitForListItemsContext actionContext)
        {
            var timeout = actionContext.Timeout.GetValueOrDefault(DefaultTimeout);

            // Get the element
            var propertyName = actionContext.PropertyName;

            var waitStartTime = DateTime.Now;
            var element = this.GetProperty(propertyName, timeout);
            var remainingTimeout = timeout - (DateTime.Now - waitStartTime);

            // Make sure the element is a list
            if (!element.IsList)
            {
                var exception =
                    new ElementExecuteException(
                        "Property '{0}' is not a list and cannot be used in this wait.",
                        propertyName);

                return ActionResult.Failure(exception);
            }

            // Setup timeout items
            var waitInterval = TimeSpan.FromMilliseconds(200);
            var waiter = new Waiter(remainingTimeout, waitInterval);

            try
            {
                waiter.WaitFor(() => this.CheckForPage(element));

                return ActionResult.Successful();
            }
            catch (TimeoutException)
            {
                var exception = new ElementExecuteException("List '{0}' did not contain elements after {1}", propertyName, timeout);
                return ActionResult.Failure(exception);
            }
        }

        /// <summary>
        /// Gets a property within a specified timeout.
        /// </summary>
        /// <param name="propertyName">The property to locate.</param>
        /// <param name="timeout">The duration to keep trying.</param>
        /// <returns>The located property.</returns>
        protected IPropertyData GetProperty(string propertyName, TimeSpan? timeout = null)
        {
            IPropertyData element = null;

            timeout = timeout.GetValueOrDefault(DefaultTimeout);
            var waiter = new Waiter<IElementLocator>(timeout.Value);
            try
            {
                waiter.WaitFor(this.ElementLocator, e => e.TryGetProperty(propertyName, out element));
                return element;
            }
            catch (TimeoutException)
            {
                // This will throw the appropriate exception if still not found
                return this.ElementLocator.GetProperty(propertyName);
            }
        }

        /// <summary>
        /// Checks for page.
        /// </summary>
        /// <param name="listElement">the list element.</param>
        /// <returns><c>true</c> if the element contains at least one list item; otherwise <c>false</c>.</returns>
        private bool CheckForPage(IPropertyData listElement)
        {
            var item = listElement.GetItemAtIndex(0);
            if (item != null)
            {
                return true;
            }

            this.logger.Debug("List did not contain any elements, waiting...");
            return false;
        }

        /// <summary>
        /// An action context for the action.
        /// </summary>
        public class WaitForListItemsContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WaitForListItemsContext" /> class.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="timeout">The timeout.</param>
            public WaitForListItemsContext(string propertyName, TimeSpan? timeout)
                : base(propertyName)
            {
                this.Timeout = timeout;
            }

            /// <summary>
            /// Gets the timeout.
            /// </summary>
            /// <value>The timeout.</value>
            public TimeSpan? Timeout { get; private set; }
        }
    }
}