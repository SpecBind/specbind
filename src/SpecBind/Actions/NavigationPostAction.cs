// <copyright file="NavigationPostAction.cs">
//    Copyright © 2014 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System.Collections.Generic;

    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// A post-action base class that only triggers when navigation is successful.
    /// </summary>
    public abstract class NavigationPostAction : IPostAction
    {
        /// <summary>
        /// Performs the post-execute action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="context">The action context.</param>
        /// <param name="result">The result.</param>
        public void PerformPostAction(IAction action, ActionContext context, ActionResult result)
        {
            // Exit if the command has failed
            if (!result.Success || action.Name != typeof(PageNavigationAction).Name)
            {
                return;
            }

            var navigationContext = (PageNavigationAction.PageNavigationActionContext)context;
            var actionType = navigationContext.PageAction;
            var pageArguments = navigationContext.PageArguments;

            // ReSharper disable once SuspiciousTypeConversion.Global
            var page = result.Result as IPage;
            this.OnPageNavigate(page, actionType, pageArguments);
        }

        /// <summary>
        /// Called when page navigation has occurred.
        /// </summary>
        /// <param name="page">The page that has been navigated to.</param>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="pageArguments">The page arguments.</param>
        protected abstract void OnPageNavigate(IPage page, PageNavigationAction.PageAction actionType, IDictionary<string, string> pageArguments);
    }
}