// <copyright file="ReflectionHelper.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
    using System.Reflection;

    /// <summary>
    /// Reflection Helper.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the property.</returns>
        public static object GetPropertyValue(object obj, string propertyName)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName, bindingFlags);
            if (propertyInfo == null)
            {
                Logger.Log($"Property '{propertyName}' not found in type '{obj.GetType()}'.");
                return null;
            }

            Logger.Log($"Getting value of property '{propertyName}' in type '{obj.GetType()}'.");

            try
            {
                return propertyInfo.GetValue(obj);
            }
            catch (TargetInvocationException ex)
            {
                // A TargetInvocationException is throw because we're using reflection.
                // Throw the inner exception which may be selenium specific so that
                // WebDriverWait can handle it for example.
                Logger.Log(ex.ToString());

                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }
    }
}
