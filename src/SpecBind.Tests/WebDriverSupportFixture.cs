// <copyright file="WebDriverSupportFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
    using System;

    using BoDi;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using SpecBind.Validation;

    using TechTalk.SpecFlow.Tracing;

    /// <summary>
    /// A test fixture for the Web Driver class.
    /// </summary>
    [TestClass]
    public class WebDriverSupportFixture
    {
        /// <summary>
        /// Tests the teardown with no errors.
        /// </summary>
        [TestMethod]
        public void TestInitializeTests()
        {
            var logger = new Mock<ILogger>();

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.RegisterInstanceAs(It.IsAny<IBrowser>(), null));
            container.Setup(c => c.RegisterInstanceAs<ISettingHelper>(It.IsAny<WrappedSettingHelper>(), null));
            container.Setup(c => c.RegisterInstanceAs(It.IsAny<IPageMapper>(), null));
            container.Setup(c => c.RegisterInstanceAs<IScenarioContextHelper>(It.IsAny<ScenarioContextHelper>(), null));
            container.Setup(c => c.RegisterInstanceAs<ITokenManager>(It.IsAny<TokenManager>(), null));
            container.Setup(c => c.RegisterInstanceAs(It.IsAny<IActionRepository>(), null));
            container.Setup(c => c.RegisterTypeAs<ActionPipelineService, IActionPipelineService>(null));
            container.Setup(c => c.RegisterTypeAs<ProxyLogger, ILogger>(null));
            container.Setup(c => c.Resolve<ILogger>()).Returns(logger.Object);

            container.Setup(c => c.Resolve(It.Is<Type>(t => typeof(ILocatorAction).IsAssignableFrom(t)), null)).Returns(new Mock<ILocatorAction>().Object);
            container.Setup(c => c.Resolve(It.Is<Type>(t => typeof(IPreAction).IsAssignableFrom(t)), null)).Returns(new Mock<IPreAction>().Object);
            container.Setup(c => c.Resolve(It.Is<Type>(t => typeof(IValidationComparer).IsAssignableFrom(t)), null)).Returns(new Mock<IValidationComparer>().Object);
            
            var driverSupport = new WebDriverSupport(container.Object);

            driverSupport.InitializeDriver();

            container.VerifyAll();
        }

        [TestMethod]
        public void TestCheckForScreenShotNoError()
        {
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns((Exception)null);

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);

            var driverSupport = new WebDriverSupport(container.Object);

            driverSupport.CheckForScreenshot();

            container.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the teardown with an error that then takes a screenshot.
        /// </summary>
        [TestMethod]
        public void TestCheckForScreenshotWithErrorTakesSueccessfulScreenshot()
        {
            var listener = new Mock<ITraceListener>(MockBehavior.Strict);
            listener.Setup(l => l.WriteTestOutput(It.Is<string>(s => s.Contains("TestFileName.jpg"))));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns(new InvalidOperationException());
            scenarioContext.Setup(s => s.GetStepFileName()).Returns("TestFileName");

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns("TestFileName.jpg");
            browser.Setup(b => b.SaveHtml(It.IsAny<string>(), "TestFileName")).Returns((string)null);

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);
            container.Setup(c => c.Resolve<ITraceListener>()).Returns(listener.Object);

            WebDriverSupport.Browser = browser.Object;
            var driverSupport = new WebDriverSupport(container.Object);

            driverSupport.CheckForScreenshot();

            container.VerifyAll();
            browser.VerifyAll();
            scenarioContext.VerifyAll();
            listener.VerifyAll();
        }

        /// <summary>
        /// Tests the teardown with an error that then takes a screenshot.
        /// </summary>
        [TestMethod]
        public void TestCheckForScreenshotWithErrorAttemptsScreenshotButFails()
        {
            var listener = new Mock<ITraceListener>(MockBehavior.Strict);
            
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns(new InvalidOperationException());
            scenarioContext.Setup(s => s.GetStepFileName()).Returns("TestFileName");

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns((string)null);
            browser.Setup(b => b.SaveHtml(It.IsAny<string>(), "TestFileName")).Returns((string)null);

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);
            container.Setup(c => c.Resolve<ITraceListener>()).Returns(listener.Object);

            WebDriverSupport.Browser = browser.Object;
            var driverSupport = new WebDriverSupport(container.Object);

            driverSupport.CheckForScreenshot();

            container.VerifyAll();
            browser.VerifyAll();
            scenarioContext.VerifyAll();
            listener.VerifyAll();
        }

        /// <summary>
        /// Tests the teardown with an error that then takes a screenshot but does not write a message.
        /// </summary>
        [TestMethod]
        public void TestCheckForScreenshotWithErrorAttemptsScreenshotButListenerIsUnavailable()
        {
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns(new InvalidOperationException());
            scenarioContext.Setup(s => s.GetStepFileName()).Returns("TestFileName");

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns((string)null);
            browser.Setup(b => b.SaveHtml(It.IsAny<string>(), "TestFileName")).Returns((string)null);

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);
            container.Setup(c => c.Resolve<ITraceListener>()).Returns((ITraceListener)null);

            WebDriverSupport.Browser = browser.Object;
            var driverSupport = new WebDriverSupport(container.Object);

            driverSupport.CheckForScreenshot();

            container.VerifyAll();
            browser.VerifyAll();
            scenarioContext.VerifyAll();
        }

        [TestMethod]
        public void TestTearDownWebDriver()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Close());
            WebDriverSupport.Browser = browser.Object;

            WebDriverSupport.TearDownWebDriver();

            browser.VerifyAll();
        }
    }
}
