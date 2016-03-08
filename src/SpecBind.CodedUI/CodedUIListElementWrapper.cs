// <copyright file="CodedUIListElementWrapper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Microsoft.VisualStudio.TestTools.UITesting;
	using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

	using SpecBind.BrowserSupport;
	using SpecBind.Pages;

	/// <summary>
	/// An implementation of <see cref="ListElementWrapper{TElement,TChildElement}"/> for Coded UI.
	/// </summary>
	/// <typeparam name="TElement">The type of the element.</typeparam>
	/// <typeparam name="TChildElement">The type of the child element.</typeparam>
	// ReSharper disable once InconsistentNaming
	public class CodedUIListElementWrapper<TElement, TChildElement> : ListElementWrapper<TElement, TChildElement>
		where TElement : UITestControl
		where TChildElement : HtmlControl
	{
		private readonly Func<TElement, IBrowser, Action<HtmlControl>, TChildElement> builderFunc;
        private readonly object lockObject;

        private List<TChildElement> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodedUIListElementWrapper{TElement, TChildElement}" /> class.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="webBrowser">The browser.</param>
		public CodedUIListElementWrapper(TElement parentElement, IBrowser webBrowser)
            : base(parentElement, webBrowser)
		{
            this.builderFunc = PageBuilder<TElement, TChildElement>.CreateElement(typeof(TChildElement));
            this.lockObject = new object();

			this.ValidateElementExists = true;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to validate the element exists.
		/// </summary>
		/// <value>
		/// <c>true</c> if the class should validate element exists; otherwise, <c>false</c>.
		/// </value>
		public bool ValidateElementExists { get; set; }

        /// <summary>
        /// Creates the element.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="index">The index.</param>
        /// <returns>The newly created element.</returns>
	    protected override TChildElement CreateElement(IBrowser browser, TElement parentElement, int index)
		{
		    var elementList = this.CreateElementList(parentElement, browser);

		    return (index > 0 && index <= elementList.Count) ? elementList[index - 1] : default(TChildElement);
		}

		/// <summary>
		/// Elements the exists.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="expectedIndex">The expected index.</param>
		/// <returns><c>true</c> if the element exists.</returns>
		protected override bool ElementExists(TChildElement element, int expectedIndex)
		{
            if (!this.ValidateElementExists)
			{
				return true;
			}

		    var rowElement = element as HtmlRow;
			if (rowElement != null && (!rowElement.Exists || rowElement.RowIndex != expectedIndex))
			{
				return false;
			}

			return element.Exists;
		}

        /// <summary>
        /// Fetches the element list.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="browser">The browser.</param>
        /// <returns>The list of child elements.</returns>
        protected virtual List<TChildElement> FetchElementList(TElement parentElement, IBrowser browser)
        {
            var templateItem = this.builderFunc(parentElement, browser, null);
            var childList = templateItem.FindMatchingControls();

            var itemCollection =
                childList.Select((control, index) => this.CreateChildProxyElement(parentElement, control, browser));

            return typeof(HtmlRow).IsAssignableFrom(typeof(TChildElement))
                       ? itemCollection.OfType<HtmlRow>().OrderBy(r => r.RowIndex).Cast<TChildElement>().ToList()
                       : itemCollection.ToList();
        }

        /// <summary>
	    /// Assigns the filter properties.
	    /// </summary>
	    /// <param name="element">The element.</param>
	    private static void ClearSearchProperties(UITestControl element)
		{
            // Clear any search and filter properties for the wrapper element.
            element.SearchProperties.Clear();
            element.FilterProperties.Clear();
		}

        /// <summary>
        /// Creates the child proxy element.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="control">The control.</param>
        /// <param name="browser">The browser.</param>
        /// <returns>The created child element.</returns>
	    private TChildElement CreateChildProxyElement(TElement parentElement, UITestControl control, IBrowser browser)
        {
            var childElement =  this.builderFunc(parentElement, browser, ClearSearchProperties);

            childElement.CopyFrom(control);

            return childElement;
        }

        /// <summary>
        /// Creates the element list.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="browser">The browser.</param>
        /// <returns>The list of child elements.</returns>
	    private List<TChildElement> CreateElementList(TElement parentElement, IBrowser browser)
	    {
	        lock (this.lockObject)
	        {
	            if (this.items == null)
	            {
                    try
	                {
                        this.items = this.FetchElementList(parentElement, browser);
	                }
	                catch (Exception)
	                {
                        this.items = new List<TChildElement>(0);
	                }
	            }

	            return this.items;
	        }
	    }
	}
}