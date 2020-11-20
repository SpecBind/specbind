// <copyright file="SeleniumInternetExplorerDriver.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System.Configuration;
    using OpenQA.Selenium;
    using OpenQA.Selenium.IE;
    using SpecBind.Configuration;

    /// <summary>
    /// Selenium Internet Explorer Driver.
    /// </summary>
    internal class SeleniumInternetExplorerDriver : SeleniumDriverBase
    {
        private const string IgnoreProtectedModeSettings = "IgnoreProtectedModeSettings";

        /// <summary>
        /// Creates the web driver from the specified browser factory configuration.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The configured web driver.</returns>
        protected override IWebDriver CreateLocalDriver(BrowserFactoryConfiguration browserFactoryConfiguration)
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
                IntroduceInstabilityByIgnoringProtectedModeSettings = ignoreProtectedModeSettings
            };

            var internetExplorerDriverService = InternetExplorerDriverService.CreateDefaultService();
            internetExplorerDriverService.HideCommandPromptWindow = true;
            return new InternetExplorerDriver(internetExplorerDriverService, explorerOptions);
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