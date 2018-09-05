// <copyright file="SeleniumDriverBase.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using Configuration;
    using Extensions;
    using Helpers;
    using OpenQA.Selenium;

    /// <summary>
    /// The base class for Selenium Drivers.
    /// </summary>
    internal abstract class SeleniumDriverBase : ISeleniumDriver
    {
        private const string RemoteUrlSetting = "RemoteUrl";

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumDriverBase" /> class.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        public SeleniumDriverBase(BrowserFactoryConfigurationElement browserFactoryConfiguration)
        {
            this.EnsureCleanSession = browserFactoryConfiguration.EnsureCleanSession;
            this.Settings = browserFactoryConfiguration.Settings
                .ToKeyValuePairs()
                .ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the session cache and cookies should be cleared before starting.
        /// </summary>
        /// <value><c>true</c> if the session should be cleared; otherwise <c>false</c>.</value>
        public bool EnsureCleanSession { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Creates the remote driver.
        /// </summary>
        /// <returns>The created remote web driver.</returns>
        public IWebDriver Create()
        {
            var remoteUri = this.GetRemoteDriverUri();
            if (remoteUri == null)
            {
                return this.CreateLocalDriver();
            }

            DriverOptions driverOptions = this.CreateRemoteDriverOptions();

            // Add any additional settings that are not reserved
            var envRegex = new System.Text.RegularExpressions.Regex("\\$\\{(.+)\\}");
            var reservedSettings = new[] { RemoteUrlSetting };
            foreach (var setting in this.Settings
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
        /// <param name="seleniumDriverPath">The selenium driver path.</param>
        public void Validate(string seleniumDriverPath)
        {
            // If we're using a remote driver, don't check paths
            if (this.GetRemoteDriverUri() != null)
            {
                return;
            }

            IWebDriver driver = null;
            try
            {
                driver = this.CreateLocalDriver();
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
        public virtual void Stop()
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
        /// <returns>The configured web driver.</returns>
        protected abstract IWebDriver CreateLocalDriver();

        /// <summary>
        /// Downloads the driver to the specified path.
        /// </summary>
        /// <param name="driverPath">The driver path.</param>
        protected abstract void Download(string driverPath);

        /// <summary>
        /// Creates the driver options.
        /// </summary>
        /// <returns>The driver options.</returns>
        protected abstract DriverOptions CreateRemoteDriverOptions();

        /// <summary>
        /// Gets the remote driver URI.
        /// </summary>
        /// <returns>The URI if the setting is valid, otherwise <c>null</c>.</returns>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Thrown if the URI is not valid.</exception>
        private Uri GetRemoteDriverUri()
        {
            if (!this.Settings.ContainsKey(RemoteUrlSetting))
            {
                return null;
            }

            var remoteSetting = this.Settings[RemoteUrlSetting];
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
