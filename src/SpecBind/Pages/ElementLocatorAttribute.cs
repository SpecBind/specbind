// <copyright file="ElementLocatorAttribute.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
	using System;

	/// <summary>
	/// An attribute for locating element on a page.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ElementLocatorAttribute : Attribute
	{
	    private string tagName;

	    #region Constructors and Destructors

		/// <summary>
		/// Finds a component based on its attributes.
		/// </summary>
		public ElementLocatorAttribute()
		{
			this.Index = -1;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the alt text to find.
		/// </summary>
		/// <value>
		/// The alt.
		/// </value>
		public string Alt { get; set; }

		/// <summary>
		/// Gets or sets the (CSS) class name to find.
		/// </summary>
		/// <value>
		/// The class.
		/// </value>
		public string Class { get; set; }

		/// <summary>
		/// Gets or sets the element id to find.
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the zero-based index of the element to find, or -1 if not constrained by index.
		/// </summary>
		/// <value>
		/// The index.
		/// </value>
		public int Index { get; set; }

		/// <summary>
		/// Gets or sets the element name to find.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; }

        /// <summary>
        /// Gets tag name property trimmed and lower case.
        /// </summary>
        /// <value>The name of the normalized tag.</value>
        public string NormalizedTagName { get; private set; }

	    /// <summary>
	    /// Gets or sets the name of the tag for a custom control. This should only be used with HtmlCustom type element.
	    /// </summary>
	    /// <value>
	    /// The name of the tag.
	    /// </value>
	    public string TagName
	    {
	        get
	        {
	            return this.tagName;
	        }

	        set
	        {
	            this.tagName = value;
	            this.NormalizedTagName = value != null ? value.Trim().ToLowerInvariant() : null;
	        }
	    }

        /// <summary>
	    /// Gets or sets the (inner) text to find.
	    /// </summary>
	    /// <value>
	    /// The text.
	    /// </value>
	    public string Text { get; set; }

		/// <summary>
		/// Gets or sets the title to find.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the type of control this is.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the Url to find.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the value attribute to filter by.
		/// </summary>
		/// <value>The value attribute.</value>
		public string Value { get; set; }

        /// <summary>
        /// Gets or sets the CSS selector to use
        /// </summary>
        /// <value>The CSS selector.</value>
	    public string CssSelector { get; set; }

	    #endregion
	}
}