// <copyright file="SeleniumWindowsDriver.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.ExceptionServices;
    using System.Threading;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Appium;
    using SpecBind.Configuration;
    using SpecBind.Helpers;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Selenium Windows Driver.
    /// </summary>
    public class SeleniumWindowsDriver : SeleniumDriverBase
    {
        private const string WinAppDriverExeFilePath = @"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe";
        private const bool SaveLogFile = true;
        private static readonly Uri WinAppDriverUri = new Uri("http://127.0.0.1:4723/wd/hub");

        private static SeleniumWindowsDriverProcess winAppDriverProcess;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumWindowsDriver" /> class.
        /// </summary>
        public SeleniumWindowsDriver()
        {
            this.SupportsPageLoadTimeout = false;
            this.MaximizeWindow = false;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        public override void Stop(ScenarioContext scenarioContext = null)
        {
            if (winAppDriverProcess != null)
            {
                winAppDriverProcess.Stop();

                if (!winAppDriverProcess.IsRunning)
                {
                    winAppDriverProcess = null;
                }
            }
        }

        /// <summary>
        /// Creates the remote driver options.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>The remote driver options.</returns>
        protected override DriverOptions CreateRemoteDriverOptions(BrowserFactoryConfiguration browserFactoryConfiguration)
        {
            return null;
        }

        /// <summary>
        /// Creates the web driver from the specified browser factory configuration.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <returns>
        /// The configured web driver.
        /// </returns>
        protected override IWebDriverEx CreateLocalDriver(
            BrowserFactoryConfiguration browserFactoryConfiguration,
            ScenarioContext scenarioContext)
        {
            if (winAppDriverProcess == null)
            {
                TestResultFileNotifier testResultFileNotifier = scenarioContext?.ScenarioContainer.Resolve<TestResultFileNotifier>();

                string logFilePath = browserFactoryConfiguration.LogFilePath;
                if (SaveLogFile && string.IsNullOrEmpty(logFilePath))
                {
                    // write a log file unique to each test to prevent write conflicts
                    IScenarioContextHelper scenarioHelper = scenarioContext?.ScenarioContainer.Resolve<IScenarioContextHelper>();
                    if (scenarioHelper != null)
                    {
                        bool isError = scenarioHelper.GetError() != null;
                        string stepFileName = scenarioHelper.GetStepFileName(isError);

                        logFilePath = Path.Combine(scenarioHelper.TestResultsDirectory, $"{stepFileName}.log");
                    }
                }

                winAppDriverProcess = new SeleniumWindowsDriverProcess(
                    WinAppDriverExeFilePath,
                    WinAppDriverUri,
                    SaveLogFile,
                    logFilePath,
                    testResultFileNotifier);

                winAppDriverProcess.Start();
            }

            AppiumOptions options = new AppiumOptions
            {
                PlatformName = "Windows"
            };
            options.AddAdditionalCapability("platformVersion", "1.0");

            foreach (KeyValuePair<string, string> setting in browserFactoryConfiguration.Settings)
            {
                options.AddAdditionalCapability(setting.Key, setting.Value);
            }

            Exception lastException = null;
            for (int tries = 0; tries < 3; tries++)
            {
                try
                {
                    return new WindowsDriverEx(WinAppDriverUri, options);
                }
                catch (WebDriverException ex)
                {
                    // OpenQA.Selenium.WebDriverException: Unexpected error.
                    // System.Net.WebException: Unable to connect to the remote server
                    // ---> System.Net.Sockets.SocketException: No connection could be made because the target machine actively refused it 127.0.0.1:4723
                    Logger.Log(ex);
                    lastException = ex;
                    Thread.Sleep(1000);
                }
            }

            ExceptionDispatchInfo.Capture(lastException).Throw();
            throw lastException;
        }

        /// <summary>
        /// Downloads the driver to the specified path.
        /// </summary>
        /// <param name="driverPath">The driver path.</param>
        protected override void Download(string driverPath)
        {
            throw new NotImplementedException();
        }
    }
}
