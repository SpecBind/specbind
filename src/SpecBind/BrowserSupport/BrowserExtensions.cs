// <copyright file="BrowserExtensions.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.BrowserSupport
{
    using System;

    using SpecBind.Pages;

    /// <summary>
	/// A set of extension methods for <see cref="IBrowser"/> to assist with navigation.
	/// </summary>
	public static class BrowserExtensions
	{
		/// <summary>
		/// Ensures the browser is on the given page.
		/// </summary>
		/// <typeparam name="TPage">The type of the page.</typeparam>
		/// <param name="browser">The browser.</param>
		/// <exception cref="PageNavigationException">Thrown if the page is not the current page.</exception>
		public static void EnsureOnPage<TPage>(this IBrowser browser) where TPage : class, new()
		{
			IPage page;
			EnsureOnPage(browser, typeof(TPage), out page);
		}

		/// <summary>
		/// Ensures the browser is on the given page.
		/// </summary>
		/// <param name="browser">The browser.</param>
		/// <param name="pageType">Type of the page.</param>
		/// <param name="page">The page.</param>
		/// <exception cref="PageNavigationException">Thrown if the page is not the current page.</exception>
		public static void EnsureOnPage(this IBrowser browser, Type pageType, out IPage page)
		{
			page = browser.Page(pageType);
			browser.EnsureOnPage(page);
		}

		/// <summary>
		/// Ensures the browser is on the given page. If not, it navigates to the page.
		/// </summary>
		/// <typeparam name="TPage">The type of the page.</typeparam>
		/// <param name="browser">The browser.</param>
		/// <returns>The page that was navigated to.</returns>
		public static IPage GoToPage<TPage>(this IBrowser browser) where TPage : class, new()
		{
			return browser.GoToPage(typeof(TPage), null);
		}
	}
}