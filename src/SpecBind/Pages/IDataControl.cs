// <copyright file="IDataControl.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
    /// <summary>
    /// An interface that allows a control to manually define how data is set in it.
    /// </summary>
    public interface IDataControl
    {
        /// <summary>
        /// Sets the value in the control.
        /// </summary>
        /// <param name="value">The value to set.</param>
        void SetValue(string value);
    }
}