// <copyright file="ValidationResultFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
	using System;
	using System.Text;

	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using SpecBind.Pages;
	using SpecBind.Tests.Validation;
	
    /// <summary>
	/// A test fixture for the ValidationResult class.
	/// </summary>
	[TestClass]
	public class ValidationResultFixture
	{
		/// <summary>
		/// Tests the get comparison table by rule when multiple results throws exception.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestGetComparisonTableByRuleWhenMultipleResultsThrowsException()
		{
			var validations = new[] { ItemValidationHelper.Create("MyField", "Something") };
			
			var validationResult = new ValidationResult(validations);
			validationResult.CheckedItems.Add(new ValidationItemResult());
			validationResult.CheckedItems.Add(new ValidationItemResult());

			validationResult.GetComparisonTableByRule();
		}

		/// <summary>
		/// Tests the get comparison table by rule with valid fields.
		/// </summary>
		[TestMethod]
		public void TestGetComparisonTableByRuleWithValidFields()
		{
            var validation = ItemValidationHelper.Create("MyField", "Something");
			var validationResult = new ValidationResult(new[] { validation });

			var itemResult = new ValidationItemResult();
			itemResult.NoteValidationResult(validation, true, null);
			
			validationResult.CheckedItems.Add(itemResult);
			
			var result = validationResult.GetComparisonTableByRule();

			var expectedTable = new StringBuilder()
										.AppendLine("| Field   | Rule   | Value     |")
											.Append("| MyField | equals | Something |");

			Assert.AreEqual(expectedTable.ToString(), result);
		}

		/// <summary>
		/// Tests the get comparison table by rule with invalid field value.
		/// </summary>
		[TestMethod]
		public void TestGetComparisonTableByRuleWithInvalidFieldValue()
		{
            var validation = ItemValidationHelper.Create("MyField", "Something");
			var validationResult = new ValidationResult(new[] { validation });

			var itemResult = new ValidationItemResult();
			itemResult.NoteValidationResult(validation, false, "Nothing");

			validationResult.CheckedItems.Add(itemResult);

			var result = validationResult.GetComparisonTableByRule();

			var expectedTable = new StringBuilder()
										.AppendLine("| Field   | Rule   | Value               |")
											.Append("| MyField | equals | Something [Nothing] |");

			Assert.AreEqual(expectedTable.ToString(), result);
		}

		/// <summary>
		/// Tests the get comparison table by rule with missing field.
		/// </summary>
		[TestMethod]
		public void TestGetComparisonTableByRuleWithMissingField()
		{
            var validation = ItemValidationHelper.Create("MyField", "Something");
			var validationResult = new ValidationResult(new[] { validation });

			var itemResult = new ValidationItemResult();
			itemResult.NoteMissingProperty(validation);

			validationResult.CheckedItems.Add(itemResult);

			var result = validationResult.GetComparisonTableByRule();

			var expectedTable = new StringBuilder()
										.AppendLine("| Field               | Rule   | Value     |")
											.Append("| MyField [Not Found] | equals | Something |");

			Assert.AreEqual(expectedTable.ToString(), result);
		}

		/// <summary>
		/// Tests the get comparison table with valid fields.
		/// </summary>
		[TestMethod]
		public void TestGetComparisonTableWithValidFields()
		{
            var validation = ItemValidationHelper.Create("MyField", "Something");
			var validationResult = new ValidationResult(new[] { validation });

			var itemResult = new ValidationItemResult();
			itemResult.NoteValidationResult(validation, true, "Something");

			validationResult.CheckedItems.Add(itemResult);

			var result = validationResult.GetComparisonTable();

			var expectedTable = new StringBuilder()
										.AppendLine("| MyField equals Something |")
											.Append("| Something                |");

			Assert.AreEqual(expectedTable.ToString(), result);
		}

		/// <summary>
		/// Tests the get comparison table with invalid fields.
		/// </summary>
		[TestMethod]
		public void TestGetComparisonTableWithInvalidFields()
		{
            var validation = ItemValidationHelper.Create("MyField", "Something");
			var validationResult = new ValidationResult(new[] { validation });

			var itemResult = new ValidationItemResult();
			itemResult.NoteValidationResult(validation, false, "Else");

			validationResult.CheckedItems.Add(itemResult);

			var result = validationResult.GetComparisonTable();

			var expectedTable = new StringBuilder()
										.AppendLine("| MyField equals Something |")
											.Append("| Else                     |");

			Assert.AreEqual(expectedTable.ToString(), result);
		}

		/// <summary>
		/// Tests the get comparison table with invalid null fields.
		/// </summary>
		[TestMethod]
		public void TestGetComparisonTableWithInvalidNullFields()
		{
            var validation = ItemValidationHelper.Create("MyField", "Something");
			var validationResult = new ValidationResult(new[] { validation });

			var itemResult = new ValidationItemResult();
			itemResult.NoteValidationResult(validation, false, null);

			validationResult.CheckedItems.Add(itemResult);

			var result = validationResult.GetComparisonTable();

			var expectedTable = new StringBuilder()
										.AppendLine("| MyField equals Something |")
											.Append("| <EMPTY>                  |");

			Assert.AreEqual(expectedTable.ToString(), result);
		}

		/// <summary>
		/// Tests the get comparison table with missing fields.
		/// </summary>
		[TestMethod]
		public void TestGetComparisonTableWithMissingFields()
		{
            var validation = ItemValidationHelper.Create("MyField", "Something");
			var validationResult = new ValidationResult(new[] { validation });

			var itemResult = new ValidationItemResult();
			itemResult.NoteMissingProperty(validation);

			validationResult.CheckedItems.Add(itemResult);

			var result = validationResult.GetComparisonTable();

			var expectedTable = new StringBuilder()
										.AppendLine("| MyField equals Something |")
											.Append("| <NOT FOUND>              |");

			Assert.AreEqual(expectedTable.ToString(), result);
		}
	}
}
