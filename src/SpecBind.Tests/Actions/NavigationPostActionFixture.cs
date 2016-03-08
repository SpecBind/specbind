// <copyright file="NavigationPostActionFixture.cs">
//    Copyright © 2014 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;
    using Moq.Protected;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the NavigationPostAction.
    /// </summary>
    [TestClass]
    public class NavigationPostActionFixture
    {
        /// <summary>
        /// Tests that the method is not called when action fails.
        /// </summary>
        [TestMethod]
        public void TestMethodIsNotCalledWhenActionFails()
        {
            var action = new Mock<IAction>(MockBehavior.Strict);
            var context = new Mock<ActionContext>(MockBehavior.Strict, "testproperty");
            var postAction = new Mock<NavigationPostAction>(MockBehavior.Strict);

            var result = ActionResult.Failure();
            postAction.Object.PerformPostAction(action.Object, context.Object, result);

            postAction.VerifyAll();
            action.VerifyAll();
            context.VerifyAll();
        }

        /// <summary>
        /// Tests that the method is not called when action fails.
        /// </summary>
        [TestMethod]
        public void TestMethodIsNotCalledWhenActionIsNotNavigation()
        {
            var action = new Mock<IAction>(MockBehavior.Strict);
            action.SetupGet(a => a.Name).Returns("OtherAction");

            var context = new Mock<ActionContext>(MockBehavior.Strict, "testproperty");
            var postAction = new Mock<NavigationPostAction>(MockBehavior.Strict);

            var result = ActionResult.Successful();
            postAction.Object.PerformPostAction(action.Object, context.Object, result);

            postAction.VerifyAll();
            action.VerifyAll();
            context.VerifyAll();
        }

        /// <summary>
        /// Tests that the method is called when action is navigation and successful.
        /// </summary>
        [TestMethod]
        public void TestMethodIsCalledWhenActionIsNavigationAndSuccessful()
        {
            var page = new Mock<IPage>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var logger = new Mock<ILogger>();
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var action = new PageNavigationAction(browser.Object, logger.Object, pageMapper.Object);
            var context = new PageNavigationAction.PageNavigationActionContext("testproperty", PageNavigationAction.PageAction.NavigateToPage);

            var postAction = new Mock<NavigationPostAction>(MockBehavior.Strict);
            postAction.Protected().Setup("OnPageNavigate", page.Object, PageNavigationAction.PageAction.NavigateToPage, ItExpr.IsNull<Dictionary<string, string>>());

            var result = ActionResult.Successful(page.Object);
            postAction.Object.PerformPostAction(action, context, result);

            postAction.VerifyAll();
            pageMapper.VerifyAll();
            browser.VerifyAll();
            page.VerifyAll();
        }

        /// <summary>
        /// Tests that the method is called when action is navigation and has arguments and successful.
        /// </summary>
        [TestMethod]
        public void TestMethodIsCalledWhenActionIsNavigationAndHasArgumentsAndSuccessful()
        {
            var page = new Mock<IPage>(MockBehavior.Strict);
            var pageMapper = new Mock<IPageMapper>(MockBehavior.Strict);
            var logger = new Mock<ILogger>();
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var action = new PageNavigationAction(browser.Object, logger.Object, pageMapper.Object);
            var arguments = new Dictionary<string, string> { { "MyKey", "MyValue" } };
            var context = new PageNavigationAction.PageNavigationActionContext("testproperty", PageNavigationAction.PageAction.NavigateToPage, arguments);

            var postAction = new Mock<NavigationPostAction>(MockBehavior.Strict);
            postAction.Protected().Setup("OnPageNavigate", page.Object, PageNavigationAction.PageAction.NavigateToPage, arguments);

            var result = ActionResult.Successful(page.Object);
            postAction.Object.PerformPostAction(action, context, result);

            postAction.VerifyAll();
            pageMapper.VerifyAll();
            browser.VerifyAll();
            page.VerifyAll();
        }
    }
}