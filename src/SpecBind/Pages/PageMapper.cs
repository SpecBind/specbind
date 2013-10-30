// <copyright file="PageMapper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using SpecBind.Helpers;

	/// <summary>
	/// A mapping class to process all the items.
	/// </summary>
	public class PageMapper : IPageMapper
	{
		private readonly Dictionary<string, Type> pageTypeCache;

		/// <summary>
		/// Initializes the <see cref="PageMapper" /> class.
		/// </summary>
		public PageMapper()
		{
			this.pageTypeCache = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Gets the map count.
		/// </summary>
		/// <value>
		/// The map count.
		/// </value>
		public int MapCount
		{
			get
			{
				return this.pageTypeCache.Count;
			}
		}

		/// <summary>
		/// Gets the page type from the given name
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		/// <returns>The resolved type; otherwise <c>null</c>.</returns>
		public Type GetTypeFromName(string typeName)
		{
			if (string.IsNullOrWhiteSpace(typeName))
			{
				return null;
			}

			Type pageType;
			return this.pageTypeCache.TryGetValue(typeName.ToLookupKey(), out pageType) ? pageType : null;
		}

		/// <summary>
		/// Maps the loaded assemblies into the type mapper.
		/// </summary>
		/// <param name="pageBaseType">Type of the page base.</param>
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public void Initialize(Type pageBaseType)
		{
			// There are several blank catches to avoid loading bad asssemblies.
			try
			{
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && !a.GlobalAssemblyCache))
				{
					try
					{
						// Map all public types.
						this.MapAssemblyTypes(assembly.GetExportedTypes(), pageBaseType);
					}
					catch (SystemException)
					{
					}
				}
			}
			catch (SystemException)
			{
			}
		}

		/// <summary>
		/// Maps the assembly types.
		/// </summary>
		/// <param name="types">The types.</param>
		/// <param name="pageBaseType">Type of the page base.</param>
		internal void MapAssemblyTypes(IEnumerable<Type> types, Type pageBaseType)
		{
			foreach (var pageType in types.Where(t => t.IsClass &&  !t.IsAbstract
				&& (pageBaseType == null || (t.BaseType != null && pageBaseType.IsAssignableFrom(t.BaseType)))))
			{
				var initialName = pageType.Name;
				if (initialName.EndsWith("Page", StringComparison.InvariantCultureIgnoreCase))
				{
					initialName = initialName.Substring(0, initialName.Length - 4);
				}

				if (!this.pageTypeCache.ContainsKey(initialName))
				{
					this.pageTypeCache.Add(initialName, pageType);
				}

				foreach (var aliasAttribute in pageType.GetCustomAttributes(typeof(PageAliasAttribute), true).OfType<PageAliasAttribute>())
				{
					var key = aliasAttribute.Name;
					if (!this.pageTypeCache.ContainsKey(key))
					{
						this.pageTypeCache.Add(key, pageType);
					}
				}
			}
		}
	}
}