namespace SpecBind.Tests.Validation
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for the starts with string comparison.
    /// </summary>
    [TestClass]
    public class StartsWithComparerFixture : ComparisonTestBase<StartsWithComparer>
    {
        /// <summary>
        /// Tests the rule key property for the correct tags.
        /// </summary>
        [TestMethod]
        public void TestRuleValuesReturnsProperTags()
        {
            var item = new StartsWithComparer();

            CollectionAssert.AreEquivalent(new[] { "startswith" }, item.RuleKeys.ToList());
        }

        /// <summary>
        /// Tests the start with method when it matches the substring returns true.
        /// </summary>
        [TestMethod]
        public void TestStartWithWhenMatchesReturnsTrue()
        {
            RunItemCompareTest("My", "My Field", true);
        }

        /// <summary>
        /// Tests the start with method when it doesn't match the substring returns false.
        /// </summary>
        [TestMethod]
        public void TestStartWithWhenDoesNotMatchReturnsFalse()
        {
            RunItemCompareTest("foo", "My Field", false);
        }

        /// <summary>
        /// Tests the start with method when the actual value is null returns false.
        /// </summary>
        [TestMethod]
        public void TestStartWithWhenActualValueIsNullReturnsFalse()
        {
            RunItemCompareTest("foo", null, false);
        }
    }
}