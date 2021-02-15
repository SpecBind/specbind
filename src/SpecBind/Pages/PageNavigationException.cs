// <copyright file="PageNavigationException.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
    using System;

    /// <summary>
    /// An exception that is thrown when navigation fails.
    /// </summary>
    public class PageNavigationException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageNavigationException" /> class.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="url">The URL.</param>
        /// <param name="actualUri">The actual URI.</param>
        public PageNavigationException(Type pageType, string url, string actualUri = null)
            : base(string.Format("Cannot navigate to page: {0}, Url: {1}, Actual: {2}", pageType.Name, url, actualUri))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageNavigationException" /> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments for the format string.</param>
        public PageNavigationException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageNavigationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
        public PageNavigationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}