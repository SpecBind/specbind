// <copyright file="SpecBindConfigurationProvider.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Plugin
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml;

    using SpecBind.Configuration;

    using TechTalk.SpecFlow.Generator.Interfaces;

    /// <summary>
    /// An implementation of <see cref="ISpecBindConfigurationProvider"/>.
    /// </summary>
    public class SpecBindConfigurationProvider : ISpecBindConfigurationProvider
    {
        private readonly ProjectSettings projectSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecBindConfigurationProvider"/> class.
        /// </summary>
        /// <param name="projectSettings">The project settings.</param>
        public SpecBindConfigurationProvider(ProjectSettings projectSettings)
        {
            this.projectSettings = projectSettings;
        }

        /// <summary>
        /// Gets the type of the browser driver.
        /// </summary>
        /// <returns>The browser driver type as a string.</returns>
        public string GetBrowserDriverType()
        {
            var config = this.GetConfigurationSection();

            WriteLine(string.Format("Found Configuration Section: {0}", config != null ? "true" : "false"));
            if (config == null || config.BrowserFactory == null || string.IsNullOrEmpty(config.BrowserFactory.Provider))
            {
                return Constants.CodedUiDriverAssembly;
            }

            WriteLine(string.Format("Provider Name: {0}", config.BrowserFactory.Provider));
            var provider = config.BrowserFactory.Provider;

            var dllIndex = provider.LastIndexOf(",", StringComparison.Ordinal);
            if (dllIndex != -1 && (dllIndex + 1) < provider.Length)
            {
                provider = provider.Substring(dllIndex + 1).Trim();
            }

            if (!provider.EndsWith(".dll"))
            {
                provider = string.Concat(provider, ".dll");
            }

            return provider;
        }

        /// <summary>
        /// Gets the configuration section.
        /// </summary>
        /// <returns>The located configuration section, otherwise <c>null</c>.</returns>
        private ConfigurationSectionHandler GetConfigurationSection()
        {
            // Reflect content for now
            WriteLine("Project Folder: {0}", this.projectSettings != null ? this.projectSettings.ProjectFolder : "NONE");
            if (this.projectSettings == null || string.IsNullOrEmpty(this.projectSettings.ProjectFolder))
            {
                return null;
            }

            var directory = new DirectoryInfo(this.projectSettings.ProjectFolder);
            var file = directory.GetFiles("app.config", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (file == null)
            {
                WriteLine(@"Cannot find app.config in directory: {0}", directory.FullName);
                return null;
            }

            WriteLine("Found Configuration File: {0}", file.FullName);

            string content;
            using (var streamReader = file.OpenText())
            {
                content = streamReader.ReadToEnd();
            }

            var configDocument = new XmlDocument();
            configDocument.LoadXml(content);

            var node = configDocument.SelectSingleNode("/configuration/specBind");
            if (node == null)
            {
                WriteLine("Could not locate specBind configuration node");
                return null;
            }

            var xml = node.OuterXml;
            WriteLine("Creating configuration from XML: {0}", xml);
            return ConfigurationSectionHandler.CreateFromXml(xml);
        }

        /// <summary>
        /// Writes the line to the diagnostics.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        private static void WriteLine(string format, params object[] args)
        {
            var content = string.Format(format, args);

            if (System.Diagnostics.Debugger.IsLogging())
            {
                System.Diagnostics.Debug.WriteLine(content, "SpecBind");
            }
        }
    }
}