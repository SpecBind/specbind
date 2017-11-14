namespace SpecBind.Tests.Validation
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for the not enabled comparison.
    /// </summary>
    [TestClass]
    public class NotEnabledComparerFixture : ComparisonTestBase<NotEnabledComparer>
    {
        /// <summary>
        /// Tests the rule key property for the correct tags.
        /// </summary>
        [TestMethod]
        public void TestRuleValuesReturnsProperTags()
        {
            var item = new NotEnabledComparer();

            CollectionAssert.AreEquivalent(new[] { "notenabled", "isnotenabled", "disabled" }, item.RuleKeys.ToList());
        }

        /// <summary>
        /// Tests the compare method for simple comparisons.
        /// </summary>
        [TestMethod]
        public void TestCompareNotEnabledCase()
        {
            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            propertyData.Setup(p => p.CheckElementEnabled()).Returns(false);

            RunItemCompareTest(null, null, true, propertyData);
        }

        /// <summary>
        /// Tests the compare method for simple comparisons.
        /// </summary>
        [TestMethod]
        public void TestCompareEnabledCase()
        {
            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            propertyData.Setup(p => p.CheckElementEnabled()).Returns(true);

            RunItemCompareTest(null, null, false, propertyData);
        }

        /// <summary>
        /// Tests the compare method for simple comparisons.
        /// </summary>
        [TestMethod]
        public void TestCompareNotEnabledValueSetCaseShouldIgnoreValue()
        {
            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            propertyData.Setup(p => p.CheckElementEnabled()).Returns(false);

            RunItemCompareTest("False", null, true, propertyData);
        }
    }
}