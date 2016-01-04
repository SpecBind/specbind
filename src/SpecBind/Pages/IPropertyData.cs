// <copyright file="IPropertyData.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
	using System;
	using System.Collections.Generic;

	using SpecBind.Actions;
	using SpecBind.Validation;

    /// <summary>
	/// The property data interface for interaction.
	/// </summary>
	public interface IPropertyData
	{
		#region Public Properties

		/// <summary>
		///     Gets a value indicating whether this instance represents a page element.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is a page element; otherwise, <c>false</c>.
		/// </value>
		bool IsElement { get; }

		/// <summary>
		/// Gets a value indicating whether this instance is a list.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is a list; otherwise, <c>false</c>.
		/// </value>
		bool IsList { get; }

		/// <summary>
		/// Gets the name if the property.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		string Name { get; }

		/// <summary>
		///     Gets the type of the property.
		/// </summary>
		/// <value>
		///     The type of the property.
		/// </value>
		Type PropertyType { get; }

		#endregion

		/// <summary>
		/// Clears the data on the element.
		/// </summary>
		void ClearData();

		/// <summary>
		/// Clicks the element that this property represents.
		/// </summary>
		void ClickElement();

		/// <summary>
		/// Checks to see if the element exists.
		/// </summary>
		/// <returns><c>true</c> if the element exists.</returns>
		bool CheckElementEnabled();

		/// <summary>
		/// Checks to see if the element exists.
		/// </summary>
		/// <returns><c>true</c> if the element exists.</returns>
		bool CheckElementExists();

        /// <summary>
        /// Checks to see if the element doesn't exist.
        /// Unlike ELementExistsCheck, this doesn't let the web driver wait first for the element to exist.
        /// </summary>
        /// <returns><c>true</c> if the element does not exist.</returns>
        bool CheckElementNotExists();

        /// <summary>
		/// Fills the data on the element.
		/// </summary>
		/// <param name="data">The data.</param>
		void FillData(string data);

		/// <summary>
		/// Gets the current value of the property.
		/// </summary>
		/// <returns>The current value as a string.</returns>
		string GetCurrentValue();

		/// <summary>
		/// Gets the index of the item at.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The item as an <see cref="IPage"/> item; otherwise <c>null</c>.</returns>
		IPage GetItemAtIndex(int index);

		/// <summary>
		/// Gets the item as page.
		/// </summary>
		/// <returns>The item as a page.</returns>
		IPage GetItemAsPage();

        /// <summary>
        /// Highlights this instance.
        /// </summary>
        void Highlight();

		/// <summary>
		/// Validates the item or property matches the expected expression.
		/// </summary>
		/// <param name="validation">The validation item.</param>
		/// <param name="actualValue">The actual value if validation fails.</param>
		/// <returns>
		///   <c>true</c> if the items are valid; otherwise <c>false</c>.
		/// </returns>
		bool ValidateItem(ItemValidation validation, out string actualValue);

		/// <summary>
		/// Validates the list.
		/// </summary>
		/// <param name="compareType">Type of the compare.</param>
		/// <param name="validations">The validations.</param>
		/// <returns>The validation result including checks performed.</returns>
		ValidationResult ValidateList(ComparisonType compareType, ICollection<ItemValidation> validations);

        /// <summary>
        /// Validates the list row count.
        /// </summary>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <param name="expectedRowCount">The expected row count.</param>
        /// <returns>A tuple indicating if the results were successful and the actual row count.</returns>
        Tuple<bool, int> ValidateListRowCount(NumericComparisonType comparisonType, int expectedRowCount);

        /// <summary>
        /// Waits for element condition.
        /// </summary>
        /// <param name="waitCondition">The wait condition.</param>
        /// <param name="timeout">The timeout to wait before failing.</param>
        /// <returns><c>true</c> if the condition is met, <c>false</c> otherwise.</returns>
        bool WaitForElementCondition(WaitConditions waitCondition, TimeSpan? timeout);

        /// <summary>
        /// Validates the list.
        /// </summary>
        /// <param name="validations">The validations.</param>
        /// <returns>The validation result including checks performed.</returns>
        Tuple<IPage, ValidationResult> FindItemInList(ICollection<ItemValidation> validations);
	}
}