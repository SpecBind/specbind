// <copyright file="ChromeDriverEx.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System.Collections.Generic;
    using OpenQA.Selenium.Chrome;

    /// <summary>
    /// Chrome Driver Extended
    /// </summary>
    public class ChromeDriverEx : ChromeDriver, IWebDriverEx
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeDriverEx" /> class.
        /// </summary>
        /// <param name="chromeDriverService">The Chrome driver service.</param>
        /// <param name="chromeOptions">The Chrome options.</param>
        public ChromeDriverEx(ChromeDriverService chromeDriverService, ChromeOptions chromeOptions)
            : base(chromeDriverService, chromeOptions)
        {
            this.ProcessId = chromeDriverService.ProcessId;
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
            return WebDriverExHelper.GetMainBrowserWindowHandle(this.ProcessId, "chrome");
        }

        /// <summary>
        /// Sets the timezone.
        /// </summary>
        /// <param name="timeZoneId">The time zone identifier.</param>
        public void SetTimezone(string timeZoneId)
        {
            // https://en.wikipedia.org/wiki/List_of_tz_database_time_zones#List
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["timezoneId"] = timeZoneId
            };

            this.ExecuteChromeCommand("Emulation.setTimezoneOverride", parameters);
        }
    }
}
