// <copyright file="WaitForPageAction.cs">
//    Copyright © 2014 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using SpecBind.ActionPipeline;
    using SpecBind.BrowserSupport;
    using SpecBind.Pages;

    /// <summary>
    /// An action that waits for the framework url to resolve to a certain page.
    /// </summary>
    public class WaitForPageAction : ContextActionBase<WaitForPageAction.WaitForPageActionContext>
    {
        private readonly IBrowser browser;
        private readonly ILogger logger;
        private readonly IPageMapper pageMapper;

        /// <summary>
        /// Initializes the <see cref="WaitForPageAction"/> class.
        /// </summary>
        static WaitForPageAction()
        {
            DefaultTimeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForPageAction" /> class.
        /// </summary>
        /// <param name="pageMapper">The page mapper.</param>
        /// <param name="browser">The browser.</param>
        /// <param name="logger">The logger.</param>
        public WaitForPageAction(IPageMapper pageMapper, IBrowser browser, ILogger logger)
            : base(typeof(WaitForPageAction).Name)
        {
            this.pageMapper = pageMapper;
            this.browser = browser;
            this.logger = logger;
        }

        /// <summary>
        /// Gets or sets the default timeout to wait, if none is specified.
        /// </summary>
        /// <value>
        /// The default timeout, 30 seconds.
        /// </value>
        public static TimeSpan DefaultTimeout { get; set; }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(WaitForPageActionContext actionContext)
        {
            var type = this.pageMapper.GetTypeFromName(actionContext.PropertyName);

            if (type == null)
            {
                return ActionResult.Failure(new PageNavigationException(
                    "Cannot locate a page for name: {0}. Check page aliases in the test assembly.", actionContext.PropertyName));
            }

            var timeout = actionContext.Timeout.GetValueOrDefault(DefaultTimeout);
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(timeout);
            var token = cancellationTokenSource.Token;

            try
            {
                var task = Task.Run(() => this.CheckForPage(type, token), token);
                task.Wait(token);

                return ActionResult.Successful(task.Result);
            }
            catch (OperationCanceledException)
            {
                var exception = new PageNavigationException("Browser did not resolve to the '{0}' page in {1}", type.Name, timeout);
                return ActionResult.Failure(exception);
            }
        }

        /// <summary>
        /// Checks for page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="token">The token.</param>
        /// <returns>The page object once the item is located.</returns>
        private IPage CheckForPage(Type pageType, CancellationToken token)
        {
            var page = this.browser.Page(pageType);
            while (true)
            {
                try
                {
                    this.browser.EnsureOnPage(page);
                    return page;
                }
                catch (PageNavigationException ex)
                {
                    this.logger.Debug("Browser is not on page. Details: {0}", ex.Message);
                    token.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(200));
                    token.ThrowIfCancellationRequested();
                }
            }
        }

        /// <summary>
        /// An action context for the action.
        /// </summary>
        public class WaitForPageActionContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ActionContext" /> class.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="timeout">The timeout.</param>
            public WaitForPageActionContext(string propertyName, TimeSpan? timeout)
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