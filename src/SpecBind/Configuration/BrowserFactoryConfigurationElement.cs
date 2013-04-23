// <copyright file="BrowserFactoryConfigurationElement.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Configuration
{
	using System.Configuration;

	/// <summary>
	/// A configuration element for the browser factory.
	/// </summary>
	public class BrowserFactoryConfigurationElement : ConfigurationElement
	{
		private const string ProviderElementName = "provider";
		private const string BrowserTypeElementName = "browserType";


		/// <summary>
		/// Gets or sets the type of the browser to use for testing.
		/// </summary>
		/// <value>The type of the browser to use for testing.</value>
		[ConfigurationProperty(BrowserTypeElementName, DefaultValue = "IE", IsRequired = false)]
		public string BrowserType
		{
			get
			{
				return (string)this[BrowserTypeElementName];
			}

			set
			{
				this[BrowserTypeElementName] = value;
			}
		}

		/// <summary>
		/// Gets or sets the provider for the element.
		/// </summary>
		/// <value>The provider.</value>
		[ConfigurationProperty(ProviderElementName, DefaultValue = null, IsRequired = true)]
		public string Provider
		{
			get
			{
				return (string)this[ProviderElementName];
			}

			set
			{
				this[ProviderElementName] = value;
			}
		}
	}
}