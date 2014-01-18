// <copyright file="NotEqualsComparerFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Validation
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for the inequality comparison across supported data types.
    /// </summary>
    [TestClass]
    public class NotEqualsComparerFixture : ComparisonTestBase<NotEqualsComparer>
    {
        /// <summary>
        /// Tests the rule key property for the correct tags.
        /// </summary>
        [TestMethod]
        public void TestRuleValuesReturnsProperTags()
        {
            var item = new NotEqualsComparer();

            CollectionAssert.AreEquivalent(new[] { "notequals", "notequal", "doesnotequal" }, item.RuleKeys.ToList());
        }

        /// <summary>
        /// Tests the comparison with equal string members returns false.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithEqualStringsReturnsFalse()
        {
            RunItemCompareTest("foo", "foo", false);
        }

        /// <summary>
        /// Tests the comparison with equal different case string members returns false.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithEqualDifferentCaseStringsReturnsFalse()
        {
            RunItemCompareTest("foo", "Foo", false);
        }

        /// <summary>
        /// Tests the comparison with equal integer members returns false.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithEqualIntsReturnsFalse()
        {
            RunItemCompareTest("1", "1", false);
        }

        /// <summary>
        /// Tests the comparison with equal double members returns false.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithEqualDoublesReturnsFalse()
        {
            RunItemCompareTest("2.0", "2", false);
        }

        /// <summary>
        /// Tests the comparison with equal boolean members returns false.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithEqualBooleansReturnsFalse()
        {
            RunItemCompareTest("false", "False", false);
        }

        /// <summary>
        /// Tests the comparison with equal date time members returns false.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithEqualDateTimeReturnsFalse()
        {
            RunItemCompareTest("2/22/2013", "February 22, 2013", false);
        }
    }
}