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
        /// Checks that the control exists and is visible,
        /// waiting until the condition is met or times out.
        /// </summary>
        BecomesExistent = 0,

        /// <summary>
        /// Checks that the control does not exist,
        /// waiting until the condition is met or times out.
        /// </summary>
        BecomesNonExistent = 1,

        /// <summary>
        /// Checks that the control is enabled,
        /// waiting until the condition is met or times out.
        /// </summary>
        BecomesEnabled = 2,

        /// <summary>
        /// Checks that the control is disabled,
        /// waiting until the condition is met or times out.
        /// </summary>
        BecomesDisabled = 3,

        /// <summary>
        /// Checks that the control exists and is visible,
        /// waiting until the condition is not met or times out.
        /// To succeed, the timeout must be reached.
        /// </summary>
        RemainsExistent = 4,

        /// <summary>
        /// Checks that the control does not exist,
        /// waiting until the condition is not met or times out.
        /// To succeed, the timeout must be reached.
        /// </summary>
        RemainsNonExistent = 5,

        /// <summary>
        /// Checks that the control is enabled,
        /// waiting until the condition is not met or times out.
        /// To succeed, the timeout must be reached.
        /// </summary>
        RemainsEnabled = 6,

        /// <summary>
        /// Checks that the control is disabled,
        /// waiting until the condition is not met or times out.
        /// To succeed, the timeout must be reached.
        /// </summary>
        RemainsDisabled = 7,

        /// <summary>
        /// Checks that the control exists and is no longer moving,
        /// waiting until the condition is met or times out.
        /// </summary>
        NotMoving = 8,

        /// <summary>
        /// Checks that the control exists and is visible.
        /// Equivalent to BecomesExistent.
        /// </summary>
        Exists = BecomesExistent,

        /// <summary>
        /// Checks that the control does not exist.
        /// Equivalent to BecomesNonExistent.
        /// </summary>
        NotExists = BecomesNonExistent,

        /// <summary>
        /// Checks that the control is enabled.
        /// Equivalent to BecomesEnabled.
        /// </summary>
        Enabled = BecomesEnabled,

        /// <summary>
        /// Checks that the control is not enabled.
        /// Equivalent to BecomesDisabled.
        /// </summary>
        NotEnabled = BecomesDisabled,
    }
}