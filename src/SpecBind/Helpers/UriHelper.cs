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

			Console.WriteLine("Application Base URI: {0}", BaseUri);
		}

        /// <summary>
        /// Gets or sets the base URI.
        /// </summary>
        /// <value>The base URI.</value>
        internal static Uri BaseUri { private get; set; }

		/// <summary>
		/// Gets the fully qualified page URI.
		/// </summary>
		/// <param name="subPath">The sub path.</param>
		/// <returns>The fully qualifies URI.</returns>
		public static Uri GetQualifiedPageUri(string subPath)
		{
		    return new Uri(CreateCompleteUri(new UriStructure(subPath, false), false));
		}

		/// <summary>
		/// Gets the fully qualified page URI.
		/// </summary>
		/// <param name="browser">The browser.</param>
		/// <param name="pageType">Type of the page.</param>
		/// <returns>
		/// The fully qualified URI.
		/// </returns>
		public static Uri GetQualifiedPageUri(IBrowser browser, Type pageType)
		{
		    var compiledUri = CreateCompleteUri(GetPageUriInternal(browser, pageType), false);
            return new Uri(compiledUri);
		}

        /// <summary>
        /// Gets the qualified page URI regex.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="pageType">Type of the page.</param>
        /// <returns>The fully qualified URI.</returns>
	    public static Regex GetQualifiedPageUriRegex(IBrowser browser, Type pageType)
	    {
	        var detailPath = GetPageUriInternal(browser, pageType);
            return new Regex(CreateCompleteUri(detailPath, true));
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
		    return GetPageUriInternal(browser, pageType).Path;
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
		    var uriStructure = GetPageUriInternal(browser, pageType);

            if (string.IsNullOrWhiteSpace(uriStructure.UrlTemplate))
			{
                return CreateCompleteUri(uriStructure, false);
			}

			pageArguments = pageArguments ?? new Dictionary<string, string>(0);
			pageArguments = new Dictionary<string, string>(pageArguments, StringComparer.InvariantCultureIgnoreCase);

			var uriRegex = new Regex(@"\{([A-Za-z]+)\}");

		    var filledPage = uriRegex.Replace(
		        uriStructure.UrlTemplate,
		        m =>
		            {
		                var groupName = m.Groups[1].Value;
		                return pageArguments.ContainsKey(groupName) ? pageArguments[groupName] : m.Value;
		            });

		    return CreateCompleteUri(new UriStructure(filledPage, false), false);
		}

		/// <summary>
        /// Creates the complete URI.
        /// </summary>
        /// <param name="uriStructure">The URI structure.</param>
        /// <param name="isRegex">if set to <c>true</c> the result should be is regex escaped.</param>
        /// <returns>The formatted URI.</returns>
        private static string CreateCompleteUri(UriStructure uriStructure, bool isRegex)
        {
            if (uriStructure.IsAbsolute)
            {
                return uriStructure.Path;
            }

            var subPath = uriStructure.Path ?? string.Empty;

	        var basePath = BaseUri.ToString().TrimEnd('/', ' ');

            if (isRegex)
            {
                basePath = Regex.Escape(basePath);
            }
            
            var seperator = subPath.StartsWith("/") ? string.Empty : "/";

            return string.Concat(basePath, seperator, subPath);
        }

        /// <summary>
        /// Gets the page URL via the page attributes.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="pageType">Type of the page.</param>
        /// <returns>
        /// The URL stricture from the page.
        /// </returns>
        /// <exception cref="PageNavigationException">No PageAttribute or PageNavigationAttribute exists on type: {0}</exception>
        /// <exception cref="PageNavigationException">Thrown if the page is not able to navigate to.</exception>
        private static UriStructure GetPageUriInternal(IBrowser browser, Type pageType)
        {
            PageNavigationAttribute pageNavigationAttribute;
            if (pageType.TryGetAttribute(out pageNavigationAttribute))
            {
                return new UriStructure(pageNavigationAttribute.Url, 
                                        pageNavigationAttribute.IsAbsoluteUrl,
                                        pageNavigationAttribute.UrlTemplate);
            }

            var browserUri = browser.GetUriForPageType(pageType);
            if (!string.IsNullOrWhiteSpace(browserUri))
            {
                return new UriStructure(browserUri, false);
            }

            throw new PageNavigationException("No PageNavigationAttribute exists on type: {0}", pageType.Name);
        }

        #region Private Class - UriStructure

        /// <summary>
        /// A support class to pass around parsed parameters of the URI.
        /// </summary>
	    private class UriStructure
	    {
            /// <summary>
            /// Initializes a new instance of the <see cref="UriStructure" /> class.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="isAbsolute">if set to <c>true</c> the path is an absolute URI.</param>
            /// <param name="urlTemplate">The fill pattern when publishing a URI.</param>
            public UriStructure(string path, bool isAbsolute, string urlTemplate = null)
            {
                this.IsAbsolute = isAbsolute;
                this.Path = path;
                this.UrlTemplate = urlTemplate;
            }

            /// <summary>
            /// Gets the fill pattern.
            /// </summary>
            /// <value>The fill pattern.</value>
            public string UrlTemplate { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this instance is absolute.
            /// </summary>
            /// <value><c>true</c> if this instance is absolute; otherwise, <c>false</c>.</value>
	        public bool IsAbsolute { get; private set; }

            /// <summary>
            /// Gets the path.
            /// </summary>
            /// <value>The path.</value>
	        public string Path { get; private set; }
	    }

        #endregion
    }
}