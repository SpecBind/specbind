// <copyright file="BrowserFactory.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.BrowserSupport
{
    using System;
    using System.Linq;
    using System.Reflection;
    using SpecBind.Actions;
    using SpecBind.Configuration;
    using SpecBind.Extensions;
    using SpecBind.Helpers;
    using TechTalk.SpecFlow;

    /// <summary>
    /// A factory class that helps create a browser instance.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class BrowserFactory : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserFactory" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected BrowserFactory(BrowserFactoryConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public BrowserFactoryConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the test results directory.
        /// </summary>
        /// <value>
        /// The test results directory.
        /// </value>
        private string TestResultsDirectory { get; set; }

        /// <summary>
        /// Gets the browser for the test run.
        /// </summary>
        /// <returns>A new browser object.</returns>
        public IBrowser GetBrowser()
        {
            return this.CreateBrowser(this.Logger, this.TestResultsDirectory);
        }

        /// <summary>
        /// Resets the driver.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public abstract void ResetDriver(IBrowser browser);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Gets the browser factory.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="testResultsDirectory">The test results directory.</param>
        /// <returns>
        /// A created browser factory.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The specBind config section must have a browser factor with a provider configured.</exception>
        internal static BrowserFactory GetBrowserFactory(
            ILogger logger,
            ScenarioContext scenarioContext,
            string testResultsDirectory)
        {
            var configSection = SettingHelper.GetConfigurationSection();
            if (configSection == null || configSection.BrowserFactory == null || string.IsNullOrWhiteSpace(configSection.BrowserFactory.Provider))
            {
                throw new InvalidOperationException("The specBind config section must have a browser factory with a provider configured.");
            }

            var type = Type.GetType(configSection.BrowserFactory.Provider, AssemblyLoader.OnAssemblyCheck, AssemblyLoader.OnGetType);
            if (type == null || !typeof(BrowserFactory).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(string.Format("Could not load factory type: {0}. Make sure this is fully qualified and the assembly exists. Also ensure the base type is BrowserFactory", configSection.BrowserFactory.Provider));
            }

            BrowserFactory factory;

            // find a public constructor which accepts a ScenarioContext as the only parameter
            if (type.GetConstructors().Any(x => x.GetParameters().SingleOrDefault(y => y.ParameterType == typeof(ScenarioContext)) != null))
            {
                // found a public constructor that accepts a ScenarioContext as the only parameter
                factory = (BrowserFactory)Activator.CreateInstance(type, new object[] { scenarioContext });
            }
            else
            {
                // default constructor
                factory = (BrowserFactory)Activator.CreateInstance(type);
            }

            factory.Logger = logger;
            factory.TestResultsDirectory = testResultsDirectory;

            return factory;
        }

        /// <summary>
        /// Validates the driver setup.
        /// </summary>
        internal void ValidateDriverSetup()
        {
            // By default the driver doesn't need to validate anything.
            if (!this.Configuration.ValidateWebDriver)
            {
                return;
            }

            this.ValidateDriverSetup(this.Logger, this.TestResultsDirectory);
        }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        /// <returns>The browser factory configuration.</returns>
        protected static BrowserFactoryConfiguration LoadConfiguration()
        {
            BrowserFactoryConfiguration configuration = new BrowserFactoryConfiguration();

            var configSection = SettingHelper.GetConfigurationSection();

            if (configSection != null && configSection.BrowserFactory != null)
            {
                BrowserFactoryConfigurationElement browserFactoryConfiguration = configSection.BrowserFactory;
                configuration.BrowserType = GetBrowserType(browserFactoryConfiguration.BrowserType);
                configuration.CreateScreenshotOnExit = browserFactoryConfiguration.CreateScreenshotOnExit;
                configuration.ElementLocateTimeout = browserFactoryConfiguration.ElementLocateTimeout;
                configuration.EnsureCleanSession = browserFactoryConfiguration.EnsureCleanSession;
                configuration.PageLoadStrategy = browserFactoryConfiguration.PageLoadStrategy;
                configuration.PageLoadTimeout = browserFactoryConfiguration.PageLoadTimeout;
                configuration.Provider = browserFactoryConfiguration.Provider;
                configuration.ReuseBrowser = browserFactoryConfiguration.ReuseBrowser;
                configuration.ValidateWebDriver = browserFactoryConfiguration.ValidateWebDriver;
                configuration.WaitForPendingAjaxCallsVia = browserFactoryConfiguration.WaitForPendingAjaxCallsVia;
                configuration.Settings = browserFactoryConfiguration.Settings.ToKeyValuePairs().ToDictionary(x => x.Key, x => x.Value);
                configuration.UserProfilePreferences = browserFactoryConfiguration.UserProfilePreferences.ToKeyValuePairs().ToDictionary(x => x.Key, x => x.Value);
            }

            return configuration;
        }

        /// <summary>
        /// Creates the browser.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="testResultsDirectory">The test results directory.</param>
        /// <returns>
        /// A browser object.
        /// </returns>
        protected abstract IBrowser CreateBrowser(ILogger logger, string testResultsDirectory);

        /// <summary>
        /// Validates the driver setup.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="testResultsDirectory">The test results directory.</param>
        protected virtual void ValidateDriverSetup(ILogger logger, string testResultsDirectory)
        {
        }

        /// <summary>
        /// Gets the type of the browser to leverage.
        /// </summary>
        /// <param name="browserType">Type of the browser.</param>
        /// <returns>The browser type.</returns>
        private static BrowserType GetBrowserType(string browserType)
        {
            BrowserType browserTypeEnum;
            if (!string.IsNullOrWhiteSpace(browserType) &&
                Enum.TryParse(browserType, true, out browserTypeEnum))
            {
                return browserTypeEnum;
            }

            return BrowserType.IE;
        }
    }
}