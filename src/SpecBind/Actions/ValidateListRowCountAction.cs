// <copyright file="ValidateListRowCountAction.cs">
//    Copyright © 2015 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Actions
{
    using System;
    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// An action that validates a list of items for specific actions.
    /// </summary>
	public class ValidateListRowCountAction : ValidateActionBase<ValidateListRowCountAction.ValidateListRowCountContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateListRowCountAction"/> class.
        /// </summary>
        public ValidateListRowCountAction()
            : base(typeof(ValidateListRowCountAction).Name)
        {
        }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(ValidateListRowCountContext actionContext)
        {
            var propertyData = this.ElementLocator.GetProperty(actionContext.PropertyName);

            if (!propertyData.IsList)
            {
                return ActionResult.Failure(
                    new ElementExecuteException(
                        "Property '{0}' was found but is not a list element.",
                        propertyData.Name));
            }

            Tuple<bool, int> validationResult = null;

            this.DoValidate<IPropertyData>(propertyData, e =>
                {
                    validationResult = e.ValidateListRowCount(actionContext.CompareType, actionContext.RowCount);
                    return validationResult.Item1;
                });

            if (validationResult.Item1)
            {
                return ActionResult.Successful();
            }

            return ActionResult.Failure(
                    new ElementExecuteException(
                        "List count validation of field '{0}' failed. Expected Items: {1}, Actual Items: {2}",
                        propertyData.Name,
                        actionContext.RowCount,
                        validationResult.Item2));
        }

        /// <summary>
        /// A context for row count list validation.
        /// </summary>
        public class ValidateListRowCountContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValidateListRowCountContext" /> class.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="compareType">Type of the compare.</param>
            /// <param name="rowCount">The row count.</param>
            public ValidateListRowCountContext(string propertyName, NumericComparisonType compareType, int rowCount)
                : base(propertyName)
            {
                this.CompareType = compareType;
                this.RowCount = rowCount;
            }

            /// <summary>
            /// Gets the type of the compare.
            /// </summary>
            /// <value>The type of the compare.</value>
            public NumericComparisonType CompareType { get; private set; }

            /// <summary>
            /// Gets the row count.
            /// </summary>
            /// <value>The row count.</value>
            public int RowCount { get; private set; }
        }
    }
}