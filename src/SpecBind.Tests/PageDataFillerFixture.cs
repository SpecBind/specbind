// <copyright file="PageDataFillerFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using SpecBind.Pages;
	using SpecBind.Tests.Support;

	/// <summary>
	///     A unit test fixture for the <see cref="PageDataFiller" /> class.
	/// </summary>
	[TestClass]
	public class PageDataFillerFixture
	{
		/// <summary>
		///     Tests the fill field with a field on the page that doesn't exist.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestClickItemFieldDoesNotExist()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			IPropertyData propertyData;
			page.Setup(p => p.TryGetElement("doesnotexist", out propertyData)).Returns(false);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			page.Setup(p => p.GetPropertyNames(It.IsAny<Func<IPropertyData, bool>>())).Returns(new[] { "MyProperty" });

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.ClickItem(page.Object, "doesnotexist"), e => page.VerifyAll());
		}

		/// <summary>
		///     Tests the fill field with an element that exists and can be clicked.
		/// </summary>
		[TestMethod]
		public void TestClickItemSuccess()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.ClickElement());

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetElement("DisplayArea", out propertyData)).Returns(true);

			filler.ClickItem(page.Object, "DisplayArea");

			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		///     Tests the fill field with an element that doesn't exist.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestFillFieldElementDoesNotExist()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			IPropertyData propertyData;
			page.Setup(p => p.TryGetElement("doesnotexist", out propertyData)).Returns(false);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			page.Setup(p => p.GetPropertyNames(It.IsAny<Func<IPropertyData, bool>>())).Returns(new[] { "MyProperty" });

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.FillField(page.Object, "doesnotexist", "Hello World!"), e => page.VerifyAll());
		}

		/// <summary>
		///     Tests the fill field.
		/// </summary>
		[TestMethod]
		public void TestFillFieldSuccess()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.FillData("My Data"));

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetElement("DisplayArea", out propertyData)).Returns(true);

			filler.FillField(page.Object, "DisplayArea", "My Data");

			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		///     Tests the GetElementAsPage with a fields that is a list property.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestGetElementAsPageExistsButIsAList()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.SetupGet(p => p.IsList).Returns(true);

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetElement("doesnotexist", out propertyData)).Returns(true);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			page.Setup(p => p.GetPropertyNames(It.IsAny<Func<IPropertyData, bool>>())).Returns(new[] { "MyProperty" });

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.GetElementAsPage(page.Object, "doesnotexist"), e => page.VerifyAll());
		}

		/// <summary>
		///     Tests the GetElementAsPage method with an item that cannot be found in the property.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestGetElementAsPageItemNotFound()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.GetItemAsPage()).Returns((IPage)null);
			propData.SetupGet(p => p.IsList).Returns(false);
			propData.SetupGet(p => p.Name).Returns("MyProperty");

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetElement("name", out propertyData)).Returns(true);

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.GetElementAsPage(page.Object, "name"), 
				e =>
					{
						page.VerifyAll();
						propData.VerifyAll();
					});
		}

		/// <summary>
		///     Tests the GetElementAsPage method with a valid element.
		/// </summary>
		[TestMethod]
		public void TestGetElementAsPageSuccess()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);
			var listItem = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.GetItemAsPage()).Returns(listItem.Object);
			propData.SetupGet(p => p.IsList).Returns(false);

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetElement("name", out propertyData)).Returns(true);

			var result = filler.GetElementAsPage(page.Object, "name");

			Assert.AreSame(listItem.Object, result);

			page.VerifyAll();
			propData.VerifyAll();
			listItem.VerifyAll();
		}

		/// <summary>
		///     Tests the GetListItem method with a fields that does not exist.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestGetListItemElementDoesNotExist()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			IPropertyData propertyData;
			page.Setup(p => p.TryGetProperty("doesnotexist", out propertyData)).Returns(false);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			page.Setup(p => p.GetPropertyNames(It.IsAny<Func<IPropertyData, bool>>())).Returns(new[] { "MyProperty" });

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.GetListItem(page.Object, "doesnotexist", 1), e => page.VerifyAll());
		}

		/// <summary>
		///     Tests the GetListItem with a fields that does not exist.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestGetListItemExistsButIsNotAList()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.SetupGet(p => p.IsList).Returns(false);

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetProperty("doesnotexist", out propertyData)).Returns(true);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			page.Setup(p => p.GetPropertyNames(It.IsAny<Func<IPropertyData, bool>>())).Returns(new[] { "MyProperty" });

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.GetListItem(page.Object, "doesnotexist", 1), e => page.VerifyAll());
		}

		/// <summary>
		///     Tests the GetListItem method with a list item that cannot be found.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestGetListItemItemNotFound()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.GetItemAtIndex(0)).Returns((IPage)null);
			propData.SetupGet(p => p.IsList).Returns(true);
			propData.SetupGet(p => p.Name).Returns("MyProperty");

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetProperty("name", out propertyData)).Returns(true);

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.GetListItem(page.Object, "name", 1), 
				e =>
					{
						page.VerifyAll();
						propData.VerifyAll();
					});
		}

		/// <summary>
		///     Tests the GetListItem method with a valid list.
		/// </summary>
		[TestMethod]
		public void TestGetListItemSuccess()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);
			var listItem = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.GetItemAtIndex(0)).Returns(listItem.Object);
			propData.SetupGet(p => p.IsList).Returns(true);

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetProperty("name", out propertyData)).Returns(true);

			var result = filler.GetListItem(page.Object, "name", 1);

			Assert.AreSame(listItem.Object, result);

			page.VerifyAll();
			propData.VerifyAll();
			listItem.VerifyAll();
		}

		/// <summary>
		///     Tests the GetElementAsPage method with an element field that does not exist.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestGetPropertyAsPageDoesNotExist()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			IPropertyData propertyData;
			page.Setup(p => p.TryGetElement("doesnotexist", out propertyData)).Returns(false);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			page.Setup(p => p.GetPropertyNames(It.IsAny<Func<IPropertyData, bool>>())).Returns(new[] { "MyProperty" });

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.GetElementAsPage(page.Object, "doesnotexist"), e => page.VerifyAll());
		}

		/// <summary>
		///     Tests the validate enabled with a field on the page that should be enabled.
		/// </summary>
		[TestMethod]
		public void TestValidateEnabledFieldShouldBeEnabled()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.CheckElementEnabled()).Returns(true);

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetElement("DisplayArea", out propertyData)).Returns(true);

			filler.ValidateEnabled(page.Object, "DisplayArea", true);

			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		///     Tests the validate enabled with a field on the page that should be enabled but is not.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestValidateEnabledFieldShouldBeEnabledButIsNot()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.CheckElementEnabled()).Returns(false);
			propData.SetupGet(p => p.Name).Returns("Name");

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetElement("name", out propertyData)).Returns(true);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.ValidateEnabled(page.Object, "name", true), 
				e =>
					{
						page.VerifyAll();
						propData.VerifyAll();
					});
		}

		/// <summary>
		///     Tests the validate enabled with a field on the page that should not be enabled.
		/// </summary>
		[TestMethod]
		public void TestValidateEnabledFieldShouldNotBeEnabled()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.CheckElementEnabled()).Returns(false);

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetElement("DisplayArea", out propertyData)).Returns(true);

			filler.ValidateEnabled(page.Object, "DisplayArea", false);

			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		///     Tests the validate enabled with a field on the page that should not be enabled but is.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestValidateEnabledFieldShouldNotEnabledButIsEnabled()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.CheckElementEnabled()).Returns(true);
			propData.SetupGet(p => p.Name).Returns("Name");

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetElement("name", out propertyData)).Returns(true);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.ValidateEnabled(page.Object, "name", false), 
				e =>
					{
						page.VerifyAll();
						propData.VerifyAll();
					});
		}

		/// <summary>
		///     Tests the validate exists with a field on the page that doesn't exist.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestValidateExistsFieldDoesNotExist()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			IPropertyData propertyData;
			page.Setup(p => p.TryGetElement("doesnotexist", out propertyData)).Returns(false);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			page.Setup(p => p.GetPropertyNames(It.IsAny<Func<IPropertyData, bool>>())).Returns(new[] { "MyProperty" });

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.ValidateExists(page.Object, "doesnotexist", true), e => page.VerifyAll());
		}

		/// <summary>
		///     Tests the validate exists with a field on the page that exists and should.
		/// </summary>
		[TestMethod]
		public void TestValidateExistsFieldShouldExistsAndDoes()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.CheckElementExists()).Returns(true);

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetElement("DisplayArea", out propertyData)).Returns(true);

			filler.ValidateExists(page.Object, "DisplayArea", true);

			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		///     Tests the validate exists with a field on the page that exists and should not.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestValidateExistsFieldShouldExistsButDoesNotNot()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.CheckElementExists()).Returns(false);
			propData.SetupGet(p => p.Name).Returns("Name");

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetElement("name", out propertyData)).Returns(true);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.ValidateExists(page.Object, "name", true), 
				e =>
					{
						page.VerifyAll();
						propData.VerifyAll();
					});
		}

		/// <summary>
		///     Tests the validate exists with a field on the page that does not exist and should not.
		/// </summary>
		[TestMethod]
		public void TestValidateExistsFieldShouldNotExistAndDoesNot()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.CheckElementExists()).Returns(false);

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetElement("DisplayArea", out propertyData)).Returns(true);

			filler.ValidateExists(page.Object, "DisplayArea", false);

			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		///     Tests the validate exists with a field on the page that exists and should not.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestValidateExistsFieldShouldNotExistAndExists()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.CheckElementExists()).Returns(true);
			propData.SetupGet(p => p.Name).Returns("Name");

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetElement("name", out propertyData)).Returns(true);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.ValidateExists(page.Object, "name", false), 
				e =>
					{
						page.VerifyAll();
						propData.VerifyAll();
					});
		}

		/// <summary>
		///     Tests the validate item with a fields that does not exist.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestValidateItemElementDoesNotExist()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			IPropertyData propertyData;
			page.Setup(p => p.TryGetProperty("doesnotexist", out propertyData)).Returns(false);

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.ValidateItem(page.Object, new[] { new ItemValidation("doesnotexist", "My Data", ComparisonType.Equals) }), 
				e => page.VerifyAll());
		}

		/// <summary>
		///     Tests the validate item with a textbox that exists using an equality check.
		/// </summary>
		[TestMethod]
		public void TestValidateItemTextboxExistsEqualCheck()
		{
			var validation = new ItemValidation("name", "Hello", ComparisonType.Equals);
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			string actualValue;
			propData.Setup(p => p.ValidateItem(validation, out actualValue)).Returns(true);

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetProperty("name", out propertyData)).Returns(true);

			filler.ValidateItem(page.Object, new[] { validation });

			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		///     Tests the validate item with a textbox that exists using an equality check that fails.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestValidateItemTextboxExistsEqualCheckFails()
		{
			var validation = new ItemValidation("name", "wrong", ComparisonType.Equals);
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			string actualValue;
			propData.Setup(p => p.ValidateItem(validation, out actualValue)).Returns(false);

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetProperty("name", out propertyData)).Returns(true);

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.ValidateItem(page.Object, new[] { validation }), 
				e =>
					{
						page.VerifyAll();
						propData.VerifyAll();
					});
		}

		/// <summary>
		///     Tests the validate item with a fields that does not exist.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestValidateListElementDoesNotExist()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			IPropertyData propertyData;
			page.Setup(p => p.TryGetProperty("doesnotexist", out propertyData)).Returns(false);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			page.Setup(p => p.GetPropertyNames(It.IsAny<Func<IPropertyData, bool>>())).Returns(new[] { "MyProperty" });

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() =>
				filler.ValidateList(
					page.Object, 
					"doesnotexist", 
					ComparisonType.Contains, 
					new[]
						{
							new ItemValidation("doesnotexist", "My Data", ComparisonType.Equals)
						}), 
					e => page.VerifyAll());
		}

		/// <summary>
		///     Tests the validate item with a fields that does not exist.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestValidateListElementExistsButIsNotAList()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.SetupGet(p => p.IsList).Returns(false);

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetProperty("doesnotexist", out propertyData)).Returns(false);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			page.Setup(p => p.GetPropertyNames(It.IsAny<Func<IPropertyData, bool>>())).Returns(new[] { "MyProperty" });

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() =>
				filler.ValidateList(
					page.Object, 
					"doesnotexist", 
					ComparisonType.Contains, 
					new[]
						{
							new ItemValidation("doesnotexist", "My Data", ComparisonType.Equals)
						}), 
				e => page.VerifyAll());
		}

		/// <summary>
		///     Tests the validate item with a textbox that exists using an equality check.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestValidateListFailure()
		{
			var validations = new List<ItemValidation> { new ItemValidation("name", "Hello", ComparisonType.Equals) };
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var itemResult = new ValidationItemResult();
			itemResult.NoteValidationResult(validations[0], false, "World");

			var validationResult = new ValidationResult(validations) { IsValid = false, ItemCount = 1 };
			validationResult.CheckedItems.Add(itemResult);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.ValidateList(ComparisonType.Contains, validations)).Returns(validationResult);
			propData.SetupGet(p => p.IsList).Returns(true);

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetProperty("name", out propertyData)).Returns(true);

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.ValidateList(page.Object, "name", ComparisonType.Contains, validations),
				e =>
				{
					var resultTable = new StringBuilder()
												.AppendLine("| name Equals Hello |")
												    .Append("| World             |");

					StringAssert.Contains(e.Message, "'name'");
					StringAssert.Contains(e.Message, "List Item Count: 1");
					StringAssert.Contains(e.Message, resultTable.ToString());

					page.VerifyAll();
					propData.VerifyAll();
				});
		}

		/// <summary>
		///     Tests the validate item with a textbox that exists using an equality check.
		/// </summary>
		[TestMethod]
		public void TestValidateListSuccess()
		{
			var validations = new List<ItemValidation> { new ItemValidation("name", "Hello", ComparisonType.Equals) };
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var validationResult = new ValidationResult(validations) { IsValid = true };

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.ValidateList(ComparisonType.Contains, validations)).Returns(validationResult);
			propData.SetupGet(p => p.IsList).Returns(true);

			var propertyData = propData.Object;
			page.Setup(p => p.TryGetProperty("name", out propertyData)).Returns(true);

			filler.ValidateList(page.Object, "name", ComparisonType.Contains, validations);

			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		///     Tests the GetItemValue method with a field that cannot be found.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestGetItemValueItemNotFound()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			
			var propertyData = propData.Object;
			
			page.Setup(p => p.TryGetProperty("name", out propertyData)).Returns(false);
			page.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			page.Setup(p => p.GetPropertyNames(It.IsAny<Func<IPropertyData, bool>>())).Returns(new[] { "MyProperty" });

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => filler.GetItemValue(page.Object, "name"),
				e =>
				{
					page.VerifyAll();
					propData.VerifyAll();
				});
		}

		/// <summary>
		///     Tests the GetItemValue method with a field that can be found.
		/// </summary>
		[TestMethod]
		public void TestGetItemValuePropertyFound()
		{
			var filler = new PageDataFiller();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
			propData.Setup(p => p.GetCurrentValue()).Returns("My Value");
			
			var propertyData = propData.Object;
			page.Setup(p => p.TryGetProperty("name", out propertyData)).Returns(true);

			var result = filler.GetItemValue(page.Object, "name");

			Assert.AreEqual("My Value", result);

			page.VerifyAll();
			propData.VerifyAll();
		}
	}
}