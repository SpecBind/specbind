// <copyright file="PageHistoryServiceConfigurationElement.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Configuration
{
    using System.Configuration;

    /// <summary>
    /// A configuration element for the page history service.
    /// </summary>
    public class PageHistoryServiceConfigurationElement : ConfigurationElement
    {
        private const string ProviderElementName = "provider";

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