// <copyright file="BrowserFactoryConfiguration.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Configuration
{
    using System;
    using System.Collections.Generic;
    using SpecBind.BrowserSupport;

    /// <summary>
    /// Browser Factory Configuration.
    /// </summary>
    public class BrowserFactoryConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserFactoryConfiguration"/> class.
        /// </summary>
        public BrowserFactoryConfiguration()
        {
            // Default configuration settings
            this.BrowserType = BrowserType.IE;
            this.ElementLocateTimeout = TimeSpan.FromSeconds(30);
            this.PageLoadTimeout = TimeSpan.FromSeconds(30);
            this.Settings = new Dictionary<string, string>();
            this.UserProfilePreferences = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the type of the browser.
        /// </summary>
        /// <value>The type of the browser.</value>
        public BrowserType BrowserType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to create a screenshot on exit.
        /// </summary>
        /// <value><c>true</c> if a screenshot is created on exit; otherwise, <c>false</c>.</value>
        public bool CreateScreenshotOnExit { get; set; }

        /// <summary>
        /// Gets or sets the element locate timeout.
        /// </summary>
        /// <value>The element locate timeout.</value>
        public TimeSpan ElementLocateTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ensure a clean session.
        /// </summary>
        /// <value><c>true</c> if a clean session is ensured; otherwise, <c>false</c>.</value>
        public bool EnsureCleanSession { get; set; }

        /// <summary>
        /// Gets or sets the page load timeout.
        /// </summary>
        /// <value>The page load timeout.</value>
        public TimeSpan PageLoadTimeout { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to reuse the browser.
        /// </summary>
        /// <value><c>true</c> if the browser is reused; otherwise, <c>false</c>.</value>
        public bool ReuseBrowser { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Gets or sets the user profile preferences.
        /// </summary>
        /// <value>The user profile preferences.</value>
        public Dictionary<string, string> UserProfilePreferences { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the web driver.
        /// </summary>
        /// <value><c>true</c> if the web driver is validated; otherwise, <c>false</c>.</value>
        public bool ValidateWebDriver { get; set; }

        /// <summary>
        /// Gets or sets a value indicating what mechanism, if any,
        /// to use to check for pending AJAX requests before proceeding with each step.
        /// </summary>
        public string WaitForPendingAjaxCallsVia { get; set; }
    }
}
