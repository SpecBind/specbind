// <copyright file="ValidationItemResult.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
    using System.Collections.Generic;

    using SpecBind.Validation;

    /// <summary>
	/// Represents an individual item's validation results.
	/// </summary>
	public class ValidationItemResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationItemResult"/> class.
        /// </summary>
        public ValidationItemResult()
        {
            this.PropertyResults = new List<PropertyResult>();
        }

        /// <summary>
        /// Gets the property results.
        /// </summary>
        /// <value>The property results.</value>
        public List<PropertyResult> PropertyResults { get; private set; }

        /// <summary>
        /// Notes the missing property.
        /// </summary>
        /// <param name="itemValidation">The item validation.</param>
        internal void NoteMissingProperty(ItemValidation itemValidation)
        {
            this.PropertyResults.Add(new PropertyResult(itemValidation));
        }

        /// <summary>
        /// Notes the validation result.
        /// </summary>
        /// <param name="itemValidation">The item validation.</param>
        /// <param name="successful">if set to <c>true</c> the validation was successful.</param>
        /// <param name="actualValue">The actual value.</param>
        internal void NoteValidationResult(ItemValidation itemValidation, bool successful, string actualValue)
        {
            this.PropertyResults.Add(new PropertyResult(itemValidation, successful, actualValue));
        }

        /// <summary>
        /// The result of an individual validation result.
        /// </summary>
        public class PropertyResult
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PropertyResult"/> class.
            /// </summary>
            /// <param name="validation">The validation.</param>
            internal PropertyResult(ItemValidation validation)
            {
                this.Validation = validation;
                this.IsValid = false;
                this.FieldExists = false;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="PropertyResult" /> class.
            /// </summary>
            /// <param name="itemValidation">The item validation.</param>
            /// <param name="successful">if set to <c>true</c> [successful].</param>
            /// <param name="actualValue">The actual value.</param>
            internal PropertyResult(ItemValidation itemValidation, bool successful, string actualValue)
            {
                this.FieldExists = true;
                this.Validation = itemValidation;
                this.IsValid = successful;
                this.ActualValue = actualValue;
            }

            /// <summary>
            /// Gets the actual value.
            /// </summary>
            /// <value>The actual value.</value>
            public string ActualValue { get; private set; }

            /// <summary>
            /// Gets a value indicating whether the field exists.
            /// </summary>
            /// <value><c>true</c> if the field exists; otherwise, <c>false</c>.</value>
            public bool FieldExists { get; private set; }

            /// <summary>
            /// Gets the validation.
            /// </summary>
            /// <value>The validation.</value>
            public ItemValidation Validation { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this instance is valid.
            /// </summary>
            /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
            public bool IsValid { get; private set; }
        }
    }
}