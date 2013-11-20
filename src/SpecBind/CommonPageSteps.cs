// <copyright file="CommonPageSteps.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
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
	public class CommonPageSteps
	{
		/// <summary>
		/// The scenario context key for holding the current page.
		/// </summary>
		public const string CurrentPageKey = "CurrentPage";

		// Step regex values - in constants because they are shared.
		private const string EnsureOnPageStepRegex = @"I am on the (.+) page";
		private const string EnsureOnDialogStepRegex = @"I am on the (.+) dialog";
		private const string EnsureOnListItemRegex = @"I am on list (.+) item ([0-9]+)";
		private const string EnterDataInFieldsStepRegex = @"I enter data";
        private const string NavigateToPageStepRegex = @"I navigate to the (.+) page";
        private const string NavigateToPageWithParamsStepRegex = @"I navigate to the (.+) page with parameters";
		private const string ObserveDataStepRegex = @"I see";
		private const string ObserveListDataStepRegex = @"I see (.+) list ([A-Za-z ]+)";
		private const string ChooseALinkStepRegex = @"I choose (.+)";
		private const string WaitForActiveViewRegex = @"I wait for the view to become active";
		private const string SetTokenFromFieldRegex = @"I set token (.+) with the value of (.+)";

		// The following Regex items are for the given "past tense" form
		private const string GivenEnsureOnPageStepRegex = @"I was on the (.+) page";
		private const string GivenEnsureOnDialogStepRegex = @"I was on the (.+) dialog";
		private const string GivenEnsureOnListItemRegex = @"I was on list (.+) item ([0-9]+)";
		private const string GivenEnterDataInFieldsStepRegex = @"I entered data";
		private const string GivenObserveDataStepRegex = @"I saw";
		private const string GivenObserveListDataStepRegex = @"I saw (.+) list ([A-Za-z ]+)";
		private const string GivenChooseALinkStepRegex = @"I chose (.+)";
		private const string GivenNavigateToPageStepRegex = @"I navigated to the (.+) page";
		private const string GivenNavigateToPageWithParamsStepRegex = @"I navigated to the (.+) page with parameters";
		private const string GivenWaitForActiveViewRegex = @"I waited for the view to become active";

		private readonly IBrowser browser;
		private readonly IPageDataFiller pageDataFiller;
		private readonly IPageMapper pageMapper;
		private readonly IScenarioContextHelper scenarioContext;
		private readonly ITokenManager tokenManager;
		private readonly IActionPipelineService actionPipelineService;

		/// <summary>
		/// Initializes a new instance of the <see cref="CommonPageSteps" /> class.
		/// </summary>
		/// <param name="browser">The browser.</param>
		/// <param name="pageDataFiller">The page data filler.</param>
		/// <param name="pageMapper">The page mapper.</param>
		/// <param name="scenarioContext">The scenario context.</param>
		/// <param name="tokenManager">The token manager.</param>
		/// <param name="actionPipelineService">The action pipeline service.</param>
		public CommonPageSteps(IBrowser browser, IPageDataFiller pageDataFiller, IPageMapper pageMapper, IScenarioContextHelper scenarioContext, ITokenManager tokenManager, IActionPipelineService actionPipelineService)
		{
			this.browser = browser;
			this.pageDataFiller = pageDataFiller;
			this.pageMapper = pageMapper;
			this.scenarioContext = scenarioContext;
			this.tokenManager = tokenManager;
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

			this.scenarioContext.SetValue(page, CurrentPageKey);
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
			var item = this.pageDataFiller.GetElementAsPage(page, propertyName.ToLookupKey());

			this.scenarioContext.SetValue(item, CurrentPageKey);
		}

		/// <summary>
		/// A Given step for ensuring the browser is on the list item with the specified name and index.
		/// </summary>
		/// <param name="listName">Name of the list.</param>
		/// <param name="itemNumber">The item number.</param>
		[Given(GivenEnsureOnListItemRegex)]
        [When(EnsureOnListItemRegex)]
		[Then(EnsureOnListItemRegex)]
		public void GivenEnsureOnListItemStep(string listName, int itemNumber)
		{
			var page = this.GetPageFromContext();
			var item = this.pageDataFiller.GetListItem(page, listName.ToLookupKey(), itemNumber);

			if (this.scenarioContext.ContainsTag(TagConstants.Debug))
			{
				item.Highlight();
			}
			
			this.scenarioContext.SetValue(item, CurrentPageKey);
		}

		/// <summary>
		/// I wait for the view to be active.
		/// </summary>
		[Given(GivenWaitForActiveViewRegex)]
		[When(WaitForActiveViewRegex)]
		public void GivenIWaitForTheViewToBeActive()
		{
			var page = this.GetPageFromContext();
			page.WaitForPageToBeActive();
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
			this.scenarioContext.SetValue(page, CurrentPageKey);
		}

		/// <summary>
		/// A When step indicating a link click should occur.
		/// </summary>
		/// <param name="linkName">Name of the link.</param>
		[Given(GivenChooseALinkStepRegex)]
		[When(ChooseALinkStepRegex)]
		public void WhenIChooseALinkStep(string linkName)
		{
			var page = this.GetPageFromContext();

			var action = new ButtonClickAction(linkName.ToLookupKey());
		    this.RunAction(page, action);
		}

		/// <summary>
		/// A When step for entering data into fields.
		/// </summary>
		/// <param name="data">The field data.</param>
		[Given(GivenEnterDataInFieldsStepRegex)]
		[When(EnterDataInFieldsStepRegex)]
		[Then(EnterDataInFieldsStepRegex)]
		public void WhenIEnterDataInFieldsStep(Table data)
		{
			string fieldHeader = null;
			string valueHeader = null;

			if (data != null)
			{
				fieldHeader = data.Header.FirstOrDefault(h => h.NormalizedEquals("Field"));
				valueHeader = data.Header.FirstOrDefault(h => h.NormalizedEquals("Value"));
			}

			if (fieldHeader == null || valueHeader == null)
			{
				throw new ElementExecuteException("A table must be specified for this step with the columns 'Field' and 'Value'");
			}

			var page = this.GetPageFromContext();
			
			foreach (var tableRow in data.Rows)
			{
				var fieldName = tableRow[fieldHeader];
				var fieldValue = tableRow[valueHeader];

				fieldValue = this.tokenManager.SetToken(fieldValue);
				this.pageDataFiller.FillField(page, fieldName.ToLookupKey(), fieldValue);
			}
		}

		/// <summary>
		/// A Then step
		/// </summary>
		/// <param name="data">The field data.</param>
		[Given(GivenObserveDataStepRegex)]
		[Then(ObserveDataStepRegex)]
		public void ThenISeeStep(Table data)
		{
			var validations = this.GetItemValidations(data);

			if (validations == null || validations.Count <= 0)
			{
				return;
			}

			var page = this.GetPageFromContext();
			this.pageDataFiller.ValidateItem(page, validations);
		}

		/// <summary>
		/// A Then step for validating items in a list.
		/// </summary>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="rule">The rule.</param>
		/// <param name="data">The field data.</param>
		/// <exception cref="ElementExecuteException">A table must be specified for this step with the columns 'Field', 'Rule' and 'Value'</exception>
		[Given(GivenObserveListDataStepRegex)]
		[Then(ObserveListDataStepRegex)]
		public void ThenISeeListStep(string fieldName, string rule, Table data)
		{
			if (data == null || data.RowCount == 0)
			{
				return;
			}

			ComparisonType comparisonType;
			switch (rule.ToLookupKey())	
			{
				case "exists":
				case "contains":
					comparisonType = ComparisonType.Contains;
					break;

				case "doesnotexist":
				case "doesnotcontain":
					comparisonType = ComparisonType.DoesNotContain;
					break;

				case "startswith":
					comparisonType = ComparisonType.StartsWith;
					break;

				case "endswith":
					comparisonType = ComparisonType.EndsWith;
					break;
                case "equals":
                    comparisonType = ComparisonType.Equals;
			        break;
				default:
					throw new InvalidOperationException(string.Format("Rule type '{0}' is not supported.", rule));
			}

			var page = this.GetPageFromContext();
			var validations = this.GetItemValidations(data);

			this.pageDataFiller.ValidateList(page, fieldName.ToLookupKey(), comparisonType, validations);
		}

		/// <summary>
		/// Sets the token specified from the given property value.
		/// </summary>
		/// <param name="tokenName">Name of the token.</param>
		/// <param name="propertyName">Name of the property.</param>
		[Given(SetTokenFromFieldRegex)]
		[When(SetTokenFromFieldRegex)]
		[Then(SetTokenFromFieldRegex)]
		public void SetTokenFromFieldStep(string tokenName, string propertyName)
		{
			var page = this.GetPageFromContext();

			var fieldValue = this.pageDataFiller.GetItemValue(page, propertyName);

			this.tokenManager.SetToken(tokenName, fieldValue);
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
					"Cannot locate a page for name: {0}. Check page alises in the test assembly.", pageName);
			}

			return type;
		}

		/// <summary>
		/// Gets the page from the scenario context.
		/// </summary>
		/// <returns>The current page.</returns>
		private IPage GetPageFromContext()
		{
			var page = this.scenarioContext.GetValue<IPage>(CurrentPageKey);
			if (page == null)
			{
				throw new PageNavigationException("No page has been set as being the current page.");
			}

			return page;
		}

		/// <summary>
		/// Gets the item validations from the SpecFlow Table.
		/// </summary>
		/// <param name="data">The table data.</param>
		/// <returns>A list of validation items.</returns>
		/// <exception cref="ElementExecuteException">A table must be specified for this step with the columns 'Field', 'Rule' and 'Value'.</exception>
		private ICollection<ItemValidation> GetItemValidations(Table data)
		{
			string fieldHeader = null;
			string valueHeader = null;
			string ruleHeader = null;

			if (data != null)
			{
				fieldHeader = data.Header.FirstOrDefault(h => h.NormalizedEquals("Field"));
				valueHeader = data.Header.FirstOrDefault(h => h.NormalizedEquals("Value"));
				ruleHeader = data.Header.FirstOrDefault(h => h.NormalizedEquals("Rule"));
			}

			if (fieldHeader == null || valueHeader == null || ruleHeader == null)
			{
				throw new ElementExecuteException("A table must be specified for this step with the columns 'Field', 'Rule' and 'Value'");
			}

			var validations = new List<ItemValidation>(data.RowCount);
			foreach (var tableRow in data.Rows)
			{
				var fieldName = tableRow[fieldHeader].ToLookupKey();
				var comparisonValue = tableRow[valueHeader];
				var ruleValue = tableRow[ruleHeader].ToLookupKey();
				var checkToken = true;

				ComparisonType comparisonType;
				switch (ruleValue)
				{
					case "exists":
						comparisonType = ComparisonType.Exists;
						comparisonValue = true.ToString(CultureInfo.InvariantCulture);
						checkToken = false;
						break;
					case "doesnotexist":
						comparisonType = ComparisonType.Exists;
						comparisonValue = false.ToString(CultureInfo.InvariantCulture);
						checkToken = false;
						break;
					case "isenabled":
					case "enabled":
						comparisonType = ComparisonType.Enabled;
						comparisonValue = true.ToString(CultureInfo.InvariantCulture);
						checkToken = false;
						break;
					case "isnotenabled":
					case "notenabled":
					case "disabled":
						comparisonType = ComparisonType.Enabled;
						comparisonValue = false.ToString(CultureInfo.InvariantCulture);
						checkToken = false;
						break;
					case "contains":
						comparisonType = ComparisonType.Contains;
						break;
					case "doesnotcontain":
						comparisonType = ComparisonType.DoesNotContain;
						break;
					case "startswith":
						comparisonType = ComparisonType.StartsWith;
						break;
					case "endswith":
						comparisonType = ComparisonType.EndsWith;
						break;
					case "doesnotequal":
					case "notequals":
					case "notequal":
						comparisonType = ComparisonType.DoesNotEqual;
						break;
					default:
						comparisonType = ComparisonType.Equals;
						break;
				}

				if (checkToken)
				{
					comparisonValue = this.tokenManager.GetToken(comparisonValue); 
				}

				validations.Add(new ItemValidation(fieldName, comparisonValue, comparisonType));
			}

			return validations;
		}

        /// <summary>
        /// Runs the action.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="action">The action.</param>
        /// <returns>The result of the action.</returns>
	    private ActionResult RunAction(IPage page, IAction action)
        {
            var result = this.actionPipelineService.PerformAction(page, action);

            if (!result.Success && result.Exception != null)
            {
                throw result.Exception;
            }

            return result;
        }
	}
}