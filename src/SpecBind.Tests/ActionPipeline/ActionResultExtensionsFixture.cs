// <copyright file="ActionResultExtensionsFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.ActionPipeline
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the <see cref="ActionResultExtensions"/> class.
    /// </summary>
    [TestClass]
    public class ActionResultExtensionsFixture
    {
        /// <summary>
        /// Tests the check result when successful does nothing.
        /// </summary>
        [TestMethod]
        public void TestCheckResultWhenSuccessfulDoesNothing()
        {
            var result = ActionResult.Successful();
            result.CheckResult();
        }

        /// <summary>
        /// Tests the check result when failed throws an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestCheckResultWhenFailedThrowsAnException()
        {
            var result = ActionResult.Failure(new ElementExecuteException("Something Failed!"));
            result.CheckResult();
        }

        /// <summary>
        /// Tests the check result with a value when successful returns the value.
        /// </summary>
        [TestMethod]
        public void TestCheckResultWithItemWhenSuccessfulReturnsTheContent()
        {
            var resultItem = new object();
            var result = ActionResult.Successful(resultItem);
            
            var item = result.CheckResult<object>();

            Assert.AreSame(resultItem, item);
        }

        /// <summary>
        /// Tests the check result with a value when failed throws an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestCheckResultWithItemWhenFailedThrowsAnException()
        {
            var result = ActionResult.Failure(new ElementExecuteException("Something Failed!"));
            result.CheckResult<object>();
        }

        /// <summary>
        /// Tests the check result when it is null does nothing.
        /// </summary>
        [TestMethod]
        public void TestCheckResultWhenNullReturnsNothing()
        {
            var actionResult = (ActionResult)null;
            // ReSharper disable once ExpressionIsAlwaysNull
            var result = actionResult.CheckResult<object>();

            Assert.IsNull(result);
        }
    }
}