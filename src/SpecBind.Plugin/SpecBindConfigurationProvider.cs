// <copyright file="SpecBindConfigurationProvider.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace Specflow.CodedUI
{
    using System.Configuration;
    using System.Diagnostics;

    using SpecBind.Configuration;

    /// <summary>
    /// An implementation of <see cref="ISpecBindConfigurationProvider"/>.
    /// </summary>
    public class SpecBindConfigurationProvider : ISpecBindConfigurationProvider
    {
        /// <summary>
        /// Gets the type of the browser driver.
        /// </summary>
        /// <returns>The browser driver type as a string.</returns>
        public string GetBrowserDriverType()
        {
            var config = (ConfigurationSectionHandler)ConfigurationManager.GetSection("specBind");

            Debug.WriteLine(string.Format("Found Configuration Section: {0}", config != null ? "true" : "false"));
            if (config == null || config.BrowserFactory == null || string.IsNullOrEmpty(config.BrowserFactory.Provider))
            {
                return Constants.CodedUiDriverAssembly;
            }

            Debug.WriteLine(string.Format("Provider Name: {0}", config.BrowserFactory.Provider));
            var provider = config.BrowserFactory.Provider;

            var dllIndex = provider.LastIndexOf(",", System.StringComparison.Ordinal);
            if (dllIndex != -1 && (dllIndex + 1) < provider.Length)
            {
                provider = provider.Substring(dllIndex + 1).Trim();
            }

            if (!provider.EndsWith(".dll"))
            {
                provider = string.Concat(provider, ".dll");
            }

            return provider;
        }
    }
}