// <copyright file="AssemblyCollection.cs">
//    Copyright © 2015 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Configuration
{
	using System.Collections.Generic;
	using System.Configuration;

	/// <summary>
	/// Configuration collection for holding assembly information.
	/// </summary>
	public class AssemblyCollection : ConfigurationElementCollection
	{
		private readonly List<AssemblyElement> assemblies = new List<AssemblyElement>();

		/// <summary>
		/// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
		/// </summary>
		/// <returns>
		/// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			var newAssembly = new AssemblyElement();
			this.assemblies.Add(newAssembly);
			return newAssembly;
		}

		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
		/// </returns>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for. </param>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return this.assemblies.Find(a => a.Equals(element));
		}
	}
}
