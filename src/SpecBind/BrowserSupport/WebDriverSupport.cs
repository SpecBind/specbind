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
        private static BrowserFactory browserFactory;
        private static IBrowser browser;
        private static ActionRepository actionRepository;

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
            using (BrowserFactory factory = BrowserFactory.GetBrowserFactory(new NullLogger()))
            {
                factory.ValidateDriverSetup();
            }
        }

        /// <summary>
        /// Things to do before each feature.
        /// </summary>
        public static void BeforeFeature()
        {
            LogDebug(() => "Feature: " + FeatureContext.Current.FeatureInfo.Title);
        }

        /// <summary>
        /// Initializes the browser.
        /// </summary>
        /// <param name="objectContainer">The object container.</param>
        /// <returns>The browser.</returns>
        public static IBrowser InitializeBrowser(IObjectContainer objectContainer)
        {
            bool reusingBrowser = true;
            if (!browserFactory.Configuration.ReuseBrowser || browser == null || browser.IsDisposed)
            {
                browser = browserFactory.GetBrowser();
                ScenarioContext.Current.Set(browser, "CurrentBrowser");
                reusingBrowser = false;
            }

            if (browserFactory.Configuration.EnsureCleanSession)
            {
                browser.ClearCookies();

                if (reusingBrowser)
                {
                    browser.ClearUrl();
                }
            }

            IPageMapper pageMapper = objectContainer.Resolve<IPageMapper>();
            if (pageMapper.MapCount == 0)
            {
                pageMapper.Initialize(browser.BasePageType);
            }

            return browser;
        }

        /// <summary>
        /// Resets the driver.
        /// </summary>
        public static void ResetDriver()
        {
            browserFactory.ResetDriver(browser);
        }

        /// <summary>
        /// Wait for all Angular pending AJAX requests to complete
        /// </summary>
        /// <param name="secondsToWait">The duration after which to stop waiting.</param>
        public static void WaitForAngular(int secondsToWait = 30)
        {
            if (browser == null)
            {
                return;
            }

            if (browser.IsDisposed)
            {
                return;
            }

            if (browser.IsClosed)
            {
                return;
            }

            if (!browser.CanGetUrl())
            {
                return;
            }

            if (!browser.Url.StartsWith("http"))
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
        /// <param name="secondsToWait">The duration after which to stop waiting.</param>
        public static void WaitForjQuery(int secondsToWait = 30)
        {
            if (browser == null)
            {
                return;
            }

            if (browser.IsDisposed)
            {
                return;
            }

            if (browser.IsClosed)
            {
                return;
            }

            if (!browser.CanGetUrl())
            {
                return;
            }

            if (!browser.Url.StartsWith("http"))
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
        [AfterScenario(Order = 2)]
        public static void TearDownAfterTest()
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
        public static void TearDownAfterScenario()
        {
            if (browser == null)
            {
                return;
            }

            try
            {
                if (browserFactory.Configuration.ReuseBrowser)
                {
                    return;
                }
            }
            catch
            {
            }

            try
            {
                browser.Close(dispose: true);
            }
            finally
            {
                browser = null;
            }
        }

        /// <summary>
        /// Things to do before each scenario.
        /// </summary>
        [BeforeScenario(Order = 100)]
        public void BeforeScenario()
        {
            LogDebug(() => "Scenario: " + ScenarioContext.Current.ScenarioInfo.Title);
            this.InitializeDriver();
        }

        /// <summary>
        /// Initializes the page mapper at the start of the test run.
        /// </summary>
        public void InitializeDriver()
        {
            this.objectContainer.RegisterTypeAs<ProxyLogger, ILogger>();

            var logger = this.objectContainer.Resolve<ILogger>();

            browserFactory = BrowserFactory.GetBrowserFactory(logger);
            this.objectContainer.RegisterInstanceAs(browserFactory, dispose: true);

            var mapper = new PageMapper();
            this.objectContainer.RegisterInstanceAs<IPageMapper>(mapper);

            this.objectContainer.RegisterFactoryAs((container) =>
            {
                return InitializeBrowser(container);
            });

            this.objectContainer.RegisterInstanceAs<ISettingHelper>(new WrappedSettingHelper());

            this.objectContainer.RegisterTypeAs<ScenarioContextHelper, IScenarioContextHelper>();
            this.objectContainer.RegisterTypeAs<TokenManager, ITokenManager>();

            actionRepository = new ActionRepository(this.objectContainer);
            this.objectContainer.RegisterInstanceAs<IActionRepository>(actionRepository);

            var actionPipelineService = new ActionPipelineService(actionRepository);
            this.objectContainer.RegisterInstanceAs<IActionPipelineService>(actionPipelineService);
        }

        /// <summary>
        /// Things to do before every step.
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
            switch (browserFactory.Configuration.WaitForPendingAjaxCallsVia?.ToLowerInvariant())
            {
                case "angular":
                    this.WaitForAngular(browserFactory.Configuration.PageLoadTimeout);
                    break;

                case "jquery":
                    this.WaitForjQuery(browserFactory.Configuration.PageLoadTimeout);
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
        /// Wait for all jQuery pending AJAX requests to complete
        /// </summary>
        /// <param name="timeout">The duration after which to stop waiting.</param>
        public void WaitForjQuery(TimeSpan timeout)
        {
            WaitForjQuery(Convert.ToInt32(timeout.TotalSeconds));
        }

        /// <summary>
        /// Performs AfterScenario actions in a controlled order.
        /// </summary>
        [AfterScenario(Order = 1)]
        public void ExecuteAfterScenario()
        {
            if (browser == null)
            {
                return;
            }

            try
            {
                this.CheckForScreenshot();
            }
            catch (Exception ex)
            {
                LogDebug(() => $"{ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                TearDownAfterScenario();
            }
        }

        /// <summary>
        /// Checks for screenshot.
        /// </summary>
        public void CheckForScreenshot()
        {
            if ((browser == null) || (!browser.IsCreated))
            {
                return;
            }

            var scenarioHelper = this.objectContainer.Resolve<IScenarioContextHelper>();
            var ex = scenarioHelper.GetError();
            var isError = false;
            if (ex == null)
            {
                if (!browserFactory.Configuration.CreateScreenshotOnExit)
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

            browser.SaveHtml(basePath, fileName);

            string fullPath = browser.TakeScreenshot(basePath, fileName);
            if (fullPath != null)
            {
                var traceListener = this.objectContainer.Resolve<ITraceListener>();
                if (traceListener != null)
                {
                    traceListener.WriteTestOutput("Created Screenshot: {0}", fullPath);
                }
            }

            try
            {
                browser.Close(dispose: true);
            }
            finally
            {
                browser = null;
            }
        }

        /// <summary>
        /// Sets the browser factory.
        /// </summary>
        /// <param name="newBrowserFactory">The new browser factory.</param>
        internal static void SetBrowserFactory(BrowserFactory newBrowserFactory)
        {
            browserFactory = newBrowserFactory;
        }

        /// <summary>
        /// Sets the current browser.
        /// </summary>
        /// <param name="newBrowser">The new browser.</param>
        internal static void SetCurrentBrowser(IBrowser newBrowser)
        {
            browser = newBrowser;
        }

        private static int GetAngularPendingRequestCount()
        {
            try
            {
                return
                    Convert.ToInt32(
                        browser.ExecuteScript(
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
                return Convert.ToInt32(browser.ExecuteScript(@"if (typeof jQuery === 'undefined') { return 0; } else { return jQuery.active; }"));
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
