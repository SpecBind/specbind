// <copyright file="WebDriverSupport.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.BrowserSupport
{
	using System;
	using System.IO;

	using BoDi;

	using SpecBind.ActionPipeline;
	using SpecBind.Actions;
	using SpecBind.Helpers;
	using SpecBind.Pages;

	using TechTalk.SpecFlow;
	using TechTalk.SpecFlow.Tracing;

    /// <summary>
	/// A hooks support class for the web driver.
	/// </summary>
	[Binding]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class WebDriverSupport
	{
		private readonly IObjectContainer objectContainer;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebDriverSupport" /> class.
		/// </summary>
		/// <param name="objectContainer">The object container.</param>
		public WebDriverSupport(IObjectContainer objectContainer)
		{
			this.objectContainer = objectContainer;
		}

		/// <summary>
		/// Initializes the page mapper at the start of the test run.
		/// </summary>
		[BeforeScenario]
		public void InitializeDriver()
		{
			var factory = BrowserFactory.GetBrowserFactory();
			var browser = factory.GetBrowser();
			this.objectContainer.RegisterInstanceAs(browser);

            this.objectContainer.RegisterInstanceAs<ISettingHelper>(new WrappedSettingHelper());

			var mapper = new PageMapper();
			mapper.Initialize(browser.BasePageType);
			this.objectContainer.RegisterInstanceAs<IPageMapper>(mapper);

			this.objectContainer.RegisterInstanceAs<IScenarioContextHelper>(new ScenarioContextHelper());
			this.objectContainer.RegisterInstanceAs(TokenManager.Current);

		    var repository = new ActionRepository(this.objectContainer);
			this.objectContainer.RegisterInstanceAs<IActionRepository>(repository);
			this.objectContainer.RegisterTypeAs<ActionPipelineService, IActionPipelineService>();
            this.objectContainer.RegisterTypeAs<ProxyLogger, ILogger>();

            // Initialize the repository
            repository.Initialize();
		}

		/// <summary>
		/// Tears down the web driver.
		/// </summary>
		[AfterScenario]
		public void TearDownWebDriver()
		{
            var browser = this.objectContainer.Resolve<IBrowser>();

            // Check for an error and capture a screenshot
            this.CheckForScreenshot(browser);

			browser.Close();

// ReSharper disable SuspiciousTypeConversion.Global
			var dispoable = browser as IDisposable;
// ReSharper restore SuspiciousTypeConversion.Global
			if (dispoable != null)
			{
				dispoable.Dispose();
			}
		}

        /// <summary>
        /// Checks for screenshot.
        /// </summary>
        /// <param name="browser">The browser.</param>
        private void CheckForScreenshot(IBrowser browser)
        {
            var scenarioHelper = this.objectContainer.Resolve<IScenarioContextHelper>();
            if (scenarioHelper.GetError() == null)
            {
                return;
            }
            
            var fileName = scenarioHelper.GetStepFileName();
            var basePath = Directory.GetCurrentDirectory();
            var fullPath = browser.TakeScreenshot(basePath, fileName);

            var traceListener = this.objectContainer.Resolve<ITraceListener>();
            if (fullPath != null && traceListener != null)
            {
                traceListener.WriteTestOutput("Created Error Screenshot: {0}", fullPath);       
            }
        }
	}
}