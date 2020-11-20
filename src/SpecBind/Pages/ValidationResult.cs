// <copyright file="ValidationResult.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SpecBind.Helpers;
    using SpecBind.Validation;

    /// <summary>
	/// Tracks the individual validations on the list to create a trace in the completed exception.
	/// </summary>
	public class ValidationResult
    {
        private readonly IEnumerable<ItemValidation> validations;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult" /> class.
        /// </summary>
        /// <param name="validations">The validations being used.</param>
        internal ValidationResult(IEnumerable<ItemValidation> validations)
        {
            this.validations = validations;
            this.CheckedItems = new List<ValidationItemResult>();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid { get; internal set; }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <value>The item count.</value>
        public int ItemCount { get; internal set; }

        /// <summary>
        /// Gets the checked items.
        /// </summary>
        /// <value>The checked items.</value>
        internal List<ValidationItemResult> CheckedItems { get; private set; }

        /// <summary>
        /// Gets the comparison table.
        /// </summary>
        /// <returns>The formatted comparison table.</returns>
        internal string GetComparisonTable()
        {
            var tableFormatter = new TableFormater<ValidationItemResult>();

            foreach (var itemValidation in this.validations)
            {
                tableFormatter.AddColumn(
                    itemValidation.ToString(),
                    i => i.PropertyResults.First(p => p.Validation == itemValidation),
                    f => f.FieldExists ? f.ActualValue : "<NOT FOUND>");
            }

            return tableFormatter.CreateTable(this.CheckedItems);
        }

        /// <summary>
        /// Gets the comparison table displayed by rule.
        /// </summary>
        /// <returns>The formatted table.</returns>
        internal string GetComparisonTableByRule()
        {
            if (this.CheckedItems.Count != 1)
            {
                throw new InvalidOperationException("Only one checked item can exist to process items by rule.");
            }


            var properties = this.CheckedItems.First().PropertyResults;
            var tableFormatter = new TableFormater<ValidationItemResult.PropertyResult>()
                                        .AddColumn("Field", p => p, p => p.Validation.RawFieldName, CheckFieldExists)
                                        .AddColumn("Rule", p => p.Validation.RawComparisonType, p => p)
                                        .AddColumn("Value", p => p, p => p.Validation.RawComparisonValue, CheckFieldValue);

            return tableFormatter.CreateTable(properties);
        }

        /// <summary>
        /// Checks the field exists.
        /// </summary>
        /// <param name="propertyResult">The property result.</param>
        /// <returns>The validation result.</returns>
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