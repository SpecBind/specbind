// <copyright file="GreaterThanComparerFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Validation
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for the greater than comparison across supported data types.
    /// </summary>
    [TestClass]
    public class GreaterThanComparerFixture : ComparisonTestBase<GreaterThanComparer>
    {
        /// <summary>
        /// Tests the rule key property for the correct tags.
        /// </summary>
        [TestMethod]
        public void TestRuleValuesReturnsProperTags()
        {
            var item = new GreaterThanComparer();

            CollectionAssert.AreEquivalent(new[] { "greaterthan" }, item.RuleKeys.ToList());
        }

        /// <summary>
        /// Tests the comparison with string members returns false because it's not supported.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithStringsReturnsFalse()
        {
            RunItemCompareTest("foo", "foo", false);
        }

        /// <summary>
        /// Tests the comparison with greater than integer members returns true.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithGreaterThanIntsReturnsTrue()
        {
            RunItemCompareTest("1", "2", true);
        }

        /// <summary>
        /// Tests the comparison with equal double members returns true.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithGreaterThanDoublesReturnsTrue()
        {
            RunItemCompareTest("2.0", "2.5", true);
        }

        /// <summary>
        /// Tests the comparison with boolean members returns false because it's not supported.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithBooleansThrowsNotSupportedException()
        {
            RunItemCompareTest("true", "false", false);
        }

        /// <summary>
        /// Tests the comparison with greater than date time members returns true.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithGreaterThanDateTimeReturnsTrue()
        {
            RunItemCompareTest("2/22/2013", "February 23, 2013", true);
        }
    }
}