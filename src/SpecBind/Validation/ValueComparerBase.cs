// <copyright file="ValueComparerBase.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Validation
{
    using System;

    using SpecBind.Pages;

    /// <summary>
    /// A base class for comparing values provided.
    /// </summary>
    public abstract class ValueComparerBase : ValidationComparerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueComparerBase"/> class.
        /// </summary>
        /// <param name="ruleKeys">The rule keys.</param>
        protected ValueComparerBase(params string[] ruleKeys) : base(ruleKeys)
        {
        }

        /// <summary>
        /// A delegate to represent the TryParse methods.
        /// </summary>
        /// <typeparam name="T">The parse methods.</typeparam>
        /// <param name="s">The string value.</param>
        /// <param name="value">The parse value.</param>
        /// <returns><c>true</c> if the value can be parsed, <c>false</c> otherwise.</returns>
        private delegate bool ParseDelegate<T>(string s, out T value);

        /// <summary>
        /// Compares the values using the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <returns><c>true</c> if the comparison passes, <c>false</c> otherwise.</returns>
        public override bool Compare(IPropertyData property, string expectedValue, string actualValue)
        {
            // Try matching and parsing the values for each type.
            bool comparisonResult;
            if (TryCompare<DateTime>(expectedValue, actualValue, DateTime.TryParse, this.Compare, out comparisonResult))
            {
                return comparisonResult;
            }

            if (TryCompare<bool>(expectedValue, actualValue, bool.TryParse, this.Compare, out comparisonResult))
            {
                return comparisonResult;
            }

            if (TryCompare<int>(expectedValue, actualValue, int.TryParse, this.Compare, out comparisonResult))
            {
                return comparisonResult;
            }

            if (TryCompare<double>(expectedValue, actualValue, double.TryParse, this.Compare, out comparisonResult))
            {
                return comparisonResult;
            }


            // If all specialties fail, use the string comparison.
            TryCompare<string>(expectedValue, actualValue, StringParse, this.Compare, out comparisonResult);

            return comparisonResult;
        }

        /// <summary>
        /// Compares the boolean values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected virtual bool Compare(bool expected, bool actual)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Compares the date time values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected abstract bool Compare(DateTime expected, DateTime actual);

        /// <summary>
        /// Compares the integer values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected abstract bool Compare(int expected, int actual);

        /// <summary>
        /// Compares the double values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected abstract bool Compare(double expected, double actual);

        /// <summary>
        /// Compares the string values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected virtual bool Compare(string expected, string actual)
        {
            return false;
        }

        /// <summary>
        /// Pretends to run the string through the parser to follow the format.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> always.</returns>
        private static bool StringParse(string s, out string value)
        {
            value = s ?? string.Empty;
            return true;
        }

        /// <summary>
        /// Tries to parse the expected value for a data type then uses it as a comparison.
        /// </summary>
        /// <typeparam name="T">The data type to try</typeparam>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="parseDelegate">The parse delegate.</param>
        /// <param name="comparisonFunc">The comparison function.</param>
        /// <param name="comparisonResult">The result of the comparison.</param>
        /// <returns><c>true</c> if the data type matches, <c>false</c> otherwise.</returns>
        private static bool TryCompare<T>(
            string expected, string actual, ParseDelegate<T> parseDelegate, Func<T, T, bool> comparisonFunc, out bool comparisonResult)
        {
            T typedExpected;
            if (parseDelegate(expected, out typedExpected))
            {
                T parseActual;
                var typedActual = parseDelegate(actual, out parseActual) ? parseActual : default(T);

                try
                {
                    comparisonResult = comparisonFunc(typedExpected, typedActual);
                    return true;
                }
                catch (NotSupportedException)
                {
                }
            }

            comparisonResult = false;
            return false;
        }
    }
}