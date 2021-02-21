﻿// <copyright file="ValidationTablePreAction.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SpecBind.ActionPipeline;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// An action pipeline pre-action that performs any processing on validation tables.
    /// </summary>
    public class ValidationTablePreAction : IPreAction
    {
        private readonly IActionRepository actionRepository;
        private readonly ITokenManager tokenManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationTablePreAction" /> class.
        /// </summary>
        /// <param name="actionRepository">The action repository.</param>
        /// <param name="tokenManager">The token manager.</param>
        public ValidationTablePreAction(IActionRepository actionRepository, ITokenManager tokenManager)
        {
            this.actionRepository = actionRepository;
            this.tokenManager = tokenManager;
        }

        /// <summary>
        /// Performs the pre-execute action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="context">The action context.</param>
        public void PerformPreAction(IAction action, ActionContext context)
        {
            // ReSharper disable once UsePatternMatching
            var containsTable = context as IValidationTable;
            if (containsTable == null)
            {
                return;
            }

            var ruleLookups = this.GetRuleLookups();
            var validationTable = containsTable.ValidationTable;

            // Loop through all the validations to process them
            foreach (var itemValidation in validationTable.Validations)
            {
                // Lookup a comparison rule based on the input string.
                var ruleLookupKey = ProcessText(itemValidation.RawComparisonType);

                if (!ruleLookups.TryGetValue(ruleLookupKey, out var comparer))
                {
                    throw new ElementExecuteException("Vaidation Rule could not be found for rule name: {0}", itemValidation.RawComparisonType);
                }

                itemValidation.Comparer = comparer;

                // Process the value for any tokens, then check for any transforms.
                var compareValue = this.tokenManager.GetToken(itemValidation.RawComparisonValue);
                itemValidation.ComparisonValue = compareValue;

                // Process the field name
                itemValidation.FieldName = ProcessText(itemValidation.RawFieldName);
            }
        }

        /// <summary>
        /// Processes the text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The processes text value.</returns>
        private static string ProcessText(string value)
        {
            return value.ToLookupKey();
        }

        /// <summary>
        /// Gets the rule lookups.
        /// </summary>
        /// <returns>A dictionary of lookups.</returns>
        private IDictionary<string, IValidationComparer> GetRuleLookups()
        {
            var lookups = new Dictionary<string, IValidationComparer>(StringComparer.InvariantCultureIgnoreCase);
            var ruleLookups = this.actionRepository.GetComparisonTypes();

            foreach (var lookup in ruleLookups)
            {
                foreach (var ruleKey in lookup.RuleKeys.Where(ruleKey => !lookups.ContainsKey(ruleKey)))
                {
                    lookups.Add(ruleKey, lookup);
                }
            }

            return lookups;
        }
    }
}