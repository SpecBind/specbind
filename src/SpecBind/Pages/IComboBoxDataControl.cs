// <copyright file="IComboBoxDataControl.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface that allows a control to manually define how combo items are retrieved.
    /// </summary>
    public interface IComboBoxDataControl
    {
        /// <summary>
        /// Gets the element options for multi-select or list options.
        /// </summary>
        /// <returns>The element's options if supported, otherwise <c>null</c>.</returns>
        IList<ComboBoxItem> GetElementOptions();
    }
}