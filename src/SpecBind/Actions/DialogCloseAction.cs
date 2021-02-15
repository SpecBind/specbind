// <copyright file="DialogCloseAction.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using Pages;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;

    /// <summary>
    /// Dialog Close Action.
    /// </summary>
    public class DialogCloseAction : WaitForActionBase
    {
        private readonly IBrowser browser;
        private readonly ILogger logger;
        private readonly IScenarioContextHelper contextHelper;
        private readonly PageHistoryService pageHistoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogCloseAction" /> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="contextHelper">The context helper.</param>
        /// <param name="pageHistoryService">The page history service.</param>
        public DialogCloseAction(
            IBrowser browser,
            ILogger logger,
            IScenarioContextHelper contextHelper,
            PageHistoryService pageHistoryService)
            : base(typeof(DialogCloseAction).Name, logger)
        {
            this.browser = browser;
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

            var page = this.pageHistoryService.FindPage(propertyName);
            if (page == null)
            {
                return null;
            }

            this.logger.Debug($"Waiting for dialog '{propertyName}' of type '{page.PageType}' to close...");

            waiter.WaitFor(() =>
            {
                return this.EnsureOnPage(page) == null;
            });

            this.logger.Debug($"Dialog '{page.PageType}' is closed.");

            this.pageHistoryService.Remove(page);

            var previousPage = this.pageHistoryService.GetCurrentPage();
            this.contextHelper.SetCurrentPage(previousPage);

            // remove cached element
            IPropertyData item = this.ElementLocator.GetElement(propertyName);
            item.ClearCache();

            return previousPage;
        }
    }
}
