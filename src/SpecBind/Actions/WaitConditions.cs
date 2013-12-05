// <copyright file="WaitConditions.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Actions
{
    /// <summary>
    /// Enumerates the different conditions to wait for on a control.
    /// </summary>
    public enum WaitConditions
    {
        /// <summary>
        /// Checks that the control exists and is visible.
        /// </summary>
        Exists = 0,

        /// <summary>
        /// Checks that the control does not exist.
        /// </summary>
        NotExists = 1,

        /// <summary>
        /// Checks that the control is enabled.
        /// </summary>
        Enabled = 2,

        /// <summary>
        /// Checks that the control is not enabled.
        /// </summary>
        NotEnabled = 3,
    }
}