// <copyright file="PageNavigationAttribute.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
	using System;

	/// <summary>
	/// An attribute that defines the location to call in the site to navigate to a page.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class PageNavigationAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PageNavigationAttribute" /> class.
		/// </summary>
		/// <param name="url">The URL.</param>
		public PageNavigationAttribute(string url)
		{
			this.Url = url;
		}

		/// <summary>
		/// Gets or sets the name of the frame to use within a browser element.
		/// </summary>
		/// <value>The name of the frame.</value>
		public string FrameName { get; set; }

		/// <summary>
		/// Gets the URL to navigate to.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		public string Url { get; private set; }

		/// <summary>
		/// Gets or sets the fill template to use if a pattern is needed for the URI.
		/// </summary>
		/// <value>
		/// The fill template.
		/// </value>
		public string UrlTemplate { get; set; }
	}
}