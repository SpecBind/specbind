// <copyright file="WindowsDriverEx.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// Windows Driver Extended
    /// </summary>
    /// <seealso cref="IWebDriverEx" />
    public class WindowsDriverEx : WindowsDriver<WindowsElement>, IWebDriverEx
    {
        private readonly Uri winAppDriverUri;
        private readonly AppiumOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsDriverEx"/> class.
        /// </summary>
        /// <param name="winAppDriverUri">The win application driver URI.</param>
        /// <param name="options">The options.</param>
        public WindowsDriverEx(Uri winAppDriverUri, AppiumOptions options)
            : base(winAppDriverUri, options)
        {
            this.winAppDriverUri = winAppDriverUri;
            this.options = options;

            // before navigate, get the hub port- this could be dynamic..
            // from port find the process id i.e driver pid
            this.ProcessId = ProcessHelper.ProcessHelper.GetPidFromPort(winAppDriverUri.Port);
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
        public string GetMainBrowserWindowHandle() => throw new NotImplementedException();

        /// <inheritdoc/>
        public void SetTimezone(string timeZoneId) => throw new NotImplementedException();
    }
}