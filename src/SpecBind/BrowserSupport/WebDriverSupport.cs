// <copyright file="WebDriverSupport.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.BrowserSupport
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
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
    [ExcludeFromCodeCoverage]
    public class WebDriverSupport
    {
        private static Lazy<ConfigurationSectionHandler> configurationHandler =
            new Lazy<ConfigurationSectionHandler>(SettingHelper.GetConfigurationSection);

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
        internal static IBrowser Browser { get; set; }

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
        /// Things to do before each test run.
        /// </summary>
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            LogDebug(() => "Start Test Run");
            CheckForDriver();
        }

        /// <summary>
        /// Things to do after each test run.
        /// </summary>
        [AfterTestRun]
        public static void AfterTestRun()
        {
            LogDebug(() => "End Test Run");
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
        /// Things to do before each feature.
        /// </summary>
        public static void BeforeFeature()
        {
            LogDebug(() => "Feature: " + FeatureContext.Current.FeatureInfo.Title);
        }

        /// <summary>
        /// Things to do before each scenario.
        /// </summary>
        [BeforeScenario(Order = 100)]
        public void BeforeScenario()
        {
            LogDebug(() => "Scenario: " + ScenarioContext.Current.ScenarioInfo.Title);
            this.InitializeDriver();
            ScenarioContext.Current.Set(Browser, "CurrentBrowser");
        }

        /// <summary>
        /// Initializes the page mapper at the start of the test run.
        /// </summary>
        public void InitializeDriver()
        {
            this.objectContainer.RegisterTypeAs<ProxyLogger, ILogger>();
            var logger = this.objectContainer.Resolve<ILogger>();

            var factory = BrowserFactory.GetBrowserFactory(logger);
            var configSection = configurationHandler.Value;

            bool reusingBrowser = true;
            if (!configSection.BrowserFactory.ReuseBrowser || Browser == null || Browser.IsDisposed)
            {
                Browser = factory.GetBrowser();
                reusingBrowser = false;
            }

            if (configSection.BrowserFactory.EnsureCleanSession)
            {
                Browser.ClearCookies();

                if (reusingBrowser)
                {
                    Browser.ClearUrl();
                }
            }

            // NOTE: Don't register the browser to dispose, since doing so breaks the reuseBrowser support.
            // We will dispose it after scenario or test run as appropriate.
            this.objectContainer.RegisterInstanceAs(Browser, dispose: false);

            this.objectContainer.RegisterInstanceAs<ISettingHelper>(new WrappedSettingHelper());

            var mapper = new PageMapper();
            mapper.Initialize(Browser.BasePageType);
            this.objectContainer.RegisterInstanceAs<IPageMapper>(mapper);

            this.objectContainer.RegisterTypeAs<ScenarioContextHelper, IScenarioContextHelper>();
            this.objectContainer.RegisterTypeAs<TokenManager, ITokenManager>();

            var repository = new ActionRepository(this.objectContainer);
            this.objectContainer.RegisterInstanceAs<IActionRepository>(repository);
            this.objectContainer.RegisterTypeAs<ActionPipelineService, IActionPipelineService>();

            // Initialize the repository
            repository.Initialize();
        }

        /// <summary>
        /// Things to do after every step.
        /// </summary>
        [BeforeStep]
        public void BeforeStep()
        {
            LogDebug(
                () =>
                {
                    var scenarioContextHelper = this.objectContainer.Resolve<IScenarioContextHelper>();
                    return scenarioContextHelper.GetCurrentStepText();
                });
        }

        /// <summary>
        /// Things to do after every step.
        /// </summary>
        [AfterStep]
        public void AfterStep()
        {
            var configSection = configurationHandler.Value;

            switch (configSection.BrowserFactory.WaitForPendingAjaxCallsVia.ToLowerInvariant())
            {
                case "angular":
                    this.WaitForAngular(configSection.BrowserFactory.PageLoadTimeout);
                    break;

                case "jquery":
                    this.WaitForjQuery(configSection.BrowserFactory.PageLoadTimeout);
                    break;
            }
        }

        /// <summary>
        /// Wait for all Angular pending AJAX requests to complete
        /// </summary>
        /// <param name="timeout">The duration after which to stop waiting.</param>
        public void WaitForAngular(TimeSpan timeout)
        {
            WaitForAngular(Convert.ToInt32(timeout.TotalSeconds));
        }

        /// <summary>
        /// Wait for all Angular pending AJAX requests to complete
        /// </summary>
        /// <param name="secondsToWait">The duration after which to stop waiting.</param>
        public static void WaitForAngular(int secondsToWait = 30)
        {
            if (Browser == null)
            {
                return;
            }

            if (Browser.IsDisposed)
            {
                return;
            }

            if (Browser.IsClosed)
            {
                return;
            }

            if (!Browser.Url.StartsWith("http"))
            {
                return;
            }

            var timeout = TimeSpan.FromSeconds(secondsToWait);
            var waitInterval = TimeSpan.FromMilliseconds(500);
            var waiter = new Waiter(timeout, waitInterval);

            try
            {
                waiter.WaitFor(
                    () =>
                    {
                        var pendingRequests = GetAngularPendingRequestCount();
                        if (pendingRequests <= 0)
                        {
                            return true;
                        }

                        LogDebug(
                            () =>
                                string.Format(
                                    "    (WebDriverSupport.WaitForAngular: {0} pending requests)",
                                    pendingRequests));
                        return false;
                    });
            }
            catch (TimeoutException)
            {
                throw new ElementExecuteException("Angular pending requests not completed within {0}", timeout);
            }
        }

        /// <summary>
        /// Wait for all jQuery pending AJAX requests to complete
        /// </summary>
        /// <param name="timeout">The duration after which to stop waiting.</param>
        public void WaitForjQuery(TimeSpan timeout)
        {
            WaitForjQuery(Convert.ToInt32(timeout.TotalSeconds));
        }

        /// <summary>
        /// Wait for all jQuery pending AJAX requests to complete
        /// </summary>
        /// <param name="secondsToWait">The duration after which to stop waiting.</param>
        public static void WaitForjQuery(int secondsToWait = 30)
        {
            if (Browser == null)
            {
                return;
            }

            if (Browser.IsDisposed)
            {
                return;
            }

            if (Browser.IsClosed)
            {
                return;
            }

            if (!Browser.Url.StartsWith("http"))
            {
                return;
            }

            var timeout = TimeSpan.FromSeconds(secondsToWait);
            var waitInterval = TimeSpan.FromMilliseconds(500);
            var waiter = new Waiter(timeout, waitInterval);

            try
            {
                waiter.WaitFor(
                    () =>
                    {
                        var activeCount = GetjQueryActive();
                        if (activeCount <= 0)
                        {
                            return true;
                        }

                        LogDebug(() => "    (WebDriverSupport.WaitForjQuery: still active)");
                        return false;
                    });
            }
            catch (TimeoutException)
            {
                throw new ElementExecuteException("jQuery activity not completed within {0}", timeout);
            }
        }

        /// <summary>
        /// Tears down the web driver
        /// </summary>
        [AfterTestRun]
        public static void TearDownAfterTestRun()
        {
            if (Browser == null)
            {
                return;
            }

            Browser.Close(dispose: true);
        }

        /// <summary>
        /// Performs AfterScenario actions in a controlled order.
        /// </summary>
        [AfterScenario]
        public void ExecuteAfterScenario()
        {
            if (Browser == null)
            {
                return;
            }

            try
            {
                this.CheckForScreenshot();
            }
            finally
            {
                TearDownAfterScenario();
            }
        }

        /// <summary>
        /// Tear down the web driver after scenario, if applicable
        /// </summary>
        public static void TearDownAfterScenario()
        {
            if (Browser == null)
            {
                return;
            }

            try
            {
                var configSection = configurationHandler.Value;
                if (configSection.BrowserFactory.ReuseBrowser)
                {
                    return;
                }
            }
            catch
            {
            }

            try
            {
                Browser.Close(dispose: true);
            }
            finally
            {
                Browser = null;
            }
        }

        /// <summary>
        /// Checks for screenshot.
        /// </summary>
        public void CheckForScreenshot()
        {
            if ((Browser == null) || (!Browser.IsCreated))
            {
                return;
            }

            var scenarioHelper = this.objectContainer.Resolve<IScenarioContextHelper>();
            var ex = scenarioHelper.GetError();
            var isError = false;
            if (ex == null)
            {
                var setting = configurationHandler.Value.BrowserFactory?.CreateScreenshotOnExit;
                if (!setting.GetValueOrDefault(false))
                {
                    return;
                }
            }
            else
            {
                isError = true;
                LogDebug(() => $"{ex.GetType().Name}: {ex.Message}");
            }

            var fileName = scenarioHelper.GetStepFileName(isError);
            var basePath = Directory.GetCurrentDirectory();
            var fullPath = Browser.TakeScreenshot(basePath, fileName);
            Browser.SaveHtml(basePath, fileName);

            var traceListener = this.objectContainer.Resolve<ITraceListener>();
            if (fullPath != null && traceListener != null)
            {
                traceListener.WriteTestOutput("Created Screenshot: {0}", fullPath);
            }

            try
            {
                Browser.Close(dispose: true);
            }
            finally
            {
                Browser = null;
            }
        }

        private static int GetAngularPendingRequestCount()
        {
            try
            {
                return
                    Convert.ToInt32(
                        Browser.ExecuteScript(
                            @"return angular.element(document.body).injector().get('$http').pendingRequests.length;"));
            }
            catch (InvalidOperationException ex)
            {
                LogDebug(
                    () =>
                        string.Format(
                            ":-( WebDriverSupport.GetAngularPendingRequestCount: {0}: {1}", ex.GetType().Name, ex.Message));
                return -1;
            }
        }

        private static int GetjQueryActive()
        {
            try
            {
                return Convert.ToInt32(Browser.ExecuteScript(@"if (typeof jQuery === 'undefined') { return 0; } else { return jQuery.active; }"));
            }
            catch (InvalidOperationException ex)
            {
                LogDebug(
                    () =>
                        string.Format(":-( WebDriverSupport.GetjQueryActive: {0}: {1}", ex.GetType().Name, ex.Message));
                return -1;
            }
        }

        private static void LogDebug(Func<string> messageGenerator)
        {
            if (!Debugger.IsAttached)
            {
                return;
            }

            Debug.WriteLine(messageGenerator());
        }
    }
}
