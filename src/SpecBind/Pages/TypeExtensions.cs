// <copyright file="TypeExtensions.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
    using System;
    using System.Linq;

    /// <summary>
	/// A set of extension methods to analyze types.
	/// </summary>
	public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the specified check type is a supported property type.
        /// </summary>
        /// <param name="checkType">The type to check.</param>
        /// <param name="elementType">Type of the elements used in the driver.</param>
        /// <returns><c>true</c> if the type is supported; otherwise, <c>false</c>.</returns>
        public static bool IsSupportedPropertyType(this Type checkType, Type elementType)
        {
            return elementType.IsAssignableFrom(checkType) || IsElementListType(checkType) || IsTableElementType(checkType);
        }

        /// <summary>
        /// Checks to see if the given type is an element list.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns><c>true</c> if it is a list type; otherwise <c>false</c>.</returns>
        public static bool IsElementListType(this Type propertyType)
        {
            return (propertyType.IsGenericType && typeof(IElementList<,>).IsAssignableFrom(propertyType.GetGenericTypeDefinition())) ||
                   propertyType.GetInterfaces().Any(i => i.IsGenericType && typeof(IElementList<,>).IsAssignableFrom(i.GetGenericTypeDefinition()));
        }

        /// <summary>
        /// Checks to see if the given type is an table element.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns><c>true</c> if it is a table element; otherwise <c>false</c>.</returns>
        public static bool IsTableElementType(this Type propertyType)
        {
            return propertyType.IsGenericType && typeof(TableElement<>).IsAssignableFrom(propertyType.GetGenericTypeDefinition());
        }
    }
}