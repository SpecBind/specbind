// <copyright file="IPageElementHandler.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
	using System;

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
		/// Gets the click element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>
		/// True if the click is successful.
		/// </returns>
		bool ClickElement(TElement element);

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
	}
}