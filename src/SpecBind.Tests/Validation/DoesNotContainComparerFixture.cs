// <copyright file="DoesNotContainComparerFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Validation
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for the does not contain comparison.
    /// </summary>
    [TestClass]
    public class DoesNotContainComparerFixture : ComparisonTestBase<DoesNotContainComparer>
    {
        /// <summary>
        /// Tests the rule key property for the correct tags.
        /// </summary>
        [TestMethod]
        public void TestRuleValuesReturnsProperTags()
        {
            var item = new DoesNotContainComparer();

            CollectionAssert.AreEquivalent(new[] { "doesnotcontain" }, item.RuleKeys.ToList());
        }

        /// <summary>
        /// Tests the does not contain method when it matches the substring returns false.
        /// </summary>
        [TestMethod]
        public void TestDoesNotContainWhenMatchesReturnsFalse()
        {
            RunItemCompareTest("Fie", "My Field", false);
        }

        /// <summary>
        /// Tests the does not contain method when it doesn't match the substring returns true.
        /// </summary>
        [TestMethod]
        public void TestDoesNotContainWhenDoesNotMatchReturnsFalse()
        {
            RunItemCompareTest("foo", "My Field", true);
        }

        /// <summary>
        /// Tests the does not contain method when the actual value is null returns false.
        /// </summary>
        [TestMethod]
        public void TestDoesNotContainWhenActualValueIsNullReturnsTrue()
        {
            RunItemCompareTest("foo", null, true);
        }
    }
}