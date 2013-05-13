// <copyright file="ItemValidationFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Tests
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using SpecBind.Pages;

	/// <summary>
	/// A test fixture for the <see cref="ItemValidation"/> class.
	/// </summary>
	[TestClass]
	public class ItemValidationFixture
	{
		#region Public Methods and Operators

		/// <summary>
		/// Tests the compare method for simple comparisons.
		/// </summary>
		[TestMethod]
		public void TestCompareCommonCases()
		{
			RunItemCompareTest(ComparisonType.Equals, "My Field", "My Field", true);
			RunItemCompareTest(ComparisonType.DoesNotEqual, "My Field", "Foo", true);
			RunItemCompareTest(ComparisonType.StartsWith, "My Field", "My", true);
			RunItemCompareTest(ComparisonType.EndsWith, "My Field", "ld", true);
			RunItemCompareTest(ComparisonType.Contains, "My Field", "Fie", true);
			RunItemCompareTest(ComparisonType.DoesNotContain, "My Field", "ajskldj", true);
		}

		/// <summary>
		/// Tests the compare method for simple comparisons.
		/// </summary>
		[TestMethod]
		public void TestCompareExistsCase()
		{
			var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
			propertyData.Setup(p => p.CheckElementExists()).Returns(true);

			RunItemCompareTest(ComparisonType.Exists, null, "True", true, propertyData);
		}

		/// <summary>
		/// Tests the compare method for simple comparisons.
		/// </summary>
		[TestMethod]
		public void TestCompareNotExistsCase()
		{
			var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
			propertyData.Setup(p => p.CheckElementExists()).Returns(false);

			RunItemCompareTest(ComparisonType.Exists, null, "False", true, propertyData);
		}

		/// <summary>
		/// Tests the compare method for simple comparisons.
		/// </summary>
		[TestMethod]
		public void TestCompareEnabledCase()
		{
			var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
			propertyData.Setup(p => p.CheckElementEnabled()).Returns(true);

			RunItemCompareTest(ComparisonType.Enabled, null, "True", true, propertyData);
		}

		/// <summary>
		/// Tests the compare method for simple comparisons.
		/// </summary>
		[TestMethod]
		public void TestCompareNotEnabledCase()
		{
			var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
			propertyData.Setup(p => p.CheckElementEnabled()).Returns(false);

			RunItemCompareTest(ComparisonType.Enabled, null, "False", true, propertyData);
		}

		/// <summary>
		/// Runs the item compare test.
		/// </summary>
		/// <param name="comparisonType">Type of the comparison.</param>
		/// <param name="actualValue">The actual value.</param>
		/// <param name="compareValue">The compare value.</param>
		/// <param name="isTrue">if set to <c>true</c> the result should be true.</param>
		/// <param name="propertyData">The property data.</param>
		private static void RunItemCompareTest(ComparisonType comparisonType, string actualValue, string compareValue, bool isTrue, Mock<IPropertyData> propertyData = null)
		{
			propertyData = propertyData ?? new Mock<IPropertyData>(MockBehavior.Strict);

			var validation = new ItemValidation("MyField", compareValue, comparisonType);

			var result = validation.Compare(propertyData.Object, actualValue);
			
			Assert.AreEqual(isTrue, result, "Comparison {0}, Test: {1}, Actual: {2}", comparisonType, compareValue, actualValue);

			propertyData.VerifyAll();
		}

		#endregion
	}
}