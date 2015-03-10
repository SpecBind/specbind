// <copyright file="SetCookieAttribute.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
    using System;

    /// <summary>
    /// An attribute that allows the user to add a cookie to the browser before a page loads.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class SetCookieAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetCookieAttribute"/> class.
        /// </summary>
        /// <param name="name">The cookie name.</param>
        /// <param name="value">The cookie value.</param>
        public SetCookieAttribute(string name, string value)
        {
            this.Name = name;
            this.Value = value;
            this.Path = "/";
        }

        /// <summary>
        /// Gets or sets the cookie domain.
        /// </summary>
        /// <value>The cookie domain.</value>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the expiration date and time. 
        /// Set to <see cref="DateTime.MinValue"/> to remove the cookie, or <see cref="DateTime.MaxValue"/> for a non-expiring cookie.
        /// </summary>
        /// <value>The expiration date and time.</value>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cookie is secure.
        /// </summary>
        /// <value><c>true</c> if the cookie is secure; otherwise, <c>false</c>.</value>
        public bool IsSecure { get; set; }

        /// <summary>
        /// Gets the name of the cookie.
        /// </summary>
        /// <value>The name of the cookie.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the cookie path under the URL starting with a '/'.
        /// </summary>
        /// <value>The cookie path.</value>
        public string Path { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(
                "Domain: {0}, Expires: {1}, IsSecure: {2}, Name: {3}, Path: {4}, Value: {5}",
                this.Domain,
                this.Expires,
                this.IsSecure,
                this.Name,
                this.Path,
                this.Value);
        }
    }
}