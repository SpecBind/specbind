// <copyright file="ContainsComparerFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>


namespace SpecBind.Tests.Validation
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for the contains string comparison.
    /// </summary>
    [TestClass]
    public class ContainsComparerFixture : ComparisonTestBase<ContainsComparer>
    {
        /// <summary>
        /// Tests the rule key property for the correct tags.
        /// </summary>
        [TestMethod]
        public void TestRuleValuesReturnsProperTags()
        {
            var item = new ContainsComparer();

            CollectionAssert.AreEquivalent(new[] { "contains" }, item.RuleKeys.ToList());
        }

        /// <summary>
        /// Tests the contains method when it matches the substring returns true.
        /// </summary>
        [TestMethod]
        public void TestContainsWhenMatchesReturnsTrue()
        {
            RunItemCompareTest("Fie", "My Field", true);
        }

        /// <summary>
        /// Tests the contains method when it doesn't match the substring returns false.
        /// </summary>
        [TestMethod]
        public void TestContainsWhenDoesNotMatchReturnsFalse()
        {
            RunItemCompareTest("foo", "My Field", false);
        }

        /// <summary>
        /// Tests the contains method when the actual value is null returns false.
        /// </summary>
        [TestMethod]
        public void TestContainsWhenActualValueIsNullReturnsFalse()
        {
            RunItemCompareTest("foo", null, false);
        }
    }
}