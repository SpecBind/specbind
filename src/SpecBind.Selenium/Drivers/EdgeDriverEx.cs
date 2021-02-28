// <copyright file="EdgeDriverEx.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System.Collections.Generic;
    using Microsoft.Edge.SeleniumTools;

    /// <summary>
    /// Edge Driver Extended
    /// </summary>
    public class EdgeDriverEx : EdgeDriver, IWebDriverEx
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeDriverEx" /> class.
        /// </summary>
        /// <param name="edgeDriverService">The Edge driver service.</param>
        /// <param name="edgeOptions">The Edge options.</param>
        public EdgeDriverEx(EdgeDriverService edgeDriverService, EdgeOptions edgeOptions)
            : base(edgeDriverService, edgeOptions)
        {
            this.ProcessId = edgeDriverService.ProcessId;
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
            return WebDriverExHelper.GetMainBrowserWindowHandle(this.ProcessId, "edge");
        }

        /// <inheritdoc/>
        public void SetTimezone(string timeZoneId)
        {
            // https://en.wikipedia.org/wiki/List_of_tz_database_time_zones#List
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["timezoneId"] = timeZoneId
            };

            this.ExecuteChromiumCommand("Emulation.setTimezoneOverride", parameters);
        }
    }
}
