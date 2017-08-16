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

    using Microsoft.Win32;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Edge;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;
    using OpenQA.Selenium.Opera;
    using OpenQA.Selenium.PhantomJS;
    using OpenQA.Selenium.Safari;

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
        // Constants to assist with settings
        private const string ChromeUrl = "http://chromedriver.storage.googleapis.com";
        private const string RemoteUrlSetting = "RemoteUrl";
        private const string ChromeArgumentSetting = "ChromeArguments"; 
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
            : base(ValidateWebDriver())
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
            if (!RemoteDriverExists(browserFactoryConfiguration.Settings, browserType, out IWebDriver driver))
            {
                switch (browserType)
                {
                    case BrowserType.IE:
                        var explorerOptions = new InternetExplorerOptions { EnsureCleanSession = browserFactoryConfiguration.EnsureCleanSession };
                        var internetExplorerDriverService = InternetExplorerDriverService.CreateDefaultService();
                        internetExplorerDriverService.HideCommandPromptWindow = true;
                        driver = new InternetExplorerDriver(internetExplorerDriverService, explorerOptions);
                        break;
                    case BrowserType.FireFox:
                        driver = GetFireFoxDriver(browserFactoryConfiguration);
                        break;
                    case BrowserType.Chrome:
                    case BrowserType.ChromeHeadless:
                        var chromeOptions = new ChromeOptions { LeaveBrowserRunning = false };

                        var cmdLineSetting = browserFactoryConfiguration.Settings[ChromeArgumentSetting];
                        if (!string.IsNullOrWhiteSpace(cmdLineSetting?.Value))
                        {
                            foreach (var arg in cmdLineSetting.Value.Split(';'))
                            {
                                chromeOptions.AddArgument(arg);
                            }
                        }

                        // Activate chrome headless via a command line
                        if (browserType == BrowserType.ChromeHeadless)
                        {
                            chromeOptions.AddArgument("--headless");
                        }

                        var chromeDriverService = ChromeDriverService.CreateDefaultService();
                        chromeDriverService.HideCommandPromptWindow = true;

                        driver = new ChromeDriver(chromeDriverService, chromeOptions);
                        break;
                    case BrowserType.PhantomJS:
                        var phantomJsDriverService = PhantomJSDriverService.CreateDefaultService();
                        phantomJsDriverService.HideCommandPromptWindow = true;
                        driver = new PhantomJSDriver(phantomJsDriverService);
                        break;
                    case BrowserType.Safari:
                        driver = new SafariDriver();
                        break;
                    case BrowserType.Edge:
                        var edgeOptions = new EdgeOptions { PageLoadStrategy = EdgePageLoadStrategy.Normal };
                        var edgeDriverService = EdgeDriverService.CreateDefaultService();
                        driver = new EdgeDriver(edgeDriverService, edgeOptions);
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Browser type '{browserType}' is not supported in Selenium local mode. Did you mean to configure a remote driver?");
                }
            }

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

            // Maximize window
            managementSettings.Window.Maximize();

            return driver;
        }

        /// <summary>
        /// Creates the browser.
        /// </summary>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A browser object.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the browser type is not supported.</exception>
        protected override IBrowser CreateBrowser(BrowserType browserType, BrowserFactoryConfigurationElement browserFactoryConfiguration, ILogger logger)
        {
            var launchAction = new Func<IWebDriver>(() => CreateWebDriver(browserType, browserFactoryConfiguration));
            
            var browser = new Lazy<IWebDriver>(launchAction, LazyThreadSafetyMode.None);
            return new SeleniumBrowser(browser, logger);
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
                        case BrowserType.ChromeHeadless:
                            DownloadChromeDriver();
                            break;
                        case BrowserType.PhantomJS:
                            DownloadPhantomJsDriver();
                            break;
                        case BrowserType.Edge:
                            DownloadEdgeDriver();
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
            const string FileName = "phantomjs-2.0.0-windows.zip";

            DownloadAndExtractZip("https://bitbucket.org/ariya/phantomjs/downloads", FileName);

            // Move the phantomjs.exe out of the unzipped folder
            var unzippedFolder = Path.Combine(SeleniumDriverPath, Path.GetFileNameWithoutExtension(FileName), "bin"); 
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
                var releaseNumber = webClient.DownloadString($"{ChromeUrl}/LATEST_RELEASE");    

                // Combine to download
                url = $"{ChromeUrl}/{releaseNumber.Trim()}";
            }

            DownloadAndExtractZip(url, "chromedriver_win32.zip");
        }

        /// <summary>
        /// Downloads the IE driver.
        /// </summary>
        private static void DownloadIeDriver()
        {
            // Determine bit-wise of OS
			// HACK: Only use 32-bit driver; SendKeys is unusably slow with 64-bit driver

            // Download - this is set to a single version for now
			DownloadAndExtractZip("http://selenium-release.storage.googleapis.com/2.50", "IEDriverServer_Win32_2.50.0.zip");
        }

        /// <summary>
        /// Downloads and installs the MS Edge driver based on the platform version
        /// </summary>
        private static void DownloadEdgeDriver()
        {
            var winVersion = GetWindowsBuildNumber();

            string downloadUrl;
            switch (winVersion)
            {
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
                var exePath = Path.Combine(SeleniumDriverPath, "MicrosoftWebDriver.exe");
                webClient.DownloadFile(downloadUrl, exePath);
            }
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
                var url = $"{baseUri}/{zipName}";
                var zipPath = Path.Combine(SeleniumDriverPath, zipName);
                webClient.DownloadFile(url, zipPath);

                // Unzip the file to the parent directory
                ZipFile.ExtractToDirectory(zipPath, SeleniumDriverPath);

                // Delete the zip file
                File.Delete(zipPath);
            }
        }

        /// <summary>
        /// Gets the FireFox driver.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The configured web driver.</returns>
        private static IWebDriver GetFireFoxDriver(BrowserFactoryConfigurationElement browserFactoryConfiguration)
        {
            IWebDriver driver;

            if (browserFactoryConfiguration.Settings != null && browserFactoryConfiguration.Settings.Count > 0)
            {
                var ffprofile = new FirefoxProfile();

                foreach (NameValueConfigurationElement configurationElement in browserFactoryConfiguration.Settings)
                {
                    // Removed debug lines but left in comments for future logger support
                    // Debug.WriteLine("SpecBind.Selenium.SeleniumBrowserFactory.GetFireFoxDriver: Setting firefox profile setting:{0} with value: {1}", configurationElement.Name, configurationElement.Value);

                    if (int.TryParse(configurationElement.Value, out int intValue))
                    {
                        // Debug.WriteLine("SpecBind.Selenium.SeleniumBrowserFactory.GetFireFoxDriver: Setting firefox profile setting with int value: '{0}'", configurationElement.Name);
                        ffprofile.SetPreference(configurationElement.Name, intValue);
                    }
                    else if (bool.TryParse(configurationElement.Value, out bool boolValue))
                    {
                        // Debug.WriteLine("SpecBind.Selenium.SeleniumBrowserFactory.GetFireFoxDriver: Setting firefox profile setting with bool value: '{0}'", configurationElement.Name);
                        ffprofile.SetPreference(configurationElement.Name, boolValue);
                    }
                    else
                    {
                        // Debug.WriteLine("SpecBind.Selenium.SeleniumBrowserFactory.GetFireFoxDriver: Setting firefox profile setting with string value: '{0}'", configurationElement.Name);
                        ffprofile.SetPreference(configurationElement.Name, configurationElement.Value);
                    }
                }

                driver = new FirefoxDriver(ffprofile);
            }
            else
            {
                driver = new FirefoxDriver();
            }

            if (browserFactoryConfiguration.EnsureCleanSession)
            {
                driver.Manage().Cookies.DeleteAllCookies();
            }

            return driver;
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

            if (string.IsNullOrWhiteSpace(remoteSetting?.Value))
            {
                return null;
            }

            if (!Uri.TryCreate(remoteSetting.Value, UriKind.Absolute, out Uri remoteUri))
            {
                throw new ConfigurationErrorsException(
                    $"The {RemoteUrlSetting} setting is not a valid URI: {remoteSetting.Value}");
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

            DriverOptions driverOptions;
            switch (browserType)
            {
                case BrowserType.IE:
                    driverOptions = new InternetExplorerOptions();
                    break;
                case BrowserType.FireFox:
                    driverOptions = new FirefoxOptions();
                    break;
                case BrowserType.Chrome:
                    driverOptions = new ChromeOptions();
                    break;
                case BrowserType.Safari:
                    driverOptions = new SafariOptions();
                    break;
                case BrowserType.Opera:
                    driverOptions = new OperaOptions();
                    break;
                case BrowserType.PhantomJS:
                    driverOptions = new PhantomJSOptions();
                    break;
				case BrowserType.Edge:
					driverOptions = new EdgeOptions();
					break;
                default:
                    throw new InvalidOperationException(
                        $"Browser Type '{browserType}' is not supported as a remote driver.");
            }

            // Add any additional settings that are not reserved
            var envRegex = new System.Text.RegularExpressions.Regex("\\$\\{(.+)\\}");
            var reservedSettings = new[] { RemoteUrlSetting };
            foreach (var setting in settings.OfType<NameValueConfigurationElement>()
                                            .Where(s => reservedSettings.All(r => !string.Equals(r, s.Name, StringComparison.OrdinalIgnoreCase))))
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

            remoteWebDriver = new RemoteScreenshotWebDriver(remoteUri, driverOptions.ToCapabilities());
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
                systemPath = $"{path};{systemPath}";
                Environment.SetEnvironmentVariable("PATH", systemPath);
            }
            catch (SystemException)
            {
                return null;
            }

            return path;
        }

        /// <summary>
        /// Determines whether or not to perform web driver validation
        /// </summary>
        /// <returns><c>true</c> if the web driver should be validated; otherwise <c>false</c></returns>
        private static bool ValidateWebDriver()
        {
            var configSection = SettingHelper.GetConfigurationSection();
            return configSection.BrowserFactory.ValidateWebDriver;
        }
    }
}