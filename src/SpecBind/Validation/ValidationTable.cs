// <copyright file="ValidationTable.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Validation
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a raw table of validations to be parsed and turned into individual items.
    /// </summary>
    public class ValidationTable
    {
        private readonly List<ItemValidation> validations;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationTable"/> class.
        /// </summary>
        public ValidationTable()
        {
            this.validations = new List<ItemValidation>();
        }

        /// <summary>
        /// Gets the validation count.
        /// </summary>
        /// <value>The validation count.</value>
        public int ValidationCount
        {
            get
            {
                return this.validations.Count;
            }
        }

        /// <summary>
        /// Gets the validations.
        /// </summary>
        /// <value>The validations.</value>
        public IReadOnlyCollection<ItemValidation> Validations
        {
            get
            {
                return this.validations.AsReadOnly();
            }
        }

        /// <summary>
        /// Adds the validation to the table.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="ruleValue">The rule value.</param>
        /// <param name="comparisonValue">The comparison value.</param>
        public void AddValidation(string fieldName, string ruleValue, string comparisonValue)
        {
            this.validations.Add(new ItemValidation(fieldName, ruleValue, comparisonValue));
        }
    }
}