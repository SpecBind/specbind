// <copyright file="SeleniumBrowserFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing.Imaging;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using OpenQA.Selenium;

    using SpecBind.Pages;
    using SpecBind.Selenium.Tests.Resources;

    /// <summary>
    /// A test fixture for the Selenium Browser.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SeleniumBrowserFixture
    {
        /// <summary>
        /// Tests the getting of the page base type, expecting it to return null.
        /// </summary>
        [TestMethod]
        public void TestGetPageBaseTypeReturnsNull()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object));

            var result = browser.BasePageType;

            Assert.IsNull(result);
            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the getting of the goto page calls the appropriate driver method.
        /// </summary>
        [TestMethod]
        public void TestGotoPageCallsDriverMethod()
        {
            var url = new Uri("http://www.bing.com");

            var navigation = new Mock<INavigation>(MockBehavior.Strict);
            navigation.Setup(n => n.GoToUrl(url));

            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.Setup(d => d.Navigate()).Returns(navigation.Object);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object));

            browser.GoTo(url);

            driver.VerifyAll();
            navigation.VerifyAll();
        }

        /// <summary>
        /// Tests the close method does nothing when the driver has not been called.
        /// </summary>
        [TestMethod]
        public void TestClosesDoenNothingWhenDriverIsNotInitialized()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            
            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object));

            browser.Close();

            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the close method is called if the driver has been used.
        /// </summary>
        [TestMethod]
        public void TestClosesBrowserWhenDriverIsInitialized()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.Setup(d => d.Close());

            var lazyDriver = new Lazy<IWebDriver>(() => driver.Object);
           
            Assert.IsNotNull(lazyDriver.Value);

            var browser = new SeleniumBrowser(lazyDriver);

            browser.Close();

            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the getting the page location returns the url value.
        /// </summary>
        [TestMethod]
        public void TestGetNativePageLocationReturnsUrl()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.SetupGet(d => d.Url).Returns("http://localhost/MyPage");

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object));

            browser.EnsureOnPage<MyPage>();

            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the dispose method does nothing when the driver has not been called.
        /// </summary>
        [TestMethod]
        public void TestDisposeDoesNothingWhenDriverIsNotInitialized()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object));

            browser.Dispose();

            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the dispose method is called if the driver has been used.
        /// </summary>
        [TestMethod]
        public void TestDisposeClosesBrowserWhenDriverIsInitialized()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.Setup(d => d.Quit());
            driver.Setup(d => d.Dispose());

            var lazyDriver = new Lazy<IWebDriver>(() => driver.Object);

            Assert.IsNotNull(lazyDriver.Value);

            var browser = new SeleniumBrowser(lazyDriver);

            browser.Dispose();

            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the dispose is only called once.
        /// </summary>
        [TestMethod]
        public void TestDisposeWhenCalledTwiceOnlyDisposesOnce()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.Setup(d => d.Quit());
            driver.Setup(d => d.Dispose());

            var lazyDriver = new Lazy<IWebDriver>(() => driver.Object);

            Assert.IsNotNull(lazyDriver.Value);

            var browser = new SeleniumBrowser(lazyDriver);

            browser.Dispose();
            browser.Dispose();

            driver.Verify(d => d.Quit(), Times.Exactly(1));
            driver.Verify(d => d.Dispose(), Times.Exactly(1));
            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the take screenshot method does nothing when the interface is not supported.
        /// </summary>
        [TestMethod]
        public void TestTakeScreenshotWhenNotSupportedDoesNothing()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            
            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object));

            var fullPath = browser.TakeScreenshot(@"C:\somepath", "fileBase");

            Assert.IsNull(fullPath);
            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the take screenshot method does nothing when the interface is not supported.
        /// </summary>
        [TestMethod]
        public void TestTakeScreenshotWhenScreenshotErrorsReturnsNothing()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.As<ITakesScreenshot>().Setup(s => s.GetScreenshot()).Throws<InvalidOperationException>();

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object));

            var fullPath = browser.TakeScreenshot(@"C:\somepath", "fileBase");

            Assert.IsNull(fullPath);
            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the take screenshot method performs the appropriate calls.
        /// </summary>
        [TestMethod]
        public void TestTakeScreenshotWhenCalledWithoutErrorTakesScreenshot()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.As<ITakesScreenshot>().Setup(s => s.GetScreenshot()).Throws<InvalidOperationException>();

            var basePath = Path.GetTempPath();
            var fileName = Guid.NewGuid().ToString();
            
            Screenshot screenshot;
            using (var ms = new MemoryStream())
            {
                var image = TestResource.TestImage;
                image.Save(ms, ImageFormat.Jpeg);
                
                screenshot = new Screenshot(Convert.ToBase64String(ms.ToArray()));
            }

            var screenShot = driver.As<ITakesScreenshot>();
            screenShot.Setup(s => s.GetScreenshot()).Returns(screenshot);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object));

            string fullPath = null;
            try
            {
                fullPath = browser.TakeScreenshot(basePath, fileName);
            }
            finally
            {
                if (fullPath != null && File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            Assert.IsNotNull(fullPath);
            driver.VerifyAll();
        }

        /// <summary>
        /// A test page class.
        /// </summary>
        [PageNavigation("/MyPage")]
        public class MyPage
        {
        }
    }
}