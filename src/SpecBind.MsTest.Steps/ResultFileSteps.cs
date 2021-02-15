// <copyright file="ResultFileSteps.cs" company="SpecBind">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.MsTest.Steps
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpecBind.Helpers;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Result File Steps.
    /// </summary>
    [Binding]
    public class ResultFileSteps
    {
        private readonly TestResultFileNotifier testResultFileNotifier;
        private readonly ScenarioContext scenarioContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultFileSteps" /> class.
        /// </summary>
        /// <param name="testResultFileNotifier">The test result file notifier.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        public ResultFileSteps(
            TestResultFileNotifier testResultFileNotifier,
            ScenarioContext scenarioContext)
        {
            this.testResultFileNotifier = testResultFileNotifier;
            this.scenarioContext = scenarioContext;
        }

        /// <summary>
        /// Runs once before each test.
        /// </summary>
        [BeforeScenario(Order = 2)]
        public void BeforeScenario()
        {
            IScenarioContextHelper scenarioHelper = this.scenarioContext.ScenarioContainer.Resolve<IScenarioContextHelper>();
            TestContext testContext = this.scenarioContext.ScenarioContainer.Resolve<TestContext>();

            Console.WriteLine($"Setting test results directory to '{testContext.TestResultsDirectory}'...");

            scenarioHelper.SetTestResultsDirectory(testContext.TestResultsDirectory);

            this.testResultFileNotifier.TestResultFileCreated += (s, e) =>
            {
                Console.WriteLine($"Test result file created: '{e.TestResultFilePath}'.");
                testContext.AddResultFile(e.TestResultFilePath);
            };
        }
    }
}
