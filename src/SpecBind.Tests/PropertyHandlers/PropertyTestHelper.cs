// <copyright file="PropertyTestHelper.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Tests.PropertyHandlers
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// A test class base that helps check a property for a not supported exception.
    /// </summary>
    internal static class PropertyTestHelper
    {
        /// <summary>
        /// Tests the specified action for a not supported exception.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="action">The action.</param>
        /// <param name="catchClause">The catch clause to check for in the message.</param>
        internal static void TestForNotSupportedException<TProperty>(this TProperty property, Action<TProperty> action, string catchClause = null)
        {
            try
            {
                action(property);
            }
            catch (NotSupportedException e)
            {
                if (!string.IsNullOrEmpty(catchClause))
                {
                    StringAssert.Contains(e.Message, catchClause);
                }
            }
        }
    }
}