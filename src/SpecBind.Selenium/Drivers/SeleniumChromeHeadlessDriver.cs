// <copyright file="SeleniumChromeHeadlessDriver.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using SpecBind.Configuration;

    /// <summary>
    /// Selenium Chrome Headless Driver.
    /// </summary>
    internal class SeleniumChromeHeadlessDriver : SeleniumChromeDriver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumChromeHeadlessDriver" /> class.
        /// </summary>
        public SeleniumChromeHeadlessDriver()
        {
            this.AdditionalArguments.Add("--headless");
        }
    }
}