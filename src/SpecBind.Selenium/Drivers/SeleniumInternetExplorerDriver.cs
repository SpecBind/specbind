// <copyright file="SeleniumInternetExplorerDriver.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System;
    using System.Configuration;
    using System.IO;
    using OpenQA.Selenium;
    using OpenQA.Selenium.IE;
    using SpecBind.Configuration;
    using SpecBind.Helpers;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Selenium Internet Explorer Driver.
    /// </summary>
    internal class SeleniumInternetExplorerDriver : SeleniumDriverBase
    {
        private const string IgnoreProtectedModeSettings = "IgnoreProtectedModeSettings";
        private readonly string logFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumInternetExplorerDriver" /> class.
        /// </summary>
        /// <param name="testResultsDirectory">The test results directory.</param>
        public SeleniumInternetExplorerDriver(string testResultsDirectory)
        {
            if (testResultsDirectory != null)
            {
                this.logFilePath = Path.Combine(testResultsDirectory, "selenium.log");
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        public override void Stop(ScenarioContext scenarioContext)
        {
            base.Stop(scenarioContext);

            TestResultFileNotifier testResultFileNotifier = scenarioContext.ScenarioContainer.Resolve<TestResultFileNotifier>();

            if (!string.IsNullOrEmpty(this.logFilePath))
            {
                testResultFileNotifier.AddTestResultFile(this.logFilePath);
            }
        }

        /// <summary>
        /// Creates the web driver from the specified browser factory configuration.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <returns>
        /// The configured web driver.
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">The {IgnoreProtectedModeSettings} setting is not a valid boolean: {ignoreProtectedModeSettingsValue}</exception>
        protected override IWebDriverEx CreateLocalDriver(
            BrowserFactoryConfiguration browserFactoryConfiguration,
            ScenarioContext scenarioContext)
        {
            bool ignoreProtectedModeSettings = false;
            if (browserFactoryConfiguration.Settings.ContainsKey(IgnoreProtectedModeSettings))
            {
                string ignoreProtectedModeSettingsValue = browserFactoryConfiguration.Settings[IgnoreProtectedModeSettings];
                if (!string.IsNullOrWhiteSpace(ignoreProtectedModeSettingsValue))
                {
                    if (!bool.TryParse(ignoreProtectedModeSettingsValue, out ignoreProtectedModeSettings))
                    {
                        throw new ConfigurationErrorsException(
                            $"The {IgnoreProtectedModeSettings} setting is not a valid boolean: {ignoreProtectedModeSettingsValue}");
                    }
                }
            }

            var explorerOptions = new InternetExplorerOptions
            {
                EnsureCleanSession = browserFactoryConfiguration.EnsureCleanSession,
                IntroduceInstabilityByIgnoringProtectedModeSettings = ignoreProtectedModeSettings,
                PageLoadStrategy = (PageLoadStrategy)Enum.Parse(typeof(PageLoadStrategy), browserFactoryConfiguration.PageLoadStrategy, true)
            };

            Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");

            var internetExplorerDriverService = InternetExplorerDriverService.CreateDefaultService();
            internetExplorerDriverService.HideCommandPromptWindow = true;
            internetExplorerDriverService.LoggingLevel = InternetExplorerDriverLogLevel.Debug;

            // There's a bug in Selenium where the log file path is not passed to the IE driver in quotes if it contains spaces
            // So wrap the log file path with quotes to work-around the bug
            // See https://github.com/SeleniumHQ/selenium/issues/6912
            internetExplorerDriverService.LogFile = "\"" + this.logFilePath + "\"";

            Console.WriteLine($"IEDriverServer LogFile: '{this.logFilePath}'.");

            return new InternetExplorerDriverEx(this, internetExplorerDriverService, explorerOptions, scenarioContext);
        }

        /// <summary>
        /// Downloads the IE driver.
        /// </summary>
        /// <param name="seleniumDriverPath">The selenium driver path.</param>
        protected override void Download(string seleniumDriverPath)
        {
            // Determine bit-wise of OS
            // HACK: Only use 32-bit driver; SendKeys is unusably slow with 64-bit driver

            // Download - this is set to a single version for now
            DownloadAndExtractZip(
                "http://selenium-release.storage.googleapis.com/2.50",
                seleniumDriverPath,
                "IEDriverServer_Win32_2.50.0.zip");
        }

        /// <summary>
        /// Creates the driver options.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The driver options.</returns>
        protected override DriverOptions CreateRemoteDriverOptions(BrowserFactoryConfiguration browserFactoryConfiguration)
        {
            return new InternetExplorerOptions();
        }
    }
}