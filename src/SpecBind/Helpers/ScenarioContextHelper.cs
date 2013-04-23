// <copyright file="ScenarioContextHelper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
	using TechTalk.SpecFlow;

	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// A helper class to abstract the scenario context.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ScenarioContextHelper : IScenarioContextHelper
	{
		/// <summary>
		/// Determines whether the current scenario contains the specified tag.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <returns>
		///   <c>true</c> the current scenario contains the specified tag; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsTag(string tag)
		{
			return
				ScenarioContext.Current.ScenarioInfo.Tags.Any(
					t => string.Equals(t, tag, StringComparison.InvariantCultureIgnoreCase));
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
				return ScenarioContext.Current.Get<T>(key);
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
			ScenarioContext.Current.Set(value, key);
		}
	}
}