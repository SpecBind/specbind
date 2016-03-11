// <copyright file="HoverOverElementActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Pages;
    using System;
    /// <summary>
    /// A test fixture for hovering over an element
    /// </summary>
    [TestClass]
	public class HoverOverElementActionFixture
    {
		/// <summary>
		/// Tests getting the name of the action.
		/// </summary>
		[TestMethod]
		public void TestGetActionName()
		{
			var hoverOverElementAction = new HoverOverElementAction();

            Assert.AreEqual("HoverOverElementAction", hoverOverElementAction.Name);
		}

		/// <summary>
		///     Tests the fill field with a field on the page that doesn't exist.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ElementExecuteException))]
		public void TestClickItemFieldDoesNotExist()
		{
			var locator = new Mock<IElementLocator>(MockBehavior.Strict);
			locator.Setup(p => p.GetElement("doesnotexist")).Throws(new ElementExecuteException("Cannot find item"));

			var hoverOverElementAction = new HoverOverElementAction
			                        {
				                        ElementLocator = locator.Object
			                        };

		    var context = new ActionContext("doesnotexist");

			ExceptionHelper.SetupForException<ElementExecuteException>(
				() => hoverOverElementAction.Execute(context), e => locator.VerifyAll());
		}

		/// <summary>
		///     Tests the fill field with an element that exists and can be clicked.
		/// </summary>
		[TestMethod]
		public void TestClickItemSuccess()
		{
			var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.WaitForElementCondition(WaitConditions.NotMoving, null)).Returns(true);
            propData.Setup(p => p.WaitForElementCondition(WaitConditions.BecomesEnabled, null)).Returns(true);
			propData.Setup(p => p.ClickElement());

			var locator = new Mock<IElementLocator>(MockBehavior.Strict);
			locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

			var hoverOverElementAction = new HoverOverElementAction
			{
				ElementLocator = locator.Object
			};

            var context = new ActionContext("myproperty");
            var result = hoverOverElementAction.Execute(context);

			Assert.AreEqual(true, result.Success);

			locator.VerifyAll();
			propData.VerifyAll();
		}

        /// <summary>
		///     Tests the fill field with an element that exists and can be clicked.
		/// </summary>
		[TestMethod]
        public void TestClickItemWhenHoveringProducesSpecificErrorReturnsSuccess()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.WaitForElementCondition(WaitConditions.NotMoving, null)).Returns(true);
            propData.Setup(p => p.WaitForElementCondition(WaitConditions.BecomesEnabled, null)).Returns(true);
            propData.Setup(p => p.ClickElement()).Throws(new ApplicationException("Element is not clickable at point"));

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var hoverOverElementAction = new HoverOverElementAction
            {
                ElementLocator = locator.Object
            };

            var context = new ActionContext("myproperty");
            var result = hoverOverElementAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
		///     Tests the fill field with an element that exists and can be clicked.
		/// </summary>
		[TestMethod]
        public void TestClickItemWhenSomeOtherErrorHappensReturnsFailure()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.WaitForElementCondition(WaitConditions.NotMoving, null)).Returns(true);
            propData.Setup(p => p.WaitForElementCondition(WaitConditions.BecomesEnabled, null)).Returns(true);
            propData.Setup(p => p.ClickElement()).Throws(new ApplicationException("Some Other Error"));

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var hoverOverElementAction = new HoverOverElementAction
            {
                ElementLocator = locator.Object
            };

            var context = new ActionContext("myproperty");
            var result = hoverOverElementAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("Some Other Error", result.Exception.Message);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
		///     Tests the fill field with an element that exists and can be clicked.
		/// </summary>
		[TestMethod]
        public void TestClickItemWhenWaitIsEnabledReturnsSuccess()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.ClickElement());

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            HoverOverElementAction.WaitForStillElementBeforeClicking = false;
            
            var hoverOverElementAction = new HoverOverElementAction
            {
                ElementLocator = locator.Object
                
            };

            var context = new ActionContext("myproperty");
            var result = hoverOverElementAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
        }
    }
}