// <copyright file="DoesNotExistComparerFixture.cs">
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
    /// A test fixture for the does not exist comparison.
    /// </summary>
    [TestClass]
    public class DoesNotExistComparerFixture : ComparisonTestBase<DoesNotExistComparer>
    {
        /// <summary>
        /// Tests the rule key property for the correct tags.
        /// </summary>
        [TestMethod]
        public void TestRuleValuesReturnsProperTags()
        {
            var item = new DoesNotExistComparer();

            CollectionAssert.AreEquivalent(new[] { "doesnotexist" }, item.RuleKeys.ToList());
        }

        /// <summary>
        /// Tests the properties to ensure no other checks are needed.
        /// </summary>
        [TestMethod]
        public void TestRuleDoesNotCheckForFieldExistenceOrFieldValue()
        {
            var item = new DoesNotExistComparer();

            Assert.IsFalse(item.RequiresFieldValue);
            Assert.IsFalse(item.ShouldCheckElementExistence);
        }

        /// <summary>
        /// Tests the compare method for simple comparisons.
        /// </summary>
        [TestMethod]
        public void TestCompareWithNonBoolValueCase()
        {
            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            propertyData.Setup(p => p.CheckElementExists()).Returns(false);

            RunItemCompareTest("foo", null, true, propertyData);
        }

        /// <summary>
        /// Tests the compare method for simple comparisons.
        /// </summary>
        [TestMethod]
        public void TestCompareExistsCase()
        {
            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            propertyData.Setup(p => p.CheckElementExists()).Returns(false);

            RunItemCompareTest("True", null, true, propertyData);
        }

        /// <summary>
        /// Tests the compare method for simple comparisons.
        /// </summary>
        [TestMethod]
        public void TestCompareNotExistsCase()
        {
            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            propertyData.Setup(p => p.CheckElementExists()).Returns(true);

            RunItemCompareTest("False", null, true, propertyData);
        }
    }
}