// <copyright file="AliasPropertyAttribute.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
    using System;

    /// <summary>
    /// An attribute for specifying an alias for an element.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class AliasPropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the alias name.
        /// </summary>
        /// <value>
        /// The alias name.
        /// </value>
        public string Name { get; set; }
    }
}