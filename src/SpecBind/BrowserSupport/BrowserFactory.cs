// <copyright file="BrowserFactory.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.BrowserSupport
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;

	using SpecBind.Helpers;

	/// <summary>
	/// A factory class that helps create a browser instance.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public abstract class BrowserFactory
	{
		/// <summary>
		/// Gets the browser for the test run.
		/// </summary>
		/// <returns>A new browser object.</returns>
		public IBrowser GetBrowser()
		{
			var browserType = this.GetBrowserType();
			return this.CreateBrowser(browserType);
		}

		/// <summary>
		/// Gets the browser factory.
		/// </summary>
		/// <returns>A created browser factory.</returns>
		internal static BrowserFactory GetBrowserFactory()
		{
			var factoryType = SettingHelper.GetAppSetting("BrowserFactory");
			if (string.IsNullOrWhiteSpace(factoryType))
			{
				throw new InvalidOperationException("An application setting named 'BrowserFactory' must be specified with the target driver type.");
			}

			var type = Type.GetType(factoryType, OnAssemblyCheck, OnGetType);
			if (type == null || !typeof(BrowserFactory).IsAssignableFrom(type))
			{
				throw new InvalidOperationException("Could not load factory type: {0}. Make sure this is fully qualified and the assembly exists. Also ensure the base type is BrowserFactory");
			}

			return (BrowserFactory)Activator.CreateInstance(type);
		}

		/// <summary>
		/// Creates the browser.
		/// </summary>
		/// <param name="browserType">Type of the browser.</param>
		/// <returns>A browser object.</returns>
		protected abstract IBrowser CreateBrowser(BrowserType browserType);

		/// <summary>
		/// Gets the type of the browser to leverage.
		/// </summary>
		/// <returns>The browser type.</returns>
		protected virtual BrowserType GetBrowserType()
		{
			BrowserType browserType;
			var settingName = SettingHelper.GetAppSetting("BrowserType");
			if (!string.IsNullOrWhiteSpace(settingName) && Enum.TryParse(settingName, true, out browserType))
			{
				return browserType;
			}

			return BrowserType.IE;
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