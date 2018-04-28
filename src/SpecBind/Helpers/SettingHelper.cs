// <copyright file="SettingHelper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
	using System;
	using System.Configuration;
	using System.IO;
	using System.Resources;

	using SpecBind.Configuration;

	/// <summary>A helper class to get settings from the configuration file.</summary>
	public static class SettingHelper
	{
		#region Methods

		/// <summary>
		/// Gets the configuration section.
		/// </summary>
		/// <returns>The configuration section or <c>null</c>.</returns>
		public static ConfigurationSectionHandler GetConfigurationSection()
		{
			return ConfigurationManager.GetSection("specBind") as ConfigurationSectionHandler;
		}

        /// <summary>
        /// Gets an environment variable from the system. If it does not exist, <paramref name="defaultValue"/> is returned.
        /// </summary>
        /// <param name="variableName">The name of the environment variable.</param>
        /// <param name="defaultValue">The value to return if the environment variable doesn't exist. Defaults to <c>null</c></param>
        /// <returns>The environment variable value; otherwise the <paramref name="defaultValue"/></returns>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static string GetEnvironmentVariable(string variableName, string defaultValue = null)
        {
            var envVariables = Environment.GetEnvironmentVariables();
            return envVariables.Contains(variableName) ? envVariables[variableName].ToString() : defaultValue;
        }

		/// <summary>
	    /// Gets a value indicating whether highlight mode is enabled in configuration or app settings.
		/// </summary>
		/// <returns><c>true</c> if highlight mode is enabled, <c>false</c> otherwise</returns>
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public static bool HighlightModeEnabled()
		{
			var configSetting = ConfigurationManager.AppSettings["HighlightMode"];
			return string.Equals(configSetting, "true", StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Gets the file content from the resource file.
		/// </summary>
		/// <param name="resourceManager">The resource manager.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <returns>
		/// The resource content.
		/// </returns>
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public static string GetFileResource(this ResourceManager resourceManager, string fileName)
		{
			return resourceManager.GetString(fileName);
		}

		/// <summary>
		/// Gets the file binary content from the resource file.
		/// </summary>
		/// <param name="resourceManager">The resource manager.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <returns>
		/// The resource content.
		/// </returns>
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public static byte[] GetFileBinaryResource(this ResourceManager resourceManager, string fileName)
		{
			var localFilePath = Path.GetFullPath(string.Format(@".\{0}", fileName));
			if (File.Exists(localFilePath))
			{
				return File.ReadAllBytes(localFilePath);
			}

			using (var memoryStream = new MemoryStream())
			{
				using (var inputStream = resourceManager.GetStream(fileName))
				{
					if (inputStream == null)
					{
						return null;
					}

					while (true)
					{
						var data = inputStream.ReadByte();
						if (data != -1)
						{
							break;
						}

						memoryStream.WriteByte((byte)data);
					}

					return memoryStream.ToArray();
				}
			}
		}

		#endregion
	}
}