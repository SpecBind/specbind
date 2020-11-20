// <copyright file="IPageElementHandler.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
    using System;
    using System.Collections.Generic;
    using SpecBind.Actions;

    /// <summary>
	/// An interface that manages page elements.
	/// </summary>
	/// <typeparam name="TElement">The type of the basic element on a page.</typeparam>
	public interface IPageElementHandler<in TElement> : IPage
    {
        /// <summary>
        /// Elements the enabled check.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element is enabled.</returns>
        bool ElementEnabledCheck(TElement element);

        /// <summary>
        /// Gets the element exists check function.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// True if the element exists; otherwise false.
        /// </returns>
        bool ElementExistsCheck(TElement element);

        /// <summary>
        /// Gets the element not-exists check function.
        /// Unlike ElementExistsCheck, this doesn't let the web driver wait first for the element to exist.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// True if the element doesn't exist; otherwise false.
        /// </returns>
        bool ElementNotExistsCheck(TElement element);

        /// <summary>
        /// Gets the element attribute value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>The attribute's value.</returns>
        string GetElementAttributeValue(TElement element, string attributeName);

        /// <summary>
        /// Gets the element options for multi-select or list options.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The element's options if supported, otherwise <c>null</c>.</returns>
        IList<ComboBoxItem> GetElementOptions(TElement element);

        /// <summary>
        /// Gets the element text.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The element's text.</returns>
        string GetElementText(TElement element);

        /// <summary>
        /// Gets the page from element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The page interface.</returns>
        IPage GetPageFromElement(TElement element);

        /// <summary>
        /// Clicks the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// True if the click is successful.
        /// </returns>
        bool ClickElement(TElement element);

        /// <summary>
        /// Gets the clears method.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>
        ///  The function used to clear the data.
        /// </returns>
        Action<TElement> GetClearMethod(Type propertyType);

        /// <summary>
        /// Gets the page fill method.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>
        /// The function used to fill the data.
        /// </returns>
        Action<TElement, string> GetPageFillMethod(Type propertyType);

        /// <summary>
        /// Highlights the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
	    void Highlight(TElement element);

        /// <summary>
        /// Waits for element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="waitCondition">The wait condition.</param>
        /// <param name="timeout">The timeout to wait before failing.</param>
        /// <returns><c>true</c> if the condition is met, <c>false</c> otherwise.</returns>
        bool WaitForElement(TElement element, WaitConditions waitCondition, TimeSpan? timeout);
    }
}