// // <copyright file="SpecBindGeneratorPlugin.cs">
// //    Copyright © 2013 Dan Piessens.  All rights reserved.
// // </copyright>
namespace SpecBind.Plugin
{
    using System;

    using BoDi;

    using TechTalk.SpecFlow.Generator.Configuration;
    using TechTalk.SpecFlow.Generator.Plugins;
    using TechTalk.SpecFlow.Generator.UnitTestProvider;

    /// <summary>
    /// The main plugin class for generating SpecBind needed attributes
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class SpecBindGeneratorPlugin : IGeneratorPlugin
    {
        /// <summary>
        /// The register dependencies.
        /// </summary>
        /// <param name="container">The container.</param>
        public void RegisterDependencies(ObjectContainer container)
        {
        }

        /// <summary>
        /// The register customizations.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="generatorConfiguration">The generator configuration.</param>
        public void RegisterCustomizations(ObjectContainer container, SpecFlowProjectConfiguration generatorConfiguration)
        {
            container.RegisterTypeAs<SpecBindConfigurationProvider, ISpecBindConfigurationProvider>();

            var unitTestGenProvider = generatorConfiguration.GeneratorConfiguration.GeneratorUnitTestProvider;
            if (string.Equals(unitTestGenProvider, "mstest", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(unitTestGenProvider, "mstest.2010", StringComparison.OrdinalIgnoreCase))
            {
                container.RegisterTypeAs<SpecBindTestGeneratorProvider, IUnitTestGeneratorProvider>();
            }
        }

        /// <summary>
        /// The register configuration defaults.
        /// </summary>
        /// <param name="specFlowConfiguration">The specflow configuration.</param>
        public void RegisterConfigurationDefaults(SpecFlowProjectConfiguration specFlowConfiguration)
        {
        }
    }
}