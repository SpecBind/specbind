// <copyright file="LocatorBuilderFixture.cs">
//    Copyright © 2014 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Tests
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OpenQA.Selenium;

    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the locator builder class.
    /// </summary>
    [TestClass]
    public class LocatorBuilderFixture
    {
        /// <summary>
        /// Tests the attribute with identifier returns identifier locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithIdReturnsIdLocator()
        {
            var attribute = new ElementLocatorAttribute { Id = "MyId" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.Id("MyId"), item);
        }

        /// <summary>
        /// Tests the attribute with a CSS selector
        /// </summary>
        [TestMethod]
        public void TestAttributeWithIdReturnsCssSelectorLocator()
        {
            var attribute = new ElementLocatorAttribute { CssSelector = "div#MyId" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.CssSelector("div#MyId"), item);
        }

        /// <summary>
        /// Tests the attribute with name returns name locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithNameReturnsNameLocator()
        {
            var attribute = new ElementLocatorAttribute { Name = "MyName" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.Name("MyName"), item);
        }

        /// <summary>
        /// Tests the attribute with CSS class returns CSS class locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithCssClassReturnsCssClassLocator()
        {
            var attribute = new ElementLocatorAttribute { Class = ".something" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.ClassName(".something"), item);
        }

        /// <summary>
        /// Tests the attribute with link text returns link text locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithLinkTextReturnsLinkTextLocator()
        {
            var attribute = new ElementLocatorAttribute { Text = "Hello World" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.LinkText("Hello World"), item);
        }

        /// <summary>
        /// Tests the attribute with tag name returns tag locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithTagNameReturnsTagLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.TagName("input"), item);
        }

        /// <summary>
        /// Tests the attribute with tag name and type returns x path locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithTagNameAndTypeReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Type = "email" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//input[@type='email']"), item);
        }

        /// <summary>
        /// Tests the attribute with tag name and title returns x path locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithTagNameAndTitleReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Title = "Page" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//input[@title='Page']"), item);
        }

        /// <summary>
        /// Tests the attribute with tag name and value returns x path locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithTagNameAndValueReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Value = "test" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//input[@value='test']"), item);
        }

        /// <summary>
        /// Tests the attribute with tag name and alt returns x path locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithTagNameAndAltReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Alt = "test" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//input[@alt='test']"), item);
        }

        /// <summary>
        /// Tests the attribute with XPath property returns XPath locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithXPathReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { XPath = ".//[@element='row']" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath(".//[@element='row']"), item);
        }

        /// <summary>
        /// Tests the attribute with image tag and URL returns x path locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithImageTagAndUrlReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "img", Url = "myimage.png" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//img[@src='myimage.png']"), item);
        }

        /// <summary>
        /// Tests the attribute with link tag and URL returns x path locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithLinkTagAndUrlReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "a", Url = "mylink.htm" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//a[@href='mylink.htm']"), item);
        }

        /// <summary>
        /// Tests the attribute with link area tag and URL returns x path locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithLinkAreaTagAndUrlReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "area", Url = "mylink.htm" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//area[@href='mylink.htm']"), item);
        }

        /// <summary>
        /// Tests the attribute with tag name and index returns x path locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithTagNameAndIndexReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Index = 1 };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//input[0]"), item);
        }

        /// <summary>
        /// Tests the attribute with tag name and type and index returns complex x path locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithTagNameAndTypeAndIndexReturnsComplexXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Type = "email", Index = 1 };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("(//input[@type='email'])[0]"), item);
        }

        /// <summary>
        /// Tests the attribute with tag name and type and title returns compound x path locator.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithTagNameAndTypeAndTitleReturnsCompoundXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Type = "email", Title = "my title" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//input[@title='my title' and @type='email']"), item);
        }

        /// <summary>
        /// Tests the attribute with identifier and tag name returns two locators.
        /// </summary>
        [TestMethod]
        public void TestAttributeWithIdAndTagNameReturnsTwoLocators()
        {
            var attribute = new ElementLocatorAttribute { Id = "MyId", TagName = "a" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(2, resultList.Count);

            var item = resultList.First();
            Assert.AreEqual(By.Id("MyId"), item);

            var item2 = resultList.Last();
            Assert.AreEqual(By.TagName("a"), item2);
        }

        /// <summary>
        /// Tests the attribute with no tag name and property throws an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestAttributeWithNoTagNameAndPropertyThrowsAnException()
        {
            var attribute = new ElementLocatorAttribute { Type = "submit" };

            LocatorBuilder.GetElementLocators(attribute);
        }
    }
}