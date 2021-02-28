// <copyright file="IWebDriverEx.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using OpenQA.Selenium;

    /// <summary>
    /// Web Driver Extended Interface
    /// </summary>
    /// <seealso cref="OpenQA.Selenium.IWebDriver" />
    public interface IWebDriverEx : IWebDriver
    {
        /// <summary>
        /// Gets the process identifier of the driver
        /// </summary>
        /// <value>
        /// The process identifier.
        /// </value>
        int ProcessId { get; }

        /// <summary>
        /// Gets the main browser window handle.
        /// </summary>
        /// <returns>The main browser window handle.</returns>
        string GetMainBrowserWindowHandle();

        /// <summary>
        /// Close the current window, quitting the browser if it is the last window currently open.
        /// </summary>
        new void Close();

        /// <summary>
        /// Sets the timezone.
        /// </summary>
        /// <param name="timeZoneId">The time zone identifier.</param>
        void SetTimezone(string timeZoneId);
    }
}
