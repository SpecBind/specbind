// <copyright file="SetCookiePreActionFixture.cs">
//    Copyright © 2014 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the set cookie pre action.
    /// </summary>
    [TestClass]
    public class SetCookiePreActionFixture
    {
        /// <summary>
        /// Tests the pre-action with an action that is not navigation.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenActionIsNotNavigationDoesNothing()
        {
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var logger = new Mock<ILogger>();
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var action = new Mock<IAction>();

            var cookiePreAction = new SetCookiePreAction(browser.Object, logger.Object, pageMapper.Object);
            var context = new ActionContext("doesnotexist");

            cookiePreAction.PerformPreAction(action.Object, context);

            pageMapper.VerifyAll();
            browser.VerifyAll();
        }


        /// <summary>
        /// Tests the action on a page that doesn't exist.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenFieldDoesNotExistDoesNothing()
        {
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("doesnotexist")).Returns((Type)null);

            var logger = new Mock<ILogger>();
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var navigationAction = new PageNavigationAction(browser.Object, logger.Object, pageMapper.Object);

            var cookiePreAction = new SetCookiePreAction(browser.Object, logger.Object, pageMapper.Object);
            var context = new ActionContext("doesnotexist");

            cookiePreAction.PerformPreAction(navigationAction, context);

            pageMapper.VerifyAll();
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the execute with a page that does not have an attribute ensuring a cookie isn't placed.
        /// </summary>
        [TestMethod]
        public void TestExecuteWithPageThatDoesNotContainAnAttributeDoesNothing()
        {
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("NoAttributePage")).Returns(typeof(NoAttributePage));

            var logger = new Mock<ILogger>();

            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var navigationAction = new PageNavigationAction(browser.Object, logger.Object, pageMapper.Object);

            var cookiePreAction = new SetCookiePreAction(browser.Object, logger.Object, pageMapper.Object);
            var context = new ActionContext("NoAttributePage");

            cookiePreAction.PerformPreAction(navigationAction, context);

            pageMapper.VerifyAll();
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the execute with navigate and correct parameters and page arguments returns successfully.
        /// </summary>
        [TestMethod]
        public void TestExecuteWithNavigateAndCorrectParametersAndArgumentsReturnsSuccessful()
        {
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("AttributePage")).Returns(typeof(AttributePage));

            var logger = new Mock<ILogger>();

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.AddCookie("TestCookie", "TestValue", "/mydomain", null, "www.mydomain.com", true));

            var navigationAction = new PageNavigationAction(browser.Object, logger.Object, pageMapper.Object);

            var cookiePreAction = new SetCookiePreAction(browser.Object, logger.Object, pageMapper.Object);
            var context = new ActionContext("AttributePage");

            cookiePreAction.PerformPreAction(navigationAction, context);

            pageMapper.VerifyAll();
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the execute with a DateTime constant parses as the minimum date time.
        /// </summary>
        [TestMethod]
        public void TestExecuteWithMinDateTimePassesToBrowser()
        {
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("MinDateAttributePage")).Returns(typeof(MinDateAttributePage));

            var logger = new Mock<ILogger>();

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.AddCookie("TestCookie", "TestValue", "/", DateTime.MinValue, null, false));

            var navigationAction = new PageNavigationAction(browser.Object, logger.Object, pageMapper.Object);

            var cookiePreAction = new SetCookiePreAction(browser.Object, logger.Object, pageMapper.Object);
            var context = new ActionContext("MinDateAttributePage");

            cookiePreAction.PerformPreAction(navigationAction, context);

            pageMapper.VerifyAll();
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the execute with a DateTime constant parses as the maximum date time.
        /// </summary>
        [TestMethod]
        public void TestExecuteWithMaxDateTimePassesToBrowser()
        {
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("MaxDateAttributePage")).Returns(typeof(MaxDateAttributePage));

            var logger = new Mock<ILogger>();

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.AddCookie("TestCookie", "TestValue", "/", DateTime.MaxValue, null, false));

            var navigationAction = new PageNavigationAction(browser.Object, logger.Object, pageMapper.Object);

            var cookiePreAction = new SetCookiePreAction(browser.Object, logger.Object, pageMapper.Object);
            var context = new ActionContext("MaxDateAttributePage");

            cookiePreAction.PerformPreAction(navigationAction, context);

            pageMapper.VerifyAll();
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the execute with a DateTime constant parses with a fixed date time.
        /// </summary>
        [TestMethod]
        public void TestExecuteWithFixedDateTimePassesToBrowser()
        {
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("FixedDateAttributePage")).Returns(typeof(FixedDateAttributePage));

            var logger = new Mock<ILogger>();

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.AddCookie("TestCookie", "TestValue", "/", new DateTime(2015, 12, 21, 0, 0, 0, 0), null, false));

            var navigationAction = new PageNavigationAction(browser.Object, logger.Object, pageMapper.Object);

            var cookiePreAction = new SetCookiePreAction(browser.Object, logger.Object, pageMapper.Object);
            var context = new ActionContext("FixedDateAttributePage");

            cookiePreAction.PerformPreAction(navigationAction, context);

            pageMapper.VerifyAll();
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the execute with a DateTime constant parses with a fixed date time.
        /// </summary>
        [TestMethod]
        public void TestExecuteWithNumericSlidingDateTimePassesToBrowser()
        {
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("SlidingDateAttributePage")).Returns(typeof(SlidingDateAttributePage));

            var logger = new Mock<ILogger>();

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.AddCookie("TestCookie", "TestValue", "/", It.Is<DateTime?>(d => d.Value > DateTime.Now), null, false));

            var navigationAction = new PageNavigationAction(browser.Object, logger.Object, pageMapper.Object);

            var cookiePreAction = new SetCookiePreAction(browser.Object, logger.Object, pageMapper.Object);
            var context = new ActionContext("SlidingDateAttributePage");

            cookiePreAction.PerformPreAction(navigationAction, context);

            pageMapper.VerifyAll();
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the execute with a DateTime constant parses with a fixed date time.
        /// </summary>
        [TestMethod]
        public void TestExecuteWithInvalidDateTimeLogsInfoAndContinues()
        {
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("InvalidDateAttributePage")).Returns(typeof(InvalidDateAttributePage));

            var logger = new Mock<ILogger>();
            logger.Setup(l => l.Info("Cannot parse cookie date: {0}", new object[] { "foo" })).Verifiable();

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.AddCookie("TestCookie", "TestValue", "/", null, null, false));

            var navigationAction = new PageNavigationAction(browser.Object, logger.Object, pageMapper.Object);

            var cookiePreAction = new SetCookiePreAction(browser.Object, logger.Object, pageMapper.Object);
            var context = new ActionContext("InvalidDateAttributePage");

            cookiePreAction.PerformPreAction(navigationAction, context);

            pageMapper.VerifyAll();
            browser.VerifyAll();
            logger.VerifyAll();
        }


        /// <summary>
        /// A test page that does not contain the attribute
        /// </summary>
        private class NoAttributePage
        {
        }

        /// <summary>
        /// A test page that contains the SetCookie attribute
        /// </summary>
        [SetCookie("TestCookie", "TestValue", Domain = "www.mydomain.com", IsSecure = true, Path = "/mydomain")]
        private class AttributePage
        {
        }

        /// <summary>
        /// A test page that contains the SetCookie attribute and a never-expiring cookie
        /// </summary>
        [SetCookie("TestCookie", "TestValue", Expires = "DateTime.MaxValue")]
        private class MaxDateAttributePage
        {
        }

        /// <summary>
        /// A test page that contains the SetCookie attribute and a removal value
        /// </summary>
        [SetCookie("TestCookie", "TestValue", Expires = "DateTime.MinValue")]
        private class MinDateAttributePage
        {
        }

        /// <summary>
        /// A test page that contains the SetCookie attribute and sliding expiration date
        /// </summary>
        [SetCookie("TestCookie", "TestValue", Expires = "60")]
        private class SlidingDateAttributePage
        {
        }

        /// <summary>
        /// A test page that contains the SetCookie attribute and fixed expiration date
        /// </summary>
        [SetCookie("TestCookie", "TestValue", Expires = "12/21/2015 12:00:00 AM")]
        private class FixedDateAttributePage
        {
        }

        /// <summary>
        /// A test page that contains the SetCookie attribute and an invalid expiration date
        /// </summary>
        [SetCookie("TestCookie", "TestValue", Expires = "foo")]
        private class InvalidDateAttributePage
        {
        }
    }
}