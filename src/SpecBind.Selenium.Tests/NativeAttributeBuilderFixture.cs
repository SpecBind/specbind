// <copyright file="NativeAttributeBuilderFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    /// <summary>
    /// Unit tests for the NativeAttributeBuilder class.
    /// </summary>
    [TestClass]
    public class NativeAttributeBuilderFixture
    {
        /// <summary>
        /// Tests the locator method by ID.
        /// </summary>
        [TestMethod]
        public void TestLocatorCreateId()
        {
            var attribute = new FindsByAttribute { How = How.Id, Using = "Foo" };

            var locator = NativeAttributeBuilder.GetLocator(attribute);

            Assert.AreEqual(By.Id("Foo"), locator);
        }

        /// <summary>
        /// Tests the locator method by name.
        /// </summary>
        [TestMethod]
        public void TestLocatorCreateName()
        {
            var attribute = new FindsByAttribute { How = How.Name, Using = "Foo" };

            var locator = NativeAttributeBuilder.GetLocator(attribute);

            Assert.AreEqual(By.Name("Foo"), locator);
        }

        /// <summary>
        /// Tests the locator method by tag name.
        /// </summary>
        [TestMethod]
        public void TestLocatorCreateTagName()
        {
            var attribute = new FindsByAttribute { How = How.TagName, Using = "div" };

            var locator = NativeAttributeBuilder.GetLocator(attribute);

            Assert.AreEqual(By.TagName("div"), locator);
        }

        /// <summary>
        /// Tests the locator method by class name.
        /// </summary>
        [TestMethod]
        public void TestLocatorCreateClassName()
        {
            var attribute = new FindsByAttribute { How = How.ClassName, Using = "btn" };

            var locator = NativeAttributeBuilder.GetLocator(attribute);

            Assert.AreEqual(By.ClassName("btn"), locator);
        }

        /// <summary>
        /// Tests the locator method by CSS selector.
        /// </summary>
        [TestMethod]
        public void TestLocatorCreateCssSelector()
        {
            var attribute = new FindsByAttribute { How = How.CssSelector, Using = "btn" };

            var locator = NativeAttributeBuilder.GetLocator(attribute);

            Assert.AreEqual(By.CssSelector("btn"), locator);
        }

        /// <summary>
        /// Tests the locator method by link text.
        /// </summary>
        [TestMethod]
        public void TestLocatorCreateLinkText()
        {
            var attribute = new FindsByAttribute { How = How.LinkText, Using = "Hello" };

            var locator = NativeAttributeBuilder.GetLocator(attribute);

            Assert.AreEqual(By.LinkText("Hello"), locator);
        }

        /// <summary>
        /// Tests the locator method by partial link text.
        /// </summary>
        [TestMethod]
        public void TestLocatorCreatePartialLinkText()
        {
            var attribute = new FindsByAttribute { How = How.PartialLinkText, Using = "Hello" };

            var locator = NativeAttributeBuilder.GetLocator(attribute);

            Assert.AreEqual(By.PartialLinkText("Hello"), locator);
        }

        /// <summary>
        /// Tests the locator method by XPath text.
        /// </summary>
        [TestMethod]
        public void TestLocatorCreateXPath()
        {
            var attribute = new FindsByAttribute { How = How.XPath, Using = "//tag" };

            var locator = NativeAttributeBuilder.GetLocator(attribute);

            Assert.AreEqual(By.XPath("//tag"), locator);
        }

        /// <summary>
        /// Tests the locator method by a custom class, not supported.
        /// </summary>
        [TestMethod]
        public void TestCustomClass()
        {
            var attribute = new FindsByAttribute { How = How.Custom, Using = "notsupported" };

            var locator = NativeAttributeBuilder.GetLocator(attribute);

            Assert.AreEqual(null, locator);
        }
    }
}