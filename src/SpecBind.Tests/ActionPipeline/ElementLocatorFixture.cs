// <copyright file="ElementLocatorFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.ActionPipeline
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the ElementLocator class.
    /// </summary>
    [TestClass]
    public class ElementLocatorFixture
    {
        /// <summary>
        /// Tests the get element method when the element get is successful.
        /// </summary>
        [TestMethod]
        public void TestGetElementWhenElementGetIsSuccessful()
        {
            var resultPropertyData = new Mock<IPropertyData>().Object;

            var page = new Mock<IPage>(MockBehavior.Strict);
            page.Setup(p => p.TryGetElement("MyElement", out resultPropertyData)).Returns(true);

            var locatorAction = new Mock<ILocatorAction>(MockBehavior.Strict);
            locatorAction.Setup(p => p.OnLocate("MyElement"));
            locatorAction.Setup(p => p.OnLocateComplete("MyElement", resultPropertyData));

            var locator = new ElementLocator(page.Object, new[] { locatorAction.Object });

            var propertyData = locator.GetElement("MyElement");

            Assert.IsNotNull(propertyData);
            Assert.AreSame(resultPropertyData, propertyData);

            page.VerifyAll();
            locatorAction.VerifyAll();
        }

        /// <summary>
        /// Tests the get element method when the element get fails.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestGetElementWhenElementGetFails()
        {
            IPropertyData resultPropertyData;

            var page = new Mock<IPage>(MockBehavior.Strict);
            page.Setup(p => p.TryGetElement("MyElement", out resultPropertyData)).Returns(false);
            page.Setup(p => p.GetPropertyNames(It.IsAny<Func<IPropertyData, bool>>())).Returns(new[] { "MyProperty" });
            page.SetupGet(p => p.PageType).Returns(typeof(ElementLocatorFixture));

            var locatorAction = new Mock<ILocatorAction>(MockBehavior.Strict);
            locatorAction.Setup(p => p.OnLocate("MyElement"));
            locatorAction.Setup(p => p.OnLocateComplete("MyElement", null));

            var locator = new ElementLocator(page.Object, new[] { locatorAction.Object });

            try
            {
                locator.GetElement("MyElement");
            }
            catch (ElementExecuteException)
            {
                page.VerifyAll();
                locatorAction.VerifyAll();
                throw;
            }
        }

        /// <summary>
        /// Tests the try get element method when the element get is successful.
        /// </summary>
        [TestMethod]
        public void TestTryGetElementWhenElementGetIsSuccessful()
        {
            var resultPropertyData = new Mock<IPropertyData>().Object;

            var page = new Mock<IPage>(MockBehavior.Strict);
            page.Setup(p => p.TryGetElement("MyElement", out resultPropertyData)).Returns(true);

            var locatorAction = new Mock<ILocatorAction>(MockBehavior.Strict);
            locatorAction.Setup(p => p.OnLocate("MyElement"));
            locatorAction.Setup(p => p.OnLocateComplete("MyElement", resultPropertyData));

            var locator = new ElementLocator(page.Object, new[] { locatorAction.Object });

            IPropertyData propertyData;
            var result = locator.TryGetElement("MyElement", out propertyData);

            Assert.IsTrue(result);
            Assert.IsNotNull(propertyData);
            Assert.AreSame(resultPropertyData, propertyData);

            page.VerifyAll();
            locatorAction.VerifyAll();
        }

        /// <summary>
        /// Tests the try get element method when the element get fails.
        /// </summary>
        [TestMethod]
        public void TestTryGetElementWhenElementGetFails()
        {
            IPropertyData resultPropertyData;

            var page = new Mock<IPage>(MockBehavior.Strict);
            page.Setup(p => p.TryGetElement("MyElement", out resultPropertyData)).Returns(false);

            var locatorAction = new Mock<ILocatorAction>(MockBehavior.Strict);
            locatorAction.Setup(p => p.OnLocate("MyElement"));
            locatorAction.Setup(p => p.OnLocateComplete("MyElement", null));

            var locator = new ElementLocator(page.Object, new[] { locatorAction.Object });

            IPropertyData propertyData;
            var result = locator.TryGetElement("MyElement", out propertyData);

            Assert.IsFalse(result);
            Assert.IsNull(propertyData);

            page.VerifyAll();
            locatorAction.VerifyAll();
        }

        /// <summary>
        /// Tests the get property method when the property get is successful.
        /// </summary>
        [TestMethod]
        public void TestGetPropertyWhenPropertyGetIsSuccessful()
        {
            var resultPropertyData = new Mock<IPropertyData>().Object;

            var page = new Mock<IPage>(MockBehavior.Strict);
            page.Setup(p => p.TryGetProperty("MyProperty", out resultPropertyData)).Returns(true);

            var locatorAction = new Mock<ILocatorAction>(MockBehavior.Strict);
            locatorAction.Setup(p => p.OnLocate("MyProperty"));
            locatorAction.Setup(p => p.OnLocateComplete("MyProperty", resultPropertyData));

            var locator = new ElementLocator(page.Object, new[] { locatorAction.Object });

            var propertyData = locator.GetProperty("MyProperty");

            Assert.IsNotNull(propertyData);
            Assert.AreSame(resultPropertyData, propertyData);

            page.VerifyAll();
            locatorAction.VerifyAll();
        }

        /// <summary>
        /// Tests the get Property method when the property get fails.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestGetPropertyWhenPropertyGetFails()
        {
            IPropertyData resultPropertyData;

            var page = new Mock<IPage>(MockBehavior.Strict);
            page.Setup(p => p.TryGetProperty("MyProperty", out resultPropertyData)).Returns(false);
            page.Setup(p => p.GetPropertyNames(It.IsAny<Func<IPropertyData, bool>>())).Returns(new[] { "Property1" });
            page.SetupGet(p => p.PageType).Returns(typeof(ElementLocatorFixture));

            var locatorAction = new Mock<ILocatorAction>(MockBehavior.Strict);
            locatorAction.Setup(p => p.OnLocate("MyProperty"));
            locatorAction.Setup(p => p.OnLocateComplete("MyProperty", null));

            var locator = new ElementLocator(page.Object, new[] { locatorAction.Object });

            try
            {
                locator.GetProperty("MyProperty");
            }
            catch (ElementExecuteException)
            {
                page.VerifyAll();
                locatorAction.VerifyAll();
                throw;
            }
        }

        /// <summary>
        /// Tests the try get Property method when the property get is successful.
        /// </summary>
        [TestMethod]
        public void TestTryGetPropertyWhenPropertyGetIsSuccessful()
        {
            var resultPropertyData = new Mock<IPropertyData>().Object;

            var page = new Mock<IPage>(MockBehavior.Strict);
            page.Setup(p => p.TryGetProperty("MyProperty", out resultPropertyData)).Returns(true);

            var locatorAction = new Mock<ILocatorAction>(MockBehavior.Strict);
            locatorAction.Setup(p => p.OnLocate("MyProperty"));
            locatorAction.Setup(p => p.OnLocateComplete("MyProperty", resultPropertyData));

            var locator = new ElementLocator(page.Object, new[] { locatorAction.Object });

            IPropertyData propertyData;
            var result = locator.TryGetProperty("MyProperty", out propertyData);

            Assert.IsTrue(result);
            Assert.IsNotNull(propertyData);
            Assert.AreSame(resultPropertyData, propertyData);

            page.VerifyAll();
            locatorAction.VerifyAll();
        }

        /// <summary>
        /// Tests the try get Property method when the property get fails.
        /// </summary>
        [TestMethod]
        public void TestTryGetPropertyWhenPropertyGetFails()
        {
            IPropertyData resultPropertyData;

            var page = new Mock<IPage>(MockBehavior.Strict);
            page.Setup(p => p.TryGetProperty("MyProperty", out resultPropertyData)).Returns(false);

            var locatorAction = new Mock<ILocatorAction>(MockBehavior.Strict);
            locatorAction.Setup(p => p.OnLocate("MyProperty"));
            locatorAction.Setup(p => p.OnLocateComplete("MyProperty", null));

            var locator = new ElementLocator(page.Object, new[] { locatorAction.Object });

            IPropertyData propertyData;
            var result = locator.TryGetProperty("MyProperty", out propertyData);

            Assert.IsFalse(result);
            Assert.IsNull(propertyData);

            page.VerifyAll();
            locatorAction.VerifyAll();
        }
    }
}