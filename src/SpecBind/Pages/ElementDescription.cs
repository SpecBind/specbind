// <copyright file="ElementDescription.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
    using System;

    /// <summary>
    /// Describes information about a given element.
    /// </summary>
    public class ElementDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementDescription" /> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyType">The type of the property.</param>
        /// <param name="value">The value.</param>
        public ElementDescription(string propertyName, Type propertyType, object value)
        {
            this.PropertyName = propertyName;
            this.Value = value;
            this.PropertyType = propertyType;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; private set; }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        public Type PropertyType { get; private set; }
    }
}