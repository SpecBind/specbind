// <copyright file="BrowserBase.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.BrowserSupport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SpecBind.Actions;
    using SpecBind.Pages;
    using UriHelper = Helpers.UriHelper;

    /// <summary>
    /// A set of extension methods for the <see cref="IBrowser" /> interface.
    /// </summary>
    public abstract class BrowserBase : IBrowser
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserBase"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected BrowserBase(ILogger logger)
        {
            this.logger = logger;
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
        /// Closes this instance.
        /// </summary>
        public abstract void Close();

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
                this.logger.Debug("Navigating to URL: {0}", filledUri);
                this.GoTo(new Uri(filledUri));
            }
            catch (Exception ex)
            {
                throw new PageNavigationException("Could not navigate to URI: {0}. Details: {1}", filledUri, ex.Message);
            }

            return this.CreateNativePage(pageType, true);
        }

        /// <summary>
        /// Pages this instance.
        /// </summary>
        /// <typeparam name="TPage">The type of the page.</typeparam>
        /// <returns>A new page object.</returns>
        public IPage Page<TPage>() where TPage : class
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
        /// Checks wither the page matches the current browser URL.
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

            return actualUrls.Any(validateRegex.IsMatch);
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
    }
}