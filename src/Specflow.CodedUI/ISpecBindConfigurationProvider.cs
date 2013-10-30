// <copyright file="ISpecBindConfigurationProvider.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace Specflow.CodedUI
{
    /// <summary>
    /// A provider interface to get the type of driver being used.
    /// </summary>
    public interface ISpecBindConfigurationProvider
    {
        /// <summary>
        /// Gets the type of the browser driver.
        /// </summary>
        /// <returns>The browser driver type as a string.</returns>
        string GetBrowserDriverType();
    }
}