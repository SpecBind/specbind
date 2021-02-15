// <copyright file="WaitForPageTitleAction.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System;
    using SpecBind.ActionPipeline;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;

    /// <summary>
    /// An action that waits for the browser page title to contain the specified title.
    /// </summary>
    public class WaitForPageTitleAction : ContextActionBase<WaitForPageTitleAction.WaitForPageTitleContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForPageTitleAction" /> class.
        /// </summary>
        public WaitForPageTitleAction()
            : base(typeof(WaitForPageTitleAction).Name)
        {
        }

        /// <summary>
        /// Waits for the page with the specified property name.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The page.</returns>
        protected override ActionResult Execute(WaitForPageTitleContext actionContext)
        {
            var timeout = actionContext.Timeout.GetValueOrDefault(WaitForActionBase.DefaultTimeout);
            var waitInterval = TimeSpan.FromMilliseconds(200);
            var waiter = new Waiter(timeout, waitInterval);

            IBrowser browser = WebDriverSupport.CurrentBrowser;
            string title = actionContext.Title;

            try
            {
                waiter.WaitFor(() =>
                {
                    if (browser.Title.Contains(title))
                    {
                        return true;
                    }

                    browser.Refresh();

                    return false;
                });

                return ActionResult.Successful();
            }
            catch (TimeoutException)
            {
                var exception = new Exception($"Page title did not contain '{title}' after {timeout}");
                return ActionResult.Failure(exception);
            }
        }

        /// <summary>
        /// An action context for the action.
        /// </summary>
        public class WaitForPageTitleContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WaitForPageTitleContext" /> class.
            /// </summary>
            /// <param name="title">The title.</param>
            /// <param name="timeout">The timeout.</param>
            public WaitForPageTitleContext(string title, TimeSpan? timeout)
                : base(null)
            {
                this.Title = title;
                this.Timeout = timeout;
            }

            /// <summary>
            /// Gets the title.
            /// </summary>
            public string Title { get; }

            /// <summary>
            /// Gets the timeout.
            /// </summary>
            /// <value>The timeout.</value>
            public TimeSpan? Timeout { get; private set; }
        }
    }
}
