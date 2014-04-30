// <copyright file="BrowserFactory.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.BrowserSupport
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;

	using SpecBind.Configuration;
	using SpecBind.Helpers;

	/// <summary>
	/// A factory class that helps create a browser instance.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public abstract class BrowserFactory
	{
	    private readonly bool driverNeedsValidation;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserFactory"/> class.
        /// </summary>
        /// <param name="driverNeedsValidation">if set to <c>true</c> driver needs validation checks at startup.</param>
	    protected BrowserFactory(bool driverNeedsValidation)
	    {
	        this.driverNeedsValidation = driverNeedsValidation;
	    }

	    /// <summary>
		/// Gets the browser for the test run.
		/// </summary>
		/// <returns>A new browser object.</returns>
		public IBrowser GetBrowser()
	    {
	        return this.LoadConfigurationAndCreateBrowser(this.CreateBrowser);
		}

		/// <summary>
		/// Gets the browser factory.
		/// </summary>
		/// <returns>A created browser factory.</returns>
		internal static BrowserFactory GetBrowserFactory()
		{
			var configSection = SettingHelper.GetConfigurationSection();
			if (configSection == null || configSection.BrowserFactory == null || string.IsNullOrWhiteSpace(configSection.BrowserFactory.Provider))
			{
				throw new InvalidOperationException("The specBind config section must have a browser factor with a provider configured.");
			}

			var type = Type.GetType(configSection.BrowserFactory.Provider, OnAssemblyCheck, OnGetType);
			if (type == null || !typeof(BrowserFactory).IsAssignableFrom(type))
			{
				throw new InvalidOperationException(string.Format("Could not load factory type: {0}. Make sure this is fully qualified and the assembly exists. Also ensure the base type is BrowserFactory", configSection.BrowserFactory.Provider));
			}

			return (BrowserFactory)Activator.CreateInstance(type);
		}

        /// <summary>
        /// Validates the driver setup.
        /// </summary>
        internal void ValidateDriverSetup()
        {
            // By default the driver doesn't need to validate anything.
            if (!this.driverNeedsValidation)
            {
                return;
            }

            this.LoadConfigurationAndCreateBrowser(
                (browserType, config) =>
                    {
                        this.ValidateDriverSetup(browserType, config);
                        return null;
                    });
        }

	    /// <summary>
	    /// Creates the browser.
	    /// </summary>
	    /// <param name="browserType">Type of the browser.</param>
	    /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
	    /// <returns>A browser object.</returns>
	    protected abstract IBrowser CreateBrowser(BrowserType browserType, BrowserFactoryConfigurationElement browserFactoryConfiguration);

	    /// <summary>
	    /// Gets the type of the browser to leverage.
	    /// </summary>
	    /// <param name="section">The configuration section.</param>
	    /// <returns>The browser type.</returns>
	    protected virtual BrowserType GetBrowserType(BrowserFactoryConfigurationElement section)
		{
            BrowserType browserType;
            if (!string.IsNullOrWhiteSpace(section.BrowserType) &&
                Enum.TryParse(section.BrowserType, true, out browserType))
			{
                return browserType;
			}

			return BrowserType.IE;
		}

        /// <summary>
        /// Validates the driver setup.
        /// </summary>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        protected virtual void ValidateDriverSetup(BrowserType browserType, BrowserFactoryConfigurationElement browserFactoryConfiguration)
	    {
	    }

        /// <summary>
        /// Loads the configuration and creates the browser object.
        /// </summary>
        /// <param name="createMethod">The create method.</param>
        /// <returns>The <see cref="IBrowser"/> object.</returns>
        private IBrowser LoadConfigurationAndCreateBrowser(Func<BrowserType, BrowserFactoryConfigurationElement, IBrowser> createMethod)
	    {
            var configSection = SettingHelper.GetConfigurationSection();

            // Default configuration settings
            var browserFactoryConfiguration = new BrowserFactoryConfigurationElement
            {
                BrowserType = Enum.GetName(typeof(BrowserType), BrowserType.IE),
                ElementLocateTimeout = TimeSpan.FromSeconds(30.0),
                PageLoadTimeout = TimeSpan.FromSeconds(30.0)
            };

            if (configSection != null && configSection.BrowserFactory != null)
            {
                browserFactoryConfiguration = configSection.BrowserFactory;
            }

            var browserType = this.GetBrowserType(browserFactoryConfiguration);
            return createMethod(browserType, browserFactoryConfiguration);
	    }

		/// <summary>
		/// Called when an assembly load failure occurs, this will try to load it from the same directory as the main assembly.
		/// </summary>
		/// <param name="assemblyName">Name of the assembly.</param>
		/// <returns>The resolved assembly.</returns>
		private static Assembly OnAssemblyCheck(AssemblyName assemblyName)
		{
			var currentLocation = Path.GetFullPath(typeof(BrowserFactory).Assembly.Location);
			if (!string.IsNullOrWhiteSpace(currentLocation) && File.Exists(currentLocation))
			{
				var parentDirectory = Path.GetDirectoryName(currentLocation);
				if (!string.IsNullOrWhiteSpace(parentDirectory) && Directory.Exists(parentDirectory))
				{
					var file = string.Format("{0}.dll", assemblyName.Name);
					var assemblyPath = Directory.EnumerateFiles(parentDirectory, file, SearchOption.AllDirectories).FirstOrDefault();
					if (assemblyPath != null)
					{
						return Assembly.LoadFile(assemblyPath);
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Called when The type should be resolved.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <param name="typeName">The type name.</param>
		/// <param name="ignoreCase">if set to <c>true</c> ignore the case.</param>
		/// <returns>The resolved type.</returns>
		private static Type OnGetType(Assembly assembly, string typeName, bool ignoreCase)
		{
			return assembly.GetType(typeName, false, ignoreCase);
		}
	}
}