// <copyright file="WebElementFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium.Tests
{
    using System.Drawing;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Interactions.Internal;

    /// <summary>
    /// A test fixture for WebElement.
    /// </summary>
    [TestClass]
    public class WebElementFixture
    {
        /// <summary>
        /// Tests the clone native element to ensure it bypasses the locators.
        /// </summary>
        [TestMethod]
        public void TestCloneNativeElementBypassesLocators()
        {
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);

            var element = new WebElement(null);
            element.CloneNativeElement(mockElement.Object);

            Assert.IsTrue(element.Cache);

            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests update locators method to ensure it doesn't update if locators are null.
        /// </summary>
        [TestMethod]
        public void TestUpdateLocatorsWhenNullDoesNotUpdateLocators()
        {
            var element = new WebElement(null);
            element.UpdateLocators(null);

            Assert.AreEqual(0, element.Locators.Count);
        }

        /// <summary>
        /// Tests update locators method to ensure updates the locators.
        /// </summary>
        [TestMethod]
        public void TestUpdateLocatorsWhenExistsUpdatesLocators()
        {
            var locator = By.Id("id1");
            var element = new WebElement(null);
            element.UpdateLocators(new[] { locator });

            Assert.AreEqual(1, element.Locators.Count);
            Assert.AreEqual(locator, element.Locators.First());
        }

        /// <summary>
        /// Tests the get coordinates property to ensure it returns the wrapped value.
        /// </summary>
        [TestMethod]
        public void TestGetCoordinatesReturnsWrappedValue()
        {
            var coordinates = new Mock<ICoordinates>(MockBehavior.Strict);

            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.As<ILocatable>().SetupGet(c => c.Coordinates).Returns(coordinates.Object);

            var element = CreateBasicWrappedElement(mockElement.Object);

            var result = element.Coordinates;

            Assert.AreSame(coordinates.Object, result);
            mockElement.VerifyAll();
            coordinates.VerifyAll();
        }

        /// <summary>
        /// Tests the get displayed property to ensure it returns the wrapped value.
        /// </summary>
        [TestMethod]
        public void TestGetDisplayedReturnsWrappedValue()
        {
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.SetupGet(c => c.Displayed).Returns(true);

            var element = CreateBasicWrappedElement(mockElement.Object);

            var result = element.Displayed;

            Assert.AreEqual(true, result);
            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the get enabled property to ensure it returns the wrapped value.
        /// </summary>
        [TestMethod]
        public void TestGetEnablesReturnsWrappedValue()
        {
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.SetupGet(c => c.Enabled).Returns(true);

            var element = CreateBasicWrappedElement(mockElement.Object);

            var result = element.Enabled;

            Assert.AreEqual(true, result);
            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the get location property to ensure it returns the wrapped value.
        /// </summary>
        [TestMethod]
        public void TestGetLocationReturnsWrappedValue()
        {
            var point = new Point(22, 10);
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.SetupGet(c => c.Location).Returns(point);

            var element = CreateBasicWrappedElement(mockElement.Object);

            var result = element.Location;

            Assert.AreEqual(point, result);
            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the get location on screen property to ensure it returns the wrapped value.
        /// </summary>
        [TestMethod]
        public void TestGetLocationOnScreenReturnsWrappedValue()
        {
            var point = new Point(22, 10);
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.As<ILocatable>().SetupGet(c => c.LocationOnScreenOnceScrolledIntoView).Returns(point);

            var element = CreateBasicWrappedElement(mockElement.Object);

            var result = element.LocationOnScreenOnceScrolledIntoView;

            Assert.AreEqual(point, result);
            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the get selected property to ensure it returns the wrapped value.
        /// </summary>
        [TestMethod]
        public void TestGetSelectedReturnsWrappedValue()
        {
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.SetupGet(c => c.Selected).Returns(true);

            var element = CreateBasicWrappedElement(mockElement.Object);

            var result = element.Selected;

            Assert.AreEqual(true, result);
            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the get size property to ensure it returns the wrapped value.
        /// </summary>
        [TestMethod]
        public void TestGetSizeReturnsWrappedValue()
        {
            var point = new Size(22, 10);
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.SetupGet(c => c.Size).Returns(point);

            var element = CreateBasicWrappedElement(mockElement.Object);

            var result = element.Size;

            Assert.AreEqual(point, result);
            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the get tag name property to ensure it returns the wrapped value.
        /// </summary>
        [TestMethod]
        public void TestGetTagNameReturnsWrappedValue()
        {
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.SetupGet(c => c.TagName).Returns("div");

            var element = CreateBasicWrappedElement(mockElement.Object);

            var result = element.TagName;

            Assert.AreEqual("div", result);
            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the get text property to ensure it returns the wrapped value.
        /// </summary>
        [TestMethod]
        public void TestGetTextReturnsWrappedValue()
        {
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.SetupGet(c => c.Text).Returns("Hello");

            var element = CreateBasicWrappedElement(mockElement.Object);

            var result = element.Text;

            Assert.AreEqual("Hello", result);
            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the click method invokes the wrapped element.
        /// </summary>
        [TestMethod]
        public void TestCallClickInvokesWrappedValue()
        {
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.Setup(c => c.Click());

            var element = CreateBasicWrappedElement(mockElement.Object);

            element.Click();

            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the click method invokes the wrapped element.
        /// </summary>
        [TestMethod]
        public void TestClearClickInvokesWrappedValue()
        {
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.Setup(c => c.Clear());

            var element = CreateBasicWrappedElement(mockElement.Object);

            element.Clear();

            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the get attributes method invokes the wrapped element.
        /// </summary>
        [TestMethod]
        public void TestCallGetAttributesInvokesWrappedValue()
        {
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.Setup(c => c.GetAttribute("name")).Returns("test1");

            var element = CreateBasicWrappedElement(mockElement.Object);

            var result = element.GetAttribute("name");

            Assert.AreEqual("test1", result);
            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the get find element method invokes the wrapped element.
        /// </summary>
        [TestMethod]
        public void TestCallFindElementInvokesWrappedValue()
        {
            var innerElement = new Mock<IWebElement>();

            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.Setup(m => m.FindElement(By.Id("12"))).Returns(innerElement.Object);

            var element = CreateBasicWrappedElement(mockElement.Object);

            var result = element.FindElement(By.Id("12"));

            Assert.AreEqual(innerElement.Object, result);
            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the send keys method invokes the wrapped element.
        /// </summary>
        [TestMethod]
        public void TestCallSendKeysInvokesWrappedValue()
        {
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.Setup(c => c.SendKeys("test"));

            var element = CreateBasicWrappedElement(mockElement.Object);

            element.SendKeys("test");

            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the submit method invokes the wrapped element.
        /// </summary>
        [TestMethod]
        public void TestCallSubmitInvokesWrappedValue()
        {
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.Setup(c => c.Submit());

            var element = CreateBasicWrappedElement(mockElement.Object);

            element.Submit();

            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the get CSS value method invokes the wrapped element.
        /// </summary>
        [TestMethod]
        public void TestCallGetCssValueInvokesWrappedValue()
        {
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            mockElement.Setup(m => m.GetCssValue("myprop")).Returns("propval");

            var element = CreateBasicWrappedElement(mockElement.Object);

            var result = element.GetCssValue("myprop");

            Assert.AreEqual("propval", result);
            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the get hash code method invokes the wrapped element.
        /// </summary>
        [TestMethod]
        public void TestCallGetHashCodeInvokesWrappedValue()
        {
            var mockElement = new Mock<IWebElement>(MockBehavior.Strict);
            
            var element = CreateBasicWrappedElement(mockElement.Object);

            var result = element.GetHashCode();

            Assert.IsNotNull(result);
            mockElement.VerifyAll();
        }

        /// <summary>
        /// Tests the get hash code method invokes the wrapped element.
        /// </summary>
        [TestMethod]
        public void TestLocateElementBySingleLocatorFindsTheElement()
        {
            var targetElement = new Mock<IWebElement>();

            var searchContext = new Mock<ISearchContext>(MockBehavior.Strict);
            searchContext.Setup(s => s.FindElement(By.Id("1234"))).Returns(targetElement.Object);

            var element = new WebElement(searchContext.Object);
            element.UpdateLocators(new[] { By.Id("1234") });

            var result = element.WrappedElement;

            Assert.IsNotNull(result);
            Assert.AreEqual(targetElement.Object, result);
            searchContext.VerifyAll();
        }

        /// <summary>
        /// Tests the single locator element fails to find the element.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NoSuchElementException))]
        public void TestLocateElementBySingleLocatorDoesNotFindTheElement()
        {
            var searchContext = new Mock<ISearchContext>(MockBehavior.Strict);
            searchContext.Setup(s => s.FindElement(By.Id("1234"))).Throws<NoSuchElementException>();

            var element = new WebElement(searchContext.Object);
            element.UpdateLocators(new[] { By.Id("1234") });

            try
            {
                // ReSharper disable once UnusedVariable
                var result = element.WrappedElement;
            }
            catch (NoSuchElementException ex)
            {
                Assert.AreEqual("Could not find element by: By.Id: 1234", ex.Message);
                
                searchContext.VerifyAll();
                
                throw;
            }
        }

        /// <summary>
        /// Tests the element locator for finding multiple items.
        /// </summary>
        [TestMethod]
        public void TestLocateElementByMultipleLocatorsFindsTheElement()
        {
            var targetElement = new Mock<IWebElement>();

            var searchContext = new Mock<ISearchContext>(MockBehavior.Strict);
            searchContext.Setup(s => s.FindElement(By.Name("1234"))).Throws<NoSuchElementException>();
            searchContext.Setup(s => s.FindElement(By.Id("1234"))).Returns(targetElement.Object);

            var element = new WebElement(searchContext.Object);
            element.UpdateLocators(new[] { By.Name("1234"), By.Id("1234") });

            var result = element.WrappedElement;

            Assert.IsNotNull(result);
            Assert.AreEqual(targetElement.Object, result);
            searchContext.VerifyAll();
        }

        /// <summary>
        /// Tests the multiple locator element fails to find the element.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NoSuchElementException))]
        public void TestLocateElementByMultipleLocatorsDoesNotFindTheElement()
        {
            var searchContext = new Mock<ISearchContext>(MockBehavior.Strict);
            searchContext.Setup(s => s.FindElement(By.Id("1234"))).Throws<NoSuchElementException>();
            searchContext.Setup(s => s.FindElement(By.Name("test"))).Throws<NoSuchElementException>();

            var element = new WebElement(searchContext.Object);
            element.UpdateLocators(new[] { By.Id("1234"), By.Name("test") });

            try
            {
                // ReSharper disable once UnusedVariable
                var result = element.WrappedElement;
            }
            catch (NoSuchElementException ex)
            {
                Assert.AreEqual("Could not find element by: By.Id: 1234, or: By.Name: test", ex.Message);

                searchContext.VerifyAll();

                throw;
            }
        }

        /// <summary>
        /// Creates the basic wrapped element.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <returns>The created element.</returns>
        private static WebElement CreateBasicWrappedElement(IWebElement targetElement)
        {
            var element = new WebElement(null);
            element.CloneNativeElement(targetElement);

            return element;
        }
    }
}