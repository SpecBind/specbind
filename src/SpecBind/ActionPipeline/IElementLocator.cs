// <copyright file="IElementLocator.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>


namespace SpecBind.ActionPipeline
{
	using SpecBind.Pages;

	/// <summary>
	/// A locator for elements to return a given item based on the current context.
	/// </summary>
	public interface IElementLocator
	{
		/// <summary>
		/// Gets the element from the context.
		/// </summary>
		/// <param name="propertyName">The property name to locate.</param>
		/// <returns>The resulting property data.</returns>
		/// <exception cref="ElementExecuteException">Thrown when the element could not be found.</exception>
		IPropertyData GetElement(string propertyName);

		/// <summary>
		/// Gets the property from the context.
		/// </summary>
		/// <param name="propertyName">The property name to locate.</param>
		/// <returns>The resulting property data.</returns>
		/// <exception cref="ElementExecuteException">Thrown when the element could not be found.</exception>
		IPropertyData GetProperty(string propertyName);

		/// <summary>
		/// Tries the get the element.
		/// </summary>
		/// <param name="propertyName">The key.</param>
		/// <param name="propertyData">The property data.</param>
		/// <returns><c>true</c> if the element exists; otherwise <c>false</c>.</returns>
		bool TryGetElement(string propertyName, out IPropertyData propertyData);

		/// <summary>
		/// Tries the get the property on the page.
		/// </summary>
		/// <param name="propertyName">The key.</param>
		/// <param name="propertyData">The property data.</param>
		/// <returns><c>true</c> if the element exists; otherwise <c>false</c>.</returns>
		bool TryGetProperty(string propertyName, out IPropertyData propertyData);
	}
}