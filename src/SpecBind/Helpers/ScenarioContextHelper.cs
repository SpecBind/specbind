// <copyright file="ScenarioContextHelper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Text;
	using TechTalk.SpecFlow;
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
		/// Gets the text of the currently executing step.
		/// </summary>
		/// <remarks>
		/// Logic provided by Gaspar Nagy on StackOverflow
		/// </remarks>
		/// <returns>The step text.</returns>
		public string GetCurrentStepText()
		{
			int currentPositionText = 0;
			try
			{
				var frames = new StackTrace(true).GetFrames();
				if (frames != null)
				{
					var featureFileFrame = frames.FirstOrDefault(f =>
																 f.GetFileName() != null &&
																 f.GetFileName().EndsWith(".feature"));

					if (featureFileFrame != null)
					{
						var lines = File.ReadAllLines(featureFileFrame.GetFileName());
						const int frameSize = 20;
						int currentLine = featureFileFrame.GetFileLineNumber() - 1;
						int minLine = Math.Max(0, currentLine - frameSize);
						int maxLine = Math.Min(lines.Length - 1, currentLine + frameSize);

						for (int lineNo = currentLine - 1; lineNo >= minLine; lineNo--)
						{
							if (lines[lineNo].TrimStart().StartsWith("Scenario:"))
							{
								minLine = lineNo + 1;
								break;
							}
						}

						for (int lineNo = currentLine + 1; lineNo <= maxLine; lineNo++)
						{
							if (lines[lineNo].TrimStart().StartsWith("Scenario:"))
							{
								maxLine = lineNo - 1;
								break;
							}
						}

						for (int lineNo = minLine; lineNo <= maxLine; lineNo++)
						{
							if (lineNo == currentLine)
							{
								currentPositionText = lineNo - minLine;
								var result = new StringBuilder(lines[lineNo]);
								for (int i = lineNo + 1; i < lines.Length; i++)
								{
									if (!lines[i].TrimStart().StartsWith("|"))
									{
										break;
									}

									result.AppendLine();
									result.Append(lines[i]);
								}

								return result.ToString();
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}

			return "(Unable to detect current step)";
		}

	    /// <summary>
	    /// Gets the name of the step file.
	    /// </summary>
	    /// <param name="isError">A value indicating whether the file is the result of an error or not.</param>
	    /// <returns>A unique file name for the scenario.</returns>
	    public string GetStepFileName(bool isError)
	    {
            return
                $"{(isError ? "error" : "scenario")}_{(this.featureContext != null ? this.featureContext.FeatureInfo.Title.ToIdentifier() : Guid.NewGuid().ToString())}_{(this.scenarioContext != null ? this.scenarioContext.ScenarioInfo.Title.ToIdentifier() : Guid.NewGuid().ToString())}_{DateTime.Now:yyyyMMdd_HHmmss}";
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
		/// Gets the value.
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