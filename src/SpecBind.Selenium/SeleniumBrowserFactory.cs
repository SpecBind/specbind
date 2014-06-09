// <copyright file="SeleniumBrowserFactory.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Threading;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;
    using OpenQA.Selenium.PhantomJS;
    using OpenQA.Selenium.Remote;
    using OpenQA.Selenium.Safari;

    using SpecBind.BrowserSupport;
    using SpecBind.Configuration;

    /// <summary>
    /// A browser factory class for Selenium tests.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SeleniumBrowserFactory : BrowserFactory
    {
        // Constants to assist with settings
        private const string ChromeUrl = "http://chromedriver.storage.googleapis.com";
        private const string RemoteUrlSetting = "RemoteUrl";
        private const string PhantomjsExe = "phantomjs.exe";
        
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
            : base(true)
        {
        }

        /// <summary>
        /// Creates the web driver.
        /// </summary>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The created web driver.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the browser is not supported.</exception>
        internal static IWebDriver CreateWebDriver(BrowserType browserType, BrowserFactoryConfigurationElement browserFactoryConfiguration)
        {
            IWebDriver driver;
            if (!RemoteDriverExists(browserFactoryConfiguration.Settings, browserType, out driver))
            {
                switch (browserType)
                {
                    case BrowserType.IE:
                        var explorerOptions = new InternetExplorerOptions { EnsureCleanSession = browserFactoryConfiguration.EnsureCleanSession };
                        driver = new InternetExplorerDriver(explorerOptions);
                        break;
                    case BrowserType.FireFox:
                        driver = new FirefoxDriver();
                        break;
                    case BrowserType.Chrome:
                        var chromeOptions = new ChromeOptions { LeaveBrowserRunning = false };
                        if (browserFactoryConfiguration.EnsureCleanSession)
                        {
                            chromeOptions.AddArgument("--incognito");
                        }

                        driver = new ChromeDriver();
                        break;
                    case BrowserType.PhantomJS:
                        driver = new PhantomJSDriver();
                        break;
                    case BrowserType.Safari:
                        driver = new SafariDriver();
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Browser type '{0}' is not supported in Selenium local mode. Did you mean to configure a remote driver?", browserType));
                }
            }

            // Set Driver Settings
            var managementSettings = driver.Manage();
           
            // Set timeouts
            managementSettings.Timeouts()
                .ImplicitlyWait(browserFactoryConfiguration.ElementLocateTimeout)
                .SetPageLoadTimeout(browserFactoryConfiguration.PageLoadTimeout);

            // Maximize window
            managementSettings.Window.Maximize();

            return driver;
        }

        /// <summary>
        /// Creates the browser.
        /// </summary>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>A browser object.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the browser type is not supported.</exception>
        protected override IBrowser CreateBrowser(BrowserType browserType, BrowserFactoryConfigurationElement browserFactoryConfiguration)
        {
            var launchAction = new Func<IWebDriver>(() => CreateWebDriver(browserType, browserFactoryConfiguration));
            
            var browser = new Lazy<IWebDriver>(launchAction, LazyThreadSafetyMode.None);
            return new SeleniumBrowser(browser);
        }

        /// <summary>
        /// Validates the driver setup.
        /// </summary>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        protected override void ValidateDriverSetup(BrowserType browserType, BrowserFactoryConfigurationElement browserFactoryConfiguration)
        {
            // If we're using a remote driver, don't check paths
            if (GetRemoteDriverUri(browserFactoryConfiguration.Settings) != null)
            {
                return;
            }

            try
            {
                var driver = CreateWebDriver(browserType, browserFactoryConfiguration);
                driver.Quit();
            }
            catch (DriverServiceNotFoundException ex)
            {
                if (SeleniumDriverPath == null)
                {
                    // Error if we weren't able to construct a path earlier.
                    throw;
                }

                try
                {
                    switch (browserType)
                    {
                        case BrowserType.IE:
                            DownloadIeDriver();
                            break;
                        case BrowserType.Chrome:
                            DownloadChromeDriver();
                            break;
                        case BrowserType.PhantomJS:
                            DownloadPhantomJsDriver();
                            break;
                        default:
                            throw;
                    }
                }
                catch (Exception)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Downloads the Phantom JS driver.
        /// </summary>
        private static void DownloadPhantomJsDriver()
        {
            const string FileName = "phantomjs-1.9.7-windows.zip";

            DownloadAndExtractZip("https://bitbucket.org/ariya/phantomjs/downloads", FileName);

            // Move the phantomjs.exe out of the unzipped folder
            var unzippedFolder = Path.Combine(SeleniumDriverPath, Path.GetFileNameWithoutExtension(FileName)); 
            File.Move(Path.Combine(unzippedFolder, PhantomjsExe), Path.Combine(SeleniumDriverPath, PhantomjsExe));            
            Directory.Delete(unzippedFolder, true);
        }

        /// <summary>
        /// Downloads the chrome driver.
        /// </summary>
        private static void DownloadChromeDriver()
        {
            string url;
            using (var webClient = new WebClient())
            {
                // First get the latest version
                var releaseNumber = webClient.DownloadString(string.Format("{0}/LATEST_RELEASE", ChromeUrl));    

                // Combine to download
                url = string.Format("{0}/{1}", ChromeUrl, releaseNumber.Trim());
            }

            DownloadAndExtractZip(url, "chromedriver_win32.zip");
        }

        /// <summary>
        /// Downloads the IE driver.
        /// </summary>
        private static void DownloadIeDriver()
        {
            // Determine bit-wise of OS
            var fileName = string.Format("IEDriverServer_{0}_2.41.0.zip", Environment.Is64BitOperatingSystem ? "x64" : "Win32");

            // Download - this is set to a single version for now
            DownloadAndExtractZip("http://selenium-release.storage.googleapis.com/2.41", fileName);
        }

        /// <summary>
        /// Downloads the specified file from the URL and extracts it to the path.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="zipName">Name of the zip.</param>
        private static void DownloadAndExtractZip(string baseUri, string zipName)
        {
            using (var webClient = new WebClient())
            {
                // Combine to download
                var url = string.Format("{0}/{1}", baseUri, zipName);
                var zipPath = Path.Combine(SeleniumDriverPath, zipName);
                webClient.DownloadFile(url, zipPath);

                // Unzip the file to the parent directory
                ZipFile.ExtractToDirectory(zipPath, SeleniumDriverPath);

                // Delete the zip file
                File.Delete(zipPath);
            }
        }

        /// <summary>
        /// Gets the remote driver URI.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>The URI if the setting is valid, otherwise <c>null</c>.</returns>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Thrown if the URI is not valid.</exception>
        private static Uri GetRemoteDriverUri(NameValueConfigurationCollection settings)
        {
            var remoteSetting = settings[RemoteUrlSetting];

            if (remoteSetting == null || string.IsNullOrWhiteSpace(remoteSetting.Value))
            {
                return null;
            }

            Uri remoteUri;
            if (!Uri.TryCreate(remoteSetting.Value, UriKind.Absolute, out remoteUri))
            {
                throw new ConfigurationErrorsException(
                    string.Format("The {0} setting is not a valid URI: {1}", RemoteUrlSetting, remoteSetting.Value));
            }

            return remoteUri;
        }

        /// <summary>
        /// Checks to see if settings for the remote driver exists.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="remoteWebDriver">The created remote web driver.</param>
        /// <returns><c>true</c> if the settings exist; otherwise <c>false</c>.</returns>
        private static bool RemoteDriverExists(NameValueConfigurationCollection settings, BrowserType browserType, out IWebDriver remoteWebDriver)
        {
            var remoteUri = GetRemoteDriverUri(settings);

            if (remoteUri == null)
            {
                remoteWebDriver = null;
                return false;
            }

            DesiredCapabilities capability;
            switch (browserType)
            {
                case BrowserType.IE:
                    capability = DesiredCapabilities.InternetExplorer();
                    break;
                case BrowserType.FireFox:
                    capability = DesiredCapabilities.Firefox();
                    break;
                case BrowserType.Chrome:
                    capability = DesiredCapabilities.Chrome();
                    break;
                case BrowserType.Safari:
                    capability = DesiredCapabilities.Safari();
                    break;
                case BrowserType.Opera:
                    capability = DesiredCapabilities.Opera();
                    break;
                case BrowserType.Android:
                    capability = DesiredCapabilities.Android();
                    break;
                case BrowserType.iPhone:
                    capability = DesiredCapabilities.IPhone();
                    break;
                case BrowserType.iPad:
                    capability = DesiredCapabilities.IPad();
                    break;
                case BrowserType.PhantomJS:
                    capability = DesiredCapabilities.PhantomJS();
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Browser Type '{0}' is not supported as a remote driver.", browserType));
            }

            // Add any additional settings that are not reserved
            var reservedSettings = new[] { RemoteUrlSetting };
            foreach (var setting in settings.OfType<NameValueConfigurationElement>()
                                            .Where(s => reservedSettings.All(r => !string.Equals(r, s.Name, StringComparison.OrdinalIgnoreCase))))
            {
                capability.SetCapability(setting.Name, setting.Value);
            }

            remoteWebDriver = new RemoteScreenshotWebDriver(remoteUri, capability);
            return true;
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
                systemPath = string.Format("{0};{1}", path, systemPath);
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