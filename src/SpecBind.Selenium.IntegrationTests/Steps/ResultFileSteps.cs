// <copyright file="ResultFileSteps.cs" company="">
//     Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.IntegrationTests.Steps
{
    using BoDi;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpecBind.Helpers;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Result File Steps.
    /// </summary>
    [Binding]
    public class ResultFileSteps
    {
        private readonly IObjectContainer container;
        private readonly ScenarioContext scenarioContext;
        private TestResultFileNotifier testResultFileNotifier;
        private TestContext testContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultFileSteps" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        public ResultFileSteps(IObjectContainer container, ScenarioContext scenarioContext)
        {
            this.container = container;
            this.scenarioContext = scenarioContext;
        }

        /// <summary>
        /// Runs once before each test.
        /// </summary>
        [Before(Order = 1)]
        public void Before()
        {
            this.testResultFileNotifier = new TestResultFileNotifier();
            this.container.RegisterInstanceAs(this.testResultFileNotifier, dispose: true);
            this.testResultFileNotifier.TestResultFileCreated += (object sender, TestResultFileCreatedEventArgs e) =>
            {
                if (this.testContext == null)
                {
                    this.testContext = this.scenarioContext.ScenarioContainer.Resolve<TestContext>();
                }

                this.testContext.AddResultFile(e.TestResultFilePath);
            };
        }
    }
}
