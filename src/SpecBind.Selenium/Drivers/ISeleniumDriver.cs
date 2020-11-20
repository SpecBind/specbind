// <copyright file="ISeleniumDriver.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using Configuration;
    using OpenQA.Selenium;

    /// <summary>
    /// Selenium Driver
    /// </summary>
    public interface ISeleniumDriver
    {

        /// <summary>
        /// Gets or sets a value indicating whether to maximize the window.
        /// </summary>
        /// <value><c>true</c> if the window is maximized; otherwise, <c>false</c>.</value>
        bool MaximizeWindow { get; set; }

        /// <summary>
        /// Creates the web driver from the specified browser factory configuration.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The configured web driver.</returns>
        IWebDriver Create(BrowserFactoryConfiguration browserFactoryConfiguration);

        /// <summary>
        /// Validates the driver setup.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <param name="seleniumDriverPath">The selenium driver path.</param>
        void Validate(BrowserFactoryConfiguration browserFactoryConfiguration, string seleniumDriverPath);

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();
    }
}