// <copyright file="ElementPropertyDataFixture.cs">
//    Copyright © 2014 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Tests.PropertyHandlers
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.Actions;
    using SpecBind.Pages;
    using SpecBind.PropertyHandlers;
    using SpecBind.Tests.Support;
    using SpecBind.Tests.Validation;
    using SpecBind.Validation;

    /// <summary>
    /// A test fixture for the ElementPropertyData class.
    /// </summary>
    [TestClass]
    public class ElementPropertyDataFixture
    {
        /// <summary>
        /// Tests that other methods in the class are not supported.
        /// </summary>
        [TestMethod]
        public void TestMethodsAreNotSupported()
        {
            var element = new BaseElement();
            var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);

            var propertyData = CreatePropertyData(pageBase, element);

            propertyData.TestForNotSupportedException(p => p.GetItemAtIndex(1), "Getting an item at a given index");
            propertyData.TestForNotSupportedException(p => p.FindItemInList(null), "Finding an item in a list");
            propertyData.TestForNotSupportedException(p => p.ValidateList(ComparisonType.Contains, null), "Validating a list");
            propertyData.TestForNotSupportedException(p => p.ValidateListRowCount(NumericComparisonType.Equals, 0), "Validating a list row count");

            pageBase.VerifyAll();
        }

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
        /// Tests the highlight element method.
        /// </summary>
        [TestMethod]
        public void TestHighlightElement()
        {
            var element = new BaseElement();
            var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
            pageBase.Setup(p => p.Highlight(element));

            var propertyData = CreatePropertyData(pageBase, element);

            propertyData.Highlight();

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
            
            string actualValue;
            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => propertyData.ValidateItem(ItemValidationHelper.Create("MyField", "My Data"), out actualValue),
                e => pageBase.VerifyAll());
        }

        /// <summary>
        /// Tests the ValidateItem method where the element does not exist but the check is skipped.
        /// </summary>
        [TestMethod]
        public void TestValidateItemWhereElementDoesNotExistAndCheckIsDisabled()
        {
            var element = new BaseElement();
            var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
            pageBase.Setup(p => p.ElementExistsCheck(element)).Returns(false);

            var propertyData = CreatePropertyData(pageBase, element);
            
            var validation = ItemValidationHelper.Create("MyProperty", "false", new ExistsComparer());

            string actualValue;
            var result = propertyData.ValidateItem(validation, out actualValue);

            Assert.IsTrue(result);

            pageBase.VerifyAll();
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
            
            string actualValue;
            var result = propertyData.ValidateItem(ItemValidationHelper.Create("MyProperty", "My Data"), out actualValue);

            Assert.IsTrue(result);

            pageBase.VerifyAll();
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
        /// Tests that GetCurrentValue from an element property.
        /// </summary>
        [TestMethod]
        public void TestGetCurrentValueFromElementProperty()
        {
            var element = new BaseElement();
            var propData = new Mock<IPropertyData>();
            var page = new Mock<IPage>(MockBehavior.Strict);

            var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
            pageBase.Setup(p => p.ElementExistsCheck(element)).Returns(true);
            pageBase.Setup(p => p.GetElementText(element)).Returns("My Value");

            var propertyData = CreatePropertyData(pageBase, element);
            
            var result = propertyData.GetCurrentValue();

            Assert.AreEqual("My Value", result);

            pageBase.VerifyAll();
            page.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests WaitForElement method.
        /// </summary>
        [TestMethod]
        public void TestWaitForElementCondition()
        {
            var timeout = TimeSpan.FromSeconds(15);
            var element = new BaseElement();
            var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
            pageBase.Setup(p => p.WaitForElement(element, WaitConditions.Enabled, timeout)).Returns(true);

            var propertyData = CreatePropertyData(pageBase, element);

            propertyData.WaitForElementCondition(WaitConditions.Enabled, timeout);

            pageBase.VerifyAll();
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
        private static ElementPropertyData<TElement> CreatePropertyData<TElement>(Mock<IPageElementHandler<TElement>> mock, TElement element)
        {
            return new ElementPropertyData<TElement>(
                mock.Object,
                "MyProperty",
                typeof(TElement),
                (p, f) =>
                    {
                        Assert.AreSame(p, mock.Object);
                        return f(element);
                    });
        }
    }
}