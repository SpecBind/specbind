// <copyright file="SeleniumEdgeDriver.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System.IO;
    using System.Net;
    using Configuration;
    using Microsoft.Win32;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Edge;

    /// <summary>
    /// Selenium Edge Driver.
    /// </summary>
    internal class SeleniumEdgeDriver : SeleniumDriverBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumEdgeDriver"/> class.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        public SeleniumEdgeDriver(BrowserFactoryConfigurationElement browserFactoryConfiguration)
            : base(browserFactoryConfiguration)
        {
        }

        /// <summary>
        /// Creates the web driver from the specified browser factory configuration.
        /// </summary>
        /// <returns>The configured web driver.</returns>
        protected override IWebDriver CreateLocalDriver()
        {
            var edgeOptions = new EdgeOptions { PageLoadStrategy = EdgePageLoadStrategy.Normal };
            var edgeDriverService = EdgeDriverService.CreateDefaultService();
            return new EdgeDriver(edgeDriverService, edgeOptions);
        }

        /// <summary>
        /// Downloads and installs the MS Edge driver based on the platform version
        /// </summary>
        /// <param name="seleniumDriverPath">The selenium driver path.</param>
        protected override void Download(string seleniumDriverPath)
        {
            var winVersion = GetWindowsBuildNumber();

            string downloadUrl;
            switch (winVersion)
            {
                case "17134":
                    downloadUrl = "https://download.microsoft.com/download/F/8/A/F8AF50AB-3C3A-4BC4-8773-DC27B32988DD/MicrosoftWebDriver.exe";
                    break;
                case "16299":
                    downloadUrl = "https://download.microsoft.com/download/D/4/1/D417998A-58EE-4EFE-A7CC-39EF9E020768/MicrosoftWebDriver.exe";
                    break;
                case "15063":
                    downloadUrl = "https://download.microsoft.com/download/3/4/2/342316D7-EBE0-4F10-ABA2-AE8E0CDF36DD/MicrosoftWebDriver.exe";
                    break;
                case "Insiders":
                    downloadUrl = "https://download.microsoft.com/download/1/4/1/14156DA0-D40F-460A-B14D-1B264CA081A5/MicrosoftWebDriver.exe";
                    break;
                case "14393":
                    downloadUrl = "https://download.microsoft.com/download/3/2/D/32D3E464-F2EF-490F-841B-05D53C848D15/MicrosoftWebDriver.exe";
                    break;
                case "10586":
                    downloadUrl = "https://download.microsoft.com/download/C/0/7/C07EBF21-5305-4EC8-83B1-A6FCC8F93F45/MicrosoftWebDriver.msi";
                    break;
                case "10240":
                    downloadUrl = "https://download.microsoft.com/download/8/D/0/8D0D08CF-790D-4586-B726-C6469A9ED49C/MicrosoftWebDriver.msi";
                    break;
                default: return;
            }

            using (var webClient = new WebClient())
            {
                // Combine to download
                var exePath = Path.Combine(seleniumDriverPath, "MicrosoftWebDriver.exe");
                webClient.DownloadFile(downloadUrl, exePath);
            }
        }

        /// <summary>
        /// Creates the driver options.
        /// </summary>
        /// <returns>The driver options.</returns>
        protected override DriverOptions CreateRemoteDriverOptions()
        {
            return new EdgeOptions();
        }

        /// <summary>
        /// Gets the Windows current Build version
        /// </summary>
        /// <returns>The build number if located; otherwise <c>null</c>.</returns>
        private static string GetWindowsBuildNumber()
        {
            var registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            return registryKey?.GetValue("CurrentBuildNumber", null).ToString();
        }
    }
}
