// <copyright file="ApplicationStepsFixture.cs">
//    Copyright © 2013 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using BoDi;

    using Configuration;
    using SpecBind.Helpers;
    using TechTalk.SpecFlow;


    /// <summary>
    /// A set of unit tests for <see cref="ApplicationSteps"/>.
    /// </summary>
    [TestClass]
    public class ApplicationStepsFixture
    {
        [TestInitialize]
        public void Initialize()
        {
            // reset the configuration to the default values
            var configSection = SettingHelper.GetConfigurationSection();
            ApplicationConfigurationElement currentApplicationConfiguration = configSection.Application;
            currentApplicationConfiguration.StartUrl = "http://localhost:2222";
            currentApplicationConfiguration.RetryValidationUntilTimeout = false;
            currentApplicationConfiguration.WaitForStillElementBeforeClicking = true;
        }

        /// <summary>
        /// Tests that passing an application configuration succeeds.
        /// </summary>
        [TestMethod]
        public void GivenTheApplicationConfiguration_WithMockFactory_Succeeds()
        {
            var objectContainer = new Mock<IObjectContainer>(MockBehavior.Strict);

            var steps = new ApplicationSteps(objectContainer.Object);

            const string startUrl = "http://myapp.com";

            steps.GivenTheApplicationConfiguration(new ApplicationConfigurationElement
            {
                StartUrl = startUrl
            });

            var configSection = SettingHelper.GetConfigurationSection();
            Assert.AreEqual(startUrl, configSection.Application.StartUrl);

            objectContainer.VerifyAll();
        }

        /// <summary>
        /// Tests that passing the default application configuration returns values from the application configuration file.
        /// </summary>
        [TestMethod]
        public void GivenTheApplicationConfiguration_WithDefaultConfiguration_ReturnsValuesFromApplicationConfigurationFile()
        {
            var objectContainer = new Mock<IObjectContainer>(MockBehavior.Strict);

            var steps = new ApplicationSteps(objectContainer.Object);

            ApplicationConfigurationElement applicationConfigurationElement = new ApplicationConfigurationElement();
            steps.GivenTheApplicationConfiguration(applicationConfigurationElement);

            var configSection = SettingHelper.GetConfigurationSection();

            Assert.AreEqual("http://localhost:2222", configSection.Application.StartUrl);
            Assert.AreEqual(false, configSection.Application.RetryValidationUntilTimeout);
            Assert.AreEqual(true, configSection.Application.WaitForStillElementBeforeClicking);

            objectContainer.VerifyAll();
        }

        /// <summary>
        /// Tests that calling Transform with an application configuration table returns an application configuration element.
        /// </summary>
        [TestMethod]
        public void Transform_WithApplicationConfigurationTable_ReturnsApplicationConfigurationElement()
        {
            var objectContainer = new Mock<IObjectContainer>(MockBehavior.Strict);

            var steps = new ApplicationSteps(objectContainer.Object);

            Table table = new Table(new[] { "Field", "Value" });
            table.AddRow(new[] { "StartUrl", "http://myapp.com" });

            ApplicationConfigurationElement config = steps.Transform(table);

            Assert.AreEqual("http://myapp.com", config.StartUrl);

            objectContainer.VerifyAll();
        }
    }
}