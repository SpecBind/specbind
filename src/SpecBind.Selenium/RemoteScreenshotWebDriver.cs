// <copyright file="RemoteScreenshotWebDriver.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium
{
    using System;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Remote;
    using SpecBind.Selenium.Drivers;

    /// <summary>
    /// A <see cref="RemoteWebDriver"/> class extension that supports taking a screenshot.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class RemoteScreenshotWebDriver : RemoteWebDriver, ITakesScreenshot, IWebDriverEx
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteScreenshotWebDriver"/> class.
        /// </summary>
        /// <param name="remoteAddress">URI containing the address of the WebDriver remote server (e.g. http://127.0.0.1:4444).</param>
        /// <param name="desiredCapabilities">An <see cref="T:OpenQA.Selenium.ICapabilities" /> object containing the desired capabilities of the browser.</param>
        public RemoteScreenshotWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities)
            : base(remoteAddress, desiredCapabilities)
        {
        }

        /// <summary>
        /// Gets the process identifier of the driver
        /// </summary>
        /// <value>
        /// The process identifier.
        /// </value>
        public int ProcessId => throw new NotImplementedException();

        /// <summary>
        /// Gets the main browser window handle.
        /// </summary>
        /// <returns>The main browser window handle.</returns>
        public string GetMainBrowserWindowHandle() => throw new NotImplementedException();

        /// <inheritdoc/>
        public void SetTimezone(string timeZoneId) => throw new NotImplementedException();
    }
}