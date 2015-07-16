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

		protected override ConfigurationElement CreateNewElement()
		{
			var newAssembly = new AssemblyElement();
			this.assemblies.Add(newAssembly);
			return newAssembly;
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return this.assemblies.Find(a => a.Equals(element));
		}
	}
}
