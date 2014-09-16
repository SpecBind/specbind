// <copyright file="ValidateTokenActionFixture.cs">
//    Copyright © 2014 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Tests.Validation;

    /// <summary>
    /// A test fixture for a validate token action
    /// </summary>
    [TestClass]
    public class ValidateTokenActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var buttonClickAction = new ValidateTokenAction(null);

            Assert.AreEqual("ValidateTokenAction", buttonClickAction.Name);
        }

        /// <summary>
        /// Tests the execute method with a property that does not exist.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenTokenReturnsNullReturnsFailure()
        {
            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
            tokenManager.Setup(p => p.GetTokenByKey("doesnotexist")).Returns((string)null);

            var validateItemAction = new ValidateTokenAction(tokenManager.Object);

            var context = new ValidateTokenAction.ValidateTokenActionContext("doesnotexist", "equals", "my value");
            context.ValidationTable.Process();

            var result = validateItemAction.Execute(context);

            Assert.AreEqual(false, result.Success);

            Assert.IsNotNull(result.Exception);
            StringAssert.Contains(result.Exception.Message, "doesnotexist");

            tokenManager.VerifyAll();
        }

        /// <summary>
        /// Tests the execute method with a valid token and value returns success.
        /// </summary>
        [TestMethod]
        public void TestExecuteWhenTokenReturnsValidValueReturnsSuccess()
        {
            var tokenManager = new Mock<ITokenManager>(MockBehavior.Strict);
            tokenManager.Setup(p => p.GetTokenByKey("token1")).Returns("my value");

            var validateItemAction = new ValidateTokenAction(tokenManager.Object);

            var context = new ValidateTokenAction.ValidateTokenActionContext("token1", "equals", "my value");
            context.ValidationTable.Process();

            var result = validateItemAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            tokenManager.VerifyAll();
        }

    }
}