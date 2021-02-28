// <copyright file="PageNavigationActionFixture.cs">
//    Copyright © 2014 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Pages;
    using SpecBind.Tests.Support;

    /// <summary>
    /// A test fixture for a page navigation steps
    /// </summary>
    [TestClass]
    public class PageNavigationActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var buttonClickAction = new PageNavigationAction(null, null);

            Assert.AreEqual("PageNavigationAction", buttonClickAction.Name);
        }

        /// <summary>
        /// Tests the action on a page that doesn't exist.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenFieldDoesNotExistThrowsAnException()
        {
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("doesnotexist")).Returns((Type)null);

            var logger = new Mock<ILogger>();
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            WebDriverSupport.CurrentBrowser = browser.Object;

            var navigationAction = new PageNavigationAction(logger.Object, pageMapper.Object);

            var context = new PageNavigationAction.PageNavigationActionContext("doesnotexist", PageNavigationAction.PageAction.EnsureOnPage);

            var result = navigationAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.IsNotNull(result.Exception);
            Assert.AreEqual("Cannot locate a page for name: doesnotexist. Check page aliases in the test assembly.", result.Exception.Message);

            pageMapper.VerifyAll();
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the execute with navigate and correct parameters returns successfully.
        /// </summary>
        [TestMethod]
        public void TestExecuteWithNavigateAndCorrectParametersReturnsSuccessful()
        {
            var testPage = new Mock<IPage>();
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("MyPage")).Returns(typeof(TestBase));

            var logger = new Mock<ILogger>();

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.GoToPage(typeof(TestBase), null)).Returns(testPage.Object);

            WebDriverSupport.CurrentBrowser = browser.Object;

            var navigationAction = new PageNavigationAction(logger.Object, pageMapper.Object);

            var context = new PageNavigationAction.PageNavigationActionContext("MyPage", PageNavigationAction.PageAction.NavigateToPage);

            var result = navigationAction.Execute(context);

            Assert.AreEqual(true, result.Success);
            Assert.AreSame(testPage.Object, result.Result);

            pageMapper.VerifyAll();
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the execute with navigate and correct parameters and page arguments returns successfully.
        /// </summary>
        [TestMethod]
        public void TestExecuteWithNavigateAndCorrectParametersAndArgumentsReturnsSuccessful()
        {
            var testPage = new Mock<IPage>();
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("MyPage")).Returns(typeof(TestBase));

            var logger = new Mock<ILogger>();

            var arguments = new Dictionary<string, string> { { "MyKey", "value1" } };

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.GoToPage(typeof(TestBase), arguments)).Returns(testPage.Object);

            WebDriverSupport.CurrentBrowser = browser.Object;

            var navigationAction = new PageNavigationAction(logger.Object, pageMapper.Object);

            var context = new PageNavigationAction.PageNavigationActionContext("MyPage", PageNavigationAction.PageAction.NavigateToPage, arguments);

            var result = navigationAction.Execute(context);

            Assert.AreEqual(true, result.Success);
            Assert.AreSame(testPage.Object, result.Result);

            pageMapper.VerifyAll();
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the execute with ensure on page and correct parameters returns successfully.
        /// </summary>
        [TestMethod]
        public void TestExecuteWithEnsureOnPageAndCorrectParametersReturnsSuccessful()
        {
            var testPage = new Mock<IPage>();
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("MyPage")).Returns(typeof(TestBase));

            var logger = new Mock<ILogger>();

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Page(typeof(TestBase))).Returns(testPage.Object);
            browser.Setup(b => b.EnsureOnPage(testPage.Object));

            WebDriverSupport.CurrentBrowser = browser.Object;

            var navigationAction = new PageNavigationAction(logger.Object, pageMapper.Object);

            var context = new PageNavigationAction.PageNavigationActionContext("MyPage", PageNavigationAction.PageAction.EnsureOnPage);

            var result = navigationAction.Execute(context);

            Assert.AreEqual(true, result.Success);
            Assert.AreSame(testPage.Object, result.Result);

            pageMapper.VerifyAll();
            browser.VerifyAll();
        }
    }
}