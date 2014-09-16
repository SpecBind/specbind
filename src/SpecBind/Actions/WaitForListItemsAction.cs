// <copyright file="WaitForListItemsAction.cs">
//    Copyright © 2014 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using SpecBind.ActionPipeline;
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
            // Get the element
            var propertyName = actionContext.PropertyName;
            var element = this.ElementLocator.GetProperty(propertyName);

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
            var timeout = actionContext.Timeout.GetValueOrDefault(TimeSpan.FromSeconds(20));
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(timeout);
            var token = cancellationTokenSource.Token;

            try
            {
                var task = Task.Run(() => this.CheckForPage(element, token), token);
                task.Wait(token);

                return ActionResult.Successful();
            }
            catch (OperationCanceledException)
            {
                var exception = new PageNavigationException("List '{0}' did not contain elements after {1}", propertyName, timeout);
                return ActionResult.Failure(exception);
            }
        }

        /// <summary>
        /// Checks for page.
        /// </summary>
        /// <param name="listElement">the list element.</param>
        /// <param name="token">The cancellation token.</param>
        private void CheckForPage(IPropertyData listElement, CancellationToken token)
        {
            while (true)
            {
                var item = listElement.GetItemAtIndex(0);
                if (item != null)
                {
                    return;
                }

                this.logger.Debug("List did not contain any elements, waiting...");
                token.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
                token.ThrowIfCancellationRequested();
            }
        }

        /// <summary>
        /// An action context for the action.
        /// </summary>
        public class WaitForListItemsContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ActionContext" /> class.
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