// <copyright file="IElementProvider.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface that instructs the page builder to ask the implementation for property values.
    /// </summary>
    public interface IElementProvider
    {
        /// <summary>
        /// Gets the element properties on the control.
        /// </summary>
        /// <returns>A collection of properties.</returns>
        IEnumerable<ElementDescription> GetElements();
    }
}