// <copyright file="WatinBrowser.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Watin
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using SpecBind.BrowserSupport;
	using SpecBind.Helpers;
	using SpecBind.Pages;

	using WatiN.Core;

	/// <summary>
	///     An <see cref="IBrowser" /> implementation for Watin
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class WatinBrowser : IBrowser
	{
		private readonly Lazy<Browser> browser;

		private bool disposed;

		/// <summary>Initializes a new instance of the <see cref="WatinBrowser"/> class.</summary>
		/// <param name="browser">The browser.</param>
		public WatinBrowser(Lazy<Browser> browser)
		{
			this.browser = browser;
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="WatinBrowser" /> class.
		/// </summary>
		~WatinBrowser()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// Gets the type of the base page.
		/// </summary>
		/// <value>
		/// The type of the base page.
		/// </value>
		public Type BasePageType
		{
			get
			{
				return typeof(Page);
			}
		}

		/// <summary>
		/// Closes this instance.
		/// </summary>
		public void Close()
		{
			if (this.browser.IsValueCreated)
			{
				this.browser.Value.Close();
			}
		}

        /// <summary>
        /// Dismisses the alert.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="text">The text to enter.</param>
        /// <exception cref="System.NotSupportedException">This feature is not supported.</exception>
	    public void DismissAlert(AlertBoxAction action, string text)
	    {
            throw new NotSupportedException("This feature is not supported.");
	    }

	    /// <summary>
		/// Ensures the on page.
		/// </summary>
		/// <param name="page">The page.</param>
		public void EnsureOnPage(IPage page)
		{
			var nativePage = page.GetNativePage<Page>();
			if (!nativePage.IsCurrentDocument)
			{
				throw new PageNavigationException(page.PageType, UriHelper.GetPageUri(this, page.PageType));
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
		/// Gets the URI for the page if supported by the browser.
		/// </summary>
		/// <param name="pageType">Type of the page.</param>
		/// <returns>
		/// The URI partial string if found.
		/// </returns>
		public string GetUriForPageType(Type pageType)
		{
			var pageAttribute = pageType.GetCustomAttributes(typeof(PageAttribute), true).OfType<PageAttribute>().FirstOrDefault();
			return pageAttribute != null ? pageAttribute.UrlRegex : null;
		}

		/// <summary>Navigates the browser to the given <paramref name="url"/>.</summary>
		/// <param name="url">The URL specified as a well formed Uri.</param>
		public void GoTo(Uri url)
		{
			this.browser.Value.GoTo(url);
		}

		/// <summary>
		/// Navigates to the specified URL defined by the page.
		/// </summary>
		/// <param name="pageType">Type of the page.</param>
		/// <param name="parameters">The parameters to fill in for any values.</param>
		/// <returns>
		/// The page object when navigated to.
		/// </returns>
		public IPage GoToPage(Type pageType, IDictionary<string, string> parameters)
		{
			var page = this.Page(pageType);
			try
			{
				this.EnsureOnPage(page);
			}
			catch (PageNavigationException)
			{
				//Not on the page, navigate to it.
				this.NavigateTo(pageType);

				page = this.Page(pageType);
				this.EnsureOnPage(page);
			}

			return page;
		}

		/// <summary>
		///     Gets the page instance from the browser.
		/// </summary>
		/// <typeparam name="TPage">The type of the page.</typeparam>
		/// <returns>The page object.</returns>
		public IPage Page<TPage>() where TPage : class
		{
			return this.Page(typeof(TPage));
		}

		/// <summary>Gets the page instance from the browser.</summary>
		/// <param name="pageType">Type of the page.</param>
		/// <returns>The page object.</returns>
		public IPage Page(Type pageType)
		{
			return new WatinPage(WatiN.Core.Page.CreatePage(pageType, this.browser.Value));
		}

        /// <summary>
        /// Takes the screenshot from the native browser.
        /// </summary>
        /// <param name="imageFolder">The image folder.</param>
        /// <param name="fileNameBase">The file name base.</param>
        /// <returns>The full path of the image file.</returns>
	    public string TakeScreenshot(string imageFolder, string fileNameBase)
        {
            return null;
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script to execute.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The result of the script if needed.</returns>
	    public object ExecuteScript(string script, params object[] args)
	    {
	        this.browser.Value.RunScript(script);
            return null;
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

			if (this.browser.IsValueCreated)
			{
				this.browser.Value.Dispose();
			}
			
			this.disposed = true;
		}
	}
}