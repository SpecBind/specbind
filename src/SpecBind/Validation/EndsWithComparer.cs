namespace SpecBind.Validation
{
    using System;

    using SpecBind.Pages;

    /// <summary>
    /// A string comparison class for a ends with comparison
    /// </summary>
    public class EndsWithComparer : ValidationComparerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndsWithComparer"/> class.
        /// </summary>
        public EndsWithComparer()
            : base("endswith")
        {
        }

        /// <summary>
        /// Compares the values using the specificed property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <returns><c>true</c> if the comparison passes, <c>false</c> otherwise.</returns>
        public override bool Compare(IPropertyData property, string expectedValue, string actualValue)
        {
            return (actualValue != null) && actualValue.EndsWith(expectedValue, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}