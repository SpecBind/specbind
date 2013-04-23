// <copyright file="CodedUIBrowser.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	using Microsoft.VisualStudio.TestTools.UITesting;
	using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

	using SpecBind.BrowserSupport;
	using SpecBind.Pages;

	/// <summary>
	/// An IBrowser implementation for Coded UI.
	/// </summary>
	public class CodedUIBrowser : IBrowser, IDisposable
	{
		private readonly Dictionary<Type, Func<BrowserWindow, Action<HtmlDocument>, HtmlDocument>> pageCache;
		private readonly Lazy<BrowserWindow> window;

		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="CodedUIBrowser" /> class.
		/// </summary>
		/// <param name="browserWindow">The browser window.</param>
		public CodedUIBrowser(Lazy<BrowserWindow> browserWindow)
		{
			this.window = browserWindow;
			this.pageCache = new Dictionary<Type, Func<BrowserWindow, Action<HtmlDocument>, HtmlDocument>>();
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="CodedUIBrowser" /> class.
		/// </summary>
		~CodedUIBrowser()
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
				return typeof(HtmlDocument);
			}
		}

		/// <summary>
		/// Closes this instance.
		/// </summary>
		public void Close()
		{
			if (this.window.IsValueCreated)
			{
				this.window.Value.Close();
			}
		}

		/// <summary>
		/// Ensures the page is current in the browser window.
		/// </summary>
		/// <param name="page">The page.</param>
		public void EnsureOnPage(IPage page)
		{
			var localWindow = this.window.Value;

			string actualPath;
			string expectedPath;
			if (!this.CheckIsOnPage(localWindow, page.PageType, out actualPath, out expectedPath))
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
		public string GetUriForPageType(Type pageType)
		{
			return null;
		}

		/// <summary>
		/// Navigates the browser to the given <paramref name="url" />.
		/// </summary>
		/// <param name="url">The URL specified as a well formed Uri.</param>
		public void GoTo(Uri url)
		{
			this.window.Value.NavigateToUrl(url);
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
			var localWindow = this.window.Value;

			string actualPath;
			string expectedPath;
			if (!this.CheckIsOnPage(localWindow, pageType, out actualPath, out expectedPath))
			{
				var filledUri = Helpers.UriHelper.FillPageUri(this, pageType, parameters);
				try
				{
					var qualifiedUri = Helpers.UriHelper.GetQualifiedPageUri(filledUri);
					localWindow.NavigateToUrl(qualifiedUri);
				}
				catch (Exception ex)
				{
					throw new PageNavigationException("Could not navigate to URI: {0}. Details: {1}", filledUri, ex.Message);
				}
			}

			var nativePage = this.CreateNativePage(pageType);
			nativePage.Find();

			return new CodedUIPage<HtmlDocument>(nativePage);
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
			return new CodedUIPage<HtmlDocument>(this.CreateNativePage(pageType));
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
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || this.disposed)
			{
				return;
			}

			if (this.window.IsValueCreated)
			{
				this.window.Value.Dispose();
			}
			this.disposed = true;
		}

		/// <summary>
		/// Creates the native page.
		/// </summary>
		/// <param name="pageType">Type of the page.</param>
		/// <returns>The internal document.</returns>
		private HtmlDocument CreateNativePage(Type pageType)
		{
			Func<BrowserWindow, Action<HtmlDocument>, HtmlDocument> function;
			if (!this.pageCache.TryGetValue(pageType, out function))
			{
				function = PageBuilder.CreateElement<BrowserWindow, HtmlDocument>(pageType);
				this.pageCache.Add(pageType, function);
			}

			return function(this.window.Value, null);
		}

		/// <summary>
		/// Checks wither the page matches the current browser URL.
		/// </summary>
		/// <param name="localWindow">The local window.</param>
		/// <param name="pageType">Type of the page.</param>
		/// <param name="actualPath">The actual path.</param>
		/// <param name="expectedPath">The expected path.</param>
		/// <returns>
		///   <c>true</c> if it is a match.
		/// </returns>
		private bool CheckIsOnPage(BrowserWindow localWindow, Type pageType, out string actualPath, out string expectedPath)
		{
			var uri = Helpers.UriHelper.GetPageUri(this, pageType);
			var validateRegex = new Regex(uri);
			
			actualPath = localWindow.Uri.PathAndQuery + localWindow.Uri.Fragment;
			expectedPath = uri;
			return validateRegex.IsMatch(actualPath);
		}
	}
}