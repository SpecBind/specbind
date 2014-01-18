namespace SpecBind.Validation
{
    using System;

    /// <summary>
    /// A comparer that checks for inequality.
    /// </summary>
    public class NotEqualsComparer : ValueComparerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotEqualsComparer"/> class.
        /// </summary>
        public NotEqualsComparer()
            : base("notequals", "notequal", "doesnotequal")
        {
        }

        /// <summary>
        /// Compares the string values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected override bool Compare(string expected, string actual)
        {
            return !string.Equals(expected, actual, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Compares the double values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected override bool Compare(double expected, double actual)
        {
            return !Equals(expected, actual);
        }

        /// <summary>
        /// Compares the integer values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected override bool Compare(int expected, int actual)
        {
            return !Equals(expected, actual);
        }

        /// <summary>
        /// Compares the date time values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected override bool Compare(DateTime expected, DateTime actual)
        {
            return !Equals(expected, actual);
        }

        /// <summary>
        /// Compares the boolean values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected override bool Compare(bool expected, bool actual)
        {
            return !Equals(expected, actual);
        }
    }
}