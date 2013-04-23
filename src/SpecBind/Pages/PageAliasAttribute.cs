// <copyright file="PageAliasAttribute.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
	using System;

	/// <summary>
	/// An attribute that maps the page to a name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class PageAliasAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PageAliasAttribute" /> class.
		/// </summary>
		/// <param name="name">The alias name.</param>
		public PageAliasAttribute(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; private set; } 
	}
}