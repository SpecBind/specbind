// <copyright file="HighlightPreActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the HighlightPreAction.
    /// </summary>
    [TestClass]
    public class HighlightPreActionFixture
    {
        /// <summary>
        /// Tests that the on locate does nothing.
        /// </summary>
        [TestMethod]
        public void TestOnLocateDoesNothing()
        {
            var settingHelper = new Mock<ISettingHelper>(MockBehavior.Strict);
            var helper = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var highlightAction = new HighlightPreAction(helper.Object, settingHelper.Object);

            highlightAction.OnLocate("MyProperty");

            helper.VerifyAll();
            settingHelper.VerifyAll();
        }

        /// <summary>
        /// Tests that the on locate complete method does nothing when the result is null.
        /// </summary>
        [TestMethod]
        public void TestOnLocateCompleteWhenResultIsNullDoesNothing()
        {
            var settingHelper = new Mock<ISettingHelper>(MockBehavior.Strict);
            var helper = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var highlightAction = new HighlightPreAction(helper.Object, settingHelper.Object);

            highlightAction.OnLocateComplete("MyProperty", null);

            helper.VerifyAll();
            settingHelper.VerifyAll();
        }

        /// <summary>
        /// Tests that the on locate complete method does nothing when the context is disabled and setting is off.
        /// </summary>
        [TestMethod]
        public void TestOnLocateCompleteWhenSettingIsDisabledAndContextAreOffDoesNothing()
        {
            var settingHelper = new Mock<ISettingHelper>(MockBehavior.Strict);
            settingHelper.SetupGet(s => s.HighlightModeEnabled).Returns(false);

            var helper = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            helper.Setup(h => h.ContainsTag(HighlightPreAction.HighlightMode)).Returns(false);
            helper.Setup(h => h.FeatureContainsTag(HighlightPreAction.HighlightMode)).Returns(false);
            
            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);

            var highlightAction = new HighlightPreAction(helper.Object, settingHelper.Object);

            highlightAction.OnLocateComplete("MyProperty", propertyData.Object);

            helper.VerifyAll();
            propertyData.VerifyAll();
            settingHelper.VerifyAll();
        }

        /// <summary>
        /// Tests that the on locate complete method highlights the element when the item context is enabled.
        /// </summary>
        [TestMethod]
        public void TestOnLocateCompleteWhenSettingIsEnabledHighlightsElement()
        {
            var settingHelper = new Mock<ISettingHelper>(MockBehavior.Strict);
            settingHelper.SetupGet(s => s.HighlightModeEnabled).Returns(true);

            var helper = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            
            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            propertyData.Setup(p => p.Highlight());

            var highlightAction = new HighlightPreAction(helper.Object, settingHelper.Object);

            highlightAction.OnLocateComplete("MyProperty", propertyData.Object);

            helper.VerifyAll();
            propertyData.VerifyAll();
            settingHelper.VerifyAll();
        }

        /// <summary>
        /// Tests that the on locate complete method highlights the element when the item context is enabled.
        /// </summary>
        [TestMethod]
        public void TestOnLocateCompleteWhenSettingIsDisabledAndItemContextIsEnabledHighlightsElement()
        {
            var settingHelper = new Mock<ISettingHelper>(MockBehavior.Strict);
            settingHelper.SetupGet(s => s.HighlightModeEnabled).Returns(false);

            var helper = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            helper.Setup(h => h.ContainsTag(HighlightPreAction.HighlightMode)).Returns(true);
            helper.Setup(h => h.FeatureContainsTag(HighlightPreAction.HighlightMode)).Returns(false);

            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            propertyData.Setup(p => p.Highlight());

            var highlightAction = new HighlightPreAction(helper.Object, settingHelper.Object);

            highlightAction.OnLocateComplete("MyProperty", propertyData.Object);

            helper.VerifyAll();
            propertyData.VerifyAll();
            settingHelper.VerifyAll();
        }

        /// <summary>
        /// Tests that the on locate complete method highlights the element when the item context is enabled.
        /// </summary>
        [TestMethod]
        public void TestOnLocateCompleteWhenSettingIsDisabledAndFeatureContextIsEnabledHighlightsElement()
        {
            var settingHelper = new Mock<ISettingHelper>(MockBehavior.Strict);
            settingHelper.SetupGet(s => s.HighlightModeEnabled).Returns(false);

            var helper = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            helper.Setup(h => h.FeatureContainsTag(HighlightPreAction.HighlightMode)).Returns(true);

            var propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            propertyData.Setup(p => p.Highlight());

            var highlightAction = new HighlightPreAction(helper.Object, settingHelper.Object);

            highlightAction.OnLocateComplete("MyProperty", propertyData.Object);

            helper.VerifyAll();
            propertyData.VerifyAll();
            settingHelper.VerifyAll();
        }
    }
}