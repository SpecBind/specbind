// <copyright file="WaitForActionBase.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System;

    using SpecBind.ActionPipeline;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// An action that waits for the framework url to resolve to a certain page.
    /// </summary>
    public abstract class WaitForActionBase : ContextActionBase<WaitForActionBase.WaitForActionBaseContext>
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes static members of the <see cref="WaitForActionBase"/> class.
        /// </summary>
        static WaitForActionBase()
        {
            DefaultTimeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForActionBase" /> class.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="logger">The logger.</param>
        public WaitForActionBase(string actionName, ILogger logger)
            : base(actionName)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Gets or sets the default timeout to wait, if none is specified.
        /// </summary>
        /// <value>
        /// The default timeout, 30 seconds.
        /// </value>
        public static new TimeSpan DefaultTimeout { get; set; }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(WaitForActionBaseContext actionContext)
        {
            var timeout = actionContext.Timeout.GetValueOrDefault(WaitForActionBase.DefaultTimeout);
            var waitInterval = TimeSpan.FromMilliseconds(200);
            var waiter = new Waiter(timeout, waitInterval);

            try
            {
                IPage page = this.ExecuteWait(waiter, actionContext);
                if (page == null)
                {
                    return ActionResult.Failure(new PageNavigationException(
                        "Cannot locate a page for name: {0}. Check page aliases in the test assembly.", actionContext.PropertyName));
                }

                return ActionResult.Successful(page);
            }
            catch (TimeoutException)
            {
                var exception = new PageNavigationException("Browser did not resolve to the '{0}' page in {1}", actionContext.PropertyName, timeout);
                return ActionResult.Failure(exception);
            }
        }

        /// <summary>
        /// Waits for the page with the specified property name.
        /// </summary>
        /// <param name="waiter">The waiter.</param>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The page.</returns>
        protected abstract IPage ExecuteWait(Waiter waiter, WaitForActionBaseContext actionContext);

        /// <summary>
        /// Checks for page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The page object once the item is located.</returns>
        protected IPage EnsureOnPage(IPage page)
        {
            IBrowser browser = WebDriverSupport.CurrentBrowser;

            while (true)
            {
                try
                {
                    browser.EnsureOnPage(page);
                    return page;
                }
                catch (InvalidOperationException ex)
                {
                    // An element command failed because the referenced element is no longer attached to the DOM.
                    this.logger.Debug(ex.Message);
                    return null;
                }
                catch (PageNavigationException ex)
                {
                    this.logger.Debug("Browser is not on page. Details: {0}", ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// An action context for the action.
        /// </summary>
        public class WaitForActionBaseContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WaitForActionBaseContext" /> class.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="timeout">The timeout.</param>
            public WaitForActionBaseContext(string propertyName, TimeSpan? timeout)
                : base(propertyName)
            {
                this.Timeout = timeout;
            }

            /// <summary>
            /// Gets the page.
            /// </summary>
            /// <value>The page.</value>
            public IPage Page { get; internal set; }

            /// <summary>
            /// Gets the timeout.
            /// </summary>
            /// <value>The timeout.</value>
            public TimeSpan? Timeout { get; private set; }
        }
    }
}