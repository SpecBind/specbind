// <copyright file="IUriHelper.cs" company="">
//     Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using SpecBind.BrowserSupport;

    /// <summary>
    /// URI Helper
    /// </summary>
    public interface IUriHelper
    {
        /// <summary>
        /// Fills the page URI with any substitutions.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="pageArguments">The page arguments.</param>
        /// <returns>The completed string.</returns>
        string FillPageUri(IBrowser browser, Type pageType, IDictionary<string, string> pageArguments);

        /// <summary>
        /// Gets the fully qualified page URI.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="pageType">Type of the page.</param>
        /// <returns>
        /// The fully qualified URI.
        /// </returns>
        Regex GetQualifiedPageUriRegex(IBrowser browser, Type pageType);

        /// <summary>
        /// Gets the fully qualified page URI.
        /// </summary>
        /// <param name="subPath">The sub path.</param>
        /// <returns>The fully qualifies URI.</returns>
        Uri GetQualifiedPageUri(string subPath);
    }
}