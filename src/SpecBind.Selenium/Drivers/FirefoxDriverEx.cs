// <copyright file="FirefoxDriverEx.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System;
    using OpenQA.Selenium.Firefox;

    /// <summary>
    /// Firefox Driver Extended
    /// </summary>
    public class FirefoxDriverEx : FirefoxDriver, IWebDriverEx
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriverEx" /> class.
        /// </summary>
        /// <param name="firefoxDriverService">The firefox driver service.</param>
        public FirefoxDriverEx(FirefoxDriverService firefoxDriverService)
        {
            this.ProcessId = firefoxDriverService.ProcessId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriverEx" /> class.
        /// </summary>
        /// <param name="firefoxDriverService">The firefox driver service.</param>
        /// <param name="firefoxOptions">The Firefox options.</param>
        public FirefoxDriverEx(FirefoxDriverService firefoxDriverService, FirefoxOptions firefoxOptions)
            : base(firefoxOptions)
        {
            this.ProcessId = firefoxDriverService.ProcessId;
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
            return WebDriverExHelper.GetMainBrowserWindowHandle(this.ProcessId, "firefox");
        }

        /// <inheritdoc/>
        public void SetTimezone(string timeZoneId)
        {
            throw new NotImplementedException();
        }
    }
}
