// <copyright file="ILocationProvider.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
    using System.Collections.Generic;

    /// <summary>
    /// Location Provider
    /// </summary>
    public interface ILocationProvider
    {
        /// <summary>
        /// Gets the page location.
        /// </summary>
        /// <returns>A collection of URIs to validate.</returns>
        IList<string> GetPageLocation();
    }
}