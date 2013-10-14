// <copyright file="ActionResultFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Tests.ActionPipeline
{
	using System;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using SpecBind.ActionPipeline;

	/// <summary>
	/// A test fixture for the ActionResult class.
	/// </summary>
	[TestClass]
	public class ActionResultFixture
	{
		/// <summary>
		/// Tests a successful action with no object.
		/// </summary>
		[TestMethod]
		public void TestSuccessfulActionResultWithNoObject()
		{
			var result = ActionResult.Successful();

			Assert.AreEqual(true, result.Success);
			Assert.AreEqual(null, result.Result);
			Assert.AreEqual(null, result.Exception);
		}

		/// <summary>
		/// Tests a successful action with a populated object.
		/// </summary>
		[TestMethod]
		public void TestSuccessfulActionResultWithAPopulatedObject()
		{
			const string HelloItem = "Hello!";
			var result = ActionResult.Successful(HelloItem);

			Assert.AreEqual(true, result.Success);
			Assert.AreEqual(HelloItem, result.Result);
			Assert.AreEqual(null, result.Exception);
		}

		/// <summary>
		/// Tests a failure action with no object.
		/// </summary>
		[TestMethod]
		public void TestFailureActionResultWithNoObject()
		{
			var result = ActionResult.Failure();

			Assert.AreEqual(false, result.Success);
			Assert.AreEqual(null, result.Result);
			Assert.AreEqual(null, result.Exception);
		}

		/// <summary>
		/// Tests a failure action with a corresponding exception.
		/// </summary>
		[TestMethod]
		public void TestFailureActionResultWithAnExceptionItem()
		{
			var exception = new Exception();
			var result = ActionResult.Failure(exception);

			Assert.AreEqual(false, result.Success);
			Assert.AreEqual(null, result.Result);
			Assert.AreEqual(exception, result.Exception);
		}
	}
}
