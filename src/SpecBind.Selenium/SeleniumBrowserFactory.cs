// <copyright file="SeleniumBrowserFactory.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.IO;
    using System.Threading;
    using Drivers;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Configuration;
    using SpecBind.Helpers;
    using TechTalk.SpecFlow;

    /// <summary>
    /// A browser factory class for Selenium tests.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SeleniumBrowserFactory : BrowserFactory
    {
        private static readonly string SeleniumDriverPath;
        private readonly ScenarioContext scenarioContext;

        /// <summary>
        /// Initializes static members of the <see cref="SeleniumBrowserFactory"/> class.
        /// </summary>
        static SeleniumBrowserFactory()
        {
            SeleniumDriverPath = SetupDriverFolder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumBrowserFactory" /> class.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        public SeleniumBrowserFactory(ScenarioContext scenarioContext)
            : base(LoadConfiguration())
        {
            this.scenarioContext = scenarioContext;
        }

        /// <summary>
        /// Gets the selenium driver.
        /// </summary>
        /// <value>The selenium driver.</value>
        public Lazy<ISeleniumDriver> SeleniumDriver { get; private set; }

        /// <summary>
        /// Resets the driver.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public override void ResetDriver(IBrowser browser)
        {
            var launchAction = new Func<IWebDriverEx>(() =>
            {
                return this.CreateWebDriver(this.SeleniumDriver.Value, this.Configuration, this.scenarioContext);
            });

            var lazyBrowser = new Lazy<IWebDriverEx>(launchAction, LazyThreadSafetyMode.None);

            (browser as SeleniumBase).UpdateDriver(lazyBrowser);
        }

        /// <summary>
        /// Creates the web driver.
        /// </summary>
        /// <param name="seleniumDriver">The selenium driver.</param>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <returns>
        /// The created web driver.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the browser is not supported.</exception>
        internal IWebDriverEx CreateWebDriver(
            ISeleniumDriver seleniumDriver,
            BrowserFactoryConfiguration browserFactoryConfiguration,
            ScenarioContext scenarioContext)
        {
            IWebDriverEx driver = seleniumDriver.Create(browserFactoryConfiguration, scenarioContext);

            // Set Driver Settings
            var managementSettings = driver.Manage();

            // Set timeouts
            var applicationConfiguration = SettingHelper.GetConfigurationSection().Application;

            var timeouts = managementSettings.Timeouts();
            timeouts.ImplicitWait = browserFactoryConfiguration.ElementLocateTimeout;

            if (seleniumDriver.SupportsPageLoadTimeout)
            {
                timeouts.PageLoad = browserFactoryConfiguration.PageLoadTimeout;
            }

            ActionBase.DefaultTimeout = browserFactoryConfiguration.ElementLocateTimeout;
            WaitForPageAction.DefaultTimeout = browserFactoryConfiguration.PageLoadTimeout;
            ActionBase.RetryValidationUntilTimeout = applicationConfiguration.RetryValidationUntilTimeout;

            if (seleniumDriver.MaximizeWindow)
            {
                // Maximize window
                managementSettings.Window.Maximize();
            }

            return driver;
        }

        /// <summary>
        /// Creates the selenium driver.
        /// </summary>
        /// <param name="testResultsDirectory">The test results directory.</param>
        /// <returns>
        /// The selenium driver.
        /// </returns>
        internal ISeleniumDriver CreateSeleniumDriver(string testResultsDirectory)
        {
            switch (this.Configuration.BrowserType)
            {
                case BrowserType.IE:
                    return new SeleniumInternetExplorerDriver(testResultsDirectory);
                case BrowserType.FireFox:
                    return new SeleniumFirefoxDriver();
                case BrowserType.Chrome:
                    return new SeleniumChromeDriver();
                case BrowserType.ChromeHeadless:
                    return new SeleniumChromeDriver();
                case BrowserType.Safari:
                    return new SeleniumSafariDriver();
                case BrowserType.Edge:
                    return new SeleniumEdgeDriver(testResultsDirectory);
                case BrowserType.WinApp:
                    return new SeleniumWindowsDriver();
                default:
                    throw new InvalidOperationException(
                        $"Browser type '{this.Configuration.BrowserType}' is not supported in Selenium local mode. Did you mean to configure a remote driver?");
            }
        }

        /// <summary>
        /// Validates the driver setup.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="testResultsDirectory">The test results directory.</param>
        protected override void ValidateDriverSetup(ILogger logger, string testResultsDirectory)
        {
            ISeleniumDriver seleniumDriver = this.CreateSeleniumDriver(testResultsDirectory);

            seleniumDriver.Validate(this.Configuration, this.scenarioContext, SeleniumDriverPath);
        }

        /// <summary>
        /// Creates the browser.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="testResultsDirectory">The test results directory.</param>
        /// <returns>
        /// A browser object.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the browser type is not supported.</exception>
        protected override IBrowser CreateBrowser(ILogger logger, string testResultsDirectory)
        {
            this.SeleniumDriver = new Lazy<ISeleniumDriver>(() => this.CreateSeleniumDriver(testResultsDirectory));
            var launchAction = new Func<IWebDriverEx>(() =>
            {
                return this.CreateWebDriver(this.SeleniumDriver.Value, this.Configuration, this.scenarioContext);
            });

            var browser = new Lazy<IWebDriverEx>(launchAction, LazyThreadSafetyMode.None);

            if (this.Configuration.BrowserType == BrowserType.WinApp)
            {
                return new SeleniumApplication(browser, this.scenarioContext, logger, this.SeleniumDriver);
            }

            return new SeleniumBrowser(browser, logger);
        }

        /// <summary>
        /// Sets up the driver folder.
        /// </summary>
        /// <returns>The driver folder path.</returns>
        private static string SetupDriverFolder()
        {
            var path = Path.Combine(Path.GetTempPath(), "SeleniumDrivers");

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Append our directory to the system path
                var systemPath = Environment.GetEnvironmentVariable("PATH");
                systemPath = $"{path};{systemPath}";
                Environment.SetEnvironmentVariable("PATH", systemPath);
            }
            catch (SystemException)
            {
                return null;
            }

            return path;
        }
    }
}