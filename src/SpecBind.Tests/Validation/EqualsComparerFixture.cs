namespace SpecBind.Tests.Validation
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for the equal comparison across supported data types.
    /// </summary>
    [TestClass]
    public class EqualsComparerFixture : ComparisonTestBase<EqualsComparer>
    {
        /// <summary>
        /// Tests the rule key property for the correct tags.
        /// </summary>
        [TestMethod]
        public void TestRuleValuesReturnsProperTags()
        {
            var item = new EqualsComparer();

            CollectionAssert.AreEquivalent(new[] { "equals" }, item.RuleKeys.ToList());
        }

        /// <summary>
        /// Tests the comparison with equal string members returns true.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithEqualStringsReturnsTrue()
        {
            RunItemCompareTest("foo", "foo", true);
        }

        /// <summary>
        /// Tests the comparison with equal different case string members returns true.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithEqualDifferentCaseStringsReturnsTrue()
        {
            RunItemCompareTest("foo", "Foo", true);
        }

        /// <summary>
        /// Tests the comparison with equal int members returns true.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithEqualIntsReturnsTrue()
        {
            RunItemCompareTest("1", "1", true);
        }

        /// <summary>
        /// Tests the comparison with equal double members returns true.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithEqualDoublesReturnsTrue()
        {
            RunItemCompareTest("2.0", "2", true);
        }

        /// <summary>
        /// Tests the comparison with equal boolean members returns true.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithEqualBooleansReturnsTrue()
        {
            RunItemCompareTest("true", "True", true);
        }

        /// <summary>
        /// Tests the comparison with equal date time members returns true.
        /// </summary>
        [TestMethod]
        public void TestComparisonWithEqualDateTimeReturnsTrue()
        {
            RunItemCompareTest("2/22/2013", "February 22, 2013", true);
        }
    }
}