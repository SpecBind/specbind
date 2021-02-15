// <copyright file="NavigationPostAction.cs">
//    Copyright © 2014 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System.Collections.Generic;
    using Helpers;
    using SpecBind.ActionPipeline;
    using SpecBind.BrowserSupport;
    using SpecBind.Pages;

    /// <summary>
    /// A post-action base class that only triggers when navigation is successful.
    /// </summary>
    public class NavigationPostAction : IPostAction
    {
        private readonly PageHistoryService pageHistoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationPostAction" /> class.
        /// </summary>
        /// <param name="pageHistoryService">The page history service.</param>
        public NavigationPostAction(PageHistoryService pageHistoryService)
        {
            this.pageHistoryService = pageHistoryService;
        }

        /// <summary>
        /// Performs the post-execute action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="context">The action context.</param>
        /// <param name="result">The result.</param>
        public void PerformPostAction(IAction action, ActionContext context, ActionResult result)
        {
            // Exit if the command has failed
            if (!result.Success
                || ((action.Name != typeof(PageNavigationAction).Name)
                    && (action.Name != typeof(WaitForPageAction).Name)
                    && (action.Name != typeof(GetElementAsPageAction).Name)
                    && (action.Name != typeof(GetElementAsContextInPageAction).Name)
                    && (action.Name != typeof(GetListItemByCriteriaAction).Name)
                    && (action.Name != typeof(GetListItemByIndexAction).Name)))
            {
                return;
            }

            var page = result.Result as IPage;

            if ((page != null) && (this.pageHistoryService != null))
            {
                if ((!WebDriverSupport.CurrentBrowser.SupportsPageHistoryService)
                    && (action.Name != typeof(GetListItemByCriteriaAction).Name)
                    && (action.Name != typeof(GetElementAsPageAction).Name)
                    && (action.Name != typeof(GetElementAsContextInPageAction).Name)
                    && (action.Name != typeof(GetListItemByIndexAction).Name))
                {
                    this.pageHistoryService.PageHistory.Clear();
                }

                if (!this.pageHistoryService.Contains(page))
                {
                    this.pageHistoryService.Add(page);
                }
            }

            if (action.Name != typeof(PageNavigationAction).Name)
            {
                return;
            }

            var navigationContext = (PageNavigationAction.PageNavigationActionContext)context;
            var actionType = navigationContext.PageAction;
            var pageArguments = navigationContext.PageArguments;

            // ReSharper disable once SuspiciousTypeConversion.Global
            this.OnPageNavigate(page, actionType, pageArguments);
        }

        /// <summary>
        /// Called when page navigation has occurred.
        /// </summary>
        /// <param name="page">The page that has been navigated to.</param>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="pageArguments">The page arguments.</param>
        protected virtual void OnPageNavigate(IPage page, PageNavigationAction.PageAction actionType, IDictionary<string, string> pageArguments)
        {
        }
    }
}