// <copyright file="BrowserBase.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.BrowserSupport
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Net;
    using SpecBind.Actions;
    using SpecBind.Pages;
    using UriHelper = Helpers.UriHelper;

    /// <summary>
    /// A set of extension methods for the <see cref="IBrowser" /> interface.
    /// </summary>
    public abstract class BrowserBase : IBrowser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserBase" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected BrowserBase(ILogger logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the type of the base page.
        /// </summary>
        /// <value>The type of the base page.</value>
        public abstract Type BasePageType { get; }

        /// <summary>
        /// Gets the url of the current page.
        /// </summary>
        /// <value>
        /// The url of the base page.
        /// </value>
        public abstract string Url { get; }

        /// <summary>
        /// Gets the title of the current page.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Gets a value indicating whether or not the browser has been closed.
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the browser is created.
        /// </summary>
        public abstract bool IsCreated { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the browser supports page history service.
        /// </summary>
        /// <value>
        ///   <c>true</c> if page history service is supported; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsPageHistoryService { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// Adds the cookie to the browser.
        /// </summary>
        /// <param name="name">The cookie name.</param>
        /// <param name="value">The cookie value.</param>
        /// <param name="path">The path.</param>
        /// <param name="expireDateTime">The expiration date time.</param>
        /// <param name="domain">The cookie domain.</param>
        /// <param name="secure">if set to <c>true</c> the cookie is secure.</param>
        public abstract void AddCookie(string name, string value, string path, DateTime? expireDateTime, string domain, bool secure);

        /// <summary>
        /// Get a cookie from the browser
        /// </summary>
        /// <param name="name">The name of the cookie</param>
        /// <returns>The cookie (if exists)</returns>
        public abstract Cookie GetCookie(string name);

        /// <summary>
        /// Clear all browser cookies
        /// </summary>
        public abstract void ClearCookies();

        /// <summary>
        /// Clears the URL.
        /// </summary>
        public abstract void ClearUrl();

        /// <summary>
        /// Determines whether the URL can be retrieved.
        /// </summary>
        /// <returns><c>true</c> if the URL can be retrieved; otherwise, <c>false</c>.</returns>
        public abstract bool CanGetUrl();

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Closes the instance and optionally dispose of all resources
        /// </summary>
        /// <param name="dispose">Whether or not resources should get disposed</param>
        public void Close(bool dispose)
        {
            try
            {
                this.Close();
            }
            catch
            {
            }

            if (dispose)
            {
                this.Dispose();
            }

            this.IsClosed = true;
        }

        /// <summary>
        /// Dismisses the alert.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="text">The text to enter.</param>
        public abstract void DismissAlert(AlertBoxAction action, string text);

        /// <summary>
        /// Navigates the browser to the given <paramref name="url" />.
        /// </summary>
        /// <param name="url">The URL specified as a well formed Uri.</param>
        public abstract void GoTo(Uri url);

        /// <summary>
        /// Ensures the page is current in the browser window.
        /// </summary>
        /// <param name="page">The page.</param>
        public void EnsureOnPage(IPage page)
        {
            string actualPath;
            string expectedPath;
            if (!this.CheckIsOnPage(page.PageType, page, out actualPath, out expectedPath))
            {
                throw new PageNavigationException(page.PageType, expectedPath, actualPath);
            }
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script to execute.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The result of the script if needed.</returns>
        public abstract object ExecuteScript(string script, params object[] args);

        /// <summary>
        /// Gets the URI for the page if supported by the browser.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <returns>
        /// The URI partial string if found.
        /// </returns>
        public virtual string GetUriForPageType(Type pageType)
        {
            return null;
        }

        /// <summary>
        /// Navigates to the specified URL defined by the page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="parameters">The parameters to fill in any blanks.</param>
        /// <returns>
        /// The page object when navigated to.
        /// </returns>
        public IPage GoToPage(Type pageType, IDictionary<string, string> parameters)
        {
            var filledUri = UriHelper.FillPageUri(this, pageType, parameters);
            try
            {
                this.Logger.Debug("Navigating to URL: {0}", filledUri);
                this.GoTo(new Uri(filledUri));
            }
            catch (Exception ex)
            {
                throw new PageNavigationException($"Could not navigate to URI: {filledUri}.", ex);
            }

            return this.CreateNativePage(pageType, true);
        }

        /// <summary>
        /// Goes back to the previous page using the browser's back button.
        /// </summary>
        public abstract void GoBack();

        /// <summary>
        /// Refreshes the current page.
        /// </summary>
        public abstract void Refresh();

        /// <summary>
        /// Pages this instance.
        /// </summary>
        /// <typeparam name="TPage">The type of the page.</typeparam>
        /// <returns>A new page object.</returns>
        public IPage Page<TPage>()
            where TPage : class
        {
            return this.Page(typeof(TPage));
        }

        /// <summary>
        /// Gets the page instance from the browser.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <returns>
        /// The page object.
        /// </returns>
        public IPage Page(Type pageType)
        {
            return this.CreateNativePage(pageType, false);
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
        /// Takes the screenshot from the native browser.
        /// </summary>
        /// <param name="imageFolder">The image folder.</param>
        /// <param name="fileNameBase">The file name base.</param>
        /// <returns>The complete file path if created; otherwise <c>null</c>.</returns>
        public abstract string TakeScreenshot(string imageFolder, string fileNameBase);

        /// <summary>
        /// Save the html from the native browser.
        /// </summary>
        /// <param name="destinationFolder">The destination folder.</param>
        /// <param name="fileNameBase">The file name base.</param>
        /// <returns>The complete file path if created; otherwise <c>null</c>.</returns>
        public abstract string SaveHtml(string destinationFolder, string fileNameBase);

        /// <summary>
        /// Sends the keys.
        /// </summary>
        /// <param name="text">The text.</param>
        public abstract void SendKeys(string text);

        /// <summary>
        /// Presses the keys.
        /// </summary>
        /// <param name="text">The text.</param>
        public abstract void PressKeys(string text);

        /// <summary>
        /// Releases the keys.
        /// </summary>
        /// <param name="text">The text.</param>
        public abstract void ReleaseKeys(string text);

        /// <summary>
        /// Maximizes the current browser window.
        /// </summary>
        public abstract void Maximize();

        /// <summary>
        /// Gets the window rectangle.
        /// </summary>
        /// <returns>The window rectangle.</returns>
        public abstract Rectangle GetWindowRectangle();

        /// <summary>
        /// Switches to the window with the specified page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <returns>
        /// The page object.
        /// </returns>
        public abstract IPage SwitchToWindow(Type pageType);

        /// <summary>
        /// Checks whether the page matches the current browser URL.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="page">The page to do further testing if it exists.</param>
        /// <param name="actualPath">The actual path.</param>
        /// <param name="expectedPath">The expected path.</param>
        /// <returns><c>true</c> if it is a match.</returns>
        protected bool CheckIsOnPage(Type pageType, IPage page, out string actualPath, out string expectedPath)
        {
            var validateRegex = UriHelper.GetQualifiedPageUriRegex(this, pageType);

            var actualUrls = this.GetNativePageLocation(page);

            expectedPath = validateRegex.ToString();
            actualPath = string.Join(", ", actualUrls);
            if (string.IsNullOrEmpty(actualPath))
            {
                return false;
            }

            return actualUrls.Any(validateRegex.IsMatch);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.DisposeWindow(disposing);

            this.IsDisposed = true;
        }

        /// <summary>
        /// Gets the native page location.
        /// </summary>
        /// <param name="page">The page interface.</param>
        /// <returns>A collection of URIs to validate.</returns>
        protected abstract IList<string> GetNativePageLocation(IPage page);

        /// <summary>
        /// Creates the native page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="verifyPageValidity">if set to <c>true</c> verify the page's validity.</param>
        /// <returns>A page interface.</returns>
        protected abstract IPage CreateNativePage(Type pageType, bool verifyPageValidity);

        /// <summary>
        /// Releases windows and driver specific resources. This method is already protected by the base instance.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected abstract void DisposeWindow(bool disposing);
    }
}