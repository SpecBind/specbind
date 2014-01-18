// <copyright file="IValidationComparer.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Validation
{
    using System.Collections.Generic;

    using SpecBind.Pages;

    /// <summary>
    /// An interface that is used to provide value comparisons.
    /// </summary>
    public interface IValidationComparer
    {
        /// <summary>
        /// Gets a value indicating whether this instance is the default rule to use.
        /// </summary>
        /// <value><c>true</c> if this instance is a default rule; otherwise, <c>false</c>.</value>
        bool IsDefault { get; }

        /// <summary>
        /// Gets the rule keys.
        /// </summary>
        /// <value>The rule keys.</value>
        IEnumerable<string> RuleKeys { get; }

        /// <summary>
        /// Compares the values using the specificed property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <returns><c>true</c> if the comparison passes, <c>false</c> otherwise.</returns>
        bool Compare(IPropertyData property, string expectedValue, string actualValue);
    }
}