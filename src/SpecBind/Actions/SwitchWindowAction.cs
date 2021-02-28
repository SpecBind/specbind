// <copyright file="SwitchWindowAction.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// Switch Window Action
    /// </summary>
    public class SwitchWindowAction : WaitForActionBase
    {
        private readonly ILogger logger;
        private readonly IPageMapper pageMapper;
        private readonly IScenarioContextHelper contextHelper;
        private readonly PageHistoryService pageHistoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchWindowAction" /> class.
        /// </summary>
        /// <param name="pageMapper">The page mapper.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="contextHelper">The context helper.</param>
        /// <param name="pageHistoryService">The page history service.</param>
        public SwitchWindowAction(
            IPageMapper pageMapper,
            ILogger logger,
            IScenarioContextHelper contextHelper,
            PageHistoryService pageHistoryService)
            : base(typeof(SwitchWindowAction).Name, logger)
        {
            this.logger = logger;
            this.pageMapper = pageMapper;
            this.contextHelper = contextHelper;
            this.pageHistoryService = pageHistoryService;
        }

        /// <summary>
        /// Waits for the page with the specified property name.
        /// </summary>
        /// <param name="waiter">The waiter.</param>
        /// <param name="actionContext">The action context.</param>
        /// <returns>
        /// The page.
        /// </returns>
        protected override IPage ExecuteWait(Waiter waiter, WaitForActionBaseContext actionContext)
        {
            var propertyName = actionContext.PropertyName;
            var type = this.pageMapper.GetTypeFromName(propertyName);
            if (type == null)
            {
                return null;
            }

            this.logger.Debug($"Switching to window with page {propertyName} ({type.FullName})...");

            IPage page = null;
            waiter.WaitFor(() =>
            {
                IBrowser browser = WebDriverSupport.CurrentBrowser;

                page = browser.SwitchToWindow(type);

                return page != null;
            });

            this.pageHistoryService.Add(page);
            this.contextHelper.SetCurrentPage(page);

            return page;
        }
    }
}
