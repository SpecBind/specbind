// <copyright file="BrowserBaseFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Tests
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;
    using Moq.Protected;

    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the <see cref="BrowserBase"/> class.
    /// </summary>
    [TestClass]
    public class BrowserBaseFixture
    {
        /// <summary>
        /// Tests the GetPage generic method as an interface creates the page from a native page.
        /// </summary>
        [TestMethod]
        public void TestGetPageAsInterfaceMethodCreatesFromNativePage()
        {
            var testPage = new Mock<IPage>();
            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new Mock<BrowserBase>(MockBehavior.Strict, logger.Object);
            browser.Protected().Setup<IPage>("CreateNativePage", typeof(TestPage), false).Returns(testPage.Object);

            var result = browser.Object.Page<TestPage>();

            Assert.IsNotNull(result);
            Assert.AreSame(testPage.Object, result);

            browser.VerifyAll();
            testPage.VerifyAll();
        }

        /// <summary>
        /// Tests the GetPage method as an interface creates the page from a native page.
        /// </summary>
        [TestMethod]
        public void TestGetPageMethodCreatesFromNativePage()
        {
            var testPage = new Mock<IPage>();
            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new Mock<BrowserBase>(MockBehavior.Strict, logger.Object);
            browser.Protected().Setup<IPage>("CreateNativePage", typeof(TestPage), false).Returns(testPage.Object);

            var result = browser.Object.Page(typeof(TestPage));

            Assert.IsNotNull(result);
            Assert.AreSame(testPage.Object, result);

            browser.VerifyAll();
            testPage.VerifyAll();
        }

        /// <summary>
        /// Tests the GetPage method as an interface creates the page from a native page.
        /// </summary>
        [TestMethod]
        public void TestGetUriForPageTypeReturnsNullInBaseClass()
        {
            var testPage = new Mock<IPage>();
            var logger = new Mock<ILogger>(MockBehavior.Loose);

            var browser = new Mock<BrowserBase>(logger.Object) { CallBase = true };

            var result = browser.Object.GetUriForPageType(typeof(TestPage));

            Assert.IsNull(result);

            browser.VerifyAll();
            testPage.VerifyAll();
        }

        /// <summary>
        /// Tests the EnsureOnPage method when it is on the correct page.
        /// </summary>
        [TestMethod]
        public void TestEnsureOnPageWhenUrlIsOnPageDoesNothing()
        {
            var testPage = new Mock<IPage>();
            testPage.SetupGet(p => p.PageType).Returns(typeof(TestPage));
            var logger = new Mock<ILogger>(MockBehavior.Loose);


            var browser = new Mock<BrowserBase>(MockBehavior.Strict, logger.Object);
            browser.Protected().Setup<IList<string>>("GetNativePageLocation", testPage.Object).Returns(new[] { "http://localhost:2222/foo" });

            browser.Object.EnsureOnPage(testPage.Object);

            browser.VerifyAll();
            testPage.VerifyAll();
        }

        /// <summary>
        /// Tests the EnsureOnPage method when it is not on the correct page throws an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(PageNavigationException))]
        public void TestEnsureOnPageWhenUrlIsNotOnPageThrowsException()
        {
            UriHelper.BaseUri = new Uri("http://localhost:2222");
            var testPage = new Mock<IPage>();
            testPage.SetupGet(p => p.PageType).Returns(typeof(TestPage));
            var logger = new Mock<ILogger>(MockBehavior.Loose);


            var browser = new Mock<BrowserBase>(MockBehavior.Strict, logger.Object);
            browser.Protected().Setup<IList<string>>("GetNativePageLocation", testPage.Object).Returns(new[] { "http://localhost:2222/" });

            ExceptionHelper.SetupForException<PageNavigationException>(
                () => browser.Object.EnsureOnPage(testPage.Object),
                ex =>
                {
                    browser.VerifyAll();
                    testPage.VerifyAll();
                });
        }

        /// <summary>
        /// Tests the GoToPage method when it needs to navigate to the page first.
        /// </summary>
        [TestMethod]
        public void TestGoToPageWhenUrlIsNotOnPageReturnsNativeClassAfterNavigating()
        {
            var testPage = new Mock<IPage>();
            var logger = new Mock<ILogger>(MockBehavior.Strict);
            logger.Setup(l => l.Debug("Navigating to URL: {0}", "http://localhost:2222/foo"));

            var browser = new Mock<BrowserBase>(MockBehavior.Strict, logger.Object);
            browser.Protected().Setup<IPage>("CreateNativePage", typeof(TestPage), true).Returns(testPage.Object);
            browser.Setup(b => b.GoTo(new Uri("http://localhost:2222/foo")));

            var result = browser.Object.GoToPage(typeof(TestPage), new Dictionary<string, string>());

            Assert.IsNotNull(result);
            Assert.AreSame(testPage.Object, result);

            browser.VerifyAll();
            testPage.VerifyAll();
            logger.VerifyAll();
        }

        /// <summary>
        /// Tests the GoToPage method when navigation fails throws an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(PageNavigationException))]
        public void TestGoToPageWhenUrlIsNotOnPageAndNavigationFailsThrowsAnException()
        {
            UriHelper.BaseUri = new Uri("http://localhost:2222");
            var testPage = new Mock<IPage>();
            var logger = new Mock<ILogger>(MockBehavior.Strict);
            logger.Setup(l => l.Debug("Navigating to URL: {0}", "http://localhost:2222/foo"));

            var browser = new Mock<BrowserBase>(MockBehavior.Strict, logger.Object);
            browser.Setup(b => b.GoTo(new Uri("http://localhost:2222/foo"))).Throws<InvalidOperationException>();

            // ReSharper disable once ImplicitlyCapturedClosure
            ExceptionHelper.SetupForException<PageNavigationException>(
                () => browser.Object.GoToPage(typeof(TestPage), new Dictionary<string, string>()),
                ex =>
                    {
                        StringAssert.StartsWith(ex.Message, "Could not navigate to URI: http://localhost:2222/foo.");

                        browser.VerifyAll();
                        testPage.VerifyAll();
                        logger.VerifyAll();
                    });
        }

        /// <summary>
        /// Tests the close method when dispose is true.
        /// </summary>
        [TestMethod]
        public void TestCloseWhenDisposeIsTrue()
        {
            bool disposingManagedResources = true;

            var logger = new Mock<ILogger>(MockBehavior.Loose);
            var browser = new Mock<BrowserBase>(MockBehavior.Strict, logger.Object);
            browser.Setup(b => b.Close());
            browser.Protected().Setup("DisposeWindow", disposingManagedResources);

            var browserInstance = browser.Object;
            browserInstance.Close(dispose: true);

            Assert.IsTrue(browserInstance.IsDisposed);

            browser.Verify(b => b.Close());
            browser.Protected().Verify("DisposeWindow", Times.Once(), disposingManagedResources);
        }

        /// <summary>
        /// Tests the close method when dispose is false.
        /// </summary>
        [TestMethod]
        public void TestCloseWhenDisposeIsFalse()
        {
            bool disposingManagedResources = true;

            var logger = new Mock<ILogger>(MockBehavior.Loose);
            var browser = new Mock<BrowserBase>(MockBehavior.Strict, logger.Object);
            browser.Setup(b => b.Close());
            browser.Protected().Setup("DisposeWindow", disposingManagedResources);

            var browserInstance = browser.Object;
            browserInstance.Close(dispose: false);

            Assert.IsFalse(browserInstance.IsDisposed);

            browser.Verify(b => b.Close());
            browser.Protected().Verify("DisposeWindow", Times.Never(), disposingManagedResources);
        }


        /// <summary>
        /// A test class for the page.
        /// </summary>
        // ReSharper disable once ClassNeverInstantiated.Local
        [PageNavigation("/foo")]
        private class TestPage
        {
        }
    }
}