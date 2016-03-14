// <copyright file="ItemValidationFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Tests.Validation
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
	/// A test fixture for the <see cref="ItemValidation"/> class.
	/// </summary>
	[TestClass]
	public class ItemValidationFixture
	{
        /// <summary>
        /// Tests the compare method for simple comparisons.
        /// </summary>
        [TestMethod]
        public void TestConstructorRawValuesWhenNullSetsNulls()
        {
            var item = new ItemValidation(null, null, null);

            Assert.IsNull(item.RawFieldName);
            Assert.IsNull(item.RawComparisonType);
            Assert.IsNull(item.RawComparisonValue);
            Assert.IsNull(item.FieldName);
            Assert.IsNull(item.ComparisonValue);
        }

        /// <summary>
        /// Tests the compare method for simple comparisons.
        /// </summary>
        [TestMethod]
        public void TestConstructorRawValuesWhenValuesAreSetAreStored()
        {
            var item = new ItemValidation("Field", "equals", "value");

            Assert.AreEqual("Field", item.RawFieldName);
            Assert.AreEqual("equals", item.RawComparisonType);
            Assert.AreEqual("value", item.RawComparisonValue);

            Assert.IsNull(item.FieldName);
            Assert.IsNull(item.ComparisonValue);
        }

        /// <summary>
        /// Tests the constructor for trimming whitespace on values
        /// </summary>
        [TestMethod]
        public void TestConstructorRawValuesWhenValuesHaveWhitespaceAreTrimmedWhenStored()
        {
            var item = new ItemValidation(" Field ", " equals ", " value ");

            Assert.AreEqual("Field", item.RawFieldName);
            Assert.AreEqual("equals", item.RawComparisonType);
            Assert.AreEqual("value", item.RawComparisonValue);
        }

        /// <summary>
        /// Tests the compare method that it returns true when a valid comparison is called.
        /// </summary>
        [TestMethod]
        public void TestCompareWithValuesSetAndEqualReturnsTrue()
        {
            var item = new ItemValidation(" Field ", " equals ", " value ")
                           {
                               FieldName = "field",
                               ComparisonValue = "value",
                               Comparer = new EqualsComparer()
                           };

            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);

            var result = item.Compare(propertyData.Object, "value");

            Assert.AreEqual(true, result);

            propertyData.VerifyAll();
        }

        /// <summary>
        /// Tests the compare method that it returns false.
        /// </summary>
        [TestMethod]
        public void TestCompareWithNullComparerReturnsFalse()
        {
            var item = new ItemValidation(" Field ", " equals ", " value ")
            {
                FieldName = "field",
                ComparisonValue = "value",
                Comparer = null
            };

            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);

            var result = item.Compare(propertyData.Object, "value");

            Assert.AreEqual(false, result);

            propertyData.VerifyAll();
        }

        /// <summary>
        /// Tests the ToString method to return the correct string.
        /// </summary>
        [TestMethod]
        public void TestToStringReturnsRelevantItemData()
        {
            var item = new ItemValidation(" Field ", " equals ", " value ");

            var result = item.ToString();

            Assert.AreEqual("Field equals value", result);
        }

        /// <summary>
        /// Tests the ToString method with a null value to return the correct string.
        /// </summary>
        [TestMethod]
        public void TestToStringWithNullValueReturnsRelevantItemData()
        {
            var item = new ItemValidation(" Field ", " equals ", null);

            var result = item.ToString();

            Assert.AreEqual("Field equals <NULL>", result);
        }
    }
}