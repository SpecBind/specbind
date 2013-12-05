// <copyright file="ConfigurationFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Configuration;

    /// <summary>
    /// Test classes for verifying configuration.
    /// </summary>
    [TestClass]
    public class ConfigurationFixture
    {
        /// <summary>
        /// Tests the read and write of the configuration.
        /// </summary>
        [TestMethod]
        public void TestReadWriteConfiguration()
        {
            var section = new ConfigurationSectionHandler
                              {
                                 Application = new ApplicationConfigurationElement
                                      {
                                          StartUrl = "http://myapp.com"
                                      },
                                 BrowserFactory = new BrowserFactoryConfigurationElement
                                                      {
                                                          BrowserType = "Chrome",
                                                          Provider = "MyProvider, MyProvider.Class",
                                                          ElementLocateTimeout = TimeSpan.FromSeconds(10),
                                                          PageLoadTimeout = TimeSpan.FromSeconds(15),
                                                      }
                              };

            Assert.IsNotNull(section.Application);
            Assert.AreEqual("http://myapp.com", section.Application.StartUrl);

            Assert.IsNotNull(section.BrowserFactory);
            Assert.AreEqual("MyProvider, MyProvider.Class", section.BrowserFactory.Provider);
            Assert.AreEqual("Chrome", section.BrowserFactory.BrowserType);
            Assert.AreEqual(TimeSpan.FromSeconds(10), section.BrowserFactory.ElementLocateTimeout);
            Assert.AreEqual(TimeSpan.FromSeconds(15), section.BrowserFactory.PageLoadTimeout);
            Assert.IsNotNull(section.BrowserFactory.Settings);
        }
    }
}