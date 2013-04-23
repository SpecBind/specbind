// <copyright file="PropertyDataFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Tests
{
	using System;
	using System.Collections.Generic;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using SpecBind.Pages;
	using SpecBind.Tests.Support;

	/// <summary>
	/// A test fixture for the PageBase abstract class.
	/// </summary>
	[TestClass]
	public class PropertyDataFixture
	{
		/// <summary>
		/// Tests the click element method.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestClickElementWhereElementDoesNotExist()
		{
			var element = new BaseElement();
			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			pageBase.Setup(p => p.ElementExistsCheck(element)).Returns(false);

			var propertyData = CreatePropertyData(pageBase, element);

			ExceptionHelper.SetupForException<ElementExecuteException>(
				propertyData.ClickElement,
				e => pageBase.VerifyAll());
		}

		/// <summary>
		/// Tests the click element method.
		/// </summary>
		[TestMethod]
		public void TestClickElement()
		{
			var element = new BaseElement();
			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.ElementExistsCheck(element)).Returns(true);
			pageBase.Setup(p => p.ClickElement(element)).Returns(true);

			var propertyData = CreatePropertyData(pageBase, element);

			propertyData.ClickElement();

			pageBase.VerifyAll();
		}

		/// <summary>
		/// Tests the click element method.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestClickElementFails()
		{
			var element = new BaseElement();
			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.ElementExistsCheck(element)).Returns(true);
			pageBase.Setup(p => p.ClickElement(element)).Returns(false);

			var propertyData = CreatePropertyData(pageBase, element);

			ExceptionHelper.SetupForException<ElementExecuteException>(
				propertyData.ClickElement,
				e => pageBase.VerifyAll());
		}

		/// <summary>
		/// Tests the CheckElementEnabled method.
		/// </summary>
		[TestMethod]
		public void TestCheckElementEnabled()
		{
			var element = new BaseElement();
			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.ElementEnabledCheck(element)).Returns(true);
			
			var propertyData = CreatePropertyData(pageBase, element);

			var result = propertyData.CheckElementEnabled();

			Assert.IsTrue(result);

			pageBase.VerifyAll();
		}

		/// <summary>
		/// Tests the CheckElementExists method.
		/// </summary>
		[TestMethod]
		public void TestCheckElementExists()
		{
			var element = new BaseElement();
			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.ElementExistsCheck(element)).Returns(true);

			var propertyData = CreatePropertyData(pageBase, element);

			var result = propertyData.CheckElementExists();

			Assert.IsTrue(result);

			pageBase.VerifyAll();
		}

		/// <summary>
		/// Tests the FillData method where the element does not exist.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestFillDataWhereElementDoesNotExist()
		{
			var element = new BaseElement();
			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			pageBase.Setup(p => p.ElementExistsCheck(element)).Returns(false);

			var propertyData = CreatePropertyData(pageBase, element);

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => propertyData.FillData("My Data"),
				e => pageBase.VerifyAll());
		}

		/// <summary>
		/// Tests the FillData method where the element does not exist.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestFillDataWhereHandlerIsNotFound()
		{
			var element = new BaseElement();
			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			pageBase.Setup(p => p.ElementExistsCheck(element)).Returns(true);
			pageBase.Setup(p => p.GetPageFillMethod(typeof(BaseElement))).Returns((Action<BaseElement, string>)null);

			var propertyData = CreatePropertyData(pageBase, element);

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => propertyData.FillData("My Data"),
				e => pageBase.VerifyAll());
		}

		/// <summary>
		/// Tests the FillData method.
		/// </summary>
		[TestMethod]
		public void TestFillData()
		{
			var element = new BaseElement();

			var fillMethod = new Action<BaseElement, string>((e, s) => Assert.AreEqual(s, "My Data"));

			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.ElementExistsCheck(element)).Returns(true);
			pageBase.Setup(p => p.GetPageFillMethod(typeof(BaseElement))).Returns(fillMethod);

			var propertyData = CreatePropertyData(pageBase, element);

			propertyData.FillData("My Data");

			pageBase.VerifyAll();
		}

		/// <summary>
		/// Tests the ValidateItem method where the element does not exist.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestValidateItemWhereElementDoesNotExist()
		{
			var element = new BaseElement();
			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.SetupGet(p => p.PageType).Returns(typeof(TestBase));
			pageBase.Setup(p => p.ElementExistsCheck(element)).Returns(false);

			var propertyData = CreatePropertyData(pageBase, element);
			propertyData.IsElement = true;

			string actualValue;
			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => propertyData.ValidateItem(new ItemValidation("MyField", "My Data", ComparisonType.Equals), out actualValue),
				e => pageBase.VerifyAll());
		}

		/// <summary>
		/// Tests the ValidateItem method for an element.
		/// </summary>
		[TestMethod]
		public void TestValidateItemAsElement()
		{
			var element = new BaseElement();

			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.ElementExistsCheck(element)).Returns(true);
			pageBase.Setup(p => p.GetElementText(element)).Returns("My Data");

			var propertyData = CreatePropertyData(pageBase, element);
			propertyData.IsElement = true;

			string actualValue;
			var result = propertyData.ValidateItem(new ItemValidation("MyProperty", "My Data", ComparisonType.Equals), out actualValue);

			Assert.IsTrue(result);

			pageBase.VerifyAll();
		}

		/// <summary>
		/// Tests the ValidateItem method for a list parent.
		/// </summary>
		[TestMethod]
		public void TestValidateItemAsList()
		{
			var element = new BaseElement();
			var parentElement = new BaseElement();

			var listMock = new Mock<IElementList<BaseElement, BaseElement>>(MockBehavior.Strict);
			listMock.SetupGet(l => l.Parent).Returns(parentElement);

			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.GetElementText(parentElement)).Returns("My Data");

			var propertyData = CreatePropertyData(pageBase, element);
			propertyData.Action = (p, f) => f(listMock.Object);
			propertyData.IsList = true;

			string actualValue;
			var result = propertyData.ValidateItem(new ItemValidation("MyProperty", "My Data", ComparisonType.Equals), out actualValue);

			Assert.IsTrue(result);

			pageBase.VerifyAll();
			listMock.VerifyAll();
		}

		/// <summary>
		/// Tests the ValidateItem method for a property.
		/// </summary>
		[TestMethod]
		public void TestValidateItemAsProperty()
		{
			var element = new BaseElement();

			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			var propertyData = CreatePropertyData(pageBase, element);

			string actualValue;
			var result = propertyData.ValidateItem(new ItemValidation("MyProperty", typeof(BaseElement).FullName, ComparisonType.Equals), out actualValue);

			Assert.IsTrue(result);

			pageBase.VerifyAll();
		}

		/// <summary>
		/// Tests the ValidateItem method for a property of an enumerable string type.
		/// </summary>
		[TestMethod]
		public void TestValidateItemAsEnumerableProperty()
		{
			var element = new List<string> { "My Data", "Other Item" };

			var pageBase = new Mock<IPageElementHandler<List<string>>>(MockBehavior.Strict);
			var propertyData = CreatePropertyData(pageBase, element);

			string actualValue;
			var result = propertyData.ValidateItem(new ItemValidation("MyProperty", "My Data", ComparisonType.Equals), out actualValue);

			Assert.IsTrue(result);

			pageBase.VerifyAll();
		}

		/// <summary>
		/// Tests the ValidateList method.
		/// </summary>
		[TestMethod]
		public void TestValidateListContains()
		{
			var element = new BaseElement();
			var listElement = new BaseElement();
			var validation = new ItemValidation("MyProperty", "My Data", ComparisonType.Equals);
			var validations = new List<ItemValidation> { validation };

			var propData = new Mock<IPropertyData>();
			string actualValue;
			propData.Setup(p => p.ValidateItem(validation, out actualValue)).Returns(true);

			var page = new Mock<IPage>(MockBehavior.Strict);

			var property = propData.Object;
			page.Setup(p => p.TryGetProperty("MyProperty", out property)).Returns(true);

			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.GetPageFromElement(listElement)).Returns(page.Object);
			
			var propertyData = CreatePropertyData(pageBase, element);
			propertyData.Action = (p, f) => f(new List<BaseElement> { listElement });

			var result = propertyData.ValidateList(ComparisonType.Contains, validations);

			Assert.IsTrue(result);

			pageBase.VerifyAll();
			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		/// Tests the ValidateList method.
		/// </summary>
		[TestMethod]
		public void TestValidateListStartsWith()
		{
			var element = new BaseElement();
			var listElement = new BaseElement();
			var validation = new ItemValidation("MyProperty", "My Data", ComparisonType.Equals);
			var validations = new List<ItemValidation> { validation };

			var propData = new Mock<IPropertyData>();
			string actualValue;
			propData.Setup(p => p.ValidateItem(validation, out actualValue)).Returns(true);

			var page = new Mock<IPage>(MockBehavior.Strict);

			var property = propData.Object;
			page.Setup(p => p.TryGetProperty("MyProperty", out property)).Returns(true);

			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.GetPageFromElement(listElement)).Returns(page.Object);

			var propertyData = CreatePropertyData(pageBase, element);
			propertyData.Action = (p, f) => f(new List<BaseElement> { listElement });

			var result = propertyData.ValidateList(ComparisonType.StartsWith, validations);

			Assert.IsTrue(result);

			pageBase.VerifyAll();
			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		/// Tests the ValidateList method.
		/// </summary>
		[TestMethod]
		public void TestValidateListEndsWith()
		{
			var element = new BaseElement();
			var listElement = new BaseElement();
			var validation = new ItemValidation("MyProperty", "My Data", ComparisonType.Equals);
			var validations = new List<ItemValidation> { validation };

			var propData = new Mock<IPropertyData>();
			string actualValue;
			propData.Setup(p => p.ValidateItem(validation, out actualValue)).Returns(true);

			var page = new Mock<IPage>(MockBehavior.Strict);

			var property = propData.Object;
			page.Setup(p => p.TryGetProperty("MyProperty", out property)).Returns(true);

			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.GetPageFromElement(listElement)).Returns(page.Object);

			var propertyData = CreatePropertyData(pageBase, element);
			propertyData.Action = (p, f) => f(new List<BaseElement> { listElement });

			var result = propertyData.ValidateList(ComparisonType.StartsWith, validations);

			Assert.IsTrue(result);

			pageBase.VerifyAll();
			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		/// Tests the ValidateList method.
		/// </summary>
		[TestMethod]
		public void TestValidateListNotContains()
		{
			var element = new BaseElement();
			var listElement = new BaseElement();
			var validation = new ItemValidation("MyProperty", "My Data", ComparisonType.Equals);
			var validations = new List<ItemValidation> { validation };

			var propData = new Mock<IPropertyData>();
			string actualValue;
			propData.Setup(p => p.ValidateItem(validation, out actualValue)).Returns(false);

			var page = new Mock<IPage>(MockBehavior.Strict);

			var property = propData.Object;
			page.Setup(p => p.TryGetProperty("MyProperty", out property)).Returns(true);

			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.GetPageFromElement(listElement)).Returns(page.Object);

			var propertyData = CreatePropertyData(pageBase, element);
			propertyData.Action = (p, f) => f(new List<BaseElement> { listElement });

			var result = propertyData.ValidateList(ComparisonType.DoesNotContain, validations);

			Assert.IsTrue(result);

			pageBase.VerifyAll();
			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		/// Tests the ValidateList method.
		/// </summary>
		[TestMethod]
		public void TestValidateListInvalidComparison()
		{
			var element = new BaseElement();
			var listElement = new BaseElement();
			var validation = new ItemValidation("MyProperty", "My Data", ComparisonType.Equals);
			var validations = new List<ItemValidation> { validation };

			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			var propertyData = CreatePropertyData(pageBase, element);
			propertyData.Action = (p, f) => f(new List<BaseElement> { listElement });

			var result = propertyData.ValidateList(ComparisonType.Enabled, validations);

			Assert.IsFalse(result);

			pageBase.VerifyAll();
		}

		/// <summary>
		/// Tests the ValidateList method.
		/// </summary>
		[TestMethod]
		public void TestValidateListContainsChildElementNotFound()
		{
			var element = new BaseElement();
			var listElement = new BaseElement();
			var validation = new ItemValidation("MyProperty", "My Data", ComparisonType.Equals);
			var validations = new List<ItemValidation> { validation };

			var propData = new Mock<IPropertyData>();
			var page = new Mock<IPage>(MockBehavior.Strict);

			var property = propData.Object;
			page.Setup(p => p.TryGetProperty("MyProperty", out property)).Returns(false);

			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.GetPageFromElement(listElement)).Returns(page.Object);

			var propertyData = CreatePropertyData(pageBase, element);
			propertyData.Action = (p, f) => f(new List<BaseElement> { listElement });

			var result = propertyData.ValidateList(ComparisonType.Contains, validations);

			Assert.IsFalse(result);

			pageBase.VerifyAll();
			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		/// Tests the ValidateList method.
		/// </summary>
		[TestMethod]
		public void TestValidateListContainsValidationFails()
		{
			var element = new BaseElement();
			var listElement = new BaseElement();
			var validation = new ItemValidation("MyProperty", "My Data", ComparisonType.Equals);
			var validations = new List<ItemValidation> { validation };

			var propData = new Mock<IPropertyData>();
			string actualValue;
			propData.Setup(p => p.ValidateItem(validation, out actualValue)).Returns(false);

			var page = new Mock<IPage>(MockBehavior.Strict);

			var property = propData.Object;
			page.Setup(p => p.TryGetProperty("MyProperty", out property)).Returns(true);

			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.GetPageFromElement(listElement)).Returns(page.Object);

			var propertyData = CreatePropertyData(pageBase, element);
			propertyData.Action = (p, f) => f(new List<BaseElement> { listElement });

			var result = propertyData.ValidateList(ComparisonType.Contains, validations);

			Assert.IsFalse(result);

			pageBase.VerifyAll();
			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		/// Tests the GetItemAtIndex method.
		/// </summary>
		[TestMethod]
		public void TestGetItemAtIndexChildElementNotFound()
		{
			var element = new BaseElement();
			var propData = new Mock<IPropertyData>();
			var page = new Mock<IPage>(MockBehavior.Strict);

			
			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);

			var propertyData = CreatePropertyData(pageBase, element);
			propertyData.Action = (p, f) => f(new List<BaseElement>());

			var result = propertyData.GetItemAtIndex(0);

			Assert.IsNull(result);

			pageBase.VerifyAll();
			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		/// Tests the GetItemAtIndex method.
		/// </summary>
		[TestMethod]
		public void TestGetItemAtIndexSuccess()
		{
			var element = new BaseElement();
			var listElement = new BaseElement();
			var propData = new Mock<IPropertyData>();
			var page = new Mock<IPage>(MockBehavior.Strict);
			var listPage = new Mock<IPage>(MockBehavior.Strict);

			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.GetPageFromElement(listElement)).Returns(listPage.Object);

			var propertyData = CreatePropertyData(pageBase, element);
			propertyData.Action = (p, f) => f(new List<BaseElement> { listElement });

			var result = propertyData.GetItemAtIndex(0);

			Assert.AreSame(listPage.Object, result);

			pageBase.VerifyAll();
			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		/// Tests the GetItemAsPage method.
		/// </summary>
		[TestMethod]
		public void TestGetItemAsPageSuccess()
		{
			var element = new BaseElement();
			var propData = new Mock<IPropertyData>();
			var page = new Mock<IPage>(MockBehavior.Strict);
			var elementPage = new Mock<IPage>(MockBehavior.Strict);

			var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
			pageBase.Setup(p => p.GetPageFromElement(element)).Returns(elementPage.Object);

			var propertyData = CreatePropertyData(pageBase, element);

			var result = propertyData.GetItemAsPage();
			Assert.AreSame(elementPage.Object, result);

			pageBase.VerifyAll();
			page.VerifyAll();
			propData.VerifyAll();
		}

		/// <summary>
		/// Creates the property data.
		/// </summary>
		/// <typeparam name="TElement">The type of the element.</typeparam>
		/// <param name="mock">The mock.</param>
		/// <param name="element">The element.</param>
		/// <returns>
		/// The property data item.
		/// </returns>
		private static PropertyData<TElement> CreatePropertyData<TElement>(Mock<IPageElementHandler<TElement>> mock, TElement element)
		{
			return new PropertyData<TElement>(mock.Object)
				       {
						   Name = "MyProperty",
						   PropertyType = typeof(TElement),
						   ElementAction = (p, f) =>
						   {
							   Assert.AreSame(p, mock.Object);
							   return f(element);
						   },
						   Action = (p, f) =>
							   {
								   Assert.AreSame(p, mock.Object);
								   return f(element);
							   }
				       };
		}
	}
}