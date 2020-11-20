// <copyright file="ValidateComboBoxAction.cs">
//    Copyright © 2015 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Actions
{
    using System.Collections.Generic;
    using System.Linq;
    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// An action that validates a combo box contains given items.
    /// </summary>
    public class ValidateComboBoxAction : ValidateActionBase<ValidateComboBoxAction.ValidateComboBoxContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateComboBoxAction"/> class.
        /// </summary>
        public ValidateComboBoxAction()
            : base(typeof(ValidateComboBoxAction).Name)
        {
        }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(ValidateComboBoxContext actionContext)
        {
            var propertyData = this.ElementLocator.GetElement(actionContext.PropertyName);

            var actualItems = propertyData.GetComboBoxItems();
            if (actualItems == null)
            {
                return ActionResult.Failure(
                    new ElementExecuteException(
                        "Property '{0}' was found but is not a combo box element.",
                        propertyData.Name));
            }

            var comparisonType = actionContext.ComparisonType;
            var expectedItems = actionContext.Items;
            var failedItems = new List<ComboBoxItem>();
            foreach (var expectedItem in expectedItems)
            {
                var item = actualItems.FirstOrDefault(a =>
                    (!actionContext.MatchName || string.Equals(a.Text, expectedItem.Text)) &&
                    (!actionContext.MatchValue || string.Equals(a.Value, expectedItem.Value)));

                switch (comparisonType)
                {
                    case ComboComparisonType.Contains:
                    case ComboComparisonType.ContainsExactly:
                        if (item == null)
                        {
                            failedItems.Add(expectedItem);
                        }

                        break;
                    case ComboComparisonType.DoesNotContain:
                        if (item != null)
                        {
                            failedItems.Add(expectedItem);
                        }

                        break;
                }
            }

            if (failedItems.Count == 0 && (comparisonType != ComboComparisonType.ContainsExactly || expectedItems.Count == actualItems.Count))
            {
                return ActionResult.Successful();
            }

            if (failedItems.Count > 0)
            {
                return ActionResult.Failure(new ElementExecuteException(
                        "Combo box validation of field '{0}' failed. Expected items that {1}: {2}",
                        propertyData.Name,
                        comparisonType == ComboComparisonType.DoesNotContain ? "exists but should not have" : "do not exist",
                        string.Join(",", failedItems.Select(a => a.Text).OrderBy(s => s).ToArray())));
            }

            return ActionResult.Failure(new ElementExecuteException(
                        "Combo box exact match validation of field '{0}' failed. Expected Items: {1}; Actual Items: {2}",
                        propertyData.Name,
                        string.Join(",", expectedItems.Select(a => a.Text).OrderBy(s => s).ToArray()),
                        string.Join(",", actualItems.Select(a => a.Text).OrderBy(s => s).ToArray())));
        }

        /// <summary>
        /// A context for combo box validation.
        /// </summary>
        public class ValidateComboBoxContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValidateComboBoxContext" /> class.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="compareType">Type of the compare.</param>
            /// <param name="items">The items to validate.</param>
            /// <param name="matchName">Indicates if name matching should occur.</param>
            /// <param name="matchValue">Indicates if value matching should occur.</param>
            public ValidateComboBoxContext(string propertyName, ComboComparisonType compareType, List<ComboBoxItem> items, bool matchName, bool matchValue)
                : base(propertyName)
            {
                this.ComparisonType = compareType;
                this.Items = items;
                this.MatchName = matchName;
                this.MatchValue = matchValue;
            }

            /// <summary>
            /// Gets the comparison type for the combo box
            /// </summary>
            public ComboComparisonType ComparisonType { get; }

            /// <summary>
            /// Gets the list of items to validate
            /// </summary>
            public List<ComboBoxItem> Items { get; }

            /// <summary>
            /// Gets a value indicating whether the name column should be matched.
            /// </summary>
            public bool MatchName { get; }

            /// <summary>
            /// Gets a value indicating whether the value column should be matched.
            /// </summary>
            public bool MatchValue { get; }
        }
    }
}