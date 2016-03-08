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
        /// Gets a value indicating whether this validation requires a field value.
        /// </summary>
        /// <value><c>true</c> if a field value is required; otherwise, <c>false</c>.</value>
        public virtual bool RequiresFieldValue
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the rule keys.
        /// </summary>
        /// <value>The rule keys.</value>
        public IEnumerable<string> RuleKeys { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the element should be checked for existence.
        /// </summary>
        /// <value><c>true</c> if the element should be checked; otherwise, <c>false</c>.</value>
        public virtual bool ShouldCheckElementExistence
        {
            get
            {
                return true;
            }
        }


        /// <summary>
        /// Compares the values using the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <returns><c>true</c> if the comparison passes, <c>false</c> otherwise.</returns>
        public abstract bool Compare(IPropertyData property, string expectedValue, string actualValue);

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("Validation: {0}", string.Join(",", this.RuleKeys));
        }
    }
}