// <copyright file="BrowserBase.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.BrowserSupport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    using SpecBind.Pages;
    using UriHelper = Helpers.UriHelper;

    /// <summary>
    /// A set of extension methods for the <see cref="IBrowser" /> interface.
    /// </summary>
    public abstract class BrowserBase : IBrowser
    {
        /// <summary>
        /// Gets the type of the base page.
        /// </summary>
        /// <value>The type of the base page.</value>
        public abstract Type BasePageType { get; }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Navigates the browser to the given <paramref name="url" />.
        /// </summary>
        /// <param name="url">The URL specified as a well formed Uri.</param>
        public abstract void GoTo(Uri url);

        /// <summary>
        /// Ensures the page is current in the browser window.
        /// </summary>
        /// <param name="page">The page.</param>
        public virtual void EnsureOnPage(IPage page)
        {
            string actualPath;
            string expectedPath;
            if (!this.CheckIsOnPage(page.PageType, page, out actualPath, out expectedPath))
            {
                throw new PageNavigationException(page.PageType, expectedPath, actualPath);
            }
        }

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
        public virtual IPage GoToPage(Type pageType, IDictionary<string, string> parameters)
        {
            string actualPath;
            string expectedPath;
            if (!this.CheckIsOnPage(pageType, null, out actualPath, out expectedPath))
            {
                var filledUri = UriHelper.FillPageUri(this, pageType, parameters);
                try
                {
                    var qualifiedUri = UriHelper.GetQualifiedPageUri(filledUri);
                    System.Diagnostics.Debug.WriteLine("Navigating to URL: {0}", qualifiedUri);

                    this.GoTo(qualifiedUri);
                }
                catch (Exception ex)
                {
                    throw new PageNavigationException("Could not navigate to URI: {0}. Details: {1}", filledUri, ex.Message);
                }
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
        /// Checks wither the page matches the current browser URL.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="page">The page to do further testing if it exists.</param>
        /// <param name="actualPath">The actual path.</param>
        /// <param name="expectedPath">The expected path.</param>
        /// <returns><c>true</c> if it is a match.</returns>
        protected virtual bool CheckIsOnPage(Type pageType, IPage page, out string actualPath, out string expectedPath)
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