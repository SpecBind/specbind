// <copyright file="PageNavigationSteps.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using SpecBind.ActionPipeline;
	using SpecBind.Actions;
	using SpecBind.BrowserSupport;
	using SpecBind.Helpers;
	using SpecBind.Pages;
	
	using TechTalk.SpecFlow;

	/// <summary>
	/// A set of common step bindings that drive the underlying fixtures.
	/// </summary>
	[Binding]
	public class PageNavigationSteps : PageStepBase
	{
		// Step regex values - in constants because they are shared.
		private const string EnsureOnPageStepRegex = @"I am on the (.+) page";
		private const string EnsureOnDialogStepRegex = @"I am on the (.+) dialog";
		private const string NavigateToPageStepRegex = @"I navigate to the (.+) page";
        private const string NavigateToPageWithParamsStepRegex = @"I navigate to the (.+) page with parameters";
		
		// The following Regex items are for the given "past tense" form
		private const string GivenEnsureOnPageStepRegex = @"I was on the (.+) page";
		private const string GivenEnsureOnDialogStepRegex = @"I was on the (.+) dialog";
		private const string GivenNavigateToPageStepRegex = @"I navigated to the (.+) page";
		private const string GivenNavigateToPageWithParamsStepRegex = @"I navigated to the (.+) page with parameters";
		
		private readonly IBrowser browser;
		private readonly IPageMapper pageMapper;
	    private readonly IActionPipelineService actionPipelineService;

	    /// <summary>
	    /// Initializes a new instance of the <see cref="PageNavigationSteps" /> class.
	    /// </summary>
	    /// <param name="browser">The browser.</param>
	    /// <param name="pageMapper">The page mapper.</param>
	    /// <param name="scenarioContext">The scenario context.</param>
	    /// <param name="actionPipelineService">The action pipeline service.</param>
	    public PageNavigationSteps(IBrowser browser, IPageMapper pageMapper, IScenarioContextHelper scenarioContext, IActionPipelineService actionPipelineService)
            : base(scenarioContext)
		{
			this.browser = browser;
			this.pageMapper = pageMapper;
			this.actionPipelineService = actionPipelineService;
		}

		/// <summary>
		/// A Given step for ensuring the browser is on the page with the specified name.
		/// </summary>
		/// <param name="pageName">The page name.</param>
		[Given(GivenEnsureOnPageStepRegex)]
        [When(EnsureOnPageStepRegex)]
		[Then(EnsureOnPageStepRegex)]
		public void GivenEnsureOnPageStep(string pageName)
		{
			var type = this.GetPageType(pageName);

			IPage page;
			this.browser.EnsureOnPage(type, out page);

            this.UpdatePageContext(page);
		}

		/// <summary>
		/// A Given step for ensuring the browser is on the dialog which is a sub-element of the page.
		/// </summary>
		/// <param name="propertyName">Name of the property that represents the dialog.</param>
		[Given(GivenEnsureOnDialogStepRegex)]
		[When(EnsureOnDialogStepRegex)]
		[Then(EnsureOnDialogStepRegex)]
		public void GivenEnsureOnDialogStep(string propertyName)
		{
			var page = this.GetPageFromContext();

            var context = new ActionContext(propertyName.ToLookupKey());
            var item = this.actionPipelineService.PerformAction<GetElementAsPageAction>(page, context)
                                                 .CheckResult<IPage>();

            this.UpdatePageContext(item);
		}

		/// <summary>
		/// A Given step for navigating to a page with the specified name.
		/// </summary>
		/// <param name="pageName">The page name.</param>
		[Given(GivenNavigateToPageStepRegex)]
        [When(NavigateToPageStepRegex)]
        [Then(NavigateToPageStepRegex)]
		public void GivenNavigateToPageStep(string pageName)
		{
			this.GivenNavigateToPageWithArgumentsStep(pageName, null);
		}

		/// <summary>
		/// A Given step for navigating to a page with the specified name and url parameters.
		/// </summary>
		/// <param name="pageName">The page name.</param>
		/// <param name="pageArguments">The page arguments.</param>
		[Given(GivenNavigateToPageWithParamsStepRegex)]
        [When(NavigateToPageWithParamsStepRegex)]
        [Then(NavigateToPageWithParamsStepRegex)]
		public void GivenNavigateToPageWithArgumentsStep(string pageName, Table pageArguments)
		{
			var type = this.GetPageType(pageName);

			Dictionary<string, string> args = null;
			if (pageArguments != null && pageArguments.RowCount > 0)
			{
				args = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
				var row = pageArguments.Rows[0];
				foreach (var header in pageArguments.Header.Where(h => !args.ContainsKey(h)))
				{
					args.Add(header, row[header]);
				}
			}

			var page = this.browser.GoToPage(type, args);
            this.UpdatePageContext(page);
		}

		/// <summary>
		/// Gets the type of the page.
		/// </summary>
		/// <param name="pageName">Name of the page.</param>
		/// <returns>The page type.</returns>
		/// <exception cref="PageNavigationException">Thrown if the page type cannot be found.</exception>
		private Type GetPageType(string pageName)
		{
			var type = this.pageMapper.GetTypeFromName(pageName);

			if (type == null)
			{
				throw new PageNavigationException(
					"Cannot locate a page for name: {0}. Check page aliases in the test assembly.", pageName);
			}

			return type;
		}
	}
}