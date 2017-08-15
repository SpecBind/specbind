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
			container.Setup(c => c.RegisterInstanceAs(It.IsAny<IBrowser>(), null, false));
			container.Setup(c => c.RegisterInstanceAs<ISettingHelper>(It.IsAny<WrappedSettingHelper>(), null, false));
            container.Setup(c => c.RegisterInstanceAs(It.IsAny<IPageMapper>(), null, false));
            container.Setup(c => c.RegisterTypeAs<ScenarioContextHelper, IScenarioContextHelper>(null));
            container.Setup(c => c.RegisterTypeAs<TokenManager, ITokenManager>(null));
            container.Setup(c => c.RegisterInstanceAs(It.IsAny<IActionRepository>(), null, false));
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
            scenarioContext.Setup(s => s.GetStepFileName(true)).Returns("TestFileName");

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns("TestFileName.jpg");
            browser.Setup(b => b.SaveHtml(It.IsAny<string>(), "TestFileName")).Returns((string)null);
            browser.Setup(b => b.Close(true));

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
        public void TestCheckForScreenshotWithNoErrorButSettingEnabledTakesSuccessfulScreenshot()
        {
            var listener = new Mock<ITraceListener>(MockBehavior.Strict);
            listener.Setup(l => l.WriteTestOutput(It.Is<string>(s => s.Contains("TestFileName.jpg"))));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetError()).Returns((Exception)null);
            scenarioContext.Setup(s => s.GetStepFileName(false)).Returns("TestFileName");

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns("TestFileName.jpg");
            browser.Setup(b => b.SaveHtml(It.IsAny<string>(), "TestFileName")).Returns((string)null);
            browser.Setup(b => b.Close(true));

            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve<IScenarioContextHelper>()).Returns(scenarioContext.Object);
            container.Setup(c => c.Resolve<ITraceListener>()).Returns(listener.Object);

            WebDriverSupport.Browser = browser.Object;
            var driverSupport = new WebDriverSupport(container.Object);

            // Setup Configuration
            var config = new ConfigurationSectionHandler
                         {
                             BrowserFactory =
                                 new BrowserFactoryConfigurationElement
                                 {
                                     CreateScreenshotOnExit = true
                                 }
                         };

            WebDriverSupport.ConfigurationMethod = new Lazy<ConfigurationSectionHandler>(() => config);

            driverSupport.CheckForScreenshot();

            config.BrowserFactory.CreateScreenshotOnExit = false;
            WebDriverSupport.ConfigurationMethod = new Lazy<ConfigurationSectionHandler>(() => config);

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
            scenarioContext.Setup(s => s.GetStepFileName(true)).Returns("TestFileName");

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns((string)null);
            browser.Setup(b => b.SaveHtml(It.IsAny<string>(), "TestFileName")).Returns((string)null);
            browser.Setup(b => b.Close(true));

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
            scenarioContext.Setup(s => s.GetStepFileName(true)).Returns("TestFileName");

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.TakeScreenshot(It.IsAny<string>(), "TestFileName")).Returns((string)null);
            browser.Setup(b => b.SaveHtml(It.IsAny<string>(), "TestFileName")).Returns((string)null);
            browser.Setup(b => b.Close(true));

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

        /// <summary>
        /// Tests the tear down after test run.
        /// </summary>
        [TestMethod]
        public void TestTearDownAfterTestRun()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Close(true));
            WebDriverSupport.Browser = browser.Object;

            WebDriverSupport.TearDownAfterTestRun();

            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the tear down after scenario when reuse browser is true.
        /// </summary>
        [TestMethod]
        public void TestTearDownAfterScenarioWhenReuseBrowserIsTrue()
        {
            
            // arrange
            var config = new ConfigurationSectionHandler
            {
                BrowserFactory =
                    new BrowserFactoryConfigurationElement
                    {
                        ReuseBrowser = true
                    }
            };

            var browser = new Mock<IBrowser>(MockBehavior.Loose);
            WebDriverSupport.Browser = browser.Object;
            WebDriverSupport.ConfigurationMethod = new Lazy<ConfigurationSectionHandler>(() => config);

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
            var config = new ConfigurationSectionHandler
            {
                BrowserFactory =
                    new BrowserFactoryConfigurationElement
                    {
                        ReuseBrowser = false
                    }
            };

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Close(true));
            WebDriverSupport.Browser = browser.Object;
            WebDriverSupport.ConfigurationMethod = new Lazy<ConfigurationSectionHandler>(() => config);

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
			browser.Setup(s => s.Url).Returns("http://www.specbind.org");
			browser.Setup(s => s.ExecuteScript(It.IsAny<string>())).Returns("0");
			WebDriverSupport.Browser = browser.Object;

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
			browser.Setup(s => s.Url).Returns("http://www.specbind.org");
			browser.SetupSequence(s => s.ExecuteScript(It.IsAny<string>()))
				.Returns("1")
				.Returns("0");
			WebDriverSupport.Browser = browser.Object;

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
			browser.Setup(s => s.Url).Returns("http://www.specbind.org");
			browser.Setup(s => s.ExecuteScript(It.IsAny<string>())).Returns("0");
			WebDriverSupport.Browser = browser.Object;

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
			browser.Setup(s => s.Url).Returns("http://www.specbind.org");
			browser.SetupSequence(s => s.ExecuteScript(It.IsAny<string>()))
				.Returns("1")
				.Returns("0");
			WebDriverSupport.Browser = browser.Object;

			WebDriverSupport.WaitForjQuery();

			browser.VerifyAll();
		}
    }
}
