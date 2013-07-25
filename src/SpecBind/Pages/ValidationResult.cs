// <copyright file="ValidationResult.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using SpecBind.Helpers;

	/// <summary>
	/// Tracks the individual validations on the list to create a trace in the completed exception.
	/// </summary>
	public class ValidationResult
	{
		private readonly IEnumerable<ItemValidation> validations;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationResult"/> class.
		/// </summary>
		internal ValidationResult(IEnumerable<ItemValidation> validations)
		{
			this.validations = validations;
			CheckedItems = new List<ValidationItemResult>();
		}

		/// <summary>
		/// Gets a value indicating whether this instance is valid.
		/// </summary>
		/// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
		public bool IsValid { get; internal set; }

		/// <summary>
		/// Gets the checked items.
		/// </summary>
		/// <value>The checked items.</value>
		internal List<ValidationItemResult> CheckedItems { get; private set; }

		/// <summary>
		/// Gets or sets the item count.
		/// </summary>
		/// <value>The item count.</value>
		internal int ItemCount { get; set; }
		

		/// <summary>
		/// Gets the compairson table.
		/// </summary>
		/// <returns>The formatted comparison table.</returns>
		internal string GetCompairsonTable()
		{
			var tableFormatter = new TableFormater<ValidationItemResult>();

			foreach (var itemValidation in this.validations)
			{
				tableFormatter.AddColumn(
					itemValidation.ToString(),
					i => i.PropertyResults.First(p => p.Validation == itemValidation),
					f => f.ActualValue);
			}

			return tableFormatter.CreateTable(CheckedItems);
		}

		/// <summary>
		/// Gets the compairson table displayed by rule.
		/// </summary>
		/// <returns>The formatted table.</returns>
		internal string GetCompairsonTableByRule()
		{
			if (CheckedItems.Count != 1)
			{
				throw new InvalidOperationException("Only one checked item can exist to process items by rule.");
			}


			var properties = CheckedItems.First().PropertyResults;
			var tableFormatter = new TableFormater<ValidationItemResult.PropertyResult>()
										.AddColumn("Field", p => p, p => p.Validation.FieldName, CheckFieldExists)
										.AddColumn("Rule", p => p.Validation.ComparisonType, p => p.ToString())
										.AddColumn("Value", p => p, p => p.Validation.ComparisonValue, CheckFieldValue);

			return tableFormatter.CreateTable(properties);
		}

		/// <summary>
		/// Checks the field exists.
		/// </summary>
		/// <param name="propertyResult">The property result.</param>
		/// <returns>Tuple{System.BooleanSystem.String}.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		private static Tuple<bool, string> CheckFieldExists(ValidationItemResult.PropertyResult propertyResult)
		{
			return propertyResult.FieldExists ? null : new Tuple<bool, string>(false, "Not Found");
		}

		/// <summary>
		/// Checks the field value.
		/// </summary>
		/// <param name="propertyResult">The property result.</param>
		/// <returns>The validation result.</returns>
		private static Tuple<bool, string> CheckFieldValue(ValidationItemResult.PropertyResult propertyResult)
		{
			return propertyResult.FieldExists ? new Tuple<bool, string>(propertyResult.IsValid, propertyResult.ActualValue) : null;
		}
	}
}