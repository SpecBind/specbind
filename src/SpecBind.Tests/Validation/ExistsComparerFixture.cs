// <copyright file="ExistsComparerFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Tests.Validation
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for the exists comparison.
    /// </summary>
    [TestClass]
    public class ExistsComparerFixture : ComparisonTestBase<ExistsComparer>
    {
        /// <summary>
        /// Tests the rule key property for the correct tags.
        /// </summary>
        [TestMethod]
        public void TestRuleValuesReturnsProperTags()
        {
            var item = new ExistsComparer();

            CollectionAssert.AreEquivalent(new[] { "exists" }, item.RuleKeys.ToList());
        }

        /// <summary>
        /// Tests the compare method for simple comparisons.
        /// </summary>
        [TestMethod]
        public void TestCompareWithNonBoolValueCase()
        {
            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            propertyData.Setup(p => p.CheckElementExists()).Returns(true);

            RunItemCompareTest("foo", null, true, propertyData);
        }

        /// <summary>
        /// Tests the compare method for simple comparisons.
        /// </summary>
        [TestMethod]
        public void TestCompareExistsCase()
        {
            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            propertyData.Setup(p => p.CheckElementExists()).Returns(true);

            RunItemCompareTest("True", null, true, propertyData);
        }

        /// <summary>
        /// Tests the compare method for simple comparisons.
        /// </summary>
        [TestMethod]
        public void TestCompareNotExistsCase()
        {
            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            propertyData.Setup(p => p.CheckElementExists()).Returns(false);

            RunItemCompareTest("False", null, true, propertyData);
        }
    }
}