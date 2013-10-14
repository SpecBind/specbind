// <copyright file="WrappedSettingHelper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
    /// <summary>
    /// An implementation of <see cref="ISettingHelper"/> that wraps the static settings class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class WrappedSettingHelper : ISettingHelper
    {
        /// <summary>
        /// Gets a value indicating whether highlight mode is enabled in configuration or app settings.
        /// </summary>
        /// <returns><c>true</c> if highlight mode is enabled, <c>false</c> otherwise</returns>
        public bool HighlightModeEnabled
        {
            get
            {
                return SettingHelper.HighlightModeEnabled();
            }
        }
    }
}