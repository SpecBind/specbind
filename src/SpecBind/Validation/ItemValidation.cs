// <copyright file="ItemValidation.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Validation
{
    using SpecBind.Pages;

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
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <param name="comparisonValue">The comparison value.</param>
        public ItemValidation(string fieldName, string comparisonType, string comparisonValue)
        {
            this.RawFieldName = fieldName != null ? fieldName.Trim() : null;
            this.RawComparisonType = comparisonType != null ? comparisonType.Trim() : null;
            this.RawComparisonValue = comparisonValue != null ? comparisonValue.Trim() : null;
        }

        #endregion

		#region Public Properties

        /// <summary>
        /// Gets a value indicating whether the item should be checked first for existence.
        /// </summary>
        /// <value><c>true</c> if the item should be checked; otherwise, <c>false</c>.</value>
        public bool CheckElementExistence
        {
            get
            {
                return this.Comparer.ShouldCheckElementExistence;
            }
        }

        /// <summary>
        /// Gets the comparer.
        /// </summary>
        /// <value>The comparer.</value>
        public IValidationComparer Comparer { get; internal set; }

        /// <summary>
		/// Gets the comparison value.
		/// </summary>
		/// <value>
		/// The comparison value.
		/// </value>
		public string ComparisonValue { get; internal set; }

		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		/// <value>
		/// The name of the field.
		/// </value>
		public string FieldName { get; internal set; }

        /// <summary>
        /// Gets the comparison type as it was originally in the table.
        /// </summary>
        /// <value>The comparison type of the raw field.</value>
	    public string RawComparisonType { get; private set; }

        /// <summary>
        /// Gets the comparison value as it was originally in the table.
        /// </summary>
        /// <value>The comparison value of the raw field.</value>
        public string RawComparisonValue { get; private set; }

        /// <summary>
        /// Gets the field name as it was originally in the table.
        /// </summary>
        /// <value>The name of the raw field.</value>
        public string RawFieldName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this validation requires a field value.
        /// </summary>
        /// <value><c>true</c> if a field value is required; otherwise, <c>false</c>.</value>
        public bool RequiresFieldValue
        {
            get
            {
                return this.Comparer.RequiresFieldValue;
            }
        }

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
		    return this.Comparer != null && this.Comparer.Compare(propertyData, this.ComparisonValue, actualValue);
		}

		#endregion

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString()
		{
			return string.Format("{0} {1} {2}", this.RawFieldName, this.RawComparisonType, this.RawComparisonValue ?? "<NULL>");
		}
	}
}