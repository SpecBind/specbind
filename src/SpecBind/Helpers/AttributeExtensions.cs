// <copyright file="AttributeExtensions.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Helpers
{
    using System;
    using System.Linq;

    /// <summary>
    /// An extension class to help get attributes from a type.
    /// </summary>
    public static class AttributeExtensions
    {
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <param name="inherit">if set to <c>true</c> inherit from base classes.</param>
        /// <returns>
        /// The attribute if located; otherwise <c>null</c>.
        /// </returns>
        public static TAttribute GetAttribute<TAttribute>(this Type type, bool inherit = true) where TAttribute : Attribute
        {
            return type.GetCustomAttributes(typeof(TAttribute), inherit).OfType<TAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <param name="attribute">The returned attribute.</param>
        /// <param name="inherit">if set to <c>true</c> inherit from base classes.</param>
        /// <returns>
        ///   <c>true</c> If the attribute if located; otherwise <c>false</c>.
        /// </returns>
        public static bool TryGetAttribute<TAttribute>(this Type type, out TAttribute attribute, bool inherit = true) where TAttribute : Attribute
        {
            attribute = type.GetCustomAttributes(typeof(TAttribute), inherit).OfType<TAttribute>().FirstOrDefault();
            return !Equals(attribute, default(TAttribute));
        }
    }
}