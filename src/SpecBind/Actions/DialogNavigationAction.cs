// <copyright file="DialogNavigationAction.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;

    /// <summary>
    /// Dialog Navigation Action.
    /// </summary>
    public class DialogNavigationAction : WaitForDialogAction
    {
        private readonly ILogger logger;
        private readonly IScenarioContextHelper contextHelper;
        private readonly PageHistoryService pageHistoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogNavigationAction" /> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="contextHelper">The context helper.</param>
        /// <param name="pageHistoryService">The page history service.</param>
        public DialogNavigationAction(
            IBrowser browser,
            ILogger logger,
            IScenarioContextHelper contextHelper,
            PageHistoryService pageHistoryService)
            : base(browser, logger, contextHelper, pageHistoryService)
        {
            this.logger = logger;
            this.contextHelper = contextHelper;
            this.pageHistoryService = pageHistoryService;
        }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(WaitForActionBaseContext actionContext)
        {
            var propertyName = actionContext.PropertyName;

            this.logger.Debug($"Navigating to dialog '{propertyName}'...");

            var page = this.pageHistoryService.FindPage(propertyName);
            if (page == null)
            {
                return ActionResult.Failure(new PageHistoryException(propertyName, this.pageHistoryService.PageHistory));
            }

            // wait for dialog
            actionContext.Page = page;
            return base.Execute(actionContext);
        }
    }
}
