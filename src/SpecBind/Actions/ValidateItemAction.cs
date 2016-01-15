// <copyright file="ValidateItemAction.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.Pages;
    using SpecBind.Validation;

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
            return ValidateTableHelpers.PerformValidation(context.ValidationTable.Validations, this.ValidateProperty);
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="validation">The validation.</param>
        /// <param name="itemResult">The item result.</param>
        /// <returns><c>true</c> if the validation is successful, <c>false</c> otherwise.</returns>
        private bool ValidateProperty(ItemValidation validation, ValidationItemResult itemResult)
        {
            IPropertyData propertyData;
            if (!this.ElementLocator.TryGetProperty(validation.FieldName, out propertyData))
            {
                if (validation.Comparer is DoesNotExistComparer)
                {
                    itemResult.NoteValidationResult(validation, successful: true, actualValue: null);
                    return true;
                }

                itemResult.NoteMissingProperty(validation);
                return false;
            }

            string actualValue;
            var successful = propertyData.ValidateItem(validation, out actualValue);
            itemResult.NoteValidationResult(validation, successful, actualValue);
            return successful;
        }

        /// <summary>
        /// The data context for validating an item.
        /// </summary>
        public class ValidateItemContext : ActionContext, IValidationTable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValidateItemContext" /> class.
            /// </summary>
            /// <param name="validationTable">The validation table.</param>
            public ValidateItemContext(ValidationTable validationTable)
                : base(null)
            {
                this.ValidationTable = validationTable;
            }

            /// <summary>
            /// Gets the validation table.
            /// </summary>
            /// <value>The validation table.</value>
            public ValidationTable ValidationTable { get; private set; }
        }
    }
}