// <copyright file="BrowserSteps.cs" company="">
//     Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind
{
    using System;
    using System.Configuration;
    using System.Linq;

    using BoDi;
    using BrowserSupport;
    using Configuration;
    using Helpers;
    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    /// <summary>
    /// Browser Steps. This class cannot be inherited.
    /// </summary>
    [Binding]
    public sealed class BrowserSteps
    {
        private readonly IObjectContainer objectContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserSteps"/> class.
        /// </summary>
        /// <param name="objectContainer">The object container.</param>
        public BrowserSteps(IObjectContainer objectContainer)
        {
            this.objectContainer = objectContainer;
        }

        /// <summary>
        /// Given the browser factory configuration.
        /// </summary>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        [Given(@"the browser factory configuration")]
        public void GivenTheBrowserFactoryConfiguration(BrowserFactoryConfigurationElement browserFactoryConfiguration)
        {
            // get browser factory provider from configuration if its not already set
            if (string.IsNullOrEmpty(browserFactoryConfiguration.Provider))
            {
                var configSection = SettingHelper.GetConfigurationSection();
                browserFactoryConfiguration.Provider = configSection.BrowserFactory.Provider;
            }

            WebDriverSupport.ConfigurationMethod = new Lazy<BrowserFactoryConfigurationElement>(() => browserFactoryConfiguration);

            WebDriverSupport.InitializeBrowser(this.objectContainer);
        }

        /// <summary>
        /// Transforms the specified table to a BrowserFactoryConfigurationElement.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>The browser factory configuration element</returns>
        [StepArgumentTransformation]
        public BrowserFactoryConfigurationElement Transform(Table table)
        {
            BrowserFactoryConfigurationElement config = table.CreateInstance<BrowserFactoryConfigurationElement>();

            NameValueConfigurationCollection settings = this.GetNameValueConfigurationCollection(table, "Settings");
            foreach (NameValueConfigurationElement setting in settings)
            {
                config.Settings.Add(setting);
            }

            NameValueConfigurationCollection userProfilePreferences = this.GetNameValueConfigurationCollection(table, "User Profile Preferences");
            foreach (NameValueConfigurationElement userProfilePreference in userProfilePreferences)
            {
                config.UserProfilePreferences.Add(userProfilePreference);
            }

            return config;
        }

        private NameValueConfigurationCollection GetNameValueConfigurationCollection(Table table, string fieldName)
        {
            NameValueConfigurationCollection collection = new NameValueConfigurationCollection();

            TableRow row = table.Rows.SingleOrDefault(x =>
                (x.Values.FirstOrDefault(y => y /* Field */ == fieldName) != null));
            if (row != null)
            {
                string fieldValue = row.Values.Skip(1).Single(); // Value
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    string[] pairs = fieldValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string pair in pairs)
                    {
                        string[] nameValuePair = pair.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                        if ((nameValuePair.Length == 0) || (nameValuePair.Length > 2))
                        {
                            throw new ArgumentException($"{fieldName} must be separated by a semi-colon and name-value pairs separated by an equals sign.");
                        }

                        string name = nameValuePair[0];
                        string value = string.Empty;

                        if (nameValuePair.Length > 1)
                        {
                            value = nameValuePair[1];
                        }

                        collection.Add(new NameValueConfigurationElement(name, value));
                    }
                }
            }

            return collection;
        }
    }
}
