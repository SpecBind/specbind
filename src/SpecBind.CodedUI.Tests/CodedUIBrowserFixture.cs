// <copyright file="CodedUIBrowserFixture.cs">
//    Copyright © 2016 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI.Tests
{
    using BrowserSupport;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// A test fixture for the CodedUIBrowser class
    /// </summary>
    [CodedUITest]
    public class CodedUIBrowserFixture
    {
        [TestMethod]
        public void IsCreated_BeforeBrowserIsCreated_ReturnsFalse()
        {
            // Arrange
            CodedUIBrowserFactory browserFactory = new CodedUIBrowserFactory();

            // Act
            IBrowser browser = browserFactory.GetBrowser();

            // Assert
            Assert.IsFalse(browser.IsCreated);
        }

        [TestMethod]
        public void IsCreated_AfterBrowserIsCreated_ReturnsTrue()
        {
            // Arrange
            CodedUIBrowserFactory browserFactory = new CodedUIBrowserFactory();

            // Act
            IBrowser browser = browserFactory.GetBrowser();

            // trying to get the Url will launch the browser
            string url = browser.Url;

            // Assert
            Assert.IsTrue(browser.IsCreated);
        }

        /// <summary>
        /// Tests the can get URL method.
        /// </summary>
        [TestMethod]
        public void CanGetUrl_ReturnsTrue()
        {
            // Arrange
            CodedUIBrowserFactory browserFactory = new CodedUIBrowserFactory();

            // Act
            IBrowser browser = browserFactory.GetBrowser();

            // Assert
            Assert.IsTrue(browser.CanGetUrl());
        }
    }
}