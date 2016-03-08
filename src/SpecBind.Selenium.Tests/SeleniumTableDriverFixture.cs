// <copyright file="SeleniumTableDriverFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using OpenQA.Selenium;

    using SpecBind.BrowserSupport;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the SeleniumTableDriver.
    /// </summary>
    [TestClass]
    public class SeleniumTableDriverFixture
    {
        /// <summary>
        /// Tests the get empty list to ensure it returns no results but doesn't fail.
        /// </summary>
        [TestMethod]
        public void TestGetEmptyListReturnsNoResults()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var parent = new Mock<IWebElement>(MockBehavior.Strict);
            parent.Setup(p => p.FindElements(By.TagName("tr"))).Returns(new List<IWebElement>(0).AsReadOnly());


            var tableDriver = new SeleniumTableDriver(parent.Object, browser.Object);

            var result = tableDriver.FirstOrDefault();

            Assert.IsNull(result);

            parent.VerifyAll();
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the get empty list to ensure it returns no results but doesn't fail.
        /// </summary>
        [TestMethod]
        public void TestGetEmptyListWithNullDriverSearchResultsNoResults()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var parent = new Mock<IWebElement>(MockBehavior.Strict);
            parent.Setup(p => p.FindElements(By.TagName("tr"))).Returns((ReadOnlyCollection<IWebElement>)null);


            var tableDriver = new SeleniumTableDriver(parent.Object, browser.Object);

            var result = tableDriver.FirstOrDefault();

            Assert.IsNull(result);

            parent.VerifyAll();
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the get the item with no table headers returns the item but no columns.
        /// </summary>
        [TestMethod]
        public void TestGetItemWithNoHeadersReturnsItemButNotColumns()
        {
            var firstRow = new Mock<IWebElement>(MockBehavior.Strict);
            firstRow.SetupGet(f => f.Displayed).Returns(true);
            firstRow.Setup(f => f.FindElements(By.TagName("td")))
                    .Returns(new ReadOnlyCollection<IWebElement>(new IWebElement[0]));
            firstRow.Setup(f => f.FindElements(By.TagName("th")))
                    .Returns(new ReadOnlyCollection<IWebElement>(new IWebElement[0]));

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var parent = new Mock<IWebElement>(MockBehavior.Strict);
            parent.Setup(p => p.FindElements(By.TagName("tr")))
                  .Returns(new List<IWebElement> { firstRow.Object }.AsReadOnly());


            var tableDriver = new SeleniumTableDriver(parent.Object, browser.Object);

            var result = tableDriver.FirstOrDefault();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(SeleniumTableDriver.RowWrapper));
            Assert.IsInstanceOfType(result, typeof(IElementProvider));

            var elementList = ((IElementProvider)result).GetElements();

            Assert.IsNotNull(elementList);
            Assert.AreEqual(0, elementList.Count());

            parent.VerifyAll();
            browser.VerifyAll();
            firstRow.VerifyAll();
        }

        /// <summary>
        /// Tests the get the item with no table headers but no rows returns the columns.
        /// </summary>
        [TestMethod]
        public void TestGetItemWithHeadersButNoRowsReturnsNull()
        {
            var oneColumn = new Mock<IWebElement>(MockBehavior.Strict);
            oneColumn.SetupGet(o => o.Text).Returns("MyColumn");

            var firstRow = new Mock<IWebElement>(MockBehavior.Strict);
            firstRow.Setup(f => f.FindElements(By.TagName("th")))
                    .Returns(new List<IWebElement> { oneColumn.Object }.AsReadOnly());

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var parent = new Mock<IWebElement>(MockBehavior.Strict);
            parent.Setup(p => p.FindElements(By.TagName("tr")))
                  .Returns(new List<IWebElement> { firstRow.Object }.AsReadOnly());


            var tableDriver = new SeleniumTableDriver(parent.Object, browser.Object);

            var result = tableDriver.FirstOrDefault();

            Assert.IsNull(result);

            parent.VerifyAll();
            browser.VerifyAll();
            firstRow.VerifyAll();
            oneColumn.VerifyAll();
        }

        /// <summary>
        /// Tests the get the item with table headers and rows returns the data.
        /// </summary>
        [TestMethod]
        public void TestGetItemWithHeadersAndMultipleRowsReturnsData()
        {
            var oneColumn = new Mock<IWebElement>(MockBehavior.Strict);
            oneColumn.SetupGet(o => o.Text).Returns("MyColumn");

            var firstRow = new Mock<IWebElement>(MockBehavior.Strict);
            firstRow.Setup(f => f.FindElements(By.TagName("th")))
                    .Returns(new List<IWebElement> { oneColumn.Object }.AsReadOnly());

            var cell = new Mock<IWebElement>(MockBehavior.Strict);
            cell.Setup(c => c.Text).Returns("Hello!");

            var secondRow = new Mock<IWebElement>(MockBehavior.Strict);
            secondRow.SetupGet(s => s.Displayed).Returns(true);
            secondRow.Setup(f => f.FindElements(By.TagName("td")))
                    .Returns(new List<IWebElement> { cell.Object }.AsReadOnly());

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var parent = new Mock<IWebElement>(MockBehavior.Strict);
            parent.Setup(p => p.FindElements(By.TagName("tr")))
                  .Returns(new List<IWebElement> { firstRow.Object, secondRow.Object }.AsReadOnly());


            var tableDriver = new SeleniumTableDriver(parent.Object, browser.Object);

            var result = tableDriver.FirstOrDefault();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(SeleniumTableDriver.RowWrapper));
            Assert.IsInstanceOfType(result, typeof(IElementProvider));

            var elementList = ((IElementProvider)result).GetElements().ToList();

            Assert.IsNotNull(elementList);
            Assert.AreEqual(1, elementList.Count);

            var item = elementList.First();
            Assert.AreEqual("mycolumn", item.PropertyName);
            Assert.AreEqual(typeof(IWebElement), item.PropertyType);
            Assert.IsNotNull(item.Value);

            Assert.AreEqual("Hello!", ((IWebElement)item.Value).Text);

            parent.VerifyAll();
            browser.VerifyAll();
            firstRow.VerifyAll();
            oneColumn.VerifyAll();
            cell.VerifyAll();
        }

        /// <summary>
        /// Tests the get the item with table headers that contains whitespace are trimmed and returned.
        /// </summary>
        [TestMethod]
        public void TestGetItemWithHeadersThatContainWhitespaceAndMultipleRowsReturnsData()
        {
            var oneColumn = new Mock<IWebElement>(MockBehavior.Strict);
            oneColumn.SetupGet(o => o.Text).Returns(" MyColumn  ");

            var firstRow = new Mock<IWebElement>(MockBehavior.Strict);
            firstRow.Setup(f => f.FindElements(By.TagName("th")))
                    .Returns(new List<IWebElement> { oneColumn.Object }.AsReadOnly());

            var cell = new Mock<IWebElement>(MockBehavior.Strict);
            cell.Setup(c => c.Text).Returns("Hello!");

            var secondRow = new Mock<IWebElement>(MockBehavior.Strict);
            secondRow.SetupGet(s => s.Displayed).Returns(true);
            secondRow.Setup(f => f.FindElements(By.TagName("td")))
                    .Returns(new List<IWebElement> { cell.Object }.AsReadOnly());

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var parent = new Mock<IWebElement>(MockBehavior.Strict);
            parent.Setup(p => p.FindElements(By.TagName("tr")))
                  .Returns(new List<IWebElement> { firstRow.Object, secondRow.Object }.AsReadOnly());


            var tableDriver = new SeleniumTableDriver(parent.Object, browser.Object);

            var result = tableDriver.FirstOrDefault();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(SeleniumTableDriver.RowWrapper));
            Assert.IsInstanceOfType(result, typeof(IElementProvider));

            var elementList = ((IElementProvider)result).GetElements().ToList();

            Assert.IsNotNull(elementList);
            Assert.AreEqual(1, elementList.Count);

            var item = elementList.First();
            Assert.AreEqual("mycolumn", item.PropertyName);
            Assert.AreEqual(typeof(IWebElement), item.PropertyType);
            Assert.IsNotNull(item.Value);

            Assert.AreEqual("Hello!", ((IWebElement)item.Value).Text);

            parent.VerifyAll();
            browser.VerifyAll();
            firstRow.VerifyAll();
            oneColumn.VerifyAll();
            cell.VerifyAll();
        }
    }
}
