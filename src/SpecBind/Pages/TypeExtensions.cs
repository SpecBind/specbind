// <copyright file="TypeExtensions.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
	using System;

	/// <summary>
	/// A set of extension methods to analyze types.
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// Checks to see if the given type is an element list.
		/// </summary>
		/// <param name="propertyType">Type of the property.</param>
		/// <returns><c>true</c> if it is a list type; otherwise <c>false</c>.</returns>
		public static bool IsElementListType(this Type propertyType)
		{
			return propertyType.IsGenericType && typeof(IElementList<,>).IsAssignableFrom(propertyType.GetGenericTypeDefinition());
		}
	}
}