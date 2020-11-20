// <copyright file="IPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An interface that defines a page in the system.
    /// </summary>
    public interface IPage
    {
        /// <summary>
        /// Gets the type of the page.
        /// </summary>
        /// <value>
        /// The type of the page.
        /// </value>
        Type PageType { get; }

        /// <summary>
        /// Gets the native page object.
        /// </summary>
        /// <typeparam name="TPage">The type of the page.</typeparam>
        /// <returns>
        /// The native page object.
        /// </returns>
        TPage GetNativePage<TPage>() where TPage : class;

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>A list of matching properties.</returns>
        IEnumerable<string> GetPropertyNames(Func<IPropertyData, bool> filter);

        /// <summary>
        /// Highlights this instance.
        /// </summary>
        void Highlight();

        /// <summary>
        /// Tries the get the element.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="propertyData">
        /// The property data.
        /// </param>
        /// <returns>
        /// <c>true</c> if the element exists; otherwise <c>false</c>.
        /// </returns>
        bool TryGetElement(string key, out IPropertyData propertyData);

        /// <summary>
        /// Tries the get the element.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="propertyData">
        /// The property data.
        /// </param>
        /// <returns>
        /// <c>true</c> if the element exists; otherwise <c>false</c>.
        /// </returns>
        bool TryGetProperty(string key, out IPropertyData propertyData);

        /// <summary>
        /// Waits for the page to become active based on some user content.
        /// </summary>
        void WaitForPageToBeActive();
    }
}