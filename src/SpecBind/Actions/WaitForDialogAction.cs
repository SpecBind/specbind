// <copyright file="WaitForDialogAction.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using Helpers;
    using SpecBind.BrowserSupport;
    using SpecBind.Pages;

    /// <summary>
    /// An action that waits for the framework url to resolve to a certain dialog.
    /// </summary>
    /// <seealso cref="SpecBind.Actions.WaitForActionBase" />
    public class WaitForDialogAction : WaitForActionBase
    {
        private readonly ILogger logger;
        private readonly IScenarioContextHelper contextHelper;
        private readonly PageHistoryService pageHistoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForDialogAction" /> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="contextHelper">The context helper.</param>
        /// <param name="pageHistoryService">The page history service.</param>
        public WaitForDialogAction(
            IBrowser browser,
            ILogger logger,
            IScenarioContextHelper contextHelper,
            PageHistoryService pageHistoryService)
            : base(typeof(WaitForDialogAction).Name, logger)
        {
            this.logger = logger;
            this.contextHelper = contextHelper;
            this.pageHistoryService = pageHistoryService;
        }

        /// <summary>
        /// Waits for the page with the specified property name.
        /// </summary>
        /// <param name="waiter">The waiter.</param>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The page.</returns>
        protected override IPage ExecuteWait(Waiter waiter, WaitForActionBaseContext actionContext)
        {
            var propertyName = actionContext.PropertyName;

            this.logger.Debug($"Navigating to dialog '{propertyName}'...");

            var page = this.pageHistoryService.FindPage(propertyName);
            if (page == null)
            {
                return null;
            }

            waiter.WaitFor(() =>
            {
                return this.EnsureOnPage(page) != null;
            });

            if (!this.pageHistoryService.Contains(page))
            {
                this.pageHistoryService.Add(page);
            }

            this.contextHelper.SetCurrentPage(page);

            return page;
        }
    }
}
