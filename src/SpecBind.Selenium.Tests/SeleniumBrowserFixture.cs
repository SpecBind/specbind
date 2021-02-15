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
    using SpecBind.Selenium.Drivers;
    using SpecBind.Selenium.Tests.Resources;

    /// <summary>
    /// A test fixture for the Selenium Browser.
    /// </summary>
    /// <remarks>
    /// The browser under test needs to be disposed before the mocked driver is verified,
    /// because the browser's destructor disposes the driver.
    /// With strict mocks, we must set up the driver's disposal,
    /// but it must happen before we verify it.
    /// Therefore, we use a "using" statement for every browser we create in these tests,
    /// perform assertions related to the browser in that "using" statement,
    /// and then after that, we verify the driver.
    /// </remarks>
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
            var driver = this.CreateMockWebDriverNotExpectingInitialization();

            this.TestBrowserWith(driver, browser =>
            {
                var result = browser.BasePageType;
                Assert.IsNull(result);
            });
        }

        /// <summary>
        /// Tests the getting of the Url property returns the value from the window.
        /// </summary>
        [TestMethod]
        public void TestGetUrlReturnsBrowserUrl()
        {
            const string BrowserUrl = "http://www.mysite.com/home";
            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.SetupGet(d => d.Url).Returns(BrowserUrl);

            this.TestBrowserWith(driver, browser =>
            {
                var result = browser.Url;
                Assert.AreEqual(BrowserUrl, result);
            });
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

            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.Setup(d => d.Navigate()).Returns(navigation.Object);

            this.TestBrowserWith(driver, browser =>
            {
                browser.GoTo(url);
            });

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

            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.Setup(d => d.Manage()).Returns(options.Object);

            this.TestBrowserWith(driver, browser =>
            {
                browser.AddCookie("TestCookie", "TestValue", "/", expireDate, null, false);
            });

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

            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.Setup(d => d.Manage()).Returns(options.Object);

            this.TestBrowserWith(driver, browser =>
            {
                browser.AddCookie("TestCookie", "TestValue", "/", expireDate, null, false);
            });

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

            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.Setup(d => d.Manage()).Returns(options.Object);

            this.TestBrowserWith(driver, browser =>
            {
                browser.ClearCookies();
            });

            options.VerifyAll();
            cookies.VerifyAll();
        }

        /// <summary>
        /// Tests the GetCookie method returns a System.Net.Cookie
        /// </summary>
        [TestMethod]
        public void TestGetCookieReturnsCookieWhenExists()
        {
            const string CookieName = "TestCookie";
            const string CookieValue = "SomeValue";
            System.Net.Cookie cookie = null;
            var cookies = new Mock<ICookieJar>(MockBehavior.Strict);
            cookies.Setup(c => c.GetCookieNamed(CookieName)).Returns(new Cookie(CookieName, CookieValue));

            var options = new Mock<IOptions>(MockBehavior.Strict);
            options.Setup(o => o.Cookies).Returns(cookies.Object);

            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.Setup(d => d.Manage()).Returns(options.Object);

            this.TestBrowserWith(driver, browser =>
            {
                cookie = browser.GetCookie(CookieName);
            });

            options.VerifyAll();
            cookies.VerifyAll();
            Assert.AreEqual(CookieName, cookie.Name);
            Assert.AreEqual(CookieValue, cookie.Value);
        }

        /// <summary>
        /// Tests the GetCookie method returns null when a cookie with a given name was not found
        /// </summary>
        [TestMethod]
        public void TestGetCookieReturnsNullWhenCookieDoesNotExist()
        {
            System.Net.Cookie cookie = null;
            var cookies = new Mock<ICookieJar>(MockBehavior.Strict);
            cookies.Setup(c => c.GetCookieNamed(It.IsAny<string>())).Returns(null as Cookie);

            var options = new Mock<IOptions>(MockBehavior.Strict);
            options.Setup(o => o.Cookies).Returns(cookies.Object);

            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.Setup(d => d.Manage()).Returns(options.Object);

            this.TestBrowserWith(driver, browser =>
            {
                cookie = browser.GetCookie("not a cookie");
            });

            options.VerifyAll();
            cookies.VerifyAll();
            Assert.IsNull(cookie);
        }

        /// <summary>
        /// Tests the clear URL method.
        /// </summary>
        [TestMethod]
        public void TestClearUrl()
        {
            var expected = "about:blank";

            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.SetupSet(d => d.Url = expected);
            driver.SetupGet(d => d.Url).Returns(expected);

            this.TestBrowserWith(driver, browser =>
            {
                browser.ClearUrl();
                var actual = browser.Url;

                Assert.AreEqual(expected, actual);
            });
        }

        /// <summary>
        /// Tests the can get URL method without an alert box displayed.
        /// </summary>
        [TestMethod]
        public void CanGetUrl_WithoutAlertBoxDisplayed_ReturnsTrue()
        {
            var locator = new Mock<ITargetLocator>(MockBehavior.Strict);
            locator.Setup(l => l.Alert()).Returns<IAlert>(null);

            var url = new Uri("http://www.bing.com");

            var navigation = new Mock<INavigation>(MockBehavior.Strict);
            navigation.Setup(n => n.GoToUrl(url));

            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.Setup(d => d.Navigate()).Returns(navigation.Object);
            driver.Setup(d => d.SwitchTo()).Returns(locator.Object);

            this.TestBrowserWith(driver, browser =>
            {
                browser.GoTo(url);
                Assert.IsTrue(browser.CanGetUrl());
            });
        }

        /// <summary>
        /// Tests the can get URL method with an alert box displayed.
        /// </summary>
        [TestMethod]
        public void CanGetUrl_WithAlertBoxDisplayed_ReturnsFalse()
        {
            var alerter = new Mock<IAlert>(MockBehavior.Strict);

            var locator = new Mock<ITargetLocator>(MockBehavior.Strict);
            locator.Setup(l => l.Alert()).Returns(alerter.Object);

            var url = new Uri("http://www.bing.com");

            var navigation = new Mock<INavigation>(MockBehavior.Strict);
            navigation.Setup(n => n.GoToUrl(url));

            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.Setup(d => d.Navigate()).Returns(navigation.Object);
            driver.Setup(d => d.SwitchTo()).Returns(locator.Object);

            this.TestBrowserWith(driver, browser =>
            {
                browser.GoTo(url);
                Assert.IsFalse(browser.CanGetUrl());
            });
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when ok is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenOkIsChoosen()
        {
            this.TestAlertScenario(AlertBoxAction.Ok, true);
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when yes is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenYesIsChoosen()
        {
            this.TestAlertScenario(AlertBoxAction.Yes, true);
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when retry is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenRetryIsChoosen()
        {
            this.TestAlertScenario(AlertBoxAction.Retry, true);
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when cancel is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenCancelIsChoosen()
        {
            this.TestAlertScenario(AlertBoxAction.Cancel, false);
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when close is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenCloseIsChoosen()
        {
            this.TestAlertScenario(AlertBoxAction.Close, false);
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when ignore is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenIgnoreIsChoosen()
        {
            this.TestAlertScenario(AlertBoxAction.Ignore, false);
        }

        /// <summary>
        /// Tests the dismiss alert calls accept when no is chosen.
        /// </summary>
        [TestMethod]
        public void TestDismissAlertAcceptsWhenNoIsChoosen()
        {
            this.TestAlertScenario(AlertBoxAction.No, false);
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

            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.Setup(d => d.SwitchTo()).Returns(locator.Object);

            this.TestBrowserWith(driver, browser =>
            {
                browser.DismissAlert(AlertBoxAction.Ok, "My Text");
            });

            alerter.VerifyAll();
            locator.VerifyAll();
        }

        /// <summary>
        /// Tests the close method does nothing when the driver has not been called.
        /// </summary>
        [TestMethod]
        public void TestClosesDoesNothingWhenDriverIsNotInitialized()
        {
            var driver = new Mock<IWebDriverEx>(MockBehavior.Strict);

            this.TestBrowserWith(driver, browser =>
            {
                browser.Close();
            });
        }

        /// <summary>
        /// Tests the close method is called if the driver has been used.
        /// </summary>
        [TestMethod]
        public void TestClosesBrowserWhenDriverIsInitialized()
        {
            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.Setup(d => d.Close());

            this.InitializeDriverAndTestBrowserWith(driver, browser =>
            {
                browser.Close();
            });
        }

        /// <summary>
        /// Tests the getting the page location returns the url value.
        /// </summary>
        [TestMethod]
        public void TestGetNativePageLocationReturnsUrl()
        {
            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.SetupGet(d => d.Url).Returns("http://localhost:2222/MyPage");

            this.TestBrowserWith(driver, browser =>
            {
                browser.EnsureOnPage<MyPage>();
            });
        }

        /// <summary>
        /// Tests the dispose method does nothing when the driver has not been called.
        /// </summary>
        [TestMethod]
        public void TestDisposeDoesNothingWhenDriverIsNotInitialized()
        {
            var driver = this.CreateMockWebDriverNotExpectingInitialization();

            this.TestBrowserWith(driver, browser =>
            {
            });
        }

        /// <summary>
        /// Tests the dispose method is called if the driver has been used.
        /// </summary>
        [TestMethod]
        public void TestDisposeClosesBrowserWhenDriverIsInitialized()
        {
            var driver = this.CreateMockWebDriverExpectingInitialization();

            this.InitializeDriverAndTestBrowserWith(driver, browser =>
            {
            });
        }

        /// <summary>
        /// Tests the dispose is only called once.
        /// </summary>
        [TestMethod]
        public void TestDisposeWhenCalledTwiceOnlyDisposesOnce()
        {
            var driver = this.CreateMockWebDriverExpectingInitialization();

            this.InitializeDriverAndTestBrowserWith(driver, browser =>
            {
                browser.Dispose();
            });

            driver.Verify(d => d.Quit(), Times.Exactly(1));
            driver.Verify(d => d.Dispose(), Times.Exactly(1));
        }

        /// <summary>
        /// Tests the execute script method for the browser when it exists.
        /// </summary>
        [TestMethod]
        public void TestExecuteScriptWhenDriverSupportItRunsScript()
        {
            var result = new object();
            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.As<IJavaScriptExecutor>().Setup(e => e.ExecuteScript("some script", It.IsAny<object[]>()))
                                            .Returns(result);

            this.InitializeDriverAndTestBrowserWith(driver, browser =>
            {
                var resultObject = browser.ExecuteScript("some script", "Hello");
                Assert.AreSame(result, resultObject);
            });
        }

        /// <summary>
        /// Tests the execute script method for the browser when it exists.
        /// </summary>
        [TestMethod]
        public void TestExecuteScriptWhenResultIsNativeElementReturnsProxyClass()
        {
            var result = new Mock<IWebElement>(MockBehavior.Strict);
            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.As<IJavaScriptExecutor>().Setup(e => e.ExecuteScript("some script", It.IsAny<object[]>()))
                                            .Returns(result.Object);

            this.InitializeDriverAndTestBrowserWith(driver, browser =>
            {
                var resultObject = browser.ExecuteScript("some script", "Hello");

                Assert.IsNotNull(resultObject);
                Assert.IsInstanceOfType(resultObject, typeof(WebElement));
            });

            result.VerifyAll();
        }

        /// <summary>
        /// Tests the execute script method when it doesn't support it returns null.
        /// </summary>
        [TestMethod]
        public void TestExecuteScriptWhenDriverDoesNotSupportScriptReturnsNull()
        {
            var driver = this.CreateMockWebDriverExpectingInitialization();

            this.InitializeDriverAndTestBrowserWith(driver, browser =>
            {
                var resultObject = browser.ExecuteScript("some script", "Hello");

                Assert.IsNull(resultObject);
            });
        }

        /// <summary>
        /// Tests the take screenshot method does nothing when the interface is not supported.
        /// </summary>
        [TestMethod]
        public void TestTakeScreenshotWhenNotSupportedDoesNothing()
        {
            var driver = this.CreateMockWebDriverExpectingInitialization();

            this.TestBrowserWith(driver, browser =>
            {
                var fullPath = browser.TakeScreenshot(@"C:\somepath", "fileBase");

                Assert.IsNull(fullPath);
            });
        }

        /// <summary>
        /// Tests the take screenshot method does nothing when the interface is not supported.
        /// </summary>
        [TestMethod]
        public void TestTakeScreenshotWhenScreenshotErrorsReturnsNothing()
        {
            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.As<ITakesScreenshot>().Setup(s => s.GetScreenshot()).Throws<InvalidOperationException>();

            this.TestBrowserWith(driver, browser =>
            {
                var fullPath = browser.TakeScreenshot(@"C:\somepath", "fileBase");

                Assert.IsNull(fullPath);
            });
        }

        /// <summary>
        /// Tests the take screenshot method performs the appropriate calls.
        /// </summary>
        [TestMethod]
        public void TestTakeScreenshotWhenCalledWithoutErrorTakesScreenshot()
        {
            var driver = this.CreateMockWebDriverExpectingInitialization();
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

            this.TestBrowserWith(driver, browser =>
            {
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
            });
        }

        /// <summary>
        /// Asserts that a call to IsCreated returns false before the browser is created.
        /// </summary>
        [TestMethod]
        public void IsCreated_BeforeBrowserIsCreated_ReturnsFalse()
        {
            // Arrange
            SeleniumBrowserFactory browserFactory = new SeleniumBrowserFactory(null);

            // Act
            IBrowser browser = browserFactory.GetBrowser();

            // Assert
            Assert.IsFalse(browser.IsCreated);
        }

        /// <summary>
        /// Asserts that a call to IsCreated returns true after the browser is created.
        /// </summary>
        [TestMethod]
        [DeploymentItem("IEDriverServer.exe")]
        public void IsCreated_AfterBrowserIsCreated_ReturnsTrue()
        {
            // Arrange
            SeleniumBrowserFactory browserFactory = new SeleniumBrowserFactory(null);

            // Act
            IBrowser browser = browserFactory.GetBrowser();

            // trying to get the Url will launch the browser
            string url = browser.Url;

            // Assert
            Assert.IsTrue(browser.IsCreated);
        }

        private void InitializeDriverAndTestBrowserWith(Mock<IWebDriverEx> driver, Action<SeleniumBrowser> test)
        {
            this.TestBrowserWith(driver, true, test);
        }

        private void TestBrowserWith(Mock<IWebDriverEx> driver, Action<SeleniumBrowser> test)
        {
            this.TestBrowserWith(driver, false, test);
        }

        private void TestBrowserWith(Mock<IWebDriverEx> driver, bool initializeDriver, Action<SeleniumBrowser> test)
        {
            var lazyDriver = new Lazy<IWebDriverEx>(() => driver.Object);
            if (initializeDriver)
            {
                Assert.IsNotNull(lazyDriver.Value);
            }

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            using (var browser = new SeleniumBrowser(lazyDriver, logger.Object))
            {
                test(browser);
            }

            driver.VerifyAll();
        }

        private Mock<IWebDriverEx> CreateMockWebDriverExpectingInitialization()
        {
            var driver = new Mock<IWebDriverEx>(MockBehavior.Strict);

            // NOTE: the SeleniumBrowser quits and disposes its driver as part of its destructor,
            // So every driver we create that will get initialized has to have this set up,
            // if we're going to use MockBehavior.Strict.
            driver.Setup(d => d.Quit());
            driver.Setup(d => d.Dispose());

            return driver;
        }

        private Mock<IWebDriverEx> CreateMockWebDriverNotExpectingInitialization()
        {
            return new Mock<IWebDriverEx>(MockBehavior.Strict);
        }

        /// <summary>
        /// Tests the alert scenario.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="acceptResult">if set to <c>true</c> the result should accept; otherwise it should dismiss.</param>
        private void TestAlertScenario(AlertBoxAction action, bool acceptResult)
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

            var driver = this.CreateMockWebDriverExpectingInitialization();
            driver.Setup(d => d.SwitchTo()).Returns(locator.Object);

            this.TestBrowserWith(driver, browser =>
            {
                browser.DismissAlert(action, null);
            });

            alerter.VerifyAll();
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