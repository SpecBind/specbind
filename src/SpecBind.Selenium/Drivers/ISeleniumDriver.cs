// <copyright file="ISeleniumDriver.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System.Collections.Generic;
    using OpenQA.Selenium;

    /// <summary>
    /// Selenium Driver
    /// </summary>
    public interface ISeleniumDriver
    {
        /// <summary>
        /// Gets or sets a value indicating whether the session cache and cookies should be cleared before starting.
        /// </summary>
        /// <value><c>true</c> if the session should be cleared; otherwise <c>false</c>.</value>
        bool EnsureCleanSession { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Creates the web driver from the specified browser factory configuration.
        /// </summary>
        /// <returns>The configured web driver.</returns>
        IWebDriver Create();

        /// <summary>
        /// Validates the driver setup.
        /// </summary>
        /// <param name="seleniumDriverPath">The selenium driver path.</param>
        void Validate(string seleniumDriverPath);

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();
    }
}
