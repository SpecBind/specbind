// <copyright file="WaitForPageActionFixture.cs">
//    Copyright © 2014 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System;
    using System.Diagnostics;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for a wait for page action
    /// </summary>
    [TestClass]
    public class WaitForPageActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var buttonClickAction = new WaitForPageAction(null, null, null);

            Assert.AreEqual("WaitForPageAction", buttonClickAction.Name);
        }

        /// <summary>
        /// Tests the action execute with a page that doesn't exist.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPageDoesNotExistReturnsAFailure()
        {
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("MyPage")).Returns((Type)null);

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var logger = new Mock<ILogger>();

            var action = new WaitForPageAction(pageMapper.Object, browser.Object, logger.Object);
            var context = new WaitForPageAction.WaitForPageActionContext("MyPage", null);

            var result = action.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.IsNotNull(result.Exception);
            Assert.AreEqual("Cannot locate a page for name: MyPage. Check page aliases in the test assembly.", result.Exception.Message);

            pageMapper.VerifyAll();
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the action execute with a page exists and matches the url.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPageIsFoundAndUrlMatchesReturnsSuccess()
        {
            var page = new Mock<IPage>(MockBehavior.Strict);

            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("SamplePage")).Returns(typeof(SamplePage));

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Page(typeof(SamplePage))).Returns(page.Object);
            browser.Setup(b => b.EnsureOnPage(page.Object));

            var logger = new Mock<ILogger>();

            var action = new WaitForPageAction(pageMapper.Object, browser.Object, logger.Object);
            var context = new WaitForPageAction.WaitForPageActionContext("SamplePage", TimeSpan.FromSeconds(3));

            var result = action.Execute(context);

            Assert.AreEqual(true, result.Success);
            Assert.AreSame(page.Object, result.Result);
            
            pageMapper.VerifyAll();
            browser.VerifyAll();
            page.VerifyAll();
        }

        /// <summary>
        /// Tests the action execute with a page exists and matches the url after an initial failure.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPageIsFoundAndUrlMatchesAfterInitialFailureReturnsSuccess()
        {
            var page = new Mock<IPage>(MockBehavior.Strict);

            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("SamplePage")).Returns(typeof(SamplePage));

            var count = 0;
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Page(typeof(SamplePage))).Returns(page.Object);
            browser.Setup(b => b.EnsureOnPage(page.Object))
                   .Callback(() =>
                        {
                            count++;
                            if (count == 1)
                            {
                                throw new PageNavigationException("Cannot find URL");
                            }
                        });

            var logger = new Mock<ILogger>(MockBehavior.Strict);
            logger.Setup(l => l.Debug("Browser is not on page. Details: {0}", "Cannot find URL"));
            
            var action = new WaitForPageAction(pageMapper.Object, browser.Object, logger.Object);
            var context = new WaitForPageAction.WaitForPageActionContext("SamplePage", TimeSpan.FromSeconds(3));

            var result = action.Execute(context);

            Assert.AreEqual(true, result.Success);
            Assert.AreSame(page.Object, result.Result);

            pageMapper.VerifyAll();
            browser.Verify(b => b.EnsureOnPage(page.Object), Times.Exactly(2));
            browser.Verify(b => b.Page(typeof(SamplePage)), Times.Once());
            browser.VerifyAll();
            page.VerifyAll();
            logger.VerifyAll();
        }

        /// <summary>
        /// Tests the action execute with a page exists but never matches the URL returns a failure.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenPageIsFoundAndUrlDoesntMatchAfterTimeoutReturnsFailure()
        {
            var page = new Mock<IPage>(MockBehavior.Strict);

            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            pageMapper.Setup(p => p.GetTypeFromName("SamplePage")).Returns(typeof(SamplePage));

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Page(typeof(SamplePage))).Returns(page.Object);
            browser.Setup(b => b.EnsureOnPage(page.Object)).Throws(new PageNavigationException("Cannot find URL"));

            var logger = new Mock<ILogger>();

            var action = new WaitForPageAction(pageMapper.Object, browser.Object, logger.Object);
            var context = new WaitForPageAction.WaitForPageActionContext("SamplePage", TimeSpan.FromSeconds(1));

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var result = action.Execute(context);
            stopwatch.Stop();

            Assert.AreEqual(false, result.Success);
            Assert.IsNotNull(result.Exception);
            Assert.AreEqual("Browser did not resolve to the 'SamplePage' page in 00:00:01", result.Exception.Message);
            
            // Check rough timing
            Assert.IsTrue(stopwatch.Elapsed >= TimeSpan.FromSeconds(1), "elapsed time less than a second " + stopwatch.Elapsed);
            Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(1.3), "elapsed time less than 1.3 second " + stopwatch.Elapsed);

            pageMapper.VerifyAll();
            browser.VerifyAll();
            page.VerifyAll();
        }


        #region Class - SamplePage

        /// <summary>
        /// A test page class
        /// </summary>
        private class SamplePage
        {
        }

        #endregion
    }
}