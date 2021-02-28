// <copyright file="WebDriverSupport.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.BrowserSupport
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

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
        private static ActionRepository actionRepository;
        private static bool checkedDriver;
        private readonly IObjectContainer objectContainer;
        private readonly ScenarioContext scenarioContext;
        private readonly TestResultFileNotifier testResultFileNotifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverSupport" /> class.
        /// </summary>
        /// <param name="objectContainer">The object container.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="testResultFileNotifier">The test result file notifier.</param>
        public WebDriverSupport(
            IObjectContainer objectContainer,
            ScenarioContext scenarioContext,
            TestResultFileNotifier testResultFileNotifier)
        {
            this.objectContainer = objectContainer;
            this.scenarioContext = scenarioContext;
            this.testResultFileNotifier = testResultFileNotifier;
        }

        /// <summary>
        /// Gets or sets the current browser.
        /// </summary>
        /// <returns>The current browser.</returns>
        public static IBrowser CurrentBrowser { get; set; }

        /// <summary>
        /// Things to do after each test run.
        /// </summary>
        [AfterTestRun]
        public static void AfterTestRun()
        {
            LogDebug(() => "End Test Run");
        }

        /// <summary>
        /// Things to do before each feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            LogDebug(() => "Feature: " + featureContext.FeatureInfo.Title);
        }

        /// <summary>
        /// Initializes the browser.
        /// </summary>
        /// <param name="objectContainer">The object container.</param>
        /// <returns>The browser.</returns>
        public static IBrowser InitializeBrowser(IObjectContainer objectContainer)
        {
            bool reusingBrowser = true;
            if (!browserFactory.Configuration.ReuseBrowser || CurrentBrowser == null || CurrentBrowser.IsDisposed)
            {
                CurrentBrowser = browserFactory.GetBrowser();
                reusingBrowser = false;
            }

            if (browserFactory.Configuration.EnsureCleanSession)
            {
                CurrentBrowser.ClearCookies();

                if (reusingBrowser)
                {
                    CurrentBrowser.ClearUrl();
                }
            }

            IPageMapper pageMapper = objectContainer.Resolve<IPageMapper>();
            if (pageMapper.MapCount == 0)
            {
                pageMapper.Initialize(CurrentBrowser.BasePageType);
            }

            return CurrentBrowser;
        }

        /// <summary>
        /// Resets the driver.
        /// </summary>
        public static void ResetDriver()
        {
            browserFactory.ResetDriver(CurrentBrowser);
        }

        /// <summary>
        /// Wait for all Angular pending AJAX requests to complete
        /// </summary>
        /// <param name="secondsToWait">The duration after which to stop waiting.</param>
        public static void WaitForAngular(int secondsToWait = 30)
        {
            if (CurrentBrowser == null)
            {
                return;
            }

            if (CurrentBrowser.IsDisposed)
            {
                return;
            }

            if (CurrentBrowser.IsClosed)
            {
                return;
            }

            if (!CurrentBrowser.CanGetUrl())
            {
                return;
            }

            if (!CurrentBrowser.Url.StartsWith("http"))
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
        /// <param name="expectedJQueryDefined">if set to <c>true</c> jQuery is expected to be defined.</param>
        public static void WaitForjQuery(int secondsToWait = 30, bool expectedJQueryDefined = false)
        {
            if (CurrentBrowser == null)
            {
                return;
            }

            if (CurrentBrowser.IsDisposed)
            {
                return;
            }

            if (CurrentBrowser.IsClosed)
            {
                return;
            }

            if (!CurrentBrowser.CanGetUrl())
            {
                return;
            }

            if (!CurrentBrowser.Url.StartsWith("http"))
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
                        var activeCount = GetjQueryActive(expectedJQueryDefined);
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
        [AfterScenario(Order = int.MaxValue)]
        public static void TearDownAfterTest()
        {
            if (CurrentBrowser == null)
            {
                return;
            }

            CurrentBrowser.Close(dispose: true);
        }

        /// <summary>
        /// Tear down the web driver after scenario, if applicable
        /// </summary>
        public static void TearDownAfterScenario()
        {
            if (CurrentBrowser == null)
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
            catch (Exception ex)
            {
                LogDebug(() => ex.ToString());
            }

            try
            {
                CurrentBrowser.Close(dispose: true);
            }
            finally
            {
                CurrentBrowser = null;
            }
        }

        /// <summary>
        /// Things to do before each test run.
        /// </summary>
        [Before(Order = 1)]
        public void BeforeTestRun()
        {
            LogDebug(() => "Start Test Run");

            if (!checkedDriver)
            {
                using (BrowserFactory factory = BrowserFactory.GetBrowserFactory(new NullLogger(), this.scenarioContext, null))
                {
                    factory.ValidateDriverSetup();
                }

                checkedDriver = true;
            }
        }

        /// <summary>
        /// Registers the types.
        /// </summary>
        [BeforeScenario(Order = 1)]
        public void RegisterTypes()
        {
            this.objectContainer.RegisterTypeAs<ProxyLogger, ILogger>();
            this.objectContainer.RegisterTypeAs<ScenarioContextHelper, IScenarioContextHelper>();
            this.objectContainer.RegisterTypeAs<TokenManager, ITokenManager>();
        }

        /// <summary>
        /// Things to do before each scenario.
        /// </summary>
        [BeforeScenario(Order = 100)]
        public void BeforeScenario()
        {
            LogDebug(() => "Scenario: " + this.scenarioContext.ScenarioInfo.Title);
            this.InitializeDriver();
        }

        /// <summary>
        /// Initializes the page mapper at the start of the test run.
        /// </summary>
        public void InitializeDriver()
        {
            var logger = this.objectContainer.Resolve<ILogger>();
            var scenarioContextHelper = this.objectContainer.Resolve<IScenarioContextHelper>();

            browserFactory = BrowserFactory.GetBrowserFactory(logger, this.scenarioContext, scenarioContextHelper.TestResultsDirectory);

            this.objectContainer.RegisterInstanceAs(browserFactory, dispose: true);

            var mapper = new PageMapper();
            this.objectContainer.RegisterInstanceAs<IPageMapper>(mapper);

            this.objectContainer.RegisterFactoryAs((container) =>
            {
                return InitializeBrowser(container);
            });

            this.objectContainer.RegisterInstanceAs<ISettingHelper>(new WrappedSettingHelper());

            actionRepository = new ActionRepository(this.objectContainer);
            this.objectContainer.RegisterInstanceAs<IActionRepository>(actionRepository);

            var pageHistoryService = PageHistoryService.GetPageHistoryService(this.objectContainer);

            var actionPipelineService = new ActionPipelineService(actionRepository, pageHistoryService);
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
            if (browserFactory.Configuration.BrowserType == BrowserType.WinApp)
            {
                return;
            }

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
        /// <param name="expectedJQueryDefined">if set to <c>true</c> jQuery is expected to be defined.</param>
        public void WaitForjQuery(TimeSpan timeout, bool expectedJQueryDefined = false)
        {
            WaitForjQuery(Convert.ToInt32(timeout.TotalSeconds), expectedJQueryDefined);
        }

        /// <summary>
        /// Given I save the HTML file as "fileName".
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        [Given(@"I save the HTML file as ""(.*)""")]
        public void SaveTheHTMLFileAs(string fileName)
        {
            var scenarioHelper = this.objectContainer.Resolve<IScenarioContextHelper>();

            var basePath = scenarioHelper.TestResultsDirectory;

            this.SaveHtmlFile(CurrentBrowser, basePath, fileName);
        }

        /// <summary>
        /// Takes a screenshot.
        /// </summary>
        [AfterScenario(Order = 1)]
        public void TakeScreenshot()
        {
            if (CurrentBrowser == null)
            {
                return;
            }

            try
            {
                this.CheckForScreenshot();
            }
            catch (Exception ex)
            {
                LogDebug(() => ex.ToString());
            }
        }

        /// <summary>
        /// Tears down the browser after each scenario.
        /// </summary>
        [AfterScenario(Order = int.MaxValue)]
        public void TearDown()
        {
            TearDownAfterScenario();
        }

        /// <summary>
        /// Checks for screenshot.
        /// </summary>
        public void CheckForScreenshot()
        {
            if ((CurrentBrowser == null) || (!CurrentBrowser.IsCreated))
            {
                return;
            }

            var ex = this.GetError();
            if ((ex == null) && (!browserFactory.Configuration.CreateScreenshotOnExit))
            {
                return;
            }

            string fileName = this.GetStepFileName();

            var scenarioHelper = this.objectContainer.Resolve<IScenarioContextHelper>();

            var basePath = scenarioHelper.TestResultsDirectory;

            this.SaveHtmlFile(CurrentBrowser, basePath, fileName);

            string fullPath = CurrentBrowser.TakeScreenshot(basePath, fileName);
            if (fullPath != null)
            {
                this.testResultFileNotifier?.AddTestResultFile(fullPath);

                var traceListener = this.objectContainer.Resolve<ITraceListener>();
                if (traceListener != null)
                {
                    traceListener.WriteTestOutput("Created Screenshot: {0}", fullPath);
                }
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

        private static int GetAngularPendingRequestCount()
        {
            try
            {
                return
                    Convert.ToInt32(
                        CurrentBrowser.ExecuteScript(
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

        private static int GetjQueryActive(bool expectedJQueryDefined)
        {
            try
            {
                string script = $@"if (typeof jQuery === 'undefined') {{ return {(expectedJQueryDefined ? 1 : 0)}; }} else {{ return jQuery.active; }}";

                return Convert.ToInt32(CurrentBrowser.ExecuteScript(script));
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

        private void SaveHtmlFile(IBrowser browser, string basePath, string fileName)
        {
            string fullPath = browser.SaveHtml(basePath, fileName);
            if (fullPath != null)
            {
                this.testResultFileNotifier?.AddTestResultFile(fullPath);
            }
        }

        private Exception GetError()
        {
            var scenarioHelper = this.objectContainer.Resolve<IScenarioContextHelper>();
            return scenarioHelper.GetError();
        }

        private string GetStepFileName()
        {
            var scenarioHelper = this.objectContainer.Resolve<IScenarioContextHelper>();
            var ex = scenarioHelper.GetError();
            var isError = false;
            if (ex != null)
            {
                isError = true;
                LogDebug(() => $"{ex.GetType().Name}: {ex.Message}");
            }

            return scenarioHelper.GetStepFileName(isError);
        }
    }
}