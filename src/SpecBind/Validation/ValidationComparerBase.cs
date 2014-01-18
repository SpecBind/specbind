// <copyright file="ValidationComparerBase.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Validation
{
    using System.Collections.Generic;

    using SpecBind.Pages;

    /// <summary>
    /// A global base class for all validation comparers.
    /// </summary>
    public abstract class ValidationComparerBase : IValidationComparer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationComparerBase"/> class.
        /// </summary>
        /// <param name="ruleKeys">The rule keys.</param>
        protected ValidationComparerBase(params string[] ruleKeys)
        {
            this.RuleKeys = ruleKeys;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is the default rule to use.
        /// </summary>
        /// <value><c>true</c> if this instance is a default rule; otherwise, <c>false</c>.</value>
        public virtual bool IsDefault
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the rule keys.
        /// </summary>
        /// <value>The rule keys.</value>
        public IEnumerable<string> RuleKeys { get; private set; }

        /// <summary>
        /// Compares the values using the specificed property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <returns><c>true</c> if the comparison passes, <c>false</c> otherwise.</returns>
        public abstract bool Compare(IPropertyData property, string expectedValue, string actualValue);
    }
}