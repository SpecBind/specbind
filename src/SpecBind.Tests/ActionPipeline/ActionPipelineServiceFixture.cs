// <copyright file="ActionPipelineServiceFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.ActionPipeline
{
	using System;
	using System.Collections.Generic;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using SpecBind.ActionPipeline;
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
			var actionResult = ActionResult.Successful();

			var action = new Mock<IAction>(MockBehavior.Strict);
			action.SetupSet(a => a.ElementLocator = It.IsAny<IElementLocator>());
			action.Setup(a => a.Execute()).Returns(actionResult);
			
			var page = new Mock<IPage>(MockBehavior.Strict);

			var preAction = new Mock<IPreAction>(MockBehavior.Strict);
			preAction.Setup(p => p.PerformPreAction(action.Object));

			var postAction = new Mock<IPostAction>(MockBehavior.Strict);
			postAction.Setup(p => p.PerformPostAction(action.Object, actionResult));

			var repository = new Mock<IActionRepository>(MockBehavior.Strict);
			repository.Setup(r => r.GetPreActions()).Returns(new[] { preAction.Object });
			repository.Setup(r => r.GetPostActions()).Returns(new[] { postAction.Object });
			repository.Setup(r => r.GetLocatorActions()).Returns(new List<ILocatorAction>());

			var service = new ActionPipelineService(repository.Object);

			var result = service.PerformAction(page.Object, action.Object);

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
			var exception = new InvalidOperationException("Something Failed!");
			var action = new Mock<IAction>(MockBehavior.Strict);
			action.SetupSet(a => a.ElementLocator = It.IsAny<IElementLocator>());
			action.Setup(a => a.Execute()).Throws(exception);

			var page = new Mock<IPage>(MockBehavior.Strict);

			var preAction = new Mock<IPreAction>(MockBehavior.Strict);
			preAction.Setup(p => p.PerformPreAction(action.Object));

			var postAction = new Mock<IPostAction>(MockBehavior.Strict);
			postAction.Setup(p => p.PerformPostAction(action.Object, It.Is<ActionResult>(r => !r.Success)));

			var repository = new Mock<IActionRepository>(MockBehavior.Strict);
			repository.Setup(r => r.GetPreActions()).Returns(new[] { preAction.Object });
			repository.Setup(r => r.GetPostActions()).Returns(new[] { postAction.Object });
			repository.Setup(r => r.GetLocatorActions()).Returns(new List<ILocatorAction>());

			var service = new ActionPipelineService(repository.Object);

			var result = service.PerformAction(page.Object, action.Object);

			Assert.IsNotNull(result);
			Assert.AreEqual(false, result.Success);
			Assert.AreSame(exception, result.Exception);

			repository.VerifyAll();
			action.VerifyAll();
			page.VerifyAll();
			preAction.VerifyAll();
			postAction.VerifyAll();
		}
	}
}
