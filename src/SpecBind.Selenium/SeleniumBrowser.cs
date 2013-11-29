// <copyright file="SeleniumBrowser.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing.Imaging;
    using System.IO;

    using OpenQA.Selenium;
    
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A web browser level wrapper for selenium
    /// </summary>
    public class SeleniumBrowser : BrowserBase, IDisposable
    {
        private readonly Lazy<IWebDriver> driver;
        private readonly SeleniumPageBuilder pageBuilder;
        private readonly Dictionary<Type, Func<IWebDriver, Action<object>, object>> pageCache;

        private bool disposed;
        private bool switchedContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumBrowser"/> class.
        /// </summary>
        /// <param name="driver">The browser driver as a lazy object.</param>
        public SeleniumBrowser(Lazy<IWebDriver> driver)
        {
            this.driver = driver;

            this.pageBuilder = new SeleniumPageBuilder();
            this.pageCache = new Dictionary<Type, Func<IWebDriver, Action<object>, object>>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SeleniumBrowser" /> class.
		/// </summary>
        [ExcludeFromCodeCoverage]
        ~SeleniumBrowser()
		{
			this.Dispose(false);
		}

        /// <summary>
        /// Gets the type of the base page.
        /// </summary>
        /// <value>The type of the base page.</value>
        public override Type BasePageType
        {
            get { return null; }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close()
        {
            if (this.driver.IsValueCreated)
            {
                this.driver.Value.Close();
            }
        }

        /// <summary>
        /// Dismisses the alert.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="text">The text to enter.</param>
        public override void DismissAlert(AlertBoxAction action, string text)
        {
            var alert = this.driver.Value.SwitchTo().Alert();

            if (text != null)
            {
                alert.SendKeys(text);
            }

            switch (action)
            {
                case AlertBoxAction.Cancel:
                case AlertBoxAction.Close:
                case AlertBoxAction.Ignore:
                case AlertBoxAction.No:
                    alert.Dismiss();
                    break;
                case AlertBoxAction.Ok:
                case AlertBoxAction.Retry:
                case AlertBoxAction.Yes:
                    alert.Accept();
                    break;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Navigates the browser to the given <paramref name="url" />.
        /// </summary>
        /// <param name="url">The URL specified as a well formed Uri.</param>
        public override void GoTo(Uri url)
        {
            this.driver.Value.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Takes the screenshot from the native browser.
        /// </summary>
        /// <param name="imageFolder">The image folder.</param>
        /// <param name="fileNameBase">The file name base.</param>
        /// <returns>The complete file path if created; otherwise <c>null</c>.</returns>
        public override string TakeScreenshot(string imageFolder, string fileNameBase)
        {
            var localDriver = this.driver.Value;
            var takesScreenshot = localDriver as ITakesScreenshot;

            if (takesScreenshot == null)
            {
                return null;
            }

            try
            {
                var fullPath = Path.Combine(imageFolder, string.Format("{0}.jpg", fileNameBase));
                
                var screenshot = takesScreenshot.GetScreenshot();
                screenshot.SaveAsFile(fullPath, ImageFormat.Jpeg);

                return fullPath;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the native page location.
        /// </summary>
        /// <param name="page">The page interface.</param>
        /// <returns>A collection of URIs to validate.</returns>
        protected override IList<string> GetNativePageLocation(IPage page)
        {
            var currentLocation = this.driver.Value.Url;
            return new List<string> { currentLocation };
        }

        /// <summary>
        /// Creates the native page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="verifyPageValidity">if set to <c>true</c> verify the page validity.</param>
        /// <returns>The created page object.</returns>
        protected override IPage CreateNativePage(Type pageType, bool verifyPageValidity)
        {
            var webDriver = this.driver.Value;

            // Check to see if a frames reference exists, and switch if needed
            PageNavigationAttribute navigationAttribute;
            if (pageType.TryGetAttribute(out navigationAttribute) && !string.IsNullOrWhiteSpace(navigationAttribute.FrameName))
            {
                webDriver.SwitchTo().Frame(navigationAttribute.FrameName);
                this.switchedContext = true;
            }
            else if (this.switchedContext)
            {
                webDriver.SwitchTo().DefaultContent();
                this.switchedContext = false;
            }

            Func<IWebDriver, Action<object>, object> pageBuildMethod;
            if (!this.pageCache.TryGetValue(pageType, out pageBuildMethod))
            {
                pageBuildMethod = pageBuilder.CreatePage(pageType);
                this.pageCache.Add(pageType, pageBuildMethod);
            }

            var nativePage = pageBuildMethod(webDriver, null);

            return new SeleniumPage(nativePage);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || this.disposed)
            {
                return;
            }

            if (this.driver.IsValueCreated)
            {
                var localDriver = this.driver.Value;
                localDriver.Quit();
                localDriver.Dispose();
            }
            this.disposed = true;
        }
    }
}