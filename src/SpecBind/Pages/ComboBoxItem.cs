// <copyright file="ComboBoxItem.cs">
//    Copyright © 2015 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
    /// <summary>
    /// Represents an item from a combo box
    /// </summary>
    public class ComboBoxItem
    {
        /// <summary>
        /// Gets or sets the text (display) value of the combo box item.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the value of the combo box item.
        /// </summary>
        public string Value { get; set; }
    }
}
