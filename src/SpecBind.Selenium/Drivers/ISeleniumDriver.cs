// <copyright file="ISeleniumDriver.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using Configuration;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Selenium Driver
    /// </summary>
    public interface ISeleniumDriver
    {
        /// <summary>
        /// Gets or sets a value indicating whether the driver supports page load timeouts.
        /// </summary>
        /// <value><c>true</c> if the driver supports page load timeouts; otherwise, <c>false</c>.</value>
        bool SupportsPageLoadTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to maximize the window.
        /// </summary>
        /// <value><c>true</c> if the window is maximized; otherwise, <c>false</c>.</value>
        bool MaximizeWindow { get; set; }

        /// <summary>
        /// Creates the web driver from the specified browser factory configuration.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <returns>
        /// The configured web driver.
        /// </returns>
        IWebDriverEx Create(
            BrowserFactoryConfiguration browserFactoryConfiguration,
            ScenarioContext scenarioContext = null);

        /// <summary>
        /// Validates the driver setup.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="seleniumDriverPath">The selenium driver path.</param>
        void Validate(
            BrowserFactoryConfiguration browserFactoryConfiguration,
            ScenarioContext scenarioContext,
            string seleniumDriverPath);

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        void Stop(ScenarioContext scenarioContext);
    }
}