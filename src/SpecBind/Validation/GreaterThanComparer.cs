// <copyright file="GreaterThanComparer.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Validation
{
    using System;

    /// <summary>
    /// A comparer that checks for greater than the check value.
    /// </summary>
    public class GreaterThanComparer : ValueComparerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanComparer"/> class.
        /// </summary>
        public GreaterThanComparer()
            : base("greaterthan")
        {
        }

        /// <summary>
        /// Compares the double values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected override bool Compare(double expected, double actual)
        {
            return actual > expected;
        }

        /// <summary>
        /// Compares the integer values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected override bool Compare(int expected, int actual)
        {
            return actual > expected;
        }

        /// <summary>
        /// Compares the date time values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected override bool Compare(DateTime expected, DateTime actual)
        {
            return actual > expected;
        }
    }
}