// <copyright file="ContainsComparer.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Validation
{
    using SpecBind.Pages;

    /// <summary>
    /// A string comparison class for a contains comparison
    /// </summary>
    public class ContainsComparer : ValidationComparerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainsComparer"/> class.
        /// </summary>
        public ContainsComparer()
            : base("contains")
        {
        }

        /// <summary>
        /// Compares the values using the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <returns><c>true</c> if the comparison passes, <c>false</c> otherwise.</returns>
        public override bool Compare(IPropertyData property, string expectedValue, string actualValue)
        {
            return (actualValue != null) && actualValue.Contains(expectedValue);
        }
    }
}