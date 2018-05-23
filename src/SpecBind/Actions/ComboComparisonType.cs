// <copyright file="ComboComparisonType.cs">
//    Copyright © 2015 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    /// <summary>
    /// Enumerates the supported comparison types for the <see cref="ValidateComboBoxAction"/>
    /// </summary>
    public enum ComboComparisonType
    {
        /// <summary>
        /// The combo box contains the listed items
        /// </summary>
        Contains = 0,

        /// <summary>
        /// The combo box does not contain the listed items
        /// </summary>
        DoesNotContain = 1,

        /// <summary>
        /// The combo box contains only the listed items
        /// </summary>
        ContainsExactly = 2,
    }
}