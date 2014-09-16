// <copyright file="ValidationTableExtensions.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Helpers
{
    using System.Linq;

    using SpecBind.Pages;
    using SpecBind.Validation;

    using TechTalk.SpecFlow;

    /// <summary>
    /// A set of extension methods that build a validation table from a SpecBind table.
    /// </summary>
    public static class ValidationTableExtensions
    {
        /// <summary>
        /// Converts the SpecFlow table to a validation table. This expects a field, rule, and value column.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>The created validation table.</returns>
        /// <exception cref="ElementExecuteException">A table must be specified for this step with the columns 'Field', 'Rule' and 'Value'</exception>
        public static ValidationTable ToValidationTable(this Table table)
        {
            string fieldHeader = null;
            string valueHeader = null;
            string ruleHeader = null;

            if (table != null)
            {
                fieldHeader = table.Header.FirstOrDefault(h => h.NormalizedEquals("Field"));
                valueHeader = table.Header.FirstOrDefault(h => h.NormalizedEquals("Value"));
                ruleHeader = table.Header.FirstOrDefault(h => h.NormalizedEquals("Rule"));
            }

            if (fieldHeader == null || valueHeader == null || ruleHeader == null)
            {
                throw new ElementExecuteException("A table must be specified for this step with the columns 'Field', 'Rule' and 'Value'");
            }

            if (table.RowCount == 0)
            {
                throw new ElementExecuteException("The validation table must contain at least one validation row.");
            }

            var validationTable = new ValidationTable();

            foreach (var tableRow in table.Rows)
			{
				var fieldName = tableRow[fieldHeader];
				var comparisonValue = tableRow[valueHeader];
				var ruleValue = tableRow[ruleHeader];

                validationTable.AddValidation(fieldName, ruleValue, comparisonValue);
			}

            return validationTable;
        }
    }
}