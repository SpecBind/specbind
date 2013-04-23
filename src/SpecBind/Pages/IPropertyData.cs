// <copyright file="IPropertyData.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// The property data interface for interaction.
	/// </summary>
	public interface IPropertyData
	{
		#region Public Properties

		/// <summary>
		///     Gets or sets a value indicating whether this instance represents a page element.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is a page element; otherwise, <c>false</c>.
		/// </value>
		bool IsElement { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is a list.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is a list; otherwise, <c>false</c>.
		/// </value>
		bool IsList { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		string Name { get; set; }

		/// <summary>
		///     Gets or sets the type of the property.
		/// </summary>
		/// <value>
		///     The type of the property.
		/// </value>
		Type PropertyType { get; set; }

		#endregion

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
		/// Fills the data on the element.
		/// </summary>
		/// <param name="data">The data.</param>
		void FillData(string data);

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
		/// <returns>The list of validations.</returns>
		bool ValidateList(ComparisonType compareType, ICollection<ItemValidation> validations);
	}
}