// <copyright file="NameValueConfigurationCollectionExtensions.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Extensions
{
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// Name Value Configuration Collection Extensions.
    /// </summary>
    public static class NameValueConfigurationCollectionExtensions
    {
        /// <summary>
        /// Converts the NameValueConfigurationCollection to a collection of key value pairs.
        /// </summary>
        /// <param name="collection">The name value configuration collection.</param>
        /// <returns>A collection of key value pairs.</returns>
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(
            this NameValueConfigurationCollection collection)
        {
            foreach (string key in collection.AllKeys)
            {
                NameValueConfigurationElement element = collection[key];

                yield return new KeyValuePair<string, string>(element.Name, element.Value);
            }
        }
    }
}
