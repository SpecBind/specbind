﻿// <copyright file="SeleniumFirefoxDriver.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System;
    using System.Collections.Generic;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Firefox;
    using SpecBind.Configuration;

    /// <summary>
    /// Selenium Firefox Driver.
    /// </summary>
    internal class SeleniumFirefoxDriver : SeleniumDriverBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumFirefoxDriver"/> class.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        public SeleniumFirefoxDriver(BrowserFactoryConfigurationElement browserFactoryConfiguration)
            : base(browserFactoryConfiguration)
        {
        }

        /// <summary>
        /// Creates the web driver from the specified browser factory configuration.
        /// </summary>
        /// <returns>The configured web driver.</returns>
        protected override IWebDriver CreateLocalDriver()
        {
            IWebDriver driver;

            if (this.Settings != null && this.Settings.Count > 0)
            {
                var options = new FirefoxOptions();

                foreach (KeyValuePair<string, string> configurationElement in this.Settings)
                {
                    // Removed debug lines but left in comments for future logger support
                    // Debug.WriteLine("SpecBind.Selenium.SeleniumBrowserFactory.GetFireFoxDriver: Setting firefox profile setting:{0} with value: {1}", configurationElement.Name, configurationElement.Value);
                    int intValue;
                    bool boolValue;
                    if (int.TryParse(configurationElement.Value, out intValue))
                    {
                        // Debug.WriteLine("SpecBind.Selenium.SeleniumBrowserFactory.GetFireFoxDriver: Setting firefox profile setting with int value: '{0}'", configurationElement.Name);
                        options.SetPreference(configurationElement.Key, intValue);
                    }
                    else if (bool.TryParse(configurationElement.Value, out boolValue))
                    {
                        // Debug.WriteLine("SpecBind.Selenium.SeleniumBrowserFactory.GetFireFoxDriver: Setting firefox profile setting with bool value: '{0}'", configurationElement.Name);
                        options.SetPreference(configurationElement.Key, boolValue);
                    }
                    else
                    {
                        // Debug.WriteLine("SpecBind.Selenium.SeleniumBrowserFactory.GetFireFoxDriver: Setting firefox profile setting with string value: '{0}'", configurationElement.Name);
                        options.SetPreference(configurationElement.Key, configurationElement.Value);
                    }
                }

                driver = new FirefoxDriver(options);
            }
            else
            {
                driver = new FirefoxDriver();
            }

            if (this.EnsureCleanSession)
            {
                driver.Manage().Cookies.DeleteAllCookies();
            }

            return driver;
        }

        /// <summary>
        /// Downloads the specified selenium driver path.
        /// </summary>
        /// <param name="seleniumDriverPath">The selenium driver path.</param>
        protected override void Download(string seleniumDriverPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the driver options.
        /// </summary>
        /// <returns>The driver options.</returns>
        protected override DriverOptions CreateRemoteDriverOptions()
        {
            return new FirefoxOptions();
        }
    }
}
