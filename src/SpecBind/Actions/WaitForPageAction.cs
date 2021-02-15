// <copyright file="WaitForPageAction.cs">
//    Copyright © 2014 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// An action that waits for the framework url to resolve to a certain page.
    /// </summary>
    public class WaitForPageAction : WaitForActionBase
    {
        private readonly ILogger logger;
        private readonly IPageMapper pageMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForPageAction" /> class.
        /// </summary>
        /// <param name="pageMapper">The page mapper.</param>
        /// <param name="logger">The logger.</param>
        public WaitForPageAction(IPageMapper pageMapper, ILogger logger)
            : base(typeof(WaitForPageAction).Name, logger)
        {
            this.pageMapper = pageMapper;
            this.logger = logger;
        }

        /// <summary>
        /// Waits for the page with the specified property name.
        /// </summary>
        /// <param name="waiter">The waiter.</param>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The page.</returns>
        protected override IPage ExecuteWait(Waiter waiter, WaitForActionBaseContext actionContext)
        {
            IBrowser browser = WebDriverSupport.CurrentBrowser;

            var pageType = this.pageMapper.GetTypeFromName(actionContext.PropertyName);
            if (pageType == null)
            {
                return null;
            }

            IPage page = null;

            waiter.WaitFor(() =>
            {
                page = browser.Page(pageType);

                return this.EnsureOnPage(page) != null;
            });

            return page;
        }
    }
}