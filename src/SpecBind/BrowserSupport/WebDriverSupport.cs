﻿// <copyright file="WebDriverSupport.cs">
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
        private static Lazy<BrowserFactoryConfigurationElement> configurationHandler;
        private static Lazy<ApplicationConfigurationElement> applicationConfigurationHandler;

        private IObjectContainer objectContainer;

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
        /// Sets the browser factory configuration element.
        /// </summary>
        /// <value>
        /// The browser factory configuration element.
        /// </value>
        internal static Lazy<BrowserFactoryConfigurationElement> ConfigurationMethod
        {
            set
            {
                configurationHandler = value;
            }
        }

        /// <summary>
        /// Sets the application configuration element.
        /// </summary>
        /// <value>The application configuration element.</value>
        internal static Lazy<ApplicationConfigurationElement> ApplicationConfigurationMethod
        {
            set
            {
                applicationConfigurationHandler = value;
            }
        }

        /// <summary>
        /// Things to do before each test run.
        /// </summary>
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            LogDebug(() => "Start Test Run");
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
        /// Things to do before each feature.
        /// </summary>
        public static void BeforeFeature()
        {
            LogDebug(() => "Feature: " + FeatureContext.Current.FeatureInfo.Title);
        }

        /// <summary>
        /// Initializes the browser and page mapper at the start of the test run.
        /// </summary>
        /// <param name="objectContainer">The object container.</param>
        public static void InitializeBrowser(IObjectContainer objectContainer)
        {
            var logger = objectContainer.Resolve<ILogger>();

            var factory = BrowserFactory.GetBrowserFactory(configurationHandler.Value.Provider, logger);
            var configSection = configurationHandler.Value;
            factory.ValidateDriverSetup(configSection, applicationConfigurationHandler?.Value);

            bool reusingBrowser = true;
            if (!configSection.ReuseBrowser || Browser == null || Browser.IsDisposed)
            {
                Browser = factory.GetBrowser(configSection, applicationConfigurationHandler?.Value);
                reusingBrowser = false;
            }
            else
            {
                Browser.UriHelper = new Lazy<IUriHelper>(() => new UriHelper(applicationConfigurationHandler?.Value.StartUrl));
            }

            if (configSection.EnsureCleanSession)
            {
                Browser.ClearCookies();

                if (reusingBrowser)
                {
                    Browser.ClearUrl();
                }
            }

            objectContainer.RegisterInstanceAs(Browser);

            var mapper = new PageMapper();
            mapper.Initialize(Browser.BasePageType);
            objectContainer.RegisterInstanceAs<IPageMapper>(mapper);

            if (ScenarioContext.Current != null)
            {
                ScenarioContext.Current.Set(Browser, "CurrentBrowser");
            }
        }

        /// <summary>
        /// Initializes the default browser.
        /// </summary>
        public void InitializeDefaultBrowser()
        {
            configurationHandler = new Lazy<BrowserFactoryConfigurationElement>(() =>
            {
                var configSection = SettingHelper.GetConfigurationSection();
                if (configSection == null || configSection.BrowserFactory == null || string.IsNullOrWhiteSpace(configSection.BrowserFactory.Provider))
                {
                    throw new InvalidOperationException("The specBind config section must have a browser factory with a provider configured.");
                }

                return configSection.BrowserFactory;
            });
        }

        /// <summary>
        /// Things to do before each scenario.
        /// </summary>
        [BeforeScenario(Order = 100)]
        public void BeforeScenario()
        {
            LogDebug(() => "Scenario: " + ScenarioContext.Current.ScenarioInfo.Title);
            this.InitializeDriver();
            this.InitializeDefaultBrowser();
            this.InitializeDefaultApplication();
        }

        /// <summary>
        /// Initializes the driver.
        /// </summary>
        public void InitializeDriver()
        {
            this.objectContainer.RegisterTypeAs<ProxyLogger, ILogger>();

            this.objectContainer.RegisterInstanceAs<ISettingHelper>(new WrappedSettingHelper());

            this.objectContainer.RegisterTypeAs<ScenarioContextHelper, IScenarioContextHelper>();

            this.objectContainer.RegisterTypeAs<TokenManager, ITokenManager>();

            var repository = new ActionRepository(this.objectContainer);
            this.objectContainer.RegisterInstanceAs<IActionRepository>(repository);

            this.objectContainer.RegisterTypeAs<ActionPipelineService, IActionPipelineService>();
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
            var configSection = configurationHandler.Value;

            switch (configSection.WaitForPendingAjaxCallsVia.ToLowerInvariant())
            {
                case "angular":
                    this.WaitForAngular(configSection.PageLoadTimeout);
                    break;

                case "jquery":
                    this.WaitForjQuery(configSection.PageLoadTimeout);
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
                if (configSection.ReuseBrowser)
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
                var setting = configurationHandler.Value?.CreateScreenshotOnExit;
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

        private void InitializeDefaultApplication()
        {
            applicationConfigurationHandler = new Lazy<ApplicationConfigurationElement>(() =>
            {
                var configSection = SettingHelper.GetConfigurationSection();
                if (configSection == null || configSection.Application == null)
                {
                    throw new InvalidOperationException("The specBind config section must have an application configured.");
                }

                return configSection.Application;
            });
        }
    }
}
