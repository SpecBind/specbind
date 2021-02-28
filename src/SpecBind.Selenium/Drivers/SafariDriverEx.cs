// <copyright file="SafariDriverEx.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System;
    using OpenQA.Selenium.Safari;

    /// <summary>
    /// Safari Driver Extended
    /// </summary>
    /// <seealso cref="OpenQA.Selenium.Safari.SafariDriver" />
    /// <seealso cref="SpecBind.Selenium.Drivers.IWebDriverEx" />
    public class SafariDriverEx : SafariDriver, IWebDriverEx
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafariDriverEx" /> class.
        /// </summary>
        /// <param name="safariDriverService">The safari driver service.</param>
        public SafariDriverEx(SafariDriverService safariDriverService)
        {
            this.ProcessId = safariDriverService.ProcessId;
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
            return WebDriverExHelper.GetMainBrowserWindowHandle(this.ProcessId, "safari");
        }

        /// <inheritdoc/>
        public void SetTimezone(string timeZoneId) => throw new NotImplementedException();
    }
}