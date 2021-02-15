// <copyright file="SeleniumDriverBase.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using Configuration;
    using Helpers;
    using OpenQA.Selenium;
    using TechTalk.SpecFlow;

    /// <summary>
    /// The base class for Selenium Drivers.
    /// </summary>
    public abstract class SeleniumDriverBase : ISeleniumDriver
    {
        private const string RemoteUrlSetting = "RemoteUrl";

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumDriverBase" /> class.
        /// </summary>
        public SeleniumDriverBase()
        {
            this.SupportsPageLoadTimeout = true;
            this.MaximizeWindow = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the driver supports page load timeouts.
        /// </summary>
        /// <value><c>true</c> if the driver supports page load timeouts; otherwise, <c>false</c>.</value>
        public bool SupportsPageLoadTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to maximize the window.
        /// </summary>
        /// <value><c>true</c> if the window is maximized; otherwise, <c>false</c>.</value>
        public bool MaximizeWindow { get; set; }

        /// <summary>
        /// Creates the remote driver.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <returns>
        /// The created remote web driver.
        /// </returns>
        public IWebDriverEx Create(
            BrowserFactoryConfiguration browserFactoryConfiguration,
            ScenarioContext scenarioContext = null)
        {
            var remoteUri = this.GetRemoteDriverUri(browserFactoryConfiguration);
            if (remoteUri == null)
            {
                return this.CreateLocalDriver(browserFactoryConfiguration, scenarioContext);
            }

            DriverOptions driverOptions = this.CreateRemoteDriverOptions(browserFactoryConfiguration);

            // Add any additional settings that are not reserved
            var envRegex = new System.Text.RegularExpressions.Regex("\\$\\{(.+)\\}");
            var reservedSettings = new[] { RemoteUrlSetting };
            foreach (var setting in browserFactoryConfiguration.Settings
                .OfType<NameValueConfigurationElement>()
                .Where(s => reservedSettings
                    .All(r => !string.Equals(r, s.Name, StringComparison.OrdinalIgnoreCase))))
            {
                // Support environment variables
                var value = setting.Value;
                var match = envRegex.Match(value);
                if (match.Success)
                {
                    value = SettingHelper.GetEnvironmentVariable(match.Groups[1].Value);
                }

                driverOptions.AddAdditionalCapability(setting.Name, value);
            }

            return new RemoteScreenshotWebDriver(remoteUri, driverOptions.ToCapabilities());
        }

        /// <summary>
        /// Validates the specified browser factory configuration.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="seleniumDriverPath">The selenium driver path.</param>
        public void Validate(
            BrowserFactoryConfiguration browserFactoryConfiguration,
            ScenarioContext scenarioContext,
            string seleniumDriverPath)
        {
            // If we're using a remote driver, don't check paths
            if (this.GetRemoteDriverUri(browserFactoryConfiguration) != null)
            {
                return;
            }

            try
            {
                IWebDriver driver = this.CreateLocalDriver(browserFactoryConfiguration, scenarioContext);
                driver.Quit();
            }
            catch (DriverServiceNotFoundException ex)
            {
                if (seleniumDriverPath == null)
                {
                    // Error if we weren't able to construct a path earlier.
                    throw;
                }

                try
                {
                    this.Download(seleniumDriverPath);
                }
                catch (Exception)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        public virtual void Stop(ScenarioContext scenarioContext)
        {
        }

        /// <summary>
        /// Downloads the specified file from the URL and extracts it to the path.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="path">The path.</param>
        /// <param name="zipName">Name of the zip.</param>
        protected static void DownloadAndExtractZip(string baseUri, string path, string zipName)
        {
            using (var webClient = new WebClient())
            {
                // Combine to download
                var url = $"{baseUri}/{zipName}";
                var zipPath = Path.Combine(path, zipName);
                webClient.DownloadFile(url, zipPath);

                // Unzip the file to the parent directory
                ZipFile.ExtractToDirectory(zipPath, path);

                // Delete the zip file
                File.Delete(zipPath);
            }
        }

        /// <summary>
        /// Creates the local web driver from the specified browser factory configuration.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <returns>
        /// The configured web driver.
        /// </returns>
        protected abstract IWebDriverEx CreateLocalDriver(
            BrowserFactoryConfiguration browserFactoryConfiguration,
            ScenarioContext scenarioContext);

        /// <summary>
        /// Downloads the driver to the specified path.
        /// </summary>
        /// <param name="driverPath">The driver path.</param>
        protected abstract void Download(string driverPath);

        /// <summary>
        /// Creates the driver options.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The driver options.</returns>
        protected abstract DriverOptions CreateRemoteDriverOptions(BrowserFactoryConfiguration browserFactoryConfiguration);

        /// <summary>
        /// Gets the remote driver URI.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The URI if the setting is valid, otherwise <c>null</c>.</returns>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Thrown if the URI is not valid.</exception>
        private Uri GetRemoteDriverUri(BrowserFactoryConfiguration browserFactoryConfiguration)
        {
            if (!browserFactoryConfiguration.Settings.ContainsKey(RemoteUrlSetting))
            {
                return null;
            }

            var remoteSetting = browserFactoryConfiguration.Settings[RemoteUrlSetting];
            if (string.IsNullOrWhiteSpace(remoteSetting))
            {
                return null;
            }

            Uri remoteUri;
            if (!Uri.TryCreate(remoteSetting, UriKind.Absolute, out remoteUri))
            {
                throw new ConfigurationErrorsException(
                    $"The {RemoteUrlSetting} setting is not a valid URI: {remoteSetting}");
            }

            return remoteUri;
        }
    }
}