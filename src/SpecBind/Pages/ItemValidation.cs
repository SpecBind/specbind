// <copyright file="ItemValidation.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
	using System;

	/// <summary>
	/// An item validation class that holds the data for a field.
	/// </summary>
	public class ItemValidation
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ItemValidation" /> class.
		/// </summary>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="comparisonValue">The comparison value.</param>
		/// <param name="comparisonType">Type of the comparison.</param>
		public ItemValidation(string fieldName, string comparisonValue, ComparisonType comparisonType)
		{
			this.FieldName = fieldName;
			this.ComparisonValue = comparisonValue;
			this.ComparisonType = comparisonType;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the type of the comparison.
		/// </summary>
		/// <value>
		/// The type of the comparison.
		/// </value>
		public ComparisonType ComparisonType { get; private set; }

		/// <summary>
		/// Gets the comparison value.
		/// </summary>
		/// <value>
		/// The comparison value.
		/// </value>
		public string ComparisonValue { get; private set; }

		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		/// <value>
		/// The name of the field.
		/// </value>
		public string FieldName { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Compares the specified <see paramref="actualValue"/> to the property data <see cref="ComparisonValue"/>.
		/// </summary>
		/// <param name="propertyData">The property data.</param>
		/// <param name="actualValue">The comparison value.</param>
		/// <returns><c>true</c> if the values match; otherwise <c>false</c>.</returns>
		public bool Compare(IPropertyData propertyData, string actualValue)
		{
			switch (this.ComparisonType)
			{
				case ComparisonType.Exists:
					return (this.ComparisonValue != null && bool.Parse(this.ComparisonValue)) ? propertyData.CheckElementExists() : !propertyData.CheckElementExists();
					
				case ComparisonType.Enabled:
					return (this.ComparisonValue != null && bool.Parse(this.ComparisonValue)) ? propertyData.CheckElementEnabled() : !propertyData.CheckElementEnabled();
					
				case ComparisonType.DoesNotContain:
					return (actualValue != null) && !actualValue.Contains(this.ComparisonValue);

				case ComparisonType.Contains:
					return (actualValue != null) && actualValue.Contains(this.ComparisonValue);
					
				case ComparisonType.StartsWith:
					return (actualValue != null) && actualValue.StartsWith(this.ComparisonValue, StringComparison.InvariantCultureIgnoreCase);
					
				case ComparisonType.EndsWith:
					return (actualValue != null) && actualValue.EndsWith(this.ComparisonValue, StringComparison.InvariantCultureIgnoreCase);
					
				case ComparisonType.DoesNotEqual:
					return !string.Equals(this.ComparisonValue, actualValue, StringComparison.InvariantCultureIgnoreCase);
					
				default:
					return string.Equals(this.ComparisonValue, actualValue, StringComparison.InvariantCultureIgnoreCase);
			}
		}

		#endregion
	}
}