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
            var preActionMock = new Mock<ILocatorAction>();

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.RegisterInstanceAs(It.IsAny<IBrowser>(), null));
            container.Setup(c => c.RegisterInstanceAs<ISettingHelper>(It.IsAny<WrappedSettingHelper>(), null));
            container.Setup(c => c.RegisterInstanceAs(It.IsAny<IPageMapper>(), null));
            container.Setup(c => c.RegisterInstanceAs<IScenarioContextHelper>(It.IsAny<ScenarioContextHelper>(), null));
            container.Setup(c => c.RegisterInstanceAs<ITokenManager>(It.IsAny<TokenManager>(), null));
            container.Setup(c => c.RegisterInstanceAs(It.IsAny<IActionRepository>(), null));
            container.Setup(c => c.RegisterTypeAs<ActionPipelineService, IActionPipelineService>(null));

            container.Setup(c => c.Resolve(typeof(HighlightPreAction), null)).Returns(preActionMock.Object);
            
            var driverSupport = new WebDriverSupport(container.Object);

            driverSupport.InitializeDriver();

            container.VerifyAll();
        }

        /// <summary>
        /// Tests the teardown with no errors.
        /// </summary>
        [TestMethod]
        public void TestTeardownNoError()
        {
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns((Exception)null);
            
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Close());

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IBrowser>()).Returns(browser.Object);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);

            var driverSupport = new WebDriverSupport(container.Object);

            driverSupport.TearDownWebDriver();

            container.VerifyAll();
            browser.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the teardown with no errors.
        /// </summary>
        [TestMethod]
        public void TestTeardownNoErrorWithIDisposable()
        {
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns((Exception)null);

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.As<IDisposable>().Setup(b => b.Dispose());
            browser.Setup(b => b.Close());

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IBrowser>()).Returns(browser.Object);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);

            var driverSupport = new WebDriverSupport(container.Object);

            driverSupport.TearDownWebDriver();

            container.VerifyAll();
            browser.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the teardown with an error that then takes a screenshot.
        /// </summary>
        [TestMethod]
        public void TestTeardownWithErrorTakesSueccessfulScreenshot()
        {
            var listener = new Mock<ITraceListener>(MockBehavior.Strict);
            listener.Setup(l => l.WriteTestOutput(It.Is<string>(s => s.Contains("TestFileName.jpg"))));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns(new InvalidOperationException());
            scenarioContext.Setup(s => s.GetStepFileName()).Returns("TestFileName");

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Close());
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns("TestFileName.jpg");

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IBrowser>()).Returns(browser.Object);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);
            container.Setup(c => c.Resolve<ITraceListener>()).Returns(listener.Object);

            var driverSupport = new WebDriverSupport(container.Object);

            driverSupport.TearDownWebDriver();

            container.VerifyAll();
            browser.VerifyAll();
            scenarioContext.VerifyAll();
            listener.VerifyAll();
        }

        /// <summary>
        /// Tests the teardown with an error that then takes a screenshot.
        /// </summary>
        [TestMethod]
        public void TestTeardownWithErrorAttemptsScreenshotButFails()
        {
            var listener = new Mock<ITraceListener>(MockBehavior.Strict);
            
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns(new InvalidOperationException());
            scenarioContext.Setup(s => s.GetStepFileName()).Returns("TestFileName");

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Close());
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns((string)null);

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IBrowser>()).Returns(browser.Object);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);
            container.Setup(c => c.Resolve<ITraceListener>()).Returns(listener.Object);

            var driverSupport = new WebDriverSupport(container.Object);

            driverSupport.TearDownWebDriver();

            container.VerifyAll();
            browser.VerifyAll();
            scenarioContext.VerifyAll();
            listener.VerifyAll();
        }

        /// <summary>
        /// Tests the teardown with an error that then takes a screenshot but does not write a message.
        /// </summary>
        [TestMethod]
        public void TestTeardownWithErrorAttemptsScreenshotButListenerIsUnavailable()
        {
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns(new InvalidOperationException());
            scenarioContext.Setup(s => s.GetStepFileName()).Returns("TestFileName");

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Close());
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns((string)null);

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IBrowser>()).Returns(browser.Object);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);
            container.Setup(c => c.Resolve<ITraceListener>()).Returns((ITraceListener)null);

            var driverSupport = new WebDriverSupport(container.Object);

            driverSupport.TearDownWebDriver();

            container.VerifyAll();
            browser.VerifyAll();
            scenarioContext.VerifyAll();
        }
    }
}
