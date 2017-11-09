// <copyright file="BrowserStepsFixture.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using BoDi;
    using TechTalk.SpecFlow;

    using SpecBind.Actions;
    using SpecBind.Pages;
    using BrowserSupport;
    using Configuration;
    

    /// <summary>
    /// A set of unit tests for <see cref="BrowserSteps"/>.
    /// </summary>
    [TestClass]
    public class BrowserStepsFixture
    {
        /// <summary>
        /// Tests that passing a browser factory configuration with the mock browser factory succeeds.
        /// </summary>
        [TestMethod]
        public void GivenTheBrowserFactoryConfiguration_WithMockFactory_Succeeds()
        {
            var logger = new Mock<ILogger>(MockBehavior.Strict);
            var objectContainer = new Mock<IObjectContainer>(MockBehavior.Strict);
            objectContainer.Setup(c => c.Resolve<ILogger>()).Returns(logger.Object);
            objectContainer.Setup(c => c.RegisterInstanceAs(It.IsAny<IBrowser>(), null, false));
            objectContainer.Setup(c => c.RegisterInstanceAs(It.IsAny<IPageMapper>(), null, false));

            var steps = new BrowserSteps(objectContainer.Object);

            steps.GivenTheBrowserFactoryConfiguration(new BrowserFactoryConfigurationElement
            {
                Provider = "SpecBind.Tests.Support.MockBrowserFactory, SpecBind.Tests"
            });

            objectContainer.VerifyAll();
            logger.VerifyAll();
        }

        /// <summary>
        /// Tests that passing the default browser factory configuration returns the provider from the application configuration file.
        /// </summary>
        [TestMethod]
        public void GivenTheBrowserFactoryConfiguration_WithDefaultConfiguration_ReturnsProviderFromApplicationConfigurationFile()
        {
            var logger = new Mock<ILogger>(MockBehavior.Strict);
            var objectContainer = new Mock<IObjectContainer>(MockBehavior.Strict);
            objectContainer.Setup(c => c.Resolve<ILogger>()).Returns(logger.Object);
            objectContainer.Setup(c => c.RegisterInstanceAs(It.IsAny<IBrowser>(), null, false));
            objectContainer.Setup(c => c.RegisterInstanceAs(It.IsAny<IPageMapper>(), null, false));

            var steps = new BrowserSteps(objectContainer.Object);

            BrowserFactoryConfigurationElement browserFactoryConfigurationElement = new BrowserFactoryConfigurationElement();
            steps.GivenTheBrowserFactoryConfiguration(browserFactoryConfigurationElement);

            Assert.AreEqual("SpecBind.Tests.Support.MockBrowserFactory, SpecBind.Tests", browserFactoryConfigurationElement.Provider);

            objectContainer.VerifyAll();
            logger.VerifyAll();
        }
        /// <summary>
        /// Tests that calling Transform with an empty table returns the default browser factory configuration.
        /// </summary>
        [TestMethod]
        public void Transform_WithEmptyTable_ReturnsDefaultBrowserFactoryConfiguration()
        {
            var objectContainer = new Mock<IObjectContainer>(MockBehavior.Strict);

            var steps = new BrowserSteps(objectContainer.Object);

            BrowserFactoryConfigurationElement config = steps.Transform(new Table(new string[] { string.Empty }));

            Assert.AreEqual("IE", config.BrowserType);
            Assert.AreEqual(TimeSpan.FromSeconds(30), config.ElementLocateTimeout);
            Assert.AreEqual(TimeSpan.FromSeconds(30), config.PageLoadTimeout);
            Assert.AreEqual(false, config.CreateScreenshotOnExit);
            Assert.AreEqual(false, config.EnsureCleanSession);
            Assert.AreEqual(false, config.ReuseBrowser);
            Assert.AreEqual(true, config.ValidateWebDriver);
            Assert.AreEqual("none", config.WaitForPendingAjaxCallsVia);

            objectContainer.VerifyAll();
        }

        /// <summary>
        /// Tests that calling Transform with one setting returns a browser factory configuration with one setting.
        /// </summary>
        [TestMethod]
        public void Transform_WithOneSetting_ReturnsBrowserFactoryConfigurationWithOneSetting()
        {
            var objectContainer = new Mock<IObjectContainer>(MockBehavior.Strict);

            var steps = new BrowserSteps(objectContainer.Object);

            Table table = new Table(new[] { "Field", "Value" });
            table.AddRow(new[] { "Provider", "provider1" });
            table.AddRow(new[] { "BrowserType", "browser1" });
            table.AddRow(new[] { "ElementLocateTimeout", "0:0:30" });
            table.AddRow(new[] { "PageLoadTimeout", "0:1:00" });
            table.AddRow(new[] { "CreateScreenshotOnExit", "true" });
            table.AddRow(new[] { "EnsureCleanSession", "true" });
            table.AddRow(new[] { "ReuseBrowser", "true" });
            table.AddRow(new[] { "ValidateWebDriver", "true" });
            table.AddRow(new[] { "WaitForPendingAjaxCallsVia", "jquery" });
            table.AddRow(new[] { "Settings", "key1=value1" });

            BrowserFactoryConfigurationElement config = steps.Transform(table);

            Assert.AreEqual("provider1", config.Provider);
            Assert.AreEqual("browser1", config.BrowserType);
            Assert.AreEqual(TimeSpan.FromSeconds(30), config.ElementLocateTimeout);
            Assert.AreEqual(TimeSpan.FromMinutes(1), config.PageLoadTimeout);
            Assert.AreEqual(true, config.CreateScreenshotOnExit);
            Assert.AreEqual(true, config.EnsureCleanSession);
            Assert.AreEqual(true, config.ReuseBrowser);
            Assert.AreEqual(true, config.ValidateWebDriver);
            Assert.AreEqual("jquery", config.WaitForPendingAjaxCallsVia);

            // Settings
            Assert.AreEqual(1, config.Settings.Count);
            Assert.AreEqual("key1", config.Settings.AllKeys[0]);
            Assert.AreEqual("value1", config.Settings["key1"].Value);

            objectContainer.VerifyAll();
        }

        /// <summary>
        /// Tests that calling Transform with two settings returns a browser factory configuration with two settings.
        /// </summary>
        [TestMethod]
        public void Transform_WithTwoSettings_ReturnsBrowserFactoryConfigurationWithTwoSettings()
        {
            var objectContainer = new Mock<IObjectContainer>(MockBehavior.Strict);

            var steps = new BrowserSteps(objectContainer.Object);

            Table table = new Table(new[] { "Field", "Value" });
            table.AddRow(new[] { "Provider", "provider1" });
            table.AddRow(new[] { "Browser Type", "browser1" });
            table.AddRow(new[] { "Settings", "key1=value1;key2=value2" });

            BrowserFactoryConfigurationElement config = steps.Transform(table);

            Assert.AreEqual("provider1", config.Provider);
            Assert.AreEqual("browser1", config.BrowserType);
            Assert.AreEqual(2, config.Settings.Count);
            Assert.AreEqual("key1", config.Settings.AllKeys[0]);
            Assert.AreEqual("value1", config.Settings["key1"].Value);
            Assert.AreEqual("key2", config.Settings.AllKeys[1]);
            Assert.AreEqual("value2", config.Settings["key2"].Value);

            objectContainer.VerifyAll();
        }

        /// <summary>
        /// Tests that calling Transform with an empty setting returns a browser factory configuration without any settings.
        /// </summary>
        [TestMethod]
        public void Transform_WithEmptySettings_ReturnsBrowserFactoryConfigurationWithOneSetting()
        {
            var objectContainer = new Mock<IObjectContainer>(MockBehavior.Strict);

            var steps = new BrowserSteps(objectContainer.Object);

            Table table = new Table(new[] { "Field", "Value" });
            table.AddRow(new[] { "Provider", "provider1" });
            table.AddRow(new[] { "BrowserType", "browser1" });
            table.AddRow(new[] { "Settings", string.Empty });

            BrowserFactoryConfigurationElement config = steps.Transform(table);

            Assert.AreEqual("provider1", config.Provider);
            Assert.AreEqual("browser1", config.BrowserType);
            Assert.AreEqual(0, config.Settings.Count);

            objectContainer.VerifyAll();
        }

        /// <summary>
        /// Tests that calling Transform with a setting without a value returns a browser factory configuration with one setting with an empty value.
        /// </summary>
        [TestMethod]
        public void Transform_WithSettingWithoutValue_ReturnsBrowserFactoryConfigurationWithOneSettingWithEmptyValue()
        {
            var objectContainer = new Mock<IObjectContainer>(MockBehavior.Strict);

            var steps = new BrowserSteps(objectContainer.Object);

            Table table = new Table(new[] { "Field", "Value" });
            table.AddRow(new[] { "Provider", "provider1" });
            table.AddRow(new[] { "BrowserType", "browser1" });
            table.AddRow(new[] { "Settings", "key1" });

            BrowserFactoryConfigurationElement config = steps.Transform(table);

            Assert.AreEqual("provider1", config.Provider);
            Assert.AreEqual("browser1", config.BrowserType);
            Assert.AreEqual(1, config.Settings.Count);
            Assert.AreEqual("key1", config.Settings.AllKeys[0]);
            Assert.AreEqual(string.Empty, config.Settings["key1"].Value);

            objectContainer.VerifyAll();
        }

        /// <summary>
        /// Tests that calling Transform with an invalid setting throws an ArgumentException.
        /// </summary>
        [TestMethod]
        public void Transform_WithInvalidSettings_ThrowsArgumentException()
        {
            var objectContainer = new Mock<IObjectContainer>(MockBehavior.Strict);

            var steps = new BrowserSteps(objectContainer.Object);

            Table table = new Table(new[] { "Field", "Value" });
            table.AddRow(new[] { "Provider", "provider1" });
            table.AddRow(new[] { "BrowserType", "browser1" });
            table.AddRow(new[] { "Settings", "too=many=values" });

            try
            {
                BrowserFactoryConfigurationElement config = steps.Transform(table);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Settings must be separated by a semi-colon and name-value pairs separated by an equals sign.", ex.Message);
                return;
            }

            Assert.Fail("Expected ArgumentException");
        }

        /// <summary>
        /// Tests that calling Transform with user preferences returns a browser factory with user preferences.
        /// </summary>
        [TestMethod]
        public void Transform_WithUserPreferences_ReturnsBrowserFactoryWithUserPreferences()
        {
            var objectContainer = new Mock<IObjectContainer>(MockBehavior.Strict);

            var steps = new BrowserSteps(objectContainer.Object);

            Table table = new Table(new[] { "Field", "Value" });
            table.AddRow(new[] { "Provider", "provider1" });
            table.AddRow(new[] { "BrowserType", "browser1" });
            table.AddRow(new[] { "User Profile Preferences", "key1=value1" });

            BrowserFactoryConfigurationElement config = steps.Transform(table);

            Assert.AreEqual("provider1", config.Provider);
            Assert.AreEqual("browser1", config.BrowserType);

            // User Profile Preferences
            Assert.AreEqual(1, config.UserProfilePreferences.Count);
            Assert.AreEqual("key1", config.UserProfilePreferences.AllKeys[0]);
            Assert.AreEqual("value1", config.UserProfilePreferences["key1"].Value);

            objectContainer.VerifyAll();
        }
    }
}