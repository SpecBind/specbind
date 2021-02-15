// <copyright file="TestPostNavigateHook.cs" company="">
//     Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.IntegrationTests.Steps
{
    using System.Collections.Generic;

    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A test navigation post-action.
    /// </summary>
    public class TestPostNavigateHook : NavigationPostAction
    {
        private readonly ITokenManager tokenManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestPostNavigateHook" /> class.
        /// </summary>
        /// <param name="tokenManager">The token manager.</param>
        /// <param name="pageHistoryService">The page history service.</param>
        public TestPostNavigateHook(ITokenManager tokenManager, PageHistoryService pageHistoryService)
            : base(pageHistoryService)
        {
            this.tokenManager = tokenManager;
        }

        /// <summary>
        /// Called when page navigation has occurred.
        /// </summary>
        /// <param name="page">The page that has been navigated to.</param>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="pageArguments">The page arguments.</param>
        protected override void OnPageNavigate(IPage page, PageNavigationAction.PageAction actionType, IDictionary<string, string> pageArguments)
        {
            this.tokenManager.SetToken("NavigatedPageSuccess", page.PageType.Name);
        }
    }
}