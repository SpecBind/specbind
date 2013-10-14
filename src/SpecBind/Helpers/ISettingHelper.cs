// <copyright file="ISettingHelper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
    /// <summary>
    /// An interface that allows classes access to configuration settings.
    /// </summary>
    public interface ISettingHelper
    {
        /// <summary>
        /// Gets a value indicating whether highlight mode is enabled in configuration or app settings.
        /// </summary>
        /// <returns><c>true</c> if highlight mode is enabled, <c>false</c> otherwise</returns>
        bool HighlightModeEnabled { get; }
    }
}