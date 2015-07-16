// <copyright file="ApplicationConfigurationElement.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Configuration
{
	using System.Configuration;

	/// <summary>
	/// A configuration element that handles settings for the application.
	/// </summary>
	public class ApplicationConfigurationElement : ConfigurationElement
	{
		private const string StartUrlElement = @"startUrl";
		private const string ExcludedAssembliesElement = @"excludedAssemblies";

		/// <summary>
		/// Gets or sets the application's start URL setting.
		/// </summary>
		/// <value>The application's start URL setting.</value>
		[ConfigurationProperty(StartUrlElement, DefaultValue = null, IsRequired = false)]
		public string StartUrl
		{
			get
			{
				return (string)this[StartUrlElement];
			}

			set
			{
				this[StartUrlElement] = value;
			}
		}

		/// <summary>
		/// Gets the list of excluded assemblies, if any are supplied.
		/// </summary>
		/// <value>The list of excluded assemblies.</value>
		[ConfigurationProperty(ExcludedAssembliesElement, IsRequired = false)]
		[ConfigurationCollection(typeof(AssemblyElement))]
		public AssemblyCollection ExcludedAssemblies
		{
			get { return (AssemblyCollection)base[ExcludedAssembliesElement]; }
		}
	}
}