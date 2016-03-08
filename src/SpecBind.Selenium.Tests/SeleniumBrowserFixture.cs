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

    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
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
            var logger = new Mock<ILogger>(MockBehavior.Loose);
            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

            var result = browser.BasePageType;

            Assert.IsNull(result);
            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the getting of the Url property returns the value from the window.
        /// </summary>
        [TestMethod]
        public void TestGetUrlReturnsBrowserUrl()
        {
            const string BrowserUrl = "http://www.mysite.com/home";
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.SetupGet(d => d.Url).Returns(BrowserUrl);

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

            var result = browser.Url;

            Assert.AreEqual(BrowserUrl, result);
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

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

            browser.GoTo(url);

            driver.VerifyAll();
            navigation.VerifyAll();
        }

        /// <summary>
        /// Tests the add cookie calls driver method.
        /// </summary>
        [TestMethod]
        public void TestAddCookieWhenCookieDoesntExistCallsDriverMethod()
        {
            var expireDate = DateTime.Now;

            var cookies = new Mock<ICookieJar>(MockBehavior.Strict);
            cookies.Setup(c => c.GetCookieNamed("TestCookie")).Returns((Cookie)null);
            cookies.Setup(c => c.AddCookie(It.Is<Cookie>(ck => ck.Name == "TestCookie" && ck.Value == "TestValue" && ck.Path == "/" && ck.Expiry == expireDate)));

            var options = new Mock<IOptions>(MockBehavior.Strict);
            options.Setup(o => o.Cookies).Returns(cookies.Object);

            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.Setup(d => d.Manage()).Returns(options.Object);

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

            browser.AddCookie("TestCookie", "TestValue", "/", expireDate, null, false);

            driver.VerifyAll();
            options.VerifyAll();
            cookies.VerifyAll();
        }

        /// <summary>
        /// Tests the add cookie calls driver method.
        /// </summary>
        [TestMethod]
        public void TestAddCookieWhenCookieExistsCallsDriverMethodAndRemovesExistingCookie()
        {
            var expireDate = DateTime.Now;

            var cookies = new Mock<ICookieJar>(MockBehavior.Strict);
            cookies.Setup(c => c.GetCookieNamed("TestCookie")).Returns(new Cookie("TestCookie", "SomeValue"));
            cookies.Setup(c => c.DeleteCookieNamed("TestCookie"));
            cookies.Setup(c => c.AddCookie(It.Is<Cookie>(ck => ck.Name == "TestCookie" && ck.Value == "TestValue" && ck.Path == "/" && ck.Expiry == expireDate)));

            var options = new Mock<IOptions>(MockBehavior.Strict);
            options.Setup(o => o.Cookies).Returns(cookies.Object);

            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.Setup(d => d.Manage()).Returns(options.Object);

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

            browser.AddCookie("TestCookie", "TestValue", "/", expireDate, null, false);

            driver.VerifyAll();
            options.VerifyAll();
            cookies.VerifyAll();
        }

        /// <summary>
        /// Tests the clear cookies method.
        /// </summary>
        [TestMethod]
        public void TestClearCookies()
        {
            var cookies = new Mock<ICookieJar>(MockBehavior.Strict);
            cookies.Setup(c => c.DeleteAllCookies());

            var options = new Mock<IOptions>(MockBehavior.Strict);
            options.Setup(o => o.Cookies).Returns(cookies.Object);

            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.Setup(d => d.Manage()).Returns(options.Object);

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

            browser.ClearCookies();

            driver.VerifyAll();
            options.VerifyAll();
            cookies.VerifyAll();
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when ok is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenOkIsChoosen()
        {
            TestAlertScenario(AlertBoxAction.Ok, true);
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when yes is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenYesIsChoosen()
        {
            TestAlertScenario(AlertBoxAction.Yes, true);
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when retry is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenRetryIsChoosen()
        {
            TestAlertScenario(AlertBoxAction.Retry, true);
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when cancel is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenCancelIsChoosen()
        {
            TestAlertScenario(AlertBoxAction.Cancel, false);
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when close is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenCloseIsChoosen()
        {
            TestAlertScenario(AlertBoxAction.Close, false);
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when ignore is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenIgnoreIsChoosen()
        {
            TestAlertScenario(AlertBoxAction.Ignore, false);
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when no is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenNoIsChoosen()
        {
            TestAlertScenario(AlertBoxAction.No, false);
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when ok is chosen after entering text.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertEntersTextAndAcceptsWhenOkIsChoosen()
        {
            var alerter = new Mock<IAlert>(MockBehavior.Strict);
            alerter.Setup(a => a.SendKeys("My Text"));
            alerter.Setup(a => a.Accept());

            var locator = new Mock<ITargetLocator>(MockBehavior.Strict);
            locator.Setup(l => l.Alert()).Returns(alerter.Object);

            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.Setup(d => d.SwitchTo()).Returns(locator.Object);

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

            browser.DismissAlert(AlertBoxAction.Ok, "My Text");

            alerter.VerifyAll();
            driver.VerifyAll();
            locator.VerifyAll();
        }

        /// <summary>
        /// Tests the close method does nothing when the driver has not been called.
        /// </summary>
        [TestMethod]
        public void TestClosesDoenNothingWhenDriverIsNotInitialized()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

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

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(lazyDriver, logger.Object);

            browser.Close();

            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the close method when dispose is true.
        /// </summary>
        [TestMethod]
        public void TestCloseWhenDisposeIsTrue()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            var logger = new Mock<ILogger>(MockBehavior.Loose);
            var browser = new Mock<SeleniumBrowser>(new Lazy<IWebDriver>(() => driver.Object), logger.Object) { CallBase = true };

            browser.Object.Close(true);

            browser.Verify(b => b.Dispose());
        }

        /// <summary>
        /// Tests the close method when dispose is false.
        /// </summary>
        [TestMethod]
        public void TestCloseWhenDisposeIsFalse()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            var logger = new Mock<ILogger>(MockBehavior.Loose);
            var browser = new Mock<SeleniumBrowser>(new Lazy<IWebDriver>(() => driver.Object), logger.Object) { CallBase = true };

            browser.Object.Close(false);

            browser.Verify(b => b.Dispose(), Times.Never());
        }

        /// <summary>
        /// Tests the getting the page location returns the url value.
        /// </summary>
        [TestMethod]
        public void TestGetNativePageLocationReturnsUrl()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.SetupGet(d => d.Url).Returns("http://localhost:2222/MyPage");

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

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

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

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

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(lazyDriver, logger.Object);

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

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(lazyDriver, logger.Object);

            browser.Dispose();
            browser.Dispose();

            driver.Verify(d => d.Quit(), Times.Exactly(1));
            driver.Verify(d => d.Dispose(), Times.Exactly(1));
            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the execute script method for the browser when it exists.
        /// </summary>
        [TestMethod]
        public void TestExecuteScriptWhenDriverSupportItRunsScript()
        {
            var result = new object();
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.As<IJavaScriptExecutor>().Setup(e => e.ExecuteScript("some script", It.IsAny<object[]>()))
                                            .Returns(result);

            var lazyDriver = new Lazy<IWebDriver>(() => driver.Object);

            Assert.IsNotNull(lazyDriver.Value);

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(lazyDriver, logger.Object);

            var resultObject = browser.ExecuteScript("some script", "Hello");

            Assert.AreSame(result, resultObject);

            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the execute script method for the browser when it exists.
        /// </summary>
        [TestMethod]
        public void TestExecuteScriptWhenResultIsNativeElementReturnsProxyClass()
        {
            var result = new Mock<IWebElement>(MockBehavior.Strict);
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.As<IJavaScriptExecutor>().Setup(e => e.ExecuteScript("some script", It.IsAny<object[]>()))
                                            .Returns(result.Object);

            var lazyDriver = new Lazy<IWebDriver>(() => driver.Object);

            Assert.IsNotNull(lazyDriver.Value);

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(lazyDriver, logger.Object);

            var resultObject = browser.ExecuteScript("some script", "Hello");

            Assert.IsNotNull(resultObject);
            Assert.IsInstanceOfType(resultObject, typeof(WebElement));

            driver.VerifyAll();
            result.VerifyAll();
        }

        /// <summary>
        /// Tests the execute script method when it doesn't support it returns null.
        /// </summary>
        [TestMethod]
        public void TestExecuteScriptWhenDriverDoesNotSupportScriptReturnsNull()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);

            var lazyDriver = new Lazy<IWebDriver>(() => driver.Object);

            Assert.IsNotNull(lazyDriver.Value);

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(lazyDriver, logger.Object);

            var resultObject = browser.ExecuteScript("some script", "Hello");

            Assert.IsNull(resultObject);

            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the take screenshot method does nothing when the interface is not supported.
        /// </summary>
        [TestMethod]
        public void TestTakeScreenshotWhenNotSupportedDoesNothing()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

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

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

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

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

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
        /// Tests the alert scenario.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="acceptResult">if set to <c>true</c> the result should accept; otherwise it should dismiss.</param>
        private static void TestAlertScenario(AlertBoxAction action, bool acceptResult)
        {
            var alerter = new Mock<IAlert>(MockBehavior.Strict);

            if (acceptResult)
            {
                alerter.Setup(a => a.Accept());
            }
            else
            {
                alerter.Setup(a => a.Dismiss());
            }

            var locator = new Mock<ITargetLocator>(MockBehavior.Strict);
            locator.Setup(l => l.Alert()).Returns(alerter.Object);

            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            driver.Setup(d => d.SwitchTo()).Returns(locator.Object);

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new SeleniumBrowser(new Lazy<IWebDriver>(() => driver.Object), logger.Object);

            browser.DismissAlert(action, null);

            alerter.VerifyAll();
            driver.VerifyAll();
            locator.VerifyAll();
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