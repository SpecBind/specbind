// <copyright file="EnterDataActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the data entry action
    /// </summary>
    [TestClass]
    public class EnterDataActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var enterDataAction = new EnterDataAction(null);

            Assert.AreEqual("EnterDataAction", enterDataAction.Name);
        }

        /// <summary>
        /// Tests the get item action with a field on the page that doesn't exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestExecuteWhenFieldDoesNotExistThenExceptionIsThrown()
        {
            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);

            // ReSharper disable once RedundantAssignment
            IPropertyData property = null;
            locator.Setup(p => p.TryGetElement("doesnotexist", out property)).Returns(false);
            locator.Setup(p => p.GetProperty("doesnotexist")).Throws(new ElementExecuteException("Cannot find item"));

            var enterDataAction = new EnterDataAction(tokenManager.Object)
            {
                                        ElementLocator = locator.Object
                                    };

            var context = new EnterDataAction.EnterDataContext("doesnotexist", "some value");
            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => enterDataAction.Execute(context),
                e =>
                    {
                        locator.VerifyAll();
                        tokenManager.VerifyAll();
                    });
        }

        /// <summary>
        /// Tests the get item action with a field on the page that doesn't exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestExecuteWhenContextTypeIsInvalidThenAnExceptionIsThrown()
        {
            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);

            var enterDataAction = new EnterDataAction(tokenManager.Object)
            {
                                        ElementLocator = locator.Object
                                    };

            var context = new ActionContext("doesnotexist");
            ExceptionHelper.SetupForException<InvalidOperationException>(
                () => enterDataAction.Execute(context),
                e =>
                    {
                        StringAssert.Contains(e.Message, "EnterDataContext");

                        locator.VerifyAll();
                        tokenManager.VerifyAll();
                    });
        }

        /// <summary>
        /// Tests the get item action with a property that exists.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenContextIsCorrectFillsThePropertyData()
        {
            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
            tokenManager.Setup(t => t.SetToken("some data")).Returns("translated data");

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.FillData("translated data"));

            // ReSharper disable once RedundantAssignment
            var property = propData.Object;

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.TryGetElement("myitem", out property)).Returns(true);

            var getItemAction = new EnterDataAction(tokenManager.Object)
            {
                                        ElementLocator = locator.Object
                                    };

            var context = new EnterDataAction.EnterDataContext("myitem", "some data");
            var result = getItemAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
            tokenManager.VerifyAll();
        }

        /// <summary>
        /// Tests the get item action with a property that exists.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenContextIsValidPropertyFillsThePropertyData()
        {
            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
            tokenManager.Setup(t => t.SetToken("some data")).Returns("translated data");

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.FillData("translated data"));

            // ReSharper disable RedundantAssignment
            IPropertyData element = null;
            // ReSharper restore RedundantAssignment

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.TryGetElement("myitem", out element)).Returns(false);
            locator.Setup(p => p.GetProperty("myitem")).Returns(propData.Object);

            var getItemAction = new EnterDataAction(tokenManager.Object)
            {
                ElementLocator = locator.Object
            };

            var context = new EnterDataAction.EnterDataContext("myitem", "some data");
            var result = getItemAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
            tokenManager.VerifyAll();
        }
    }
}