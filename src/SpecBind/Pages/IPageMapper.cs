// <copyright file="IPageMapper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
	using System;

	/// <summary>
	/// An interface for a service that locates pages.
	/// </summary>
	public interface IPageMapper
	{
		/// <summary>Gets the page type from the given name</summary>
		/// <param name="typeName">Name of the type.</param>
		/// <returns>The resolved type; otherwise <c>null</c>.</returns>
		Type GetTypeFromName(string typeName);
	}
}