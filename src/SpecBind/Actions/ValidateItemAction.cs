// <copyright file="ValidateItemAction.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Actions
{
    using System;
    using System.Collections.Generic;

    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// An action that helps perform item validation.
    /// </summary>
    internal class ValidateItemAction : ContextActionBase<ValidateItemAction.ValidateItemContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateItemAction"/> class.
        /// </summary>
        public ValidateItemAction()
            : base(typeof(ValidateItemAction).Name)
        {
        }

        /// <summary>
        /// Executes the specified action based on the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(ValidateItemContext context)
        {
            var itemResult = new ValidationItemResult();
            var result = new ValidationResult(context.Validations) { IsValid = true };
            result.CheckedItems.Add(itemResult);

            foreach (var validation in context.Validations)
            {
                IPropertyData propertyData;
                if (!this.ElementLocator.TryGetProperty(validation.FieldName, out propertyData))
                {
                    itemResult.NoteMissingProperty(validation);
                    result.IsValid = false;
                    continue;
                }

                string actualValue;
                var successful = propertyData.ValidateItem(validation, out actualValue);
                itemResult.NoteValidationResult(validation, successful, actualValue);
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

        /// <summary>
        /// The data context for validating an item.
        /// </summary>
        public class ValidateItemContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValidateItemContext"/> class.
            /// </summary>
            /// <param name="validations">The validations.</param>
            public ValidateItemContext(ICollection<ItemValidation> validations)
                : base(null)
            {
                this.Validations = validations;
            }

            /// <summary>
            /// Gets the validations.
            /// </summary>
            /// <value>The validations.</value>
            public ICollection<ItemValidation> Validations { get; private set; }
        }
    }
}