namespace SpecBind.Tests.Validation
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for the ends with string comparison.
    /// </summary>
    [TestClass]
    public class EndsWithComparerFixture : ComparisonTestBase<EndsWithComparer>
    {
        /// <summary>
        /// Tests the rule key property for the correct tags.
        /// </summary>
        [TestMethod]
        public void TestRuleValuesReturnsProperTags()
        {
            var item = new EndsWithComparer();

            CollectionAssert.AreEquivalent(new[] { "endswith" }, item.RuleKeys.ToList());
        }

        /// <summary>
        /// Tests the ends with method when it matches the substring returns true.
        /// </summary>
        [TestMethod]
        public void TestEndsWithWhenMatchesReturnsTrue()
        {
            RunItemCompareTest("Field", "My Field", true);
        }

        /// <summary>
        /// Tests the ends with method when it doesn't match the substring returns false.
        /// </summary>
        [TestMethod]
        public void TestEndsWithWhenDoesNotMatchReturnsFalse()
        {
            RunItemCompareTest("foo", "My Field", false);
        }

        /// <summary>
        /// Tests the ends with method when the actual value is null returns false.
        /// </summary>
        [TestMethod]
        public void TestEndsWithWhenActualValueIsNullReturnsFalse()
        {
            RunItemCompareTest("foo", null, false);
        }
    }
}