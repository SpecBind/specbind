// <copyright file="SeleniumBrowserFactory.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.IO;
    using System.Linq;
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
            : base(LoadConfiguration())
        {
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
            var launchAction = new Func<IWebDriver>(() =>
            {
                return this.CreateWebDriver(this.SeleniumDriver.Value, this.Configuration);
            });

            var lazyBrowser = new Lazy<IWebDriver>(launchAction, LazyThreadSafetyMode.None);

            (browser as SeleniumBrowser).UpdateDriver(lazyBrowser);
        }

        /// <summary>
        /// Creates the web driver.
        /// </summary>
        /// <param name="seleniumDriver">The selenium driver.</param>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The created web driver.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the browser is not supported.</exception>
        internal IWebDriver CreateWebDriver(ISeleniumDriver seleniumDriver, BrowserFactoryConfiguration browserFactoryConfiguration)
        {
            IWebDriver driver = seleniumDriver.Create(browserFactoryConfiguration);

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
        /// <returns>The selenium driver.</returns>
        internal ISeleniumDriver CreateSeleniumDriver()
        {
            switch (this.Configuration.BrowserType)
            {
                case BrowserType.IE:
                    return new SeleniumInternetExplorerDriver();
                case BrowserType.FireFox:
                    return new SeleniumFirefoxDriver();
                case BrowserType.Chrome:
                    return new SeleniumChromeDriver();
                case BrowserType.ChromeHeadless:
                    return new SeleniumChromeDriver();
                case BrowserType.Safari:
                    return new SeleniumSafariDriver();
                case BrowserType.Edge:
                    return new SeleniumEdgeDriver();
                default:
                    throw new InvalidOperationException(
                        $"Browser type '{this.Configuration.BrowserType}' is not supported in Selenium local mode. Did you mean to configure a remote driver?");
            }
        }

        /// <summary>
        /// Validates the driver setup.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected override void ValidateDriverSetup(ILogger logger)
        {
            ISeleniumDriver seleniumDriver = this.CreateSeleniumDriver();

            seleniumDriver.Validate(this.Configuration, SeleniumDriverPath);
        }

        /// <summary>
        /// Creates the browser.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>A browser object.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the browser type is not supported.</exception>
        protected override IBrowser CreateBrowser(ILogger logger)
        {
            this.SeleniumDriver = new Lazy<ISeleniumDriver>(() => this.CreateSeleniumDriver());
            var launchAction = new Func<IWebDriver>(() =>
            {
                return this.CreateWebDriver(this.SeleniumDriver.Value, this.Configuration);
            });

            var browser = new Lazy<IWebDriver>(launchAction, LazyThreadSafetyMode.None);

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