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
    using SpecBind.Configuration;
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
        private static IBrowser browser;
        private static Lazy<ConfigurationSectionHandler> configurationHandler = new Lazy<ConfigurationSectionHandler>(SettingHelper.GetConfigurationSection);
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
        /// Gets or sets the web browser for the session.
        /// </summary>
        /// <value>
        /// The web browser.
        /// </value>
        internal static IBrowser Browser
        {
            get
            {
                return browser;
            }

            set
            {
                browser = value;
            }
        }

        /// <summary>
        /// Sets the configuration method for testing.
        /// </summary>
        /// <value>
        /// The configuration method factory.
        /// </value>
        internal static Lazy<ConfigurationSectionHandler> ConfigurationMethod
        {
            set
            {
                configurationHandler = value;
            }
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
            var configSection = configurationHandler.Value;

            if (!configSection.BrowserFactory.ReuseBrowser || browser == null)
            {
                browser = factory.GetBrowser();
            }

            if (configSection.BrowserFactory.EnsureCleanSession)
            {
                browser.ClearCookies();
            }

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

            // Initialize the repository
            repository.Initialize();
        }

        /// <summary>
        /// Tears down the web driver
        /// </summary>
        [AfterTestRun]
        public static void TearDownAfterTestRun()
        {
            if (browser == null)
            {
                return;
            }

            browser.Close(dispose: true);
        }

        /// <summary>
        /// Tear down the web driver after scenario, if applicable
        /// </summary>
        [AfterScenario]
        public static void TearDownAfterScenario()
        {
            if (browser == null)
            {
                return;
            }

            var configSection = configurationHandler.Value;
            if (!configSection.BrowserFactory.ReuseBrowser)
            {
                browser.Close(dispose: true);
                browser = null;
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
            var fullPath = browser.TakeScreenshot(basePath, fileName);
            browser.SaveHtml(basePath, fileName);

            var traceListener = this.objectContainer.Resolve<ITraceListener>();
            if (fullPath != null && traceListener != null)
            {
                traceListener.WriteTestOutput("Created Error Screenshot: {0}", fullPath);
            }
        }
    }
}