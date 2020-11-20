﻿// <copyright file="TestBase.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Tests.Support
{
    using System;
    using System.Collections.Generic;
    using SpecBind.Actions;
    using SpecBind.Pages;

    /// <summary>
    /// A test base class.
    /// </summary>
    public class TestBase : PageBase<BasePageClass, BaseElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestBase" /> class.
        /// </summary>
        public TestBase()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestBase" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public TestBase(InheritedClass item)
            : base(typeof(InheritedClass), item)
        {
        }

        /// <summary>
        /// Checks if the element is enabled.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Success of the call.</returns>
        public override bool ElementEnabledCheck(BaseElement element)
        {
            return true;
        }

        /// <summary>
        /// Checks if the element exists.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Success of the call.</returns>
        public override bool ElementExistsCheck(BaseElement element)
        {
            return true;
        }

        /// <summary>
        /// Checks if the element does not exist.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Success of the call.</returns>
        public override bool ElementNotExistsCheck(BaseElement element)
        {
            return true;
        }

        /// <summary>
        /// Gets the element attribute value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>The attribute value.</returns>
	    public override string GetElementAttributeValue(BaseElement element, string attributeName)
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the element text.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Success of the call.</returns>
        public override string GetElementText(BaseElement element)
        {
            return null;
        }

        /// <summary>
        /// Gets the page from element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The page interface.</returns>
        public override IPage GetPageFromElement(BaseElement element)
        {
            return null;
        }

        /// <summary>
        /// Clicks the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Success of the call.</returns>
        public override bool ClickElement(BaseElement element)
        {
            return true;
        }

        /// <summary>
        /// Gets the clear field method.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>
        /// The function used to clear the data.
        /// </returns>
		public override Action<BaseElement> GetClearMethod(Type propertyType)
        {
            return null;
        }

        /// <summary>
        /// Gets the page fill method.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>Success of the call.</returns>
        public override Action<BaseElement, string> GetPageFillMethod(Type propertyType)
        {
            return null;
        }

        /// <summary>
        /// Waits for element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="waitCondition">The wait condition.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns><c>true</c> if the condition is met, <c>false</c> otherwise.</returns>
	    public override bool WaitForElement(BaseElement element, WaitConditions waitCondition, TimeSpan? timeout)
        {
            return false;
        }

        public override IList<ComboBoxItem> GetElementOptions(BaseElement element)
        {
            return null;
        }
    }
}