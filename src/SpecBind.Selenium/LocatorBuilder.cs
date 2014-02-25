// <copyright file="LocatorBuilder.cs">
//    Copyright © 2014 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using OpenQA.Selenium;

    using SpecBind.Pages;

    /// <summary>
    /// A class that translates attributes into locators.
    /// </summary>
    public static class LocatorBuilder
    {
        /// <summary>
        /// Gets the element locators.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>The list of locators to use.</returns>
        public static List<By> GetElementLocators(ElementLocatorAttribute attribute)
        {
            var locators = new List<By>(3);
            SetProperty(locators, attribute, a => By.Id(a.Id), a => a.Id != null);
            SetProperty(locators, attribute, a => By.Name(a.Name), a => a.Name != null);
            SetProperty(locators, attribute, a => By.ClassName(a.Class), a => a.Class != null);
            SetProperty(locators, attribute, a => By.LinkText(a.Text), a => a.Text != null);

            var xpathTag = new XPathTag(attribute.NormalizedTagName);

            // Alt attribute.
            SetAttribute(xpathTag, "alt", attribute.Alt);

            // URL for Image and Hyperlink
            SetAttribute(xpathTag, "src", attribute.Url, () => attribute.NormalizedTagName == "img");
            SetAttribute(xpathTag, "href", attribute.Url, () => attribute.NormalizedTagName == "a" || attribute.NormalizedTagName == "area");
            
            // Value attribute
            SetAttribute(xpathTag, "value", attribute.Value);
            
            // Title attribute
            SetAttribute(xpathTag, "title", attribute.Title);

            // Type attribute
            SetAttribute(xpathTag, "type", attribute.Type);

            // Index attribute
            if (attribute.Index > 0)
            {
                xpathTag.Index = attribute.Index - 1;
            }

            // Only set tag if xpath is empty
            SetProperty(locators, attribute, a => By.TagName(a.TagName), a => a.TagName != null && !xpathTag.HasData);

            // Add the XPath tag if data exists
            if (xpathTag.HasData)
            {
                if (string.IsNullOrWhiteSpace(xpathTag.TagName))
                {
                    throw new ElementExecuteException("Element Locator contains a locator but is missing a TagName property: {0}", xpathTag.CreateLocator());
                }

                locators.Add(By.XPath(xpathTag.CreateLocator()));
            }

            return locators;
        }

        /// <summary>
        /// Sets the property of the locator by the filter.
        /// </summary>
        /// <typeparam name="T">The type of the element being set.</typeparam>
        /// <param name="locators">The locator collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="setterFunc">The setter function.</param>
        /// <param name="filterFunc">The filter function.</param>
        private static void SetProperty<T>(ICollection<By> locators, T item, Func<T, By> setterFunc, Func<T, bool> filterFunc = null)
        {
            if (filterFunc == null || filterFunc(item))
            {
                locators.Add(setterFunc(item));
            }
        }

        /// <summary>
        /// Sets the property of the locator by the filter.
        /// </summary>
        /// <param name="tag">The locator collection.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The value.</param>
        /// <param name="filterFunc">The filter function.</param>
        private static void SetAttribute(XPathTag tag, string attributeName, string value, Func<bool> filterFunc = null)
        {
            if (value != null && (filterFunc == null || filterFunc()))
            {
                tag.AddAttribute(attributeName, value);
            }
        }

        /// <summary>
        /// A class that assists in construction of the XPath tag.
        /// </summary>
        private class XPathTag
        {
            private readonly List<Tuple<string, string>> attributes;

            /// <summary>
            /// Initializes a new instance of the <see cref="XPathTag"/> class.
            /// </summary>
            /// <param name="tagName">Name of the tag.</param>
            public XPathTag(string tagName)
            {
                this.TagName = tagName;
                this.attributes = new List<Tuple<string, string>>(1);
            }

            /// <summary>
            /// Gets or sets the index.
            /// </summary>
            /// <value>The index.</value>
            public int? Index { private get; set; }

            /// <summary>
            /// Gets a value indicating whether this instance has data.
            /// </summary>
            /// <value><c>true</c> if this instance has data; otherwise, <c>false</c>.</value>
            public bool HasData
            {
                get
                {
                    return this.attributes.Count > 0 || this.Index.HasValue;
                }
            }

            /// <summary>
            /// Gets the name of the tag.
            /// </summary>
            /// <value>The name of the tag.</value>
            public string TagName { get; private set; }

            /// <summary>
            /// Adds the attribute.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="value">The value.</param>
            public void AddAttribute(string name, string value)
            {
                this.attributes.Add(new Tuple<string, string>(name.ToLowerInvariant(), value));
            }

            /// <summary>
            /// Creates the locator.
            /// </summary>
            /// <returns>The locator XPath value.</returns>
            public string CreateLocator()
            {
                var hasAttributes = this.attributes.Any();
                var hasIndex = this.Index.HasValue;

                var builder = new StringBuilder();

                // add wrapper for index if attributes exist
                if (hasIndex && hasAttributes)
                {
                    builder.Append("(");
                }

                                    
                // append the base tag
                builder.AppendFormat("//{0}[", this.TagName ?? "UNKNOWN");

                // Add any attributes
                var atributeAdded = false;
                foreach (var attribute in this.attributes)
                {
                    if (atributeAdded)
                    {
                        builder.Append(" and ");
                    }

                    builder.AppendFormat("@{0}='{1}'", attribute.Item1, attribute.Item2);
                    atributeAdded = true;
                }

                // Add the tag index here if there are no other attributes
                if (!hasAttributes && hasIndex)
                {
                    builder.Append(this.Index.Value);
                }

                builder.Append("]");

                // add wrapper for index if attributes exist
                if (hasIndex && hasAttributes)
                {
                    builder.AppendFormat(")[{0}]", this.Index.Value);
                }

                return builder.ToString();
            }
        }
    }
}