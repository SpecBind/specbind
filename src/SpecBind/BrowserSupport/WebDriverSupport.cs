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
        internal static IBrowser Browser;
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
        /// Checks the browser factory for any necessary drivers.
        /// </summary>
        [BeforeTestRun]
        public static void CheckForDriver()
        {
            var factory = BrowserFactory.GetBrowserFactory(new NullLogger());
            factory.ValidateDriverSetup();
        }
             
        /// <summary>
        /// Initializes the page mapper at the start of the test run.
        /// </summary>
        [BeforeScenario]
        public void InitializeDriver()
        {
            this.objectContainer.RegisterTypeAs<ProxyLogger, ILogger>();
            var logger = this.objectContainer.Resolve<ILogger>();

            var factory = BrowserFactory.GetBrowserFactory(logger);
            var configSection = SettingHelper.GetConfigurationSection();

            if (!configSection.BrowserFactory.ReuseBrowser || Browser == null) Browser = factory.GetBrowser();
            if (configSection.BrowserFactory.EnsureCleanSession) Browser.ClearCookies();
            this.objectContainer.RegisterInstanceAs(Browser);

            this.objectContainer.RegisterInstanceAs<ISettingHelper>(new WrappedSettingHelper());

            var mapper = new PageMapper();
            mapper.Initialize(Browser.BasePageType);
            this.objectContainer.RegisterInstanceAs<IPageMapper>(mapper);

            this.objectContainer.RegisterInstanceAs<IScenarioContextHelper>(new ScenarioContextHelper());
            this.objectContainer.RegisterInstanceAs(TokenManager.Current);

            var repository = new ActionRepository(this.objectContainer);
            this.objectContainer.RegisterInstanceAs<IActionRepository>(repository);
            this.objectContainer.RegisterTypeAs<ActionPipelineService, IActionPipelineService>();
            
            // Initialize the repository
            repository.Initialize();
        }

        /// <summary>
        /// Tears down the web driver
        /// </summary>
        [AfterTestRun]
        public static void TearDownAfterTestRun()
        {
            if (Browser == null) return;
            Browser.Close(dispose: true);
        }

        /// <summary>
        /// Tear down the web driver after scenario, if applicable
        /// </summary>
        [AfterScenario]
        public static void TearDownAfterScenario()
        {
            if (Browser == null) return;

            var configSection = SettingHelper.GetConfigurationSection();
            if (!configSection.BrowserFactory.ReuseBrowser)
            {
                Browser.Close(dispose: true);
                Browser = null;
            }

        }

        /// <summary>
        /// Checks for screenshot.
        /// </summary>
        [AfterScenario]
        public void CheckForScreenshot()
        {
            var scenarioHelper = this.objectContainer.Resolve<IScenarioContextHelper>();
            if (scenarioHelper.GetError() == null)
            {
                return;
            }
            
            var fileName = scenarioHelper.GetStepFileName();
            var basePath = Directory.GetCurrentDirectory();
            var fullPath = Browser.TakeScreenshot(basePath, fileName);
            Browser.SaveHtml(basePath, fileName);

            var traceListener = this.objectContainer.Resolve<ITraceListener>();
            if (fullPath != null && traceListener != null)
            {
                traceListener.WriteTestOutput("Created Error Screenshot: {0}", fullPath);       
            }
        }
    }
}