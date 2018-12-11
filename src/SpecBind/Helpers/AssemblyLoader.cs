// <copyright file="AssemblyLoader.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using SpecBind.BrowserSupport;

    /// <summary>
    /// Assembly Loader.
    /// </summary>
    internal static class AssemblyLoader
    {
        /// <summary>
        /// Called when an assembly load failure occurs, this will try to load it from the same directory as the main assembly.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>The resolved assembly.</returns>
        internal static Assembly OnAssemblyCheck(AssemblyName assemblyName)
        {
            try
            {
                // try load assembly from app domain first rather than filesystem as test runners
                // can place dlls in separate directories and may not always work as below.
                var assembly = Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    return assembly;
                }
            }

            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // Ignore and resume as previous.
            }

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
        internal static Type OnGetType(Assembly assembly, string typeName, bool ignoreCase)
        {
            return assembly.GetType(typeName, false, ignoreCase);
        }
    }
}
