// <copyright file="ValidateTableHelpers.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Validation
{
    using System;
    using System.Collections.Generic;

    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// A static class that performs the validation process.
    /// </summary>
    internal static class ValidateTableHelpers
    {
        /// <summary>
        /// Performs the validation.
        /// </summary>
        /// <param name="validations">The validations.</param>
        /// <param name="validationProcess">The validation process for an item.</param>
        /// <returns>The result of the action.</returns>
        public static ActionResult PerformValidation(IReadOnlyCollection<ItemValidation> validations, Func<ItemValidation, ValidationItemResult, bool> validationProcess)
        {
            var itemResult = new ValidationItemResult();
            var result = new ValidationResult(validations) { IsValid = true };
            result.CheckedItems.Add(itemResult);

            foreach (var validation in validations)
            {
                var successful = validationProcess(validation, itemResult);
                if (!successful)
                {
                    result.IsValid = false;
                }
            }

            if (!result.IsValid)
            {
                return ActionResult.Failure(new ElementExecuteException(
                    "Value comparison(s) failed. See details for validation results.{0}{1}",
                    Environment.NewLine,
                    result.GetComparisonTableByRule()));
            }

            return ActionResult.Successful();
        }
    }
}