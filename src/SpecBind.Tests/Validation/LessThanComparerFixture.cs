// <copyright file="LessThanComparerFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Validation
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for the less than comparison across supported data types.
    /// </summary>
    [TestClass]
    public class LessThanComparerFixture : ComparisonTestBase<LessThanComparer>
    {
        /// <summary>
        /// Tests the rule key property for the correct tags.
        /// </summary>
        [TestMethod]
        public void TestRuleValuesReturnsProperTags()
        {
            var item = new LessThanComparer();

            CollectionAssert.AreEquivalent(new[] { "lessthan" }, item.RuleKeys.ToList());
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
        public void TestComparisonWithLessThanIntsReturnsTrue()
        {
            RunItemCompareTest("2", "1", true);
        }

        /// <summary>
        /// Tests the comparison with equal double members returns true.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithLessThanDoublesReturnsTrue()
        {
            RunItemCompareTest("2.5", "2.0", true);
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
        public void TestComparisonWithLessThanDateTimeReturnsTrue()
        {
            RunItemCompareTest("2/22/2013", "February 21, 2013", true);
        }

        /// <summary>
        /// Tests the ToString method for an expected response.
        /// </summary>
        [TestMethod]
        public void TestToStringReturnsValidationInfo()
        {
            var comparer = new LessThanComparer();

            var result = comparer.ToString();

            Assert.AreEqual("Validation: lessthan", result);
        }
    }
}