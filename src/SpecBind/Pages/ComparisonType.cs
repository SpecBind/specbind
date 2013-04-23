// <copyright file="ComparisonType.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
	/// <summary>
	/// Enumerates the various comparison types supported.
	/// </summary>
	public enum ComparisonType
	{
		/// <summary>
		/// The values equal each other.
		/// </summary>
		Equals = 0,

		/// <summary>
		/// The values do not equal each other.
		/// </summary>
		DoesNotEqual = 1,

		/// <summary>
		/// The value contains the comparison value.
		/// </summary>
		Contains = 2,

		/// <summary>
		/// The value starts with the comparison value.
		/// </summary>
		StartsWith = 3,

		/// <summary>
		/// The value end with the comparison value.
		/// </summary>
		EndsWith = 4,

		/// <summary>
		/// The value does not contain the comparison value.
		/// </summary>
		DoesNotContain = 5,

		/// <summary>
		/// The element that contains the value is enabled.
		/// </summary>
		Enabled = 6,

		/// <summary>
		/// The element that contains the value exists.
		/// </summary>
		Exists = 7
	}
}