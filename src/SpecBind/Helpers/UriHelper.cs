// <copyright file="UriHelper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	using SpecBind.BrowserSupport;
	using SpecBind.Pages;

	/// <summary>
	/// A helper to get to a page on the site.
	/// </summary>
	public static class UriHelper
	{
		private static readonly Uri BaseUri;

		/// <summary>
		/// Initializes the <see cref="UriHelper" /> class.
		/// </summary>
		static UriHelper()
		{
			var configSection = SettingHelper.GetConfigurationSection();
			
			Uri parsedUri;
			BaseUri = configSection != null && configSection.Application != null
			          && Uri.TryCreate(configSection.Application.StartUrl, UriKind.Absolute, out parsedUri)
				          ? parsedUri
				          : new Uri("http://localhost");

			System.Diagnostics.Debug.WriteLine("Application Base URI: {0}", BaseUri);
		}

		/// <summary>
		/// Gets the fully qualified page URI.
		/// </summary>
		/// <param name="subPath">The sub path.</param>
		/// <returns>The fully qualifies URI.</returns>
		public static Uri GetQualifiedPageUri(string subPath)
		{
			return new Uri(BaseUri, subPath);
		}

		/// <summary>
		/// Gets the fully qualified page URI.
		/// </summary>
		/// <param name="browser">The browser.</param>
		/// <param name="pageType">Type of the page.</param>
		/// <returns>
		/// The fully qualifies URI.
		/// </returns>
		public static Uri GetQualifiedPageUri(IBrowser browser, Type pageType)
		{
			return new Uri(BaseUri, GetPageUri(browser, pageType));
		}

		/// <summary>
		/// Gets the page URL.
		/// </summary>
		/// <param name="browser">The browser.</param>
		/// <param name="pageType">Type of the page.</param>
		/// <returns>
		/// The URL from the page.
		/// </returns>
		/// <exception cref="PageNavigationException">No PageAttribute or PageNavigationAttribute exists on type: {0}</exception>
		/// <exception cref="PageNavigationException">Thrown if the page is not able to navigate to.</exception>
		public static string GetPageUri(IBrowser browser, Type pageType)
		{
			PageNavigationAttribute pageNavigationAttribute;
			if (pageType.TryGetAttribute(out pageNavigationAttribute))
			{
				return pageNavigationAttribute.Url;
			}

			var browserUri = browser.GetUriForPageType(pageType);
			if (!string.IsNullOrWhiteSpace(browserUri))
			{
				return browserUri;
			}

			throw new PageNavigationException("No PageNavigationAttribute exists on type: {0}", pageType.Name);
		}

		/// <summary>
		/// Fills the page URI with any substitutions.
		/// </summary>
		/// <param name="browser">The browser.</param>
		/// <param name="pageType">Type of the page.</param>
		/// <param name="pageArguments">The page arguments.</param>
		/// <returns>The completed string.</returns>
		public static string FillPageUri(IBrowser browser, Type pageType, IDictionary<string, string> pageArguments)
		{
			PageNavigationAttribute pageAttribute;
			if (!pageType.TryGetAttribute(out pageAttribute) || string.IsNullOrWhiteSpace(pageAttribute.UrlTemplate))
			{
				return GetPageUri(browser, pageType);
			}

			pageArguments = pageArguments ?? new Dictionary<string, string>(0);
			pageArguments = new Dictionary<string, string>(pageArguments, StringComparer.InvariantCultureIgnoreCase);

			var uriRegex = new Regex(@"\{([A-Za-z]+)\}");

			return uriRegex.Replace(
				pageAttribute.UrlTemplate,
				m =>
					{
						var groupName = m.Groups[1].Value;
						return pageArguments.ContainsKey(groupName) ? pageArguments[groupName] : m.Value;
					});
		}

		/// <summary>
		/// Navigates the browser to the given URL.
		/// </summary>
		/// <param name="browser">The browser.</param>
		/// <param name="subPath">The sub path under the base URL.</param>
		public static void NavigateTo(this IBrowser browser, string subPath)
		{
			var uri = GetQualifiedPageUri(subPath);
			System.Diagnostics.Debug.WriteLine("Uri Helper Navigating to URL: {0}", uri);

			browser.GoTo(uri);
		}

		/// <summary>
		/// Navigates the browser to the given URL specified by the page.
		/// </summary>
		/// <typeparam name="TPage">The type of the page.</typeparam>
		/// <param name="browser">The browser.</param>
		/// <returns>The URL the browser navigated to.</returns>
		public static string NavigateTo<TPage>(this IBrowser browser)
		{
			return NavigateTo(browser, typeof(TPage));
		}

		/// <summary>
		/// Navigates the browser to the given URL specified by the page.
		/// </summary>
		/// <param name="browser">The browser.</param>
		/// <param name="pageType">Type of the page.</param>
		/// <returns>The URL the browser navigated to.</returns>
		public static string NavigateTo(this IBrowser browser, Type pageType)
		{
			var subPath = GetPageUri(browser, pageType);
			var path = GetQualifiedPageUri(subPath);

			System.Diagnostics.Debug.WriteLine("Uri Helper Navigating to URL: {0}", path);
			browser.GoTo(path);

			return path.ToString();
		}
	}
}