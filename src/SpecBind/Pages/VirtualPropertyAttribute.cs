// <copyright file="VirtualPropertyAttribute.cs">
//    Copyright © 2014 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
    using System;

    /// <summary>
    /// An attribute for accessing a sub property of an element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class VirtualPropertyAttribute : Attribute
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the attribute of the element to access as the property.
        /// </summary>
        /// <value>The attribute.</value>
        public string Attribute { get; set; }

        /// <summary>
        /// Gets or sets the element name to find.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        #endregion
    }
}