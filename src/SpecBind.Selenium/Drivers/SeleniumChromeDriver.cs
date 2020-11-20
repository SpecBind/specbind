// <copyright file="SeleniumChromeDriver.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System.Collections.Generic;
    using System.Net;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using SpecBind.Configuration;

    /// <summary>
    /// Selenium Chrome Driver.
    /// </summary>
    internal class SeleniumChromeDriver : SeleniumDriverBase
    {
        private const string ChromeUrl = "http://chromedriver.storage.googleapis.com";
        private const string ChromeArgumentSetting = "ChromeArguments";

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumChromeDriver" /> class.
        /// </summary>
        public SeleniumChromeDriver()
        {
            this.AdditionalArguments = new List<string>();
        }

        /// <summary>
        /// Gets or sets the additional arguments.
        /// </summary>
        /// <value>The additional arguments.</value>
        protected List<string> AdditionalArguments { get; set; }

        /// <summary>
        /// Creates the web driver from the specified browser factory configuration.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The configured web driver.</returns>
        protected override IWebDriver CreateLocalDriver(BrowserFactoryConfiguration browserFactoryConfiguration)
        {
            var chromeOptions = new ChromeOptions { LeaveBrowserRunning = false };

            if (browserFactoryConfiguration.Settings.ContainsKey(ChromeArgumentSetting))
            {
                var cmdLineSetting = browserFactoryConfiguration.Settings[ChromeArgumentSetting];
                if (!string.IsNullOrWhiteSpace(cmdLineSetting))
                {
                    foreach (var arg in cmdLineSetting.Split(';'))
                    {
                        chromeOptions.AddArgument(arg);
                    }
                }
            }

            foreach (string additionArgument in this.AdditionalArguments)
            {
                chromeOptions.AddArgument(additionArgument);
            }

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            foreach (var preference in browserFactoryConfiguration.UserProfilePreferences)
            {
                chromeOptions.AddUserProfilePreference(preference.Key, preference.Value);
            }

            return new ChromeDriver(chromeDriverService, chromeOptions);
        }

        /// <summary>
        /// Downloads the chrome driver.
        /// </summary>
        /// <param name="seleniumDriverPath">The selenium driver path.</param>
        protected override void Download(string seleniumDriverPath)
        {
            string url;
            using (var webClient = new WebClient())
            {
                // First get the latest version
                var releaseNumber = webClient.DownloadString($"{ChromeUrl}/LATEST_RELEASE");

                // Combine to download
                url = $"{ChromeUrl}/{releaseNumber.Trim()}";
            }

            DownloadAndExtractZip(url, seleniumDriverPath, "chromedriver_win32.zip");
        }

        /// <summary>
        /// Creates the driver options.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The driver options.</returns>
        protected override DriverOptions CreateRemoteDriverOptions(BrowserFactoryConfiguration browserFactoryConfiguration)
        {
            ChromeOptions chromeOptions = new ChromeOptions();

            foreach (var preference in browserFactoryConfiguration.UserProfilePreferences)
            {
                chromeOptions.AddUserProfilePreference(preference.Key, preference.Value);
            }

            return chromeOptions;
        }
    }
}