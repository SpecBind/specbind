// <copyright file="ExistsComparer.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Validation
{
    using SpecBind.Pages;

    /// <summary>
    /// A validation comparer to see if something exists.
    /// </summary>
    public class ExistsComparer : ValidationComparerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExistsComparer"/> class.
        /// </summary>
        public ExistsComparer()
            : base("exists")
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
            bool parsedValue;
            return (expectedValue != null && bool.TryParse(expectedValue, out parsedValue) && !parsedValue)
                ? !property.CheckElementExists()
                : property.CheckElementExists();
        }
    }
}