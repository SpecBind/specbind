// <copyright file="BrowserFactoryConfigurationElement.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Configuration
{
    using System;
    using System.Configuration;

	/// <summary>
	/// A configuration element for the browser factory.
	/// </summary>
	public class BrowserFactoryConfigurationElement : ConfigurationElement
	{
	    private const string ElementLocateTimeoutElementName = "elementLocateTimeout";
        private const string EnsureCleanSessionElementName = "ensureCleanSession";
	    private const string PageLoadTimeoutElementName = "pageLoadTimeout";
		private const string ProviderElementName = "provider";
		private const string BrowserTypeElementName = "browserType";
        private const string SettingsElementName = "settings";


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
        /// Gets or sets the timeout for waiting to locate an element.
        /// </summary>
        /// <value>The timeout for waiting for waiting to locate an element.</value>
        [ConfigurationProperty(ElementLocateTimeoutElementName, DefaultValue = "00:00:30", IsRequired = false)]
        public TimeSpan ElementLocateTimeout
        {
            get
            {
                return (TimeSpan)this[ElementLocateTimeoutElementName];
            }

            set
            {
                this[ElementLocateTimeoutElementName] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the session cache and cookies should be cleared before starting.
        /// </summary>
        /// <value><c>true</c> if the session should be cleared; otherwise <c>false</c>.</value>
        [ConfigurationProperty(EnsureCleanSessionElementName, DefaultValue = false, IsRequired = false)]
        public bool EnsureCleanSession
        {
            get
            {
                return (bool)this[EnsureCleanSessionElementName];
            }

            set
            {
                this[EnsureCleanSessionElementName] = value;
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

        /// <summary>
        /// Gets or sets the timeout for waiting for a page to load.
        /// </summary>
        /// <value>The timeout for waiting for a page to load.</value>
        [ConfigurationProperty(PageLoadTimeoutElementName, DefaultValue = "00:00:30", IsRequired = false)]
        public TimeSpan PageLoadTimeout
        {
            get
            {
                return (TimeSpan)this[PageLoadTimeoutElementName];
            }

            set
            {
                this[PageLoadTimeoutElementName] = value;
            }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        [ConfigurationProperty(SettingsElementName, IsRequired = false)]
        public NameValueConfigurationCollection Settings
        {
            get
            {
                return this[SettingsElementName] as NameValueConfigurationCollection ?? new NameValueConfigurationCollection();
            }
        }
	}
}