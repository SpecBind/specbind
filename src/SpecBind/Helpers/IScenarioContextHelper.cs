// <copyright file="IScenarioContextHelper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Helpers
{
    using System;
    using Pages;

    /// <summary>
	/// An interface that provides the target class with the scenario context.
	/// </summary>
	public interface IScenarioContextHelper
    {
        /// <summary>
        /// Determines whether the current scenario contains the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        ///   <c>true</c> the current scenario contains the specified tag; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsTag(string tag);

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <returns>The exception if it exists; otherwise <c>null</c>.</returns>
        Exception GetError();

        /// <summary>
        /// Gets the text of the currently executing step.
        /// </summary>
        /// <returns>The step text.</returns>
        string GetCurrentStepText();

        /// <summary>
        /// Gets the name of the step file.
        /// </summary>
        /// <param name="isError">Indicates whether the file is the result of an error.</param>
        /// <returns>A unique file name for the scenario.</returns>
        string GetStepFileName(bool isError);

        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <returns>The current page.</returns>
        IPage GetCurrentPage();

        /// <summary>
        /// Sets the current page.
        /// </summary>
        /// <param name="page">The page.</param>
        void SetCurrentPage(IPage page);

        /// <summary>
        /// Determines whether the current scenario's feature contains the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns><c>true</c> the current feature contains the specified tag; otherwise, <c>false</c>.</returns>
        bool FeatureContainsTag(string tag);

        /// <summary>Gets the value.</summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The value if located.</returns>
        T GetValue<T>(string key);

        /// <summary>Sets the value.</summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        void SetValue<T>(T value, string key);
    }
}