// <copyright file="PageStepBase.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind
{
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A base class for any steps that need to get a page from the current context.
    /// </summary>
    public abstract class PageStepBase
    {
        private readonly IScenarioContextHelper scenarioContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageStepBase"/> class.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        protected PageStepBase(IScenarioContextHelper scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        /// <summary>
        /// Gets the page from the scenario context.
        /// </summary>
        /// <returns>The current page.</returns>
        protected IPage GetPageFromContext()
        {
            var page = this.scenarioContext.GetCurrentPage();
            if (page == null)
            {
                throw new PageNavigationException("No page has been set as being the current page.");
            }

            return page;
        }

        /// <summary>
        /// Updates the page context with the given <paramref name="page"/>.
        /// </summary>
        /// <param name="page">The page.</param>
        protected void UpdatePageContext(IPage page)
        {
            this.scenarioContext.SetCurrentPage(page);
        }
    }
}