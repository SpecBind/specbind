// <copyright file="ValidationResultFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
	using System;
	using System.Text;

	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using SpecBind.Pages;

	/// <summary>
	/// A test fixture for the ValidationResult class.
	/// </summary>
	[TestClass]
	public class ValidationResultFixture
	{
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestGetCompairsonTableByRuleWhenMultipleResultsThrowsException()
		{
			var validations = new[] { new ItemValidation("MyField", "Something", ComparisonType.Equals) };
			
			var validationResult = new ValidationResult(validations);
			validationResult.CheckedItems.Add(new ValidationItemResult());
			validationResult.CheckedItems.Add(new ValidationItemResult());

			validationResult.GetCompairsonTableByRule();
		}

		[TestMethod]
		public void TestGetCompairsonTableByRuleWithValidFields()
		{
			var validation = new ItemValidation("MyField", "Something", ComparisonType.Equals);
			var validationResult = new ValidationResult(new[] { validation });

			var itemResult = new ValidationItemResult();
			itemResult.NoteValidationResult(validation, true, null);
			
			validationResult.CheckedItems.Add(itemResult);
			
			var result = validationResult.GetCompairsonTableByRule();

			var expectedTable = new StringBuilder()
										.AppendLine("| Field   | Rule   | Value     |")
											.Append("| MyField | Equals | Something |");

			Assert.AreEqual(expectedTable.ToString(), result);
		}

		[TestMethod]
		public void TestGetCompairsonTableByRuleWithInvalidFieldValue()
		{
			var validation = new ItemValidation("MyField", "Something", ComparisonType.Equals);
			var validationResult = new ValidationResult(new[] { validation });

			var itemResult = new ValidationItemResult();
			itemResult.NoteValidationResult(validation, false, "Nothing");

			validationResult.CheckedItems.Add(itemResult);

			var result = validationResult.GetCompairsonTableByRule();

			var expectedTable = new StringBuilder()
										.AppendLine("| Field   | Rule   | Value               |")
											.Append("| MyField | Equals | Something [Nothing] |");

			Assert.AreEqual(expectedTable.ToString(), result);
		}

		[TestMethod]
		public void TestGetCompairsonTableByRuleWithMissingField()
		{
			var validation = new ItemValidation("MyField", "Something", ComparisonType.Equals);
			var validationResult = new ValidationResult(new[] { validation });

			var itemResult = new ValidationItemResult();
			itemResult.NoteMissingProperty(validation);

			validationResult.CheckedItems.Add(itemResult);

			var result = validationResult.GetCompairsonTableByRule();

			var expectedTable = new StringBuilder()
										.AppendLine("| Field               | Rule   | Value     |")
											.Append("| MyField [Not Found] | Equals | Something |");

			Assert.AreEqual(expectedTable.ToString(), result);
		}
	}
}
