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
        /// Initializes a new instance of the <see cref="SeleniumInternetExplorerDriver"/> class.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        public SeleniumInternetExplorerDriver(BrowserFactoryConfigurationElement browserFactoryConfiguration)
            : base(browserFactoryConfiguration)
        {
        }

        /// <summary>
        /// Creates the web driver from the specified browser factory configuration.
        /// </summary>
        /// <returns>The configured web driver.</returns>
        protected override IWebDriver CreateLocalDriver()
        {
            bool ignoreProtectedModeSettings = false;
            if (this.Settings.ContainsKey(IgnoreProtectedModeSettings))
            {
                string ignoreProtectedModeSettingsValue = this.Settings[IgnoreProtectedModeSettings];
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
                EnsureCleanSession = this.EnsureCleanSession,
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
        /// <returns>The driver options.</returns>
        protected override DriverOptions CreateRemoteDriverOptions()
        {
            return new InternetExplorerOptions();
        }
    }
}
