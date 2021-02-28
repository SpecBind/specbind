// <copyright file="SeleniumApplication.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Drivers;
    using Helpers;
    using OpenQA.Selenium.Appium.Windows;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Pages;
    using TechTalk.SpecFlow;
    using Cookie = System.Net.Cookie;
    using File = Alphaleonis.Win32.Filesystem.File;
    using Path = Alphaleonis.Win32.Filesystem.Path;

    /// <summary>
    /// An application wrapper for selenium
    /// </summary>
    public class SeleniumApplication : SeleniumBase
    {
        private readonly ScenarioContext scenarioContext;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumApplication" /> class.
        /// </summary>
        /// <param name="driver">The browser driver as a lazy object.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="seleniumDriver">The selenium driver.</param>
        public SeleniumApplication(Lazy<IWebDriverEx> driver, ScenarioContext scenarioContext, ILogger logger, Lazy<ISeleniumDriver> seleniumDriver)
            : base(driver, logger)
        {
            this.scenarioContext = scenarioContext;
            this.logger = logger;
            this.SeleniumDriver = seleniumDriver;
            this.SupportsPageHistoryService = true;
        }

        /// <summary>
        /// Gets the type of the base page.
        /// </summary>
        /// <value>The type of the base page.</value>
        public override Type BasePageType => null;

        /// <summary>
        /// Gets the selenium driver.
        /// </summary>
        /// <value>
        /// The selenium driver.
        /// </value>
        public Lazy<ISeleniumDriver> SeleniumDriver { get; }

        private WindowsDriver<WindowsElement> WindowsDriver => this.Driver as WindowsDriver<WindowsElement>;

        /// <summary>
        /// Adds the cookie to the browser.
        /// </summary>
        /// <param name="name">The cookie name.</param>
        /// <param name="value">The cookie value.</param>
        /// <param name="path">The path.</param>
        /// <param name="expireDateTime">The expiration date time.</param>
        /// <param name="domain">The cookie domain.</param>
        /// <param name="secure">if set to <c>true</c> the cookie is secure.</param>
        public override void AddCookie(
            string name,
            string value,
            string path,
            DateTime? expireDateTime,
            string domain,
            bool secure)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Get a cookie from the browser
        /// </summary>
        /// <param name="name">The name of the cookie</param>
        /// <returns>The cookie (if exists)</returns>
        public override Cookie GetCookie(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Clear all browser cookies
        /// </summary>
        public override void ClearCookies()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Clears the URL.
        /// </summary>
        public override void ClearUrl()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Dismisses the alert.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="text">The text to enter.</param>
        public override void DismissAlert(AlertBoxAction action, string text)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script to execute.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The result of the script if needed.</returns>
        public override object ExecuteScript(string script, params object[] args)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Navigates the browser to the given <paramref name="url" />.
        /// </summary>
        /// <param name="url">The URL specified as a well formed Uri.</param>
        public override void GoTo(Uri url)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Goes back to the previous page using the browser's back button.
        /// </summary>
        public override void GoBack()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Refresh()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Save the html from the native browser.
        /// </summary>
        /// <param name="destinationFolder">The destination folder.</param>
        /// <param name="fileNameBase">The file name base.</param>
        /// <returns>The complete file path if created; otherwise <c>null</c>.</returns>
        public override string SaveHtml(string destinationFolder, string fileNameBase)
        {
            var localDriver = this.Driver;
            try
            {
                // save page source to a file unique to each test
                string pageSource = this.WindowsDriver?.PageSource;

                string destinationFilePath = Path.Combine(destinationFolder, $"{fileNameBase}.xml");

                string sourceFilePath = Path.GetTempFileName();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(pageSource);
                xmlDoc.Save(sourceFilePath);

                File.Move(sourceFilePath, destinationFilePath);

                return destinationFilePath;
            }
            catch (Exception ex)
            {
                this.logger.Info(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close()
        {
            if ((!this.IsClosed) && this.IsCreated)
            {
                this.WindowsDriver?.CloseApp();
                this.WindowsDriver?.Quit();
                this.WindowsDriver?.Dispose();
            }

            if (this.SeleniumDriver.IsValueCreated)
            {
                this.SeleniumDriver.Value.Stop(this.scenarioContext);
            }
        }

        /// <summary>
        /// Determines whether the URL can be retrieved.
        /// </summary>
        /// <returns><c>true</c> if the URL can be retrieved; otherwise, <c>false</c>.</returns>
        public override bool CanGetUrl()
        {
            return true;
        }

        /// <summary>
        /// Switches to the window with the specified page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <returns>
        /// The page object.
        /// </returns>
        public override IPage SwitchToWindow(Type pageType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the native page location.
        /// </summary>
        /// <param name="page">The page interface.</param>
        /// <returns>A collection of URIs to validate.</returns>
        protected override IList<string> GetNativePageLocation(IPage page)
        {
            ILocationProvider nativePage = page.GetNativePage<ILocationProvider>();
            if (nativePage != null)
            {
                return nativePage.GetPageLocation();
            }

            return new List<string> { this.Driver.Title };
        }

        /// <summary>
        /// Creates the native page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="verifyPageValidity">if set to <c>true</c> verify the page validity.</param>
        /// <returns>The created page object.</returns>
        protected override IPage CreateNativePage(Type pageType, bool verifyPageValidity)
        {
            PageHistoryService pageHistoryService = this.scenarioContext?.ScenarioContainer.Resolve<PageHistoryService>();
            if ((pageHistoryService != null) && pageHistoryService.Contains(pageType))
            {
                return pageHistoryService[pageType];
            }

            var page = this.CreateNativePage(pageType);

            if ((pageHistoryService != null) && (!pageHistoryService.Contains(page)))
            {
                pageHistoryService.Add(page);
            }

            return page;
        }

        /// <summary>
        /// Releases windows and driver specific resources. This method is already protected by the base instance.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void DisposeWindow(bool disposing)
        {
        }

        private static string GetElementLocatorName(Type pageType)
        {
            ElementLocatorAttribute elementLocatorAttribute;
            if (pageType.TryGetAttribute(out elementLocatorAttribute))
            {
                return elementLocatorAttribute.Name;
            }

            throw new PageNavigationException("No ElementLocatorAttribute exists on type: {0}", pageType.Name);
        }
    }
}
