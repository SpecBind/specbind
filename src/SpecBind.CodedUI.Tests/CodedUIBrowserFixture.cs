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
    [TestClass]
    public class CodedUIBrowserFixture
    {
        /// <summary>
        /// Runs before executing each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            if (!Playback.IsInitialized)
            {
                Playback.Initialize();
            }
        }

        /// <summary>
        /// Runs after executing each test.
        /// </summary>
        [TestCleanup]
        public void After()
        {
            if (Playback.IsInitialized)
            {
                Playback.Cleanup();
            }
        }

        /// <summary>
        /// Asserts that the IsCreated propery value is false before the browser is created.
        /// </summary>
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

        /// <summary>
        /// Asserts that the IsCreated property value is true after the browser is created.
        /// </summary>
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