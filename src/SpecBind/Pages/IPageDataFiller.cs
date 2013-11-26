// <copyright file="IPageDataFiller.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
	using System.Collections.Generic;

	/// <summary>
	/// An interface that represents the page data filler.
	/// </summary>
	public interface IPageDataFiller
	{
		/// <summary>
		/// Gets the list item as a child page.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="itemNumber">The item number.</param>
		/// <returns>
		/// The child item as a page.
		/// </returns>
		/// <exception cref="ElementExecuteException">The item could not be found.</exception>
		IPage GetListItem(IPage page, string fieldName, int itemNumber);

	    /// <summary>
		/// Validates the item.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="validations">The validations.</param>
		void ValidateItem(IPage page, ICollection<ItemValidation> validations);

		/// <summary>
		/// Validates that the given element exists on the page and is enabled.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="shouldExist">if set to <c>true</c> the element should exist; if <c>false</c> it should not.</param>
		void ValidateEnabled(IPage page, string fieldName, bool shouldExist);

		/// <summary>
		/// Validates that the given element exists on the page.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="shouldExist">if set to <c>true</c> the element should exist; if <c>false</c> it should not.</param>
		void ValidateExists(IPage page, string fieldName, bool shouldExist);

		/// <summary>
		/// Validates the list of item follows the specified values.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the item.</param>
		/// <param name="compareType">Type of the compare.</param>
		/// <param name="validations">The validations.</param>
		/// <exception cref="ElementExecuteException">One or more comparisons failed.</exception>
		void ValidateList(IPage page, string fieldName, ComparisonType compareType, ICollection<ItemValidation> validations);
	}
}