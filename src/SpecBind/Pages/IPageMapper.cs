// <copyright file="IPageMapper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
    using System;

    /// <summary>
    /// An interface for a service that locates pages.
    /// </summary>
    public interface IPageMapper
    {
        /// <summary>
        /// Gets the map count.
        /// </summary>
        /// <value>
        /// The map count.
        /// </value>
        int MapCount { get; }

        /// <summary>
        /// Maps the loaded assemblies into the type mapper.
        /// </summary>
        /// <param name="pageBaseType">Type of the page base.</param>
        void Initialize(Type pageBaseType);

        /// <summary>Gets the page type from the given name</summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>The resolved type; otherwise <c>null</c>.</returns>
        Type GetTypeFromName(string typeName);
    }
}