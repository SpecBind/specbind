// <copyright file="CookieBuilder.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.CodedUI
{
    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Creates a JavaScript cookie from the input data.
    /// </summary>
    public static class CookieBuilder
    {
        /// <summary>
        /// Creates the cookie.
        /// </summary>
        /// <param name="name">The cookie name.</param>
        /// <param name="value">The cookie value.</param>
        /// <param name="path">The cookie path.</param>
        /// <param name="expireDateTime">The expire date time.</param>
        /// <param name="domain">The cookie domain.</param>
        /// <param name="secure">if set to <c>true</c> the cookie is in secure mode.</param>
        /// <returns>The cookie as a string.</returns>
        public static string CreateCookie(string name, string value, string path, DateTime? expireDateTime, string domain, bool secure)
        {
            if (name.Contains(";"))
            {
                throw new ArgumentException("Cookie name cannot contain ';'", "name");
            }

            // Start with the assignment
            var builder = new StringBuilder(@"document.cookie = """);
            
            // Create the base cookie value
            builder.AppendFormat("{0}={1}", name, Uri.EscapeUriString(value));

            // Append the expiration date
            if (expireDateTime.HasValue)
            {
                var dateValue = expireDateTime.Value;
                
                string dateString;
                if (dateValue == DateTime.MinValue)
                {
                    dateString = @"Thu, 01 Jan 1970 00:00:00 GMT";
                }
                else if (dateValue == DateTime.MaxValue)
                {
                    dateString = @"Fri, 31 Dec 9999 23:59:59 GMT";
                }
                else
                {
                    dateString = dateValue.ToUniversalTime().ToString("R", CultureInfo.InvariantCulture);
                }

                builder.AppendFormat("; expires={0}", dateString);
            }

            // Set the domain if it exists
            if (!string.IsNullOrWhiteSpace(domain))
            {
                builder.AppendFormat("; domain={0}", StripPort(domain));
            }

            // Append the path data
            if (string.IsNullOrWhiteSpace(path))
            {
                path = "/";
            }

            builder.AppendFormat("; path={0}", path);

            // Append the closing quote
            builder.Append('"');

            return builder.ToString();
        }

        /// <summary>
        /// Strips the port from the domain.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <returns>The cleaned domain string.</returns>
        private static string StripPort(string domain)
        {
            return string.IsNullOrEmpty(domain) ? null : domain.Split(new[] { ':' })[0];
        }
    }
}