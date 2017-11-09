// <copyright file="ApplicationSteps.cs" company="">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind
{
    using System;
    using BoDi;
    using BrowserSupport;
    using Configuration;
    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    /// <summary>
    /// Application Steps.
    /// </summary>
    [Binding]
    public sealed class ApplicationSteps
    {
        private readonly IObjectContainer objectContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSteps"/> class.
        /// </summary>
        /// <param name="objectContainer">The object container.</param>
        public ApplicationSteps(IObjectContainer objectContainer)
        {
            this.objectContainer = objectContainer;
        }

        /// <summary>
        /// Given the application configuration.
        /// </summary>
        /// <param name="applicationConfiguration">The application configuration.</param>
        [Given(@"the application configuration")]
        public void GivenTheApplicationConfiguration(ApplicationConfigurationElement applicationConfiguration)
        {
            // get the current application configuration
            var configSection = Helpers.SettingHelper.GetConfigurationSection();
            ApplicationConfigurationElement currentApplicationConfiguration = configSection.Application;

            // overwrite the start url if it was specified
            if (!string.IsNullOrEmpty(applicationConfiguration.StartUrl))
            {
                currentApplicationConfiguration.StartUrl = applicationConfiguration.StartUrl;
            }

            WebDriverSupport.ApplicationConfigurationMethod = new Lazy<ApplicationConfigurationElement>(() => currentApplicationConfiguration);
        }

        /// <summary>
        /// Transforms the specified table to a ApplicationConfigurationElement.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>The application configuration element</returns>
        [StepArgumentTransformation]
        public ApplicationConfigurationElement Transform(Table table)
        {
            ApplicationConfigurationElement config = table.CreateInstance<ApplicationConfigurationElement>();

            return config;
        }
    }
}
