// <copyright file="InternetExplorerDriverEx.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System;
    using OpenQA.Selenium.IE;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Internet Explorer Driver Extended
    /// </summary>
    public class InternetExplorerDriverEx : InternetExplorerDriver, IWebDriverEx
    {
        private readonly ISeleniumDriver driver;
        private readonly ScenarioContext scenarioContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="InternetExplorerDriverEx" /> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="internetExplorerDriverService">The Internet Explorer driver service.</param>
        /// <param name="internetExplorerOptions">The Internet Explorer options.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        public InternetExplorerDriverEx(
            ISeleniumDriver driver,
            InternetExplorerDriverService internetExplorerDriverService,
            InternetExplorerOptions internetExplorerOptions,
            ScenarioContext scenarioContext)
            : base(internetExplorerDriverService, internetExplorerOptions)
        {
            this.driver = driver;
            this.scenarioContext = scenarioContext;
            this.ProcessId = internetExplorerDriverService.ProcessId;
        }

        /// <summary>
        /// Gets the process identifier of the driver
        /// </summary>
        public int ProcessId
        {
            get;
        }

        /// <summary>
        /// Gets the main browser window handle.
        /// </summary>
        /// <returns>The main browser window handle.</returns>
        public string GetMainBrowserWindowHandle()
        {
            return WebDriverExHelper.GetMainBrowserWindowHandle(this.ProcessId, "iexplore");
        }

        /// <summary>
        /// Closes the Browser
        /// </summary>
        public new void Close()
        {
            base.Close();

            this.driver.Stop(this.scenarioContext);
        }

        /// <inheritdoc/>
        public void SetTimezone(string timeZoneId) => throw new NotImplementedException();
    }
}
