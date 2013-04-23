// <copyright file="IScenarioContextHelper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Helpers
{
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

		/// <summary>Sets the value.</summary>
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