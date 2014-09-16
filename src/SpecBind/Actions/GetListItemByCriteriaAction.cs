// <copyright file="GetListItemByCriteriaAction.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Actions
{
    using System;
    using System.Linq;

    using SpecBind.ActionPipeline;
    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
    /// An action that gets an item in the list by some given criteria.
    /// </summary>
    public class GetListItemByCriteriaAction : ContextActionBase<GetListItemByCriteriaAction.ListItemByCriteriaContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetListItemByCriteriaAction"/> class.
        /// </summary>
        public GetListItemByCriteriaAction()
            : base(typeof(GetListItemByCriteriaAction).Name)
        {
        }
        
        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(ListItemByCriteriaContext context)
        {
            var propertyData = this.ElementLocator.GetProperty(context.PropertyName);

            if (!propertyData.IsList)
            {
                return ActionResult.Failure(new ElementExecuteException("Property '{0}' was found but is not a list element.", propertyData.Name));
            }

            var result = propertyData.FindItemInList(context.ValidationTable.Validations.ToList());

            if (result.Item1 != null)
            {
                return ActionResult.Successful(result.Item1);
            }

            var validationResult = result.Item2;
            return ActionResult.Failure(
                new ElementExecuteException(
                    "Retrieving item from list '{0}' failed, no items satisfied the rule checks.{1}List Item Count: {2}{1}Validation Details:{1}{3}",
                    propertyData.Name,
                    Environment.NewLine,
                    validationResult.ItemCount,
                    validationResult.GetComparisonTable()));
        }

        /// <summary>
        /// A context for the class.
        /// </summary>
        public class ListItemByCriteriaContext : ActionContext, IValidationTable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ActionContext" /> class.
            /// </summary>
            /// <param name="listName">Name of the list.</param>
            /// <param name="validationTable">The validation table.</param>
            public ListItemByCriteriaContext(string listName, ValidationTable validationTable)
                : base(listName)
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