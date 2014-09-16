// <copyright file="ResourceLocator.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Resources;

	/// <summary>
	/// A class that assists in locating resources in the project.
	/// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class ResourceLocator
	{
		private static readonly List<Tuple<string, ResourceManager>> ResourceManagers = new List<Tuple<string, ResourceManager>>();
		private static bool initialized;

		/// <summary>
		/// Gets the resource.
		/// </summary>
		/// <param name="itemName">Name of the item.</param>
		/// <returns>The data if found; otherwise <c>null</c>.</returns>
		public static byte[] GetResource(string itemName)
		{
			Initialize();

			return ResourceManagers.Select(r => r.Item2.GetFileBinaryResource(itemName)).FirstOrDefault(d => d != null);
		}

		/// <summary>
		/// Gets the resource names.
		/// </summary>
		/// <returns>A list of resource names.</returns>
		public static IEnumerable<string> GetResourceNames()
		{
			Initialize();

			return ResourceManagers.Select(r => r.Item1);
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		private static void Initialize()
		{
			if (initialized)
			{
				return;
			}

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && !a.GlobalAssemblyCache))
			{
				try
				{
					foreach (
						var type in
							assembly.GetExportedTypes().Where(t => t.Name.EndsWith("Resources", StringComparison.InvariantCultureIgnoreCase)))
					{
						var property =
							type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty)
							    .FirstOrDefault(p => p.Name == "ResourceManager" && p.PropertyType == typeof(ResourceManager));

						if (property == null || property.GetGetMethod() == null)
						{
							continue;
						}

						var methodInfo = property.GetGetMethod();
						var resourceManager = methodInfo.Invoke(null, null) as ResourceManager;
						if (resourceManager != null)
						{
							ResourceManagers.Add(new Tuple<string, ResourceManager>(type.FullName, resourceManager));
						}
					}
				}
				catch (SystemException)
				{
					// Drop exception, nothing we can do
				}
			}

			initialized = true;
		}
	}
}