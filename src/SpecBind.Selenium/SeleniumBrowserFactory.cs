// <copyright file="SeleniumBrowserFactory.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.IO;
    using System.Threading;
    using Drivers;
    using OpenQA.Selenium;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Configuration;
    using SpecBind.Helpers;

    /// <summary>
    /// A browser factory class for Selenium tests.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SeleniumBrowserFactory : BrowserFactory
    {
        private static readonly string SeleniumDriverPath;

        /// <summary>
        /// Initializes static members of the <see cref="SeleniumBrowserFactory"/> class.
        /// </summary>
        static SeleniumBrowserFactory()
        {
            SeleniumDriverPath = SetupDriverFolder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumBrowserFactory"/> class.
        /// </summary>
        public SeleniumBrowserFactory()
            : base(ValidateWebDriver())
        {
        }

        /// <summary>
        /// Gets the selenium driver.
        /// </summary>
        /// <value>The selenium driver.</value>
        public ISeleniumDriver SeleniumDriver { get; private set; }

        /// <summary>
        /// Creates the web driver.
        /// </summary>
        /// <param name="seleniumDriver">The selenium driver.</param>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The created web driver.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the browser is not supported.</exception>
        internal IWebDriver CreateWebDriver(ISeleniumDriver seleniumDriver, BrowserFactoryConfigurationElement browserFactoryConfiguration)
        {
            IWebDriver driver = seleniumDriver.Create();

            // Set Driver Settings
            var managementSettings = driver.Manage();

            // Set timeouts
            var applicationConfiguration = SettingHelper.GetConfigurationSection().Application;

            var timeouts = managementSettings.Timeouts();
            timeouts.ImplicitWait = browserFactoryConfiguration.ElementLocateTimeout;
            timeouts.PageLoad = browserFactoryConfiguration.PageLoadTimeout;

            ActionBase.DefaultTimeout = browserFactoryConfiguration.ElementLocateTimeout;
            WaitForPageAction.DefaultTimeout = browserFactoryConfiguration.PageLoadTimeout;
            ActionBase.RetryValidationUntilTimeout = applicationConfiguration.RetryValidationUntilTimeout;

            // Maximize window
            managementSettings.Window.Maximize();

            return driver;
        }

        /// <summary>
        /// Gets the selenium driver.
        /// </summary>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The selenium driver.</returns>
        internal ISeleniumDriver GetSeleniumDriver(BrowserType browserType, BrowserFactoryConfigurationElement browserFactoryConfiguration)
        {
            if (this.SeleniumDriver != null)
            {
                return this.SeleniumDriver;
            }

            switch (browserType)
            {
                case BrowserType.IE:
                    this.SeleniumDriver = new SeleniumInternetExplorerDriver(browserFactoryConfiguration);
                    break;
                case BrowserType.FireFox:
                    this.SeleniumDriver = new SeleniumFirefoxDriver(browserFactoryConfiguration);
                    break;
                case BrowserType.Chrome:
                    this.SeleniumDriver = new SeleniumChromeDriver(browserFactoryConfiguration);
                    break;
                case BrowserType.ChromeHeadless:
                    this.SeleniumDriver = new SeleniumChromeDriver(browserFactoryConfiguration);
                    break;
                case BrowserType.PhantomJS:
                    this.SeleniumDriver = new SeleniumPhantomJSDriver(browserFactoryConfiguration);
                    break;
                case BrowserType.Safari:
                    this.SeleniumDriver = new SeleniumSafariDriver(browserFactoryConfiguration);
                    break;
                case BrowserType.Opera:
                    this.SeleniumDriver = new SeleniumOperaDriver(browserFactoryConfiguration);
                    break;
                case BrowserType.Edge:
                    this.SeleniumDriver = new SeleniumEdgeDriver(browserFactoryConfiguration);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Browser type '{browserType}' is not supported in Selenium local mode. Did you mean to configure a remote driver?");
            }

            return this.SeleniumDriver;
        }

        /// <summary>
        /// Creates the browser.
        /// </summary>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A browser object.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the browser type is not supported.</exception>
        protected override IBrowser CreateBrowser(BrowserType browserType, BrowserFactoryConfigurationElement browserFactoryConfiguration, ILogger logger)
        {
            this.SeleniumDriver = this.GetSeleniumDriver(browserType, browserFactoryConfiguration);

            var launchAction = new Func<IWebDriver>(() => this.CreateWebDriver(this.SeleniumDriver, browserFactoryConfiguration));

            var browser = new Lazy<IWebDriver>(launchAction, LazyThreadSafetyMode.None);

            return new SeleniumBrowser(browser, logger);
        }

        /// <summary>
        /// Validates the driver setup.
        /// </summary>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        protected override void ValidateDriverSetup(BrowserType browserType, BrowserFactoryConfigurationElement browserFactoryConfiguration)
        {
            ISeleniumDriver seleniumDriver = this.GetSeleniumDriver(browserType, browserFactoryConfiguration);

            seleniumDriver.Validate(SeleniumDriverPath);
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

        /// <summary>
        /// Determines whether or not to perform web driver validation
        /// </summary>
        /// <returns><c>true</c> if the web driver should be validated; otherwise <c>false</c></returns>
        private static bool ValidateWebDriver()
        {
            var configSection = SettingHelper.GetConfigurationSection();
            return configSection.BrowserFactory.ValidateWebDriver;
        }
    }
}