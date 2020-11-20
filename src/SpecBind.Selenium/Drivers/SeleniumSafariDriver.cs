// <copyright file="SeleniumSafariDriver.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System;
    using Configuration;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Safari;

    /// <summary>
    /// Selenium Safari Driver.
    /// </summary>
    /// <seealso cref="SpecBind.Selenium.Drivers.SeleniumDriverBase" />
    internal class SeleniumSafariDriver : SeleniumDriverBase
    {
        /// <summary>
        /// Creates the web driver from the specified browser factory configuration.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The configured web driver.</returns>
        protected override IWebDriver CreateLocalDriver(BrowserFactoryConfiguration browserFactoryConfiguration)
        {
            return new SafariDriver();
        }

        /// <summary>
        /// Downloads the driver to the specified path.
        /// </summary>
        /// <param name="driverPath">The driver path.</param>
        protected override void Download(string driverPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the driver options.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The driver options.</returns>
        protected override DriverOptions CreateRemoteDriverOptions(BrowserFactoryConfiguration browserFactoryConfiguration)
        {
            return new SafariOptions();
        }
    }
}