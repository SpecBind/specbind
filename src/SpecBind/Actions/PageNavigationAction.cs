// <copyright file="PageNavigationAction.cs">
//    Copyright © 2014 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System.Collections.Generic;
    using System.Linq;

    using SpecBind.ActionPipeline;
    using SpecBind.BrowserSupport;
    using SpecBind.Pages;

    /// <summary>
    /// An action that performs any necessary navigation actions.
    /// </summary>
    public class PageNavigationAction : ContextActionBase<PageNavigationAction.PageNavigationActionContext>
    {
        private readonly ILogger logger;
        private readonly IPageMapper pageMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageNavigationAction" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="pageMapper">The page mapper.</param>
        public PageNavigationAction(ILogger logger, IPageMapper pageMapper)
            : base(typeof(PageNavigationAction).Name)
        {
            this.logger = logger;
            this.pageMapper = pageMapper;
        }

        /// <summary>
        /// Enumerates the type of page action to be performed.
        /// </summary>
        public enum PageAction
        {
            /// <summary>
            /// Navigate to a page
            /// </summary>
            NavigateToPage = 0,

            /// <summary>
            /// The ensure on page
            /// </summary>
            EnsureOnPage = 1,

            /// <summary>
            /// Navigates back to the previous page using the browser's back button
            /// </summary>
            NavigateBack = 2
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">The action context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(PageNavigationActionContext context)
        {
            IBrowser browser = WebDriverSupport.CurrentBrowser;

            switch (context.PageAction)
            {
                case PageAction.NavigateBack:

                    this.logger.Debug("Navigating back.");
                    browser.GoBack();

                    return ActionResult.Successful(null);
            }

            var propertyName = context.PropertyName;
            var type = this.pageMapper.GetTypeFromName(propertyName);

            if (type == null)
            {
                return ActionResult.Failure(new PageNavigationException(
                    "Cannot locate a page for name: {0}. Check page aliases in the test assembly.", propertyName));
            }

            IPage page = null;
            switch (context.PageAction)
            {
                case PageAction.NavigateToPage:

                    this.logger.Debug("Navigating to page: {0} ({1})", propertyName, type.FullName);
                    var args = context.PageArguments;
                    if (args != null)
                    {
                        this.logger.Debug("Page Arguments: {0}", string.Join(", ", args.Select(a => string.Format("{0}={1}", a.Key, a.Value))));
                    }

                    page = browser.GoToPage(type, args);
                    break;

                case PageAction.EnsureOnPage:
                    this.logger.Debug("Ensuring browser is on page: {0} ({1})", propertyName, type.FullName);
                    browser.EnsureOnPage(type, out page);
                    break;
            }

            return ActionResult.Successful(page);
        }

        /// <summary>
        /// The action context for passing in page arguments
        /// </summary>
        public class PageNavigationActionContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PageNavigationActionContext"/> class.
            /// </summary>
            /// <param name="pageAction">The page action.</param>
            /// <param name="pageArguments">The page arguments.</param>
            public PageNavigationActionContext(PageAction pageAction, IDictionary<string, string> pageArguments = null)
                : this(null, pageAction, pageArguments)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="PageNavigationActionContext" /> class.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="pageAction">The page action.</param>
            /// <param name="pageArguments">The page arguments.</param>
            public PageNavigationActionContext(string propertyName, PageAction pageAction, IDictionary<string, string> pageArguments = null)
                : base(propertyName)
            {
                this.PageAction = pageAction;
                this.PageArguments = pageArguments;
            }

            /// <summary>
            /// Gets the page action.
            /// </summary>
            /// <value>The page action.</value>
            public PageAction PageAction { get; private set; }

            /// <summary>
            /// Gets the page arguments.
            /// </summary>
            /// <value>The page arguments.</value>
            public IDictionary<string, string> PageArguments { get; private set; }
        }
    }
}