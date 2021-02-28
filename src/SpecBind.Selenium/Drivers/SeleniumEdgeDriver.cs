// <copyright file="SeleniumEdgeDriver.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System;
    using System.IO;
    using System.Net;
    using Configuration;
    using Microsoft.Edge.SeleniumTools;
    using Microsoft.Win32;
    using OpenQA.Selenium;
    using SpecBind.Helpers;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Selenium Edge Driver.
    /// </summary>
    internal class SeleniumEdgeDriver : SeleniumDriverBase
    {
        private readonly string logFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumEdgeDriver" /> class.
        /// </summary>
        /// <param name="testResultsDirectory">The test results directory.</param>
        public SeleniumEdgeDriver(string testResultsDirectory)
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
        protected override IWebDriverEx CreateLocalDriver(
            BrowserFactoryConfiguration browserFactoryConfiguration,
            ScenarioContext scenarioContext)
        {
            var edgeOptions = new EdgeOptions
            {
                PageLoadStrategy = (PageLoadStrategy)Enum.Parse(typeof(PageLoadStrategy), browserFactoryConfiguration.PageLoadStrategy, true),
                UnhandledPromptBehavior = UnhandledPromptBehavior.DismissAndNotify,
                UseChromium = true, // TODO: make optional
            };
            var edgeDriverService = EdgeDriverService.CreateChromiumService();

            // explicitly set the host, otherwise the following exception is thrown:
            // OpenQA.Selenium.WebDriverException: Cannot start the driver service on http://localhost:17556/
            //
            // To get more details about the error, running "C:\windows\SysWOW64\MicrosoftWebDriver.exe --verbose" in an elevated command prompt displays the following error:
            // HttpAddUrlToUrlGroup failed with 1232
            //
            // The host can be specified on the command line as follows:
            // MicrosoftWebDriver.exe --host=127.0.0.1
            edgeDriverService.Host = "127.0.0.1"; // or localhost
            edgeDriverService.UseVerboseLogging = true;

            Environment.SetEnvironmentVariable("webdriver.edge.logfile", this.logFilePath);

            Console.WriteLine($"EdgeDriverService LogFile: '{this.logFilePath}'.");

            return new EdgeDriverEx(edgeDriverService, edgeOptions);
        }

        /// <summary>
        /// Downloads and installs the MS Edge driver based on the platform version
        /// </summary>
        /// <param name="seleniumDriverPath">The selenium driver path.</param>
        protected override void Download(string seleniumDriverPath)
        {
            var winVersion = GetWindowsBuildNumber();

            // Download links: https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/
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
                default:
                    return;
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
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The driver options.</returns>
        protected override DriverOptions CreateRemoteDriverOptions(BrowserFactoryConfiguration browserFactoryConfiguration)
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