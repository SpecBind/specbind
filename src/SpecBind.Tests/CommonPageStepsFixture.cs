// <copyright file="CommonPageStepsFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
	using System;
	using System.Collections.Generic;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using System.Linq;

	using SpecBind.ActionPipeline;
	using SpecBind.Actions;
	using SpecBind.BrowserSupport;
	using SpecBind.Helpers;
	using SpecBind.Pages;
	using SpecBind.Tests.Support;

	using TechTalk.SpecFlow;

	/// <summary>
	///     A test fixture for common page steps.
	/// </summary>
	[TestClass]
	public class CommonPageStepsFixture
	{
		/// <summary>
		/// Tests the GivenNavigateToPageStep with a successful result.
		/// </summary>
		[TestMethod]
		public void TestGivenNavigateToPageStep()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
			
			var testPage = new Mock<IPage>();
			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(b => b.GoToPage(typeof(TestBase), null)).Returns(testPage.Object);

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			
			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			pageMapper.Setup(p => p.GetTypeFromName("mypage")).Returns(typeof(TestBase));
			
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.SetValue(It.IsAny<IPage>(), CommonPageSteps.CurrentPageKey));

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			steps.GivenNavigateToPageStep("mypage");

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
			pipelineService.VerifyAll();
		}

		/// <summary>
		/// Tests the GivenNavigateToPageStep with a successful result.
		/// </summary>
		[TestMethod]
		public void TestGivenNavigateToPageStepWithArguments()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var testPage = new Mock<IPage>();
			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(b => b.GoToPage(typeof(TestBase), It.Is<IDictionary<string, string>>(d => d.Count == 2))).Returns(testPage.Object);

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			pageMapper.Setup(p => p.GetTypeFromName("mypage")).Returns(typeof(TestBase));

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.SetValue(It.IsAny<IPage>(), CommonPageSteps.CurrentPageKey));

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Id", "Part");
			table.AddRow("1", "A");

			steps.GivenNavigateToPageWithArgumentsStep("mypage", table);

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the GivenNavigateToPageStep with the page type not being found.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(PageNavigationException))]
		public void TestGivenNavigateToPageStepTypeNotFound()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			pageMapper.Setup(p => p.GetTypeFromName("mypage")).Returns((Type)null);

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			try
			{
				steps.GivenNavigateToPageStep("mypage");
			}
			catch (PageNavigationException ex)
			{
				StringAssert.Contains(ex.Message, "mypage");

				browser.VerifyAll();
				pageDataFiller.VerifyAll();
				pageMapper.VerifyAll();
				scenarioContext.VerifyAll();
				tokenManager.VerifyAll();

				throw;
			}
		}

		/// <summary>
		/// Tests the GivenEnsureOnPageStep with a successful result.
		/// </summary>
		[TestMethod]
		public void TestGivenEnsureOnPageStep()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var testPage = new Mock<IPage>();
			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(b => b.Page(typeof(TestBase))).Returns(testPage.Object);
			browser.Setup(b => b.EnsureOnPage(testPage.Object));

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			pageMapper.Setup(p => p.GetTypeFromName("mypage")).Returns(typeof(TestBase));

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.SetValue(It.IsAny<IPage>(), CommonPageSteps.CurrentPageKey));

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			steps.GivenEnsureOnPageStep("mypage");

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the GivenEnsureOnPageStep with the page type not being found.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(PageNavigationException))]
		public void TestGivenEnsureOnPageStepTypeNotFound()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			pageMapper.Setup(p => p.GetTypeFromName("mypage")).Returns((Type)null);

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			try
			{
				steps.GivenEnsureOnPageStep("mypage");
			}
			catch (PageNavigationException ex)
			{
				StringAssert.Contains(ex.Message, "mypage");

				browser.VerifyAll();
				pageDataFiller.VerifyAll();
				pageMapper.VerifyAll();
				scenarioContext.VerifyAll();
				tokenManager.VerifyAll();

				throw;
			}
		}

		/// <summary>
		/// Tests the GivenEnsureOnPageStep with the page not existing found.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(PageNavigationException))]
		public void TestGivenEnsureOnPageStepPageNotFound()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var testPage = new Mock<IPage>();

			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(b => b.Page(typeof(TestBase))).Returns(testPage.Object);
			browser.Setup(b => b.EnsureOnPage(testPage.Object)).Throws(new PageNavigationException("Page Not found"));
			
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			pageMapper.Setup(p => p.GetTypeFromName("mypage")).Returns(typeof(TestBase));

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			try
			{
				steps.GivenEnsureOnPageStep("mypage");
			}
			catch (PageNavigationException)
			{
				browser.VerifyAll();
				pageDataFiller.VerifyAll();
				pageMapper.VerifyAll();
				scenarioContext.VerifyAll();
				tokenManager.VerifyAll();

				throw;
			}
		}

		/// <summary>
		/// Tests the WhenIChooseALinkStep method with a successful result.
		/// </summary>
		[TestMethod]
		public void TestWhenIChooseALinkStep()
		{
			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var testPage = new Mock<IPage>();

			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
			pipelineService.Setup(p => p.PerformAction(testPage.Object, It.IsAny<ButtonClickAction>()))
						   .Returns(ActionResult.Successful());

			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(testPage.Object);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			steps.WhenIChooseALinkStep("my link");

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the WhenIChooseALinkStep method when a step has not been set.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(PageNavigationException))]
		public void TestWhenIChooseALinkStepContextNotSet()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns((IPage)null);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			ExceptionHelper.SetupForException<PageNavigationException>(
				() => steps.WhenIChooseALinkStep("my link"),
				e =>
					{
						browser.VerifyAll();
						pageDataFiller.VerifyAll();
						pageMapper.VerifyAll();
						scenarioContext.VerifyAll();
						tokenManager.VerifyAll();
					});
		}

		/// <summary>
		/// Tests the WhenIEnterDataInFieldsStep method with a null table.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestWhenIEnterDataInFieldsStepNullTable()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => steps.WhenIEnterDataInFieldsStep(null),
				e =>
				{
					StringAssert.Contains(e.Message, "A table must be specified for this step");
					
					browser.VerifyAll();
					pageDataFiller.VerifyAll();
					pageMapper.VerifyAll();
					scenarioContext.VerifyAll();
					tokenManager.VerifyAll();
				});
		}

		/// <summary>
		/// Tests the WhenIEnterDataInFieldsStep method with a table that has no field column.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestWhenIEnterDataInFieldsStepMissingFieldColumn()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Value");

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => steps.WhenIEnterDataInFieldsStep(table),
				e =>
				{
					StringAssert.Contains(e.Message, "A table must be specified for this step");

					browser.VerifyAll();
					pageDataFiller.VerifyAll();
					pageMapper.VerifyAll();
					scenarioContext.VerifyAll();
					tokenManager.VerifyAll();
				});
		}

		/// <summary>
		/// Tests the WhenIEnterDataInFieldsStep method with a table that has no field column.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestWhenIEnterDataInFieldsStepMissingValueColumn()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field");

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => steps.WhenIEnterDataInFieldsStep(table),
				e =>
				{
					StringAssert.Contains(e.Message, "A table must be specified for this step");

					browser.VerifyAll();
					pageDataFiller.VerifyAll();
					pageMapper.VerifyAll();
					scenarioContext.VerifyAll();
					tokenManager.VerifyAll();
				});
		}

		/// <summary>
		/// Tests the WhenIEnterDataInFieldsStep method with a successful result.
		/// </summary>
		[TestMethod]
		public void TestWhenIEnterDataInFieldsStep()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var testPage = new Mock<IPage>();
			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			tokenManager.Setup(t => t.SetToken("myvalue")).Returns(new Func<string, string>(s => s));

			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			pageDataFiller.Setup(p => p.FillField(testPage.Object, "myfield", "myvalue"));

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(testPage.Object);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field", "Value");
			table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "My Field" },
								 { "Value", "myvalue" }
				             });

			steps.WhenIEnterDataInFieldsStep(table);

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the ThenISeeStep method with a null table.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestThenISeeStepNullTable()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => steps.ThenISeeStep(null),
				e =>
				{
					StringAssert.Contains(e.Message, "A table must be specified for this step");

					browser.VerifyAll();
					pageDataFiller.VerifyAll();
					pageMapper.VerifyAll();
					scenarioContext.VerifyAll();
					tokenManager.VerifyAll();
				});
		}

		/// <summary>
		/// Tests the ThenISeeStep method with a table that has no field column.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestThenISeeStepMissingFieldColumn()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Value");

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => steps.ThenISeeStep(table),
				e =>
				{
					StringAssert.Contains(e.Message, "A table must be specified for this step");

					browser.VerifyAll();
					pageDataFiller.VerifyAll();
					pageMapper.VerifyAll();
					scenarioContext.VerifyAll();
					tokenManager.VerifyAll();
				});
		}

		/// <summary>
		/// Tests the ThenISeeStep method with a table that has no field column.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestThenISeeStepMissingValueColumn()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field");

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => steps.ThenISeeStep(table),
				e =>
				{
					StringAssert.Contains(e.Message, "A table must be specified for this step");

					browser.VerifyAll();
					pageDataFiller.VerifyAll();
					pageMapper.VerifyAll();
					scenarioContext.VerifyAll();
					tokenManager.VerifyAll();
				});
		}

		/// <summary>
		/// Tests the ThenISeeStep method with a table that has no rule column.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestThenISeeStepMissingRuleColumn()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field", "Value");

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => steps.ThenISeeStep(table),
				e =>
				{
					StringAssert.Contains(e.Message, "A table must be specified for this step");

					browser.VerifyAll();
					pageDataFiller.VerifyAll();
					pageMapper.VerifyAll();
					scenarioContext.VerifyAll();
					tokenManager.VerifyAll();
				});
		}

		/// <summary>
		/// Tests the ThenISeeStep method with headers but no rows just exists.
		/// </summary>
		[TestMethod]
		public void TestThenISeeStepEmptyTableRunsCorrectly()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field", "Rule", "Value");

			steps.ThenISeeStep(table);

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the ThenISeeStep method with a successful result.
		/// </summary>
		[TestMethod]
		public void TestThenISeeStepEqualsComparison()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var testPage = new Mock<IPage>();
			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			tokenManager.Setup(t => t.GetToken("myvalue")).Returns(new Func<string, string>(s => s));

			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			pageDataFiller.Setup(p => p.ValidateItem(
				testPage.Object, It.Is<ICollection<ItemValidation>>(l => l.All(v => v.FieldName == "myfield" && v.ComparisonValue == "myvalue" && v.ComparisonType == ComparisonType.Equals))));

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(testPage.Object);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field", "Rule", "Value");
			table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "myfield" },
								 { "Rule", "equals" },
								 { "Value", "myvalue" }
				             });

			steps.ThenISeeStep(table);

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the ThenISeeStep method multiple comparisons in the table.
		/// </summary>
		[TestMethod]
		public void TestThenISeeStepMultipleComparisons()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var testPage = new Mock<IPage>();
			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			tokenManager.Setup(t => t.GetToken("myvalue")).Returns(new Func<string, string>(s => s));
			tokenManager.Setup(t => t.GetToken("somevalue")).Returns(new Func<string, string>(s => s));

			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			pageDataFiller.Setup(p => p.ValidateItem(
				testPage.Object, 
				It.Is<ICollection<ItemValidation>>(
						list => list.Any(v => v.FieldName == "myfield" && v.ComparisonValue == "myvalue" && v.ComparisonType == ComparisonType.Equals) &&
						        list.Any(v => v.FieldName == "myotherfield" && v.ComparisonValue == "somevalue" && v.ComparisonType == ComparisonType.Equals))));
			
			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(testPage.Object);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field", "Rule", "Value");
			table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "myfield" },
								 { "Rule", "equals" },
								 { "Value", "myvalue" }
				             });

			table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "myotherfield" },
								 { "Rule", "equals" },
								 { "Value", "somevalue" }
				             });

			steps.ThenISeeStep(table);

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the ThenISeeStep method with other comparison types.
		/// </summary>
		[TestMethod]
		public void TestThenISeeStepContainsComparisonType()
		{
			RunSeeStepScenario("contains", ComparisonType.Contains);
		}

		/// <summary>
		/// Tests the ThenISeeStep method with other comparison types.
		/// </summary>
		[TestMethod]
		public void TestThenISeeStepStartsWithComparisonType()
		{
			RunSeeStepScenario("starts with", ComparisonType.StartsWith);
		}

		/// <summary>
		/// Tests the ThenISeeStep method with other comparison types.
		/// </summary>
		[TestMethod]
		public void TestThenISeeStepEndsWithComparisonType()
		{
			RunSeeStepScenario("ends with", ComparisonType.EndsWith);
		}

		/// <summary>
		/// Tests the ThenISeeStep method with other comparison types.
		/// </summary>
		[TestMethod]
		public void TestThenISeeStepDoesNotEqualComparisonType()
		{
			RunSeeStepScenario("does not equal", ComparisonType.DoesNotEqual);
		}

		/// <summary>
		/// Tests the ThenISeeStep method with a exists comparison.
		/// </summary>
		[TestMethod]
		public void TestThenISeeStepExistsComparison()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var testPage = new Mock<IPage>();
			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			pageDataFiller.Setup(p => p.ValidateItem(testPage.Object, It.Is<ICollection<ItemValidation>>(l => l.All(v => v.ComparisonType == ComparisonType.Exists && v.ComparisonValue == "True"))));

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(testPage.Object);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field", "Rule", "Value");
			table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "myfield" },
								 { "Rule", "exists" },
								 { "Value", "myvalue" }
				             });

			steps.ThenISeeStep(table);

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the ThenISeeStep method with a does not exist comparison.
		/// </summary>
		[TestMethod]
		public void TestThenISeeStepDoesNotExistComparison()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var testPage = new Mock<IPage>();
			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			pageDataFiller.Setup(p => p.ValidateItem(testPage.Object, It.Is<ICollection<ItemValidation>>(l => l.All(v => v.ComparisonType == ComparisonType.Exists && v.ComparisonValue == "False"))));

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(testPage.Object);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field", "Rule", "Value");
			table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "myfield" },
								 { "Rule", "does not exist" },
								 { "Value", "myvalue" }
				             });

			steps.ThenISeeStep(table);

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the ThenISeeStep method with a enabled comparison.
		/// </summary>
		[TestMethod]
		public void TestThenISeeStepEnabledComparison()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var testPage = new Mock<IPage>();
			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			pageDataFiller.Setup(p => p.ValidateItem(testPage.Object, It.Is<ICollection<ItemValidation>>(l => l.All(v => v.ComparisonType == ComparisonType.Enabled && v.ComparisonValue == "True"))));

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(testPage.Object);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field", "Rule", "Value");
			table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "myfield" },
								 { "Rule", "enabled" },
								 { "Value", "myvalue" }
				             });

			steps.ThenISeeStep(table);

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the ThenISeeStep method with a not enabled comparison.
		/// </summary>
		[TestMethod]
		public void TestThenISeeStepNotEnabledComparison()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var testPage = new Mock<IPage>();
			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);

			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			pageDataFiller.Setup(p => p.ValidateItem(testPage.Object, It.Is<ICollection<ItemValidation>>(l => l.All(v => v.ComparisonType == ComparisonType.Enabled && v.ComparisonValue == "False"))));

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(testPage.Object);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field", "Rule", "Value");
			table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "myfield" },
								 { "Rule", "disabled" },
								 { "Value", "myvalue" }
				             });

			steps.ThenISeeStep(table);

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the ThenISeeStep method with a not enabled comparison.
		/// </summary>
		[TestMethod]
		public void TestThenISeeStepListRuleScenarios()
		{
			RunStepListScenario("contains", ComparisonType.Contains);
			RunStepListScenario("does not contain", ComparisonType.DoesNotContain);
			RunStepListScenario("starts with", ComparisonType.StartsWith);
			RunStepListScenario("ends with", ComparisonType.EndsWith);
		}

		/// <summary>
		/// Runs the step list with an invalid rule type.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestThenISeeStepListInvalidRuleType()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field", "Rule", "Value");
			table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "myfield" },
								 { "Rule", "equals" },
								 { "Value", "myvalue" }
				             });

			ExceptionHelper.SetupForException<InvalidOperationException>(
				() => steps.ThenISeeListStep("myfield", "equals", table),
				ex =>
					{
						browser.VerifyAll();
						pageDataFiller.VerifyAll();
						pageMapper.VerifyAll();
						scenarioContext.VerifyAll();
						tokenManager.VerifyAll();
					});
		}

		/// <summary>
		/// Runs the step list with no table rows.
		/// </summary>
		[TestMethod]
		public void TestThenISeeStepListNoRows()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field", "Rule", "Value");

			steps.ThenISeeListStep("myfield", "contains", table);

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the GivenEnsureOnListItemStep method for common path.
		/// </summary>
		[TestMethod]
		public void TestGivenEnsureOnListItemStep()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var page = new Mock<IPage>();
			
			var listItem = new Mock<IPage>();
			listItem.Setup(l => l.Highlight());

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			pageDataFiller.Setup(p => p.GetListItem(page.Object, "myproperty", 2)).Returns(listItem.Object);

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.ContainsTag(TagConstants.Debug)).Returns(true);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(page.Object);
			scenarioContext.Setup(s => s.SetValue(listItem.Object, CommonPageSteps.CurrentPageKey));

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			steps.GivenEnsureOnListItemStep("my property", 2);

			browser.VerifyAll();
			listItem.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the GivenEnsureOnListItemStep method with debug disabled.
		/// </summary>
		[TestMethod]
		public void TestGivenEnsureOnListItemStepNoItemHighlight()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var page = new Mock<IPage>();

			var listItem = new Mock<IPage>();

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			pageDataFiller.Setup(p => p.GetListItem(page.Object, "myproperty", 2)).Returns(listItem.Object);

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.ContainsTag(TagConstants.Debug)).Returns(false);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(page.Object);
			scenarioContext.Setup(s => s.SetValue(listItem.Object, CommonPageSteps.CurrentPageKey));

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			steps.GivenEnsureOnListItemStep("my property", 2);

			browser.VerifyAll();
			listItem.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the GivenEnsureOnDialogStep method for happy path.
		/// </summary>
		[TestMethod]
		public void TestGivenEnsureOnDialogStep()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var page = new Mock<IPage>();
			var listItem = new Mock<IPage>();

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			pageDataFiller.Setup(p => p.GetElementAsPage(page.Object, "myproperty")).Returns(listItem.Object);

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(page.Object);
			scenarioContext.Setup(s => s.SetValue(listItem.Object, CommonPageSteps.CurrentPageKey));

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			steps.GivenEnsureOnDialogStep("my property");

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Tests the SetTokenFromFieldStep method pulls the value from the field and sets the value.
		/// </summary>
		[TestMethod]
		public void TestSetTokenFromFieldStepSetsCurrentValue()
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var page = new Mock<IPage>();

			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			tokenManager.Setup(t => t.SetToken("MyToken", "The Field Value"));

			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			pageDataFiller.Setup(p => p.GetItemValue(It.IsAny<IPage>(), "SomeField")).Returns("The Field Value");

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(page.Object);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			steps.SetTokenFromFieldStep("MyToken", "SomeField");

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Runs the see step scenario for different validation types.
		/// </summary>
		/// <param name="rule">The rule.</param>
		/// <param name="comparisonType">Type of the comparison.</param>
		private static void RunSeeStepScenario(string rule, ComparisonType comparisonType)
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var testPage = new Mock<IPage>();
			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			tokenManager.Setup(t => t.GetToken("myvalue")).Returns(new Func<string, string>(s => s));

			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			pageDataFiller.Setup(p => p.ValidateItem(testPage.Object, It.Is<ICollection<ItemValidation>>(l => l.All(v => v.FieldName == "myfield" && v.ComparisonValue == "myvalue" && v.ComparisonType == comparisonType))));

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(testPage.Object);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field", "Rule", "Value");
			table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "myfield" },
								 { "Rule", rule },
								 { "Value", "myvalue" }
				             });

			steps.ThenISeeStep(table);

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}

		/// <summary>
		/// Runs the step list scenario.
		/// </summary>
		/// <param name="rule">The rule.</param>
		/// <param name="comparisonType">Type of the comparison.</param>
		private static void RunStepListScenario(string rule, ComparisonType comparisonType)
		{
			var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);

			var testPage = new Mock<IPage>();

			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
			tokenManager.Setup(t => t.GetToken("myvalue")).Returns(new Func<string, string>(s => s));

			var pageDataFiller = new Mock<IPageDataFiller>(MockBehavior.Strict);
			pageDataFiller.Setup(
				p =>
				p.ValidateList(
					testPage.Object,
					"myfield",
					comparisonType,
					It.Is<ICollection<ItemValidation>>(
						v => v.Any(i => i.FieldName == "myfield" && i.ComparisonType == ComparisonType.Equals && i.ComparisonValue == "myvalue"))));

			var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);

			var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
			scenarioContext.Setup(s => s.GetValue<IPage>(CommonPageSteps.CurrentPageKey)).Returns(testPage.Object);

			var steps = new CommonPageSteps(browser.Object, pageDataFiller.Object, pageMapper.Object, scenarioContext.Object, tokenManager.Object, pipelineService.Object);

			var table = new Table("Field", "Rule", "Value");
			table.AddRow(new Dictionary<string, string>
				             {
					             { "Field", "myfield" },
								 { "Rule", "equals" },
								 { "Value", "myvalue" }
				             });

			steps.ThenISeeListStep("myfield", rule, table);

			browser.VerifyAll();
			pageDataFiller.VerifyAll();
			pageMapper.VerifyAll();
			scenarioContext.VerifyAll();
			tokenManager.VerifyAll();
		}
	}
}