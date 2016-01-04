// <copyright file="ClearDataActionFixture.cs">
//    Copyright © 2015 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Tests.Actions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for data clearing action
    /// </summary>
    [TestClass]
    public class ClearDataActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var clearDataAction = new ClearDataAction();

            Assert.AreEqual("ClearDataAction", clearDataAction.Name);
        }

        /// <summary>
        /// Tests that Execute will clear the data for the found element.
        /// </summary>
        [TestMethod]
        public void TestExecuteClearsDataIfElementIsFound()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.ClearData());

            // ReSharper disable once RedundantAssignment
            IPropertyData element = null;

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.TryGetElement("myitem", out element)).Returns(false);
            locator.Setup(p => p.GetProperty("myitem")).Returns(propData.Object);

            var getItemAction = new ClearDataAction
            {
                ElementLocator = locator.Object
            };

            var context = new ClearDataAction.ClearDataContext("myitem");
            var result = getItemAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
        }
    }
}
