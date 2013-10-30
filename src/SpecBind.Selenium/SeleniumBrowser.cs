// <copyright file="SeleniumBrowser.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Collections.Generic;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    using SpecBind.BrowserSupport;
    using SpecBind.Pages;

    /// <summary>
    /// A web browser level wrapper for selenium
    /// </summary>
    public class SeleniumBrowser : BrowserBase, IDisposable
    {
        private readonly Lazy<IWebDriver> driver;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumBrowser"/> class.
        /// </summary>
        /// <param name="driver">The browser driver as a lazy object.</param>
        public SeleniumBrowser(Lazy<IWebDriver> driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SeleniumBrowser" /> class.
		/// </summary>
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
            var item = Activator.CreateInstance(pageType);
            var webDriver = this.driver.Value;
            
            PageFactory.InitElements(webDriver, item);
            return new SeleniumPage(item);
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