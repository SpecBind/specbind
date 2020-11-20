// <copyright file="ILocatorAction.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.ActionPipeline
{
    using SpecBind.Pages;

    /// <summary>
    /// An interface that allows a plugin to participate in the
    /// location of an element.
    /// </summary>
    public interface ILocatorAction
    {
        /// <summary>
        /// Called when an element is about to be located.
        /// </summary>
        /// <param name="key">The element key.</param>
        void OnLocate(string key);

        /// <summary>
        /// Called when an element is completed.
        /// </summary>
        /// <param name="key">The element key.</param>
        /// <param name="item">The item if located; otherwise <c>null</c>.</param>
        void OnLocateComplete(string key, IPropertyData item);
    }
}