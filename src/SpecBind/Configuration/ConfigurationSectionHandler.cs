// <copyright file="ConfigurationSectionHandler.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Configuration
{
	using System.Configuration;
	using System.IO;
	using System.Xml;

    /// <summary>
	/// Handles the SpecBind configuration section to obtain settings.
	/// </summary>
	public class ConfigurationSectionHandler : ConfigurationSection
	{
		private const string ApplicationElementName = @"application";
		private const string BrowserFactoryElementName = @"browserFactory";

		/// <summary>
		/// Gets or sets the application configuration element.
		/// </summary>
		/// <value>The application configuration element.</value>
		[ConfigurationProperty(ApplicationElementName, DefaultValue = null, IsRequired = false)]
		public ApplicationConfigurationElement Application
		{
			get
			{
				return (ApplicationConfigurationElement)this[ApplicationElementName];
			}

			set
			{
				this[ApplicationElementName] = value;
			}
		}

		/// <summary>
		/// Gets or sets the browser factory configuration element.
		/// </summary>
		/// <value>The browser factory configuration element.</value>
		[ConfigurationProperty(BrowserFactoryElementName, DefaultValue = null, IsRequired = false)]
		public BrowserFactoryConfigurationElement BrowserFactory
		{
			get
			{
				return (BrowserFactoryConfigurationElement)this[BrowserFactoryElementName];
			}

			set
			{
				this[BrowserFactoryElementName] = value;
			}
		}

        /// <summary>
        /// Creates the configuration node from XML.
        /// </summary>
        /// <param name="xmlContent">Content of the XML.</param>
        /// <returns>The created configuration section.</returns>
        public static ConfigurationSectionHandler CreateFromXml(string xmlContent)
        {
            var section = new ConfigurationSectionHandler();
            section.Init();

            // ReSharper disable once AssignNullToNotNullAttribute
            section.Reset(null);

            using (var reader = new XmlTextReader(new StringReader(xmlContent.Trim())))
            {
                section.DeserializeSection(reader);
            }

            section.ResetModified();
            return section;
        }
	}
}