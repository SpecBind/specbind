// <copyright file="ValidateListAction.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System;
    using System.Collections.Generic;

    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// An action that validates a list of items for specific actions.
    /// </summary>
    public class ValidateListAction : ContextActionBase<ValidateListAction.ValidateListContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateListAction"/> class.
        /// </summary>
        public ValidateListAction()
            : base(typeof(ValidateListAction).Name)
        {
        }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(ValidateListContext actionContext)
        {
            var propertyData = this.ElementLocator.GetProperty(actionContext.PropertyName);

            if (!propertyData.IsList)
            {
                return ActionResult.Failure(
                        new ElementExecuteException(
                            "Property '{0}' was found but is not a list element.",
                            propertyData.Name));
            }

            var validationResult = propertyData.ValidateList(actionContext.CompareType, actionContext.Validations);
            if (validationResult.IsValid)
            {
                return ActionResult.Successful();
            }

            return ActionResult.Failure(
                new ElementExecuteException(
                    "List validation of field '{0}' failed, no items satisfied the rule checks.{1}List Item Count: {2}{1}Validation Details:{1}{3}",
                    propertyData.Name,
                    Environment.NewLine,
                    validationResult.ItemCount,
                    validationResult.GetComparisonTable()));
        }

        /// <summary>
        /// A context for list validation.
        /// </summary>
        public class ValidateListContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValidateListContext"/> class.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="compareType">Type of the compare.</param>
            /// <param name="validations">The validations.</param>
            public ValidateListContext(string propertyName, ComparisonType compareType, ICollection<ItemValidation> validations)
                : base(propertyName)
            {
                this.CompareType = compareType;
                this.Validations = validations;
            }

            /// <summary>
            /// Gets the type of the compare.
            /// </summary>
            /// <value>The type of the compare.</value>
            public ComparisonType CompareType { get; private set; }
                
            /// <summary>
            /// Gets the validations.
            /// </summary>
            /// <value>The validations.</value>
            public ICollection<ItemValidation> Validations { get; private set; }
        }
    }
}