// <copyright file="SeleniumBrowserFactory.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;
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
        private const string RemoteUrlSetting = "RemoteUrl";

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
                        driver = new InternetExplorerDriver();
                        break;
                    case BrowserType.FireFox:
                        driver = new FirefoxDriver();
                        break;
                    case BrowserType.Chrome:
                        driver = new ChromeDriver();
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
        /// Checks to see if settings for the remote driver exists.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="remoteWebDriver">The created remote web driver.</param>
        /// <returns><c>true</c> if the settings exist; otherwise <c>false</c>.</returns>
        private static bool RemoteDriverExists(NameValueConfigurationCollection settings, BrowserType browserType, out IWebDriver remoteWebDriver)
        {
            var remoteSetting = settings[RemoteUrlSetting];

            if (remoteSetting == null || string.IsNullOrWhiteSpace(remoteSetting.Value))
            {
                remoteWebDriver = null;
                return false;
            }

            Uri remoteUri;
            if (!Uri.TryCreate(remoteSetting.Value, UriKind.Absolute, out remoteUri))
            {
                throw new ConfigurationErrorsException(
                    string.Format("The {0} setting is not a valid URI: {1}", RemoteUrlSetting, remoteSetting.Value));
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
    }
}