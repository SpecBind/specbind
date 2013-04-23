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
	}
}