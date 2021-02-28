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
    using SpecBind.Configuration;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using TechTalk.SpecFlow;
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
            var scenarioContextHelper = new Mock<IScenarioContextHelper>();

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.RegisterInstanceAs(It.IsAny<BrowserFactory>(), null, true));
            container.Setup(c => c.RegisterFactoryAs(It.IsAny<Func<IObjectContainer, IBrowser>>(), null));
            container.Setup(c => c.RegisterInstanceAs<ISettingHelper>(It.IsAny<WrappedSettingHelper>(), null, false));
            container.Setup(c => c.RegisterInstanceAs(It.IsAny<IPageMapper>(), null, false));
            container.Setup(c => c.RegisterInstanceAs(It.IsAny<IActionRepository>(), null, false));
            container.Setup(c => c.RegisterInstanceAs(It.IsAny<IActionPipelineService>(), null, false));
            container.Setup(c => c.Resolve<ILogger>()).Returns(logger.Object);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContextHelper.Object);

            var driverSupport = new WebDriverSupport(container.Object, null, null);

            driverSupport.InitializeDriver();

            container.VerifyAll();
        }

        /// <summary>
        /// Tests the check for screen shot no error.
        /// </summary>
        [TestMethod]
        public void TestCheckForScreenShotNoError()
        {
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns((Exception)null);

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.IsCreated).Returns(true);

            BrowserFactoryConfiguration config = new BrowserFactoryConfiguration();
            var browserFactory = new Mock<BrowserFactory>(config);
            WebDriverSupport.SetBrowserFactory(browserFactory.Object);
            WebDriverSupport.CurrentBrowser = browser.Object;

            var driverSupport = new WebDriverSupport(container.Object, null, null);

            driverSupport.CheckForScreenshot();

            container.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the teardown with an error that then takes a screenshot.
        /// </summary>
        [TestMethod]
        public void TestCheckForScreenshotWithErrorTakesSuccessfulScreenshot()
        {
            var listener = new Mock<ITraceListener>(MockBehavior.Strict);
            listener.Setup(l => l.WriteTestOutput(It.Is<string>(s => s.Contains("TestFileName.jpg"))));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns(new InvalidOperationException());
            scenarioContext.Setup(s => s.GetStepFileName(true)).Returns("TestFileName");
            scenarioContext.Setup(s => s.TestResultsDirectory).Returns(string.Empty);

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns("TestFileName.jpg");
            browser.Setup(b => b.SaveHtml(It.IsAny<string>(), "TestFileName")).Returns((string)null);
            browser.Setup(b => b.Close(true));
            browser.Setup(b => b.IsCreated).Returns(true);

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);
            container.Setup(c => c.Resolve<ITraceListener>()).Returns(listener.Object);

            WebDriverSupport.CurrentBrowser = browser.Object;
            var driverSupport = new WebDriverSupport(container.Object, null, null);

            driverSupport.CheckForScreenshot();
            driverSupport.TearDown();

            container.VerifyAll();
            browser.VerifyAll();
            scenarioContext.VerifyAll();
            listener.VerifyAll();
        }

        /// <summary>
        /// Tests the teardown with an error that then takes a screenshot.
        /// </summary>
        [TestMethod]
        public void TestCheckForScreenshotWithNoErrorButSettingEnabledTakesSuccessfulScreenshot()
        {
            var listener = new Mock<ITraceListener>(MockBehavior.Strict);
            listener.Setup(l => l.WriteTestOutput(It.Is<string>(s => s.Contains("TestFileName.jpg"))));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns((Exception)null);
            scenarioContext.Setup(s => s.GetStepFileName(false)).Returns("TestFileName");
            scenarioContext.Setup(s => s.TestResultsDirectory).Returns(string.Empty);

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns("TestFileName.jpg");
            browser.Setup(b => b.SaveHtml(It.IsAny<string>(), "TestFileName")).Returns((string)null);
            browser.Setup(b => b.Close(true));
            browser.Setup(b => b.IsCreated).Returns(true);

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);
            container.Setup(c => c.Resolve<ITraceListener>()).Returns(listener.Object);

            // Setup Configuration
            BrowserFactoryConfiguration config = new BrowserFactoryConfiguration
            {
                CreateScreenshotOnExit = true
            };

            var browserFactory = new Mock<BrowserFactory>(config);

            WebDriverSupport.SetBrowserFactory(browserFactory.Object);
            WebDriverSupport.CurrentBrowser = browser.Object;
            var driverSupport = new WebDriverSupport(container.Object, null, null);
            driverSupport.CheckForScreenshot();
            driverSupport.TearDown();

            config.CreateScreenshotOnExit = false;
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
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns(new InvalidOperationException());
            scenarioContext.Setup(s => s.GetStepFileName(true)).Returns("TestFileName");
            scenarioContext.Setup(s => s.TestResultsDirectory).Returns(string.Empty);

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns((string)null);
            browser.Setup(b => b.SaveHtml(It.IsAny<string>(), "TestFileName")).Returns((string)null);
            browser.Setup(b => b.Close(true));
            browser.Setup(b => b.IsCreated).Returns(true);

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);

            WebDriverSupport.CurrentBrowser = browser.Object;
            var driverSupport = new WebDriverSupport(container.Object, null, null);

            driverSupport.CheckForScreenshot();
            driverSupport.TearDown();

            container.VerifyAll();
            browser.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the teardown with an error that then takes a screenshot but does not write a message.
        /// </summary>
        [TestMethod]
        public void TestCheckForScreenshotWithErrorAttemptsScreenshotButListenerIsUnavailable()
        {
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns(new InvalidOperationException());
            scenarioContext.Setup(s => s.GetStepFileName(true)).Returns("TestFileName");
            scenarioContext.Setup(s => s.TestResultsDirectory).Returns(string.Empty);

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns("TestFileName.jpg");
            browser.Setup(b => b.SaveHtml(It.IsAny<string>(), "TestFileName")).Returns((string)null);
            browser.Setup(b => b.Close(true));
            browser.Setup(b => b.IsCreated).Returns(true);

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);
            container.Setup(c => c.Resolve<ITraceListener>()).Returns((ITraceListener)null);

            WebDriverSupport.CurrentBrowser = browser.Object;
            var driverSupport = new WebDriverSupport(container.Object, null, null);

            driverSupport.CheckForScreenshot();
            driverSupport.TearDown();

            container.VerifyAll();
            browser.VerifyAll();
            scenarioContext.VerifyAll();
        }

        /// <summary>
        /// Tests the tear down after test.
        /// </summary>
        [TestMethod]
        public void TestTearDownAfterTest()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Close(true));
            WebDriverSupport.CurrentBrowser = browser.Object;

            WebDriverSupport.TearDownAfterTest();

            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the tear down after scenario when reuse browser is true.
        /// </summary>
        [TestMethod]
        public void TestTearDownAfterScenarioWhenReuseBrowserIsTrue()
        {

            // arrange
            BrowserFactoryConfiguration config = new BrowserFactoryConfiguration
            {
                ReuseBrowser = true
            };

            var browserFactory = new Mock<BrowserFactory>(config);
            WebDriverSupport.SetBrowserFactory(browserFactory.Object);

            var browser = new Mock<IBrowser>(MockBehavior.Loose);
            WebDriverSupport.CurrentBrowser = browser.Object;

            // act
            WebDriverSupport.TearDownAfterScenario();

            // assert
            browser.Verify(b => b.Close(true), Times.Never());
        }

        /// <summary>
        /// Tests the tear down after scenario when reuse browser is false.
        /// </summary>
        [TestMethod]
        public void TestTearDownAfterScenarioWhenReuseBrowserIsFalse()
        {
            // arrange
            BrowserFactoryConfiguration config = new BrowserFactoryConfiguration
            {
                ReuseBrowser = false
            };

            var browserFactory = new Mock<BrowserFactory>(config);
            WebDriverSupport.SetBrowserFactory(browserFactory.Object);

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Close(true));
            WebDriverSupport.CurrentBrowser = browser.Object;

            // act
            WebDriverSupport.TearDownAfterScenario();

            // assert
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests WaitForAngular with a successful result, when nothing is pending.
        /// </summary>
        [TestMethod]
        public void TestWaitForAngularWithNothingPending()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(s => s.IsClosed).Returns(false);
            browser.Setup(s => s.IsDisposed).Returns(false);
            browser.Setup(s => s.CanGetUrl()).Returns(true);
            browser.Setup(s => s.Url).Returns("http://www.specbind.org");
            browser.Setup(s => s.ExecuteScript(It.IsAny<string>())).Returns("0");
            WebDriverSupport.CurrentBrowser = browser.Object;

            WebDriverSupport.WaitForAngular();

            browser.VerifyAll();
        }

        /// <summary>
        /// Tests WaitForAngular with a successful result, when something is pending.
        /// </summary>
        [TestMethod]
        public void TestWaitForAngularWithSomethingPending()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(s => s.IsClosed).Returns(false);
            browser.Setup(s => s.IsDisposed).Returns(false);
            browser.Setup(s => s.CanGetUrl()).Returns(true);
            browser.Setup(s => s.Url).Returns("http://www.specbind.org");
            browser.SetupSequence(s => s.ExecuteScript(It.IsAny<string>()))
                .Returns("1")
                .Returns("0");
            WebDriverSupport.CurrentBrowser = browser.Object;

            WebDriverSupport.WaitForAngular();

            browser.VerifyAll();
        }

        /// <summary>
        /// Tests WaitForjQuery with a successful result, when nothing is pending.
        /// </summary>
        [TestMethod]
        public void TestWaitForjQueryWithNothingPending()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(s => s.IsClosed).Returns(false);
            browser.Setup(s => s.IsDisposed).Returns(false);
            browser.Setup(s => s.CanGetUrl()).Returns(true);
            browser.Setup(s => s.Url).Returns("http://www.specbind.org");
            browser.Setup(s => s.ExecuteScript(It.IsAny<string>())).Returns("0");
            WebDriverSupport.CurrentBrowser = browser.Object;

            WebDriverSupport.WaitForjQuery();

            browser.VerifyAll();
        }

        /// <summary>
        /// Tests WaitForjQuery with a successful result, when something is pending.
        /// </summary>
        [TestMethod]
        public void TestWaitForjQueryWithSomethingPending()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(s => s.IsClosed).Returns(false);
            browser.Setup(s => s.IsDisposed).Returns(false);
            browser.Setup(s => s.CanGetUrl()).Returns(true);
            browser.Setup(s => s.Url).Returns("http://www.specbind.org");
            browser.SetupSequence(s => s.ExecuteScript(It.IsAny<string>()))
                .Returns("1")
                .Returns("0");
            WebDriverSupport.CurrentBrowser = browser.Object;

            WebDriverSupport.WaitForjQuery();

            browser.VerifyAll();
        }

        /// <summary>
        /// Tests WaitForjQuery when the browser can get the URL.
        /// </summary>
        [TestMethod]
        public void WaitForjQuery_WhenCanGetUrlReturnsTrue_GetsUrl()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(s => s.IsClosed).Returns(false);
            browser.Setup(s => s.IsDisposed).Returns(false);
            browser.Setup(s => s.CanGetUrl()).Returns(true);
            browser.Setup(s => s.Url).Returns("http://www.specbind.org");
            browser.Setup(s => s.ExecuteScript(It.IsAny<string>())).Returns("0");
            WebDriverSupport.CurrentBrowser = browser.Object;

            WebDriverSupport.WaitForjQuery();

            browser.VerifyAll();
        }

        /// <summary>
        /// Tests WaitForjQuery when the browser cannot get the URL.
        /// </summary>
        [TestMethod]
        public void WaitForjQuery_WhenCanGetUrlReturnsFalse_DoesNotTryToGetUrl()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(s => s.IsClosed).Returns(false);
            browser.Setup(s => s.IsDisposed).Returns(false);
            browser.Setup(s => s.CanGetUrl()).Returns(false);
            WebDriverSupport.CurrentBrowser = browser.Object;

            WebDriverSupport.WaitForjQuery();

            browser.VerifyAll();
        }
    }
}
