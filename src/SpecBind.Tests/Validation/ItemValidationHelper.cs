// <copyright file="ItemValidationHelper.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Tests.Validation
{
    using SpecBind.Validation;

    /// <summary>
    /// A helper class that creates an item validations for testing.
    /// </summary>
    public static class ItemValidationHelper
    {
        /// <summary>
        /// Creates the equals validation.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparer">The comparer, uses equals by default.</param>
        /// <returns>A created validation item.</returns>
        public static ItemValidation Create(string fieldName, string value, IValidationComparer comparer = null)
        {
            return new ItemValidation(fieldName, "equals", value).Process(comparer);
        }

        /// <summary>
        /// Processes the specified validation.
        /// </summary>
        /// <param name="validation">The validation.</param>
        /// <param name="comparer">The optional comparer, uses equals by default.</param>
        /// <returns>The configured validation, same object reference.</returns>
        public static ItemValidation Process(this ItemValidation validation, IValidationComparer comparer = null)
        {
            validation.FieldName = validation.RawFieldName;
            validation.ComparisonValue = validation.RawComparisonValue;
            validation.Comparer = new EqualsComparer();

            return validation;
        }

        /// <summary>
        /// Processes the specified validation.
        /// </summary>
        /// <param name="table">The validation table.</param>
        /// <param name="comparer">The optional comparer, uses equals by default.</param>
        /// <returns>The configured table, same object reference.</returns>
        public static ValidationTable Process(this ValidationTable table, IValidationComparer comparer = null)
        {
            foreach (var validation in table.Validations)
            {
                validation.Process(comparer);
            }

            return table;
        }
    }
}