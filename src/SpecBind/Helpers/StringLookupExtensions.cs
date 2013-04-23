// <copyright file="StringLookupExtensions.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;

	/// <summary>
	/// A set of extension methods to help lookups and mappings.
	/// </summary>
	public static class StringLookupExtensions
	{
		private static readonly Regex SingularRegex;
		private static readonly Regex StartRegex;

		/// <summary>
		/// Initializes the <see cref="StringLookupExtensions" /> class.
		/// </summary>
		static StringLookupExtensions()
		{
			SingularRegex = new Regex(@"\s(a|an)\s", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			StartRegex = new Regex(@"^the\s", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// Converts the string to a lookup key by removing whitespace and making it lower case.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <returns>A normalized string value.</returns>
		public static string ToLookupKey(this string source)
		{
			return string.IsNullOrWhiteSpace(source)
				       ? string.Empty
					   : RemoveWhitespace(source).ToLowerInvariant();
		}

		/// <summary>
		/// Checks equality after normalizing the source string.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <param name="compareValue">The compare value.</param>
		/// <returns><c>true</c> if the strings are equal; otherwise <c>false</c>.</returns>
		public static bool NormalizedEquals(this string source, string compareValue)
		{
			return string.Equals(RemoveWhitespace(source), compareValue, StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Removes the whitespace from the source string.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns>The cleaned string.</returns>
		private static string RemoveWhitespace(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				return source;
			}

			// replace any "A"
			source = SingularRegex.Replace(source, string.Empty);
			
			// replace any "the"
			source = StartRegex.Replace(source, string.Empty);
			
			return new string(source.Where(char.IsLetterOrDigit).ToArray());
		}
	}
}