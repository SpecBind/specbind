﻿// <copyright file="ActionPipelineServiceFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.ActionPipeline
{
	using System;
	using System.Collections.Generic;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using SpecBind.ActionPipeline;
	using SpecBind.Actions;
	using SpecBind.Pages;

	/// <summary>
	/// A test fixture for the ActionPipelineService.
	/// </summary>
	[TestClass]
	public class ActionPipelineServiceFixture
	{
		/// <summary>
		/// Tests the pipeline call invokes all pipeline steps and does not fail.
		/// </summary>
		[TestMethod]
		public void TestPipelineCallInvokesAllPipelineStepsAndDoesNotFail()
		{
		    var context = new ActionContext("MyProperty");
			var actionResult = ActionResult.Successful();

			var action = new Mock<IAction>(MockBehavior.Strict);
			action.SetupSet(a => a.ElementLocator = It.IsAny<IElementLocator>());
            action.Setup(a => a.Execute(context)).Returns(actionResult);

			var page = new Mock<IPage>(MockBehavior.Strict);

			var preAction = new Mock<IPreAction>(MockBehavior.Strict);
			preAction.Setup(p => p.PerformPreAction(action.Object, context));

			var postAction = new Mock<IPostAction>(MockBehavior.Strict);
			postAction.Setup(p => p.PerformPostAction(action.Object, context, actionResult));

			var repository = new Mock<IActionRepository>(MockBehavior.Strict);
			repository.Setup(r => r.GetPreActions()).Returns(new[] { preAction.Object });
			repository.Setup(r => r.GetPostActions()).Returns(new[] { postAction.Object });
			repository.Setup(r => r.GetLocatorActions()).Returns(new List<ILocatorAction>());
            repository.Setup(r => r.Initialize());

            var service = new ActionPipelineService(repository.Object);

			var result = service.PerformAction(page.Object, action.Object, context);

			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Success);
			Assert.AreSame(actionResult, result);

			repository.VerifyAll();
			action.VerifyAll();
			page.VerifyAll();
			preAction.VerifyAll();
			postAction.VerifyAll();
		}

		/// <summary>
		/// Tests the pipeline call invokes all pipeline steps and does not fail.
		/// </summary>
		[TestMethod]
		public void TestPipelineCallInvokesAllPipelineStepsAndSetsStatusToFailedWhenExceptionIsThrown()
		{
            var context = new ActionContext("MyProperty");
			var exception = new InvalidOperationException("Something Failed!");

			var action = new Mock<IAction>(MockBehavior.Strict);
			action.SetupSet(a => a.ElementLocator = It.IsAny<IElementLocator>());
            action.Setup(a => a.Execute(context)).Throws(exception);

			var page = new Mock<IPage>(MockBehavior.Strict);

			var preAction = new Mock<IPreAction>(MockBehavior.Strict);
			preAction.Setup(p => p.PerformPreAction(action.Object, context));

			var postAction = new Mock<IPostAction>(MockBehavior.Strict);
			postAction.Setup(p => p.PerformPostAction(action.Object, context, It.Is<ActionResult>(r => !r.Success)));

			var repository = new Mock<IActionRepository>(MockBehavior.Strict);
			repository.Setup(r => r.GetPreActions()).Returns(new[] { preAction.Object });
			repository.Setup(r => r.GetPostActions()).Returns(new[] { postAction.Object });
			repository.Setup(r => r.GetLocatorActions()).Returns(new List<ILocatorAction>());
            repository.Setup(r => r.Initialize());

            var service = new ActionPipelineService(repository.Object);

			var result = service.PerformAction(page.Object, action.Object, context);

			Assert.IsNotNull(result);
			Assert.AreEqual(false, result.Success);
			Assert.AreSame(exception, result.Exception);

			repository.VerifyAll();
			action.VerifyAll();
			page.VerifyAll();
			preAction.VerifyAll();
			postAction.VerifyAll();
		}

	    /// <summary>
	    /// Tests the pipeline call invokes all pre-steps even if one fails and then throws that failure.
	    /// </summary>
	    [TestMethod]
	    public void TestPipelineCallInvokesAllPrePipelineStepsIfTheyExistAndThrowsExceptionOnSingleFailure()
	    {
	        var context = new ActionContext("MyProperty");
	        var exception = new InvalidOperationException("Something Failed!");

	        var action = new Mock<IAction>(MockBehavior.Strict);
	        action.SetupSet(a => a.ElementLocator = It.IsAny<IElementLocator>());

	        var page = new Mock<IPage>(MockBehavior.Strict);

	        var preAction1 = new Mock<IPreAction>(MockBehavior.Strict);
	        preAction1.Setup(p => p.PerformPreAction(action.Object, context)).Throws(exception);

	        var preAction2 = new Mock<IPreAction>(MockBehavior.Strict);
	        preAction2.Setup(p => p.PerformPreAction(action.Object, context));


            var repository = new Mock<IActionRepository>(MockBehavior.Strict);
	        repository.Setup(r => r.GetPreActions()).Returns(new[] { preAction1.Object, preAction2.Object });
	        repository.Setup(r => r.GetLocatorActions()).Returns(new List<ILocatorAction>());
            repository.Setup(r => r.Initialize());

            var service = new ActionPipelineService(repository.Object);

	        var result = service.PerformAction(page.Object, action.Object, context);

	        Assert.IsNotNull(result);
	        Assert.AreEqual(false, result.Success);
	        Assert.AreSame(exception, result.Exception);

	        repository.VerifyAll();
	        action.VerifyAll();
	        page.VerifyAll();
	        preAction1.VerifyAll();
	        preAction2.VerifyAll();
	    }

	    /// <summary>
	    /// Tests the pipeline call invokes all pre-steps even if one fails and then throws an aggregate exception.
	    /// </summary>
	    [TestMethod]
	    public void TestPipelineCallInvokesAllPrePipelineStepsIfTheyExistAndThrowsAggregateExceptionOnMultipleFailures()
	    {
	        var context = new ActionContext("MyProperty");
	        var exception1 = new InvalidOperationException("Something Failed 1!");
	        var exception2 = new InvalidOperationException("Something Failed 2!");

            var action = new Mock<IAction>(MockBehavior.Strict);
	        action.SetupSet(a => a.ElementLocator = It.IsAny<IElementLocator>());

	        var page = new Mock<IPage>(MockBehavior.Strict);

	        var preAction1 = new Mock<IPreAction>(MockBehavior.Strict);
	        preAction1.Setup(p => p.PerformPreAction(action.Object, context)).Throws(exception1);

	        var preAction2 = new Mock<IPreAction>(MockBehavior.Strict);
	        preAction2.Setup(p => p.PerformPreAction(action.Object, context)).Throws(exception2);


	        var repository = new Mock<IActionRepository>(MockBehavior.Strict);
	        repository.Setup(r => r.GetPreActions()).Returns(new[] { preAction1.Object, preAction2.Object });
	        repository.Setup(r => r.GetLocatorActions()).Returns(new List<ILocatorAction>());
            repository.Setup(r => r.Initialize());

            var service = new ActionPipelineService(repository.Object);

	        var result = service.PerformAction(page.Object, action.Object, context);

	        Assert.IsNotNull(result);
	        Assert.AreEqual(false, result.Success);

            Assert.IsInstanceOfType(result.Exception, typeof(AggregateException));

	        var aggregateException = (AggregateException)result.Exception;
            Assert.AreSame(exception1, aggregateException.InnerExceptions[0]);
	        Assert.AreSame(exception2, aggregateException.InnerExceptions[1]);

            repository.VerifyAll();
	        action.VerifyAll();
	        page.VerifyAll();
	        preAction1.VerifyAll();
	        preAction2.VerifyAll();
	    }

        /// <summary>
        /// Tests the pipeline call creates the action, invokes all pipeline steps and does not fail.
        /// </summary>
        [TestMethod]
        public void TestPipelineCallCreatesActionAndInvokesAllPipelineStepsAndDoesNotFail()
        {
            var context = new ActionContext("MyProperty");

            var page = new Mock<IPage>(MockBehavior.Strict);

            var preAction = new Mock<IPreAction>(MockBehavior.Strict);
            preAction.Setup(p => p.PerformPreAction(It.IsAny<MockAction>(), context));

            var postAction = new Mock<IPostAction>(MockBehavior.Strict);
            postAction.Setup(p => p.PerformPostAction(It.IsAny<MockAction>(), context, It.IsAny<ActionResult>()));

            var repository = new Mock<IActionRepository>(MockBehavior.Strict);
            repository.Setup(r => r.GetPreActions()).Returns(new[] { preAction.Object });
            repository.Setup(r => r.GetPostActions()).Returns(new[] { postAction.Object });
            repository.Setup(r => r.GetLocatorActions()).Returns(new List<ILocatorAction>());
            repository.Setup(r => r.Initialize());
            repository.Setup(r => r.CreateAction<MockAction>()).Returns(new MockAction());

            var service = new ActionPipelineService(repository.Object);

            var result = service.PerformAction<MockAction>(page.Object, context);

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.Success);

            repository.VerifyAll();
            page.VerifyAll();
            preAction.VerifyAll();
            postAction.VerifyAll();
        }

        /// <summary>
        /// A mock action class.
        /// </summary>
	    public class MockAction : ActionBase
	    {
            /// <summary>
            /// Initializes a new instance of the <see cref="MockAction"/> class.
            /// </summary>
            public MockAction()
                : base("Mock Action")
            {
            }

            /// <summary>
            /// Executes this instance action.
            /// </summary>
            /// <param name="actionContext">The action context.</param>
            /// <returns>The result of the action.</returns>
            public override ActionResult Execute(ActionContext actionContext)
            {
                return ActionResult.Successful();
            }
	    }

        /// <summary>
        /// A mock action class.
        /// </summary>
	    public class MockRetryAction : ActionBase
        {
            private bool hasFailed;

            /// <summary>
            /// Initializes a new instance of the <see cref="MockAction"/> class.
            /// </summary>
            public MockRetryAction()
                : base("Mock Retry Action")
            {
            }

            /// <summary>
            /// Executes this instance action.
            /// </summary>
            /// <param name="actionContext">The action context.</param>
            /// <returns>The result of the action.</returns>
            public override ActionResult Execute(ActionContext actionContext)
            {
                if (!this.hasFailed)
                {
                    this.hasFailed = true;
                    return ActionResult.Failure();
                }

                return ActionResult.Successful();
            }
        }
    }
}
