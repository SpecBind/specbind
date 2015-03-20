// <copyright file="NumericComparisonType.cs">
//    Copyright © 2015 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
    /// <summary>
    /// Enumerates the various numeric comparison types.
    /// </summary>
    public enum NumericComparisonType
    {
        /// <summary>
        /// Determines equality between two numbers.
        /// </summary>
        Equals = 0,

        /// <summary>
        /// Determines if the actual value is greater than or equal to the target.
        /// </summary>
        GreaterThanEquals = 1,

        /// <summary>
        /// Determines if the actual value is less than or equal to the target.
        /// </summary>
        LessThanEquals = 2
    }
}