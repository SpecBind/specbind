// <copyright file="ScenarioContextHelper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
    using TechTalk.SpecFlow;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TechTalk.SpecFlow.Tracing;

    /// <summary>
    /// A helper class to abstract the scenario context.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ScenarioContextHelper : IScenarioContextHelper
    {
        private readonly ScenarioContext scenarioContext;

        private readonly FeatureContext featureContext;

        /// <summary>
        /// Constructs the context helper in a thread-safe manner.
        /// </summary>
        /// <param name="scenarioContext">The current scenario context.</param>
        /// <param name="featureContext">The current feature context.</param>
        public ScenarioContextHelper(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            this.scenarioContext = scenarioContext;
            this.featureContext = featureContext;
        }

        /// <summary>
        /// Determines whether the current scenario contains the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        ///   <c>true</c> the current scenario contains the specified tag; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsTag(string tag)
        {
            return this.scenarioContext != null && FindTag(this.scenarioContext.ScenarioInfo.Tags, tag);
        }

        /// <summary>
        /// Gets the name of the step file.
        /// </summary>
        /// <returns>A unique file name for the scenario.</returns>
        public string GetStepFileName()
        {
            return string.Format(
                "error_{0}_{1}_{2}",
                this.featureContext != null
                    ? this.featureContext.FeatureInfo.Title.ToIdentifier()
                    : Guid.NewGuid().ToString(),
                this.scenarioContext != null
                    ? this.scenarioContext.ScenarioInfo.Title.ToIdentifier()
                    : Guid.NewGuid().ToString(),
                DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        }

        /// <summary>
        /// Determines whether the current scenario's feature contains the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        ///   <c>true</c> the current feature contains the specified tag; otherwise, <c>false</c>.
        /// </returns>
        public bool FeatureContainsTag(string tag)
        {
            return this.featureContext != null && FindTag(this.featureContext.FeatureInfo.Tags, tag);
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <returns>The exception if it exists; otherwise <c>null</c>.</returns>
        public Exception GetError()
        {
            return this.scenarioContext != null ? this.scenarioContext.TestError : null;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The value if located.</returns>
        public T GetValue<T>(string key)
        {
            try
            {
                return this.scenarioContext.Get<T>(key);
            }
            catch (KeyNotFoundException)
            {
                return default(T);
            }
        }


        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        public void SetValue<T>(T value, string key)
        {
            this.scenarioContext.Set(value, key);
        }

        /// <summary>
        /// Determines whether the specified tags contains the given tag.
        /// </summary>
        /// <param name="tags">The tags collection.</param>
        /// <param name="searchTag">The search tag.</param>
        /// <returns><c>true</c> if the specified tags contains the given tag; otherwise, <c>false</c>.</returns>
        private static bool FindTag(IEnumerable<string> tags, string searchTag)
        {
            return tags != null && tags.Any(t => string.Equals(t, searchTag, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}