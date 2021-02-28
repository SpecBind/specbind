// <copyright file="VirtualPropertyDataFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
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

    /// <summary>
    /// A test fixture virtual properties.
    /// </summary>
    [TestClass]
    public class VirtualPropertyDataFixture
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

            propertyData.TestForNotSupportedException(p => p.ClickElement(), "Clicking an element");
            propertyData.TestForNotSupportedException(p => p.CheckElementEnabled(), "Checking for an element being enabled");
            propertyData.TestForNotSupportedException(p => p.CheckElementExists(), "Checking for an element existing");
            propertyData.TestForNotSupportedException(p => p.FindItemInList(null), "Finding an item in a list");
            propertyData.TestForNotSupportedException(p => p.GetItemAsPage(), "Getting a property as a page item");
            propertyData.TestForNotSupportedException(p => p.GetItemAtIndex(1), "Getting an item at a given index");
            propertyData.TestForNotSupportedException(p => p.Highlight(), "Highlighting an item");
            propertyData.TestForNotSupportedException(p => p.ValidateList(ComparisonType.Contains, null), "Validating a list");
            propertyData.TestForNotSupportedException(p => p.WaitForElementCondition(WaitConditions.Exists, TimeSpan.Zero), "Waiting for an element");

            pageBase.VerifyAll();
        }

        /// <summary>
        /// Tests that GetCurrentValue from a non-element property.
        /// </summary>
        [TestMethod]
        public void TestGetCurrentValue()
        {
            var element = new BaseElement();
            var propData = new Mock<IPropertyData>();
            var page = new Mock<IPage>(MockBehavior.Strict);

            var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
            pageBase.Setup(p => p.GetElementAttributeValue(element, "href")).Returns("http://mypage.com/page");

            var propertyData = CreatePropertyData(pageBase, element);

            var result = propertyData.GetCurrentValue();

            Assert.AreEqual("http://mypage.com/page", result);

            pageBase.VerifyAll();
            page.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Tests the ValidateItem method for a property.
        /// </summary>
        [TestMethod]
        public void TestValidateItem()
        {
            var element = new BaseElement();

            var pageBase = new Mock<IPageElementHandler<BaseElement>>(MockBehavior.Strict);
            pageBase.Setup(p => p.GetElementAttributeValue(element, "href")).Returns("http://mypage.com/page");

            var propertyData = CreatePropertyData(pageBase, element);

            string actualValue;
            var result = propertyData.ValidateItem(ItemValidationHelper.Create("MyProperty", "http://mypage.com/page"), out actualValue);

            Assert.IsTrue(result);
            Assert.IsNotNull(actualValue);

            pageBase.VerifyAll();
        }

        /// <summary>
        /// Creates the property data.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="mock">The mock.</param>
        /// <param name="element">The element.</param>
        /// <returns>The property data item.</returns>
        private static VirtualPropertyData<TElement> CreatePropertyData<TElement>(Mock<IPageElementHandler<TElement>> mock, TElement element)
        {
            return new VirtualPropertyData<TElement>(
                mock.Object,
                "MyProperty",
                (p, f) =>
                    {
                        Assert.AreSame(p, mock.Object);
                        return f(element);
                    },
                "href",
                null);
        }
    }
}