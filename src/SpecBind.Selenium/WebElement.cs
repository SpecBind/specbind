// <copyright file="WebElement.cs">
//   Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
	using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Interactions.Internal;
    using OpenQA.Selenium.Internal;

    /// <summary>
    /// Represents a proxy class for an element to be used with the PageFactory.
    /// </summary>
    public class WebElement : IWebElement, ILocatable, IWrapsElement
    {
        private readonly List<By> bys;
        private readonly ISearchContext searchContext;

        private IWebElement cachedElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebElement" /> class.
        /// </summary>
        /// <param name="searchContext">The driver used to search for elements.</param>
        protected internal WebElement(ISearchContext searchContext)
        {
            this.searchContext = searchContext;

            this.bys = new List<By>();
            this.Cache = true;
        }

        /// <summary>
        /// Gets a value indicating whether to cache the element lookup.
        /// </summary>
        /// <value><c>true</c> if cache; otherwise, <c>false</c>.</value>
        public bool Cache { get; internal set; }

        /// <summary>
        /// Gets the coordinates identifying the location of this element using
        /// various frames of reference.
        /// </summary>
        /// <value>The coordinates.</value>
        public ICoordinates Coordinates
        {
            get
            {
                return ((ILocatable)this.WrappedElement).Coordinates;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this element is displayed.
        /// </summary>
        /// <value><c>true</c> if displayed; otherwise, <c>false</c>.</value>
        /// <remarks>The <see cref="P:OpenQA.Selenium.IWebElement.Displayed" /> property avoids the problem
        /// of having to parse an element's "style" attribute to determine
        /// visibility of an element.</remarks>
        public bool Displayed
        {
            get
            {
                return this.WrappedElement.Displayed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this element is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        /// <remarks>The <see cref="P:OpenQA.Selenium.IWebElement.Enabled" /> property will generally
        /// return <see langword="true" /> for everything except explicitly disabled input elements.</remarks>
        public bool Enabled
        {
            get
            {
                return this.WrappedElement.Enabled;
            }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Drawing.Point" /> object containing the coordinates of the upper-left corner
        /// of this element relative to the upper-left corner of the page.
        /// </summary>
        /// <value>The location.</value>
        public Point Location
        {
            get
            {
                return this.WrappedElement.Location;
            }
        }

        /// <summary>
        /// Gets the location of an element on the screen, scrolling it into view
        /// if it is not currently on the screen.
        /// </summary>
        /// <value>The location on screen once scrolled into view.</value>
        public Point LocationOnScreenOnceScrolledIntoView
        {
            get
            {
                return ((ILocatable)this.WrappedElement).LocationOnScreenOnceScrolledIntoView;
            }
        }

        /// <summary>
        /// Gets the locators.
        /// </summary>
        /// <value>The locators.</value>
        public ReadOnlyCollection<By> Locators
        {
            get
            {
                return this.bys.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this element is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        /// <remarks>This operation only applies to input elements such as checkboxes,
        /// options in a select element and radio buttons.</remarks>
        public bool Selected
        {
            get
            {
                return this.WrappedElement.Selected;
            }
        }

        /// <summary>
        /// Gets a <see cref="P:OpenQA.Selenium.Support.PageObjects.WebElementProxy.Size" /> object containing the height and width of this element.
        /// </summary>
        /// <value>The size.</value>
        public Size Size
        {
            get
            {
                return this.WrappedElement.Size;
            }
        }

        /// <summary>
        /// Gets the tag name of this element.
        /// </summary>
        /// <value>The name of the tag.</value>
        /// <remarks>The <see cref="P:OpenQA.Selenium.IWebElement.TagName" /> property returns the tag name of the
        /// element, not the value of the name attribute. For example, it will return
        /// "input" for an element specified by the HTML markup &lt;input name="foo" /&gt;.</remarks>
        public string TagName
        {
            get
            {
                return this.WrappedElement.TagName;
            }
        }

        /// <summary>
        /// Gets the innerText of this element, without any leading or trailing whitespace,
        /// and with other whitespace collapsed.
        /// </summary>
        /// <value>The text.</value>
        public virtual string Text
        {
            get
            {
                return this.WrappedElement.Text;
            }
        }

        /// <summary>
        /// Gets the interface through which the user can discover if there is an underlying element to be used.
        /// </summary>
        /// <value>The wrapped element.</value>
        /// <exception cref="OpenQA.Selenium.NoSuchElementException">Thrown if the element cannot be found.</exception>
        public IWebElement WrappedElement
        {
            get
            {
                if (this.Cache && this.cachedElement != null)
                {
                    return this.cachedElement;
                }

                string message = null;
                foreach (var by in this.bys)
                {
                    try
                    {
                        this.cachedElement = this.searchContext.FindElement(by);
                        return this.cachedElement;
                    }
                    catch (NoSuchElementException)
                    {
                        message = message == null
                                      ? string.Format("Could not find element by: {0}", @by)
                                      : string.Format("{0}, or: {1}", message, @by);
                    }
                }

                throw new NoSuchElementException(message);
            }
        }

        /// <summary>
        /// Clears the content of this element.
        /// </summary>
        /// <remarks>If this element is a text entry element, the <see cref="M:OpenQA.Selenium.IWebElement.Clear" />
        /// method will clear the value. It has no effect on other elements. Text entry elements
        /// are defined as elements with INPUT or TEXTAREA tags.</remarks>
        public void Clear()
        {
            this.WrappedElement.Clear();
        }

        /// <summary>
        /// Clicks this element.
        /// </summary>
        public void Click()
        {
            this.WrappedElement.Click();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified object is equal to the current object; otherwise, <see langword="false" />.</returns>
        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            return this.WrappedElement.Equals(obj);
        }

        /// <summary>
        /// Finds the first <see cref="T:OpenQA.Selenium.IWebElement" /> using the given method.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>The first matching <see cref="T:OpenQA.Selenium.IWebElement" /> on the current context.</returns>
        public IWebElement FindElement(By by)
        {
            return this.WrappedElement.FindElement(by);
        }

        /// <summary>
        /// Finds all <see cref="T:OpenQA.Selenium.IWebElement">IWebElements</see> within the current context
        /// using the given mechanism.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of all <see cref="T:OpenQA.Selenium.IWebElement">WebElements</see>
        /// matching the current criteria, or an empty list if nothing matches.</returns>
        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return this.WrappedElement.FindElements(by);
        }

        /// <summary>
        /// Gets the value of the specified attribute for this element.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>The attribute's current value. Returns a <see langword="null" /> if the
        /// value is not set.</returns>
        /// <remarks>The <see cref="M:OpenQA.Selenium.IWebElement.GetAttribute(System.String)" /> method will return the current value
        /// of the attribute, even if the value has been modified after the page has been
        /// loaded. Note that the value of the following attributes will be returned even if
        /// there is no explicit attribute on the element:
        /// <list type="table"><listheader><term>Attribute name</term><term>Value returned if not explicitly specified</term><term>Valid element types</term></listheader><item><description>checked</description><description>checked</description><description>Check Box</description></item><item><description>selected</description><description>selected</description><description>Options in Select elements</description></item><item><description>disabled</description><description>disabled</description><description>Input and other UI elements</description></item></list></remarks>
        public string GetAttribute(string attributeName)
        {
            return this.WrappedElement.GetAttribute(attributeName);
        }

        /// <summary>
        /// Gets the value of a CSS property of this element.
        /// </summary>
        /// <param name="propertyName">The name of the CSS property to get the value of.</param>
        /// <returns>The value of the specified CSS property.</returns>
        /// <remarks>The value returned by the <see cref="M:OpenQA.Selenium.IWebElement.GetCssValue(System.String)" />
        /// method is likely to be unpredictable in a cross-browser environment.
        /// Color values should be returned as hex strings. For example, a
        /// "background-color" property set as "green" in the HTML source, will
        /// return "#008000" for its value.</remarks>
        public string GetCssValue(string propertyName)
        {
            return this.WrappedElement.GetCssValue(propertyName);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return this.WrappedElement.GetHashCode();
        }

        /// <summary>
        /// Simulates typing text into the element.
        /// </summary>
        /// <param name="text">The text to type into the element.</param>
        /// <seealso cref="T:OpenQA.Selenium.Keys" />
        /// <remarks>The text to be typed may include special characters like arrow keys,
        /// backspaces, function keys, and so on. Valid special keys are defined in
        /// <see cref="T:OpenQA.Selenium.Keys" />.</remarks>
        public virtual void SendKeys(string text)
        {
			// HACK: The Selenium IE driver (or something even deeper) will intermittently shift digits.
			// E.g. "3" will end up as "#", "4" as "$".
			// Also, in rare cases, when read back, the full text sent is not in the element.
			// These problems have been known for at least 3 years, but have never been fixed.
			// To deal with these issues, read the value back and retry a few times if it doesn't match.

			string actual = null;
			for (int tries = 0; tries < 3; tries++)
			{
				this.WrappedElement.Clear();
            this.WrappedElement.SendKeys(text);

				actual = this.WrappedElement.GetAttribute("value");
                if (string.Equals(text, actual, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

				System.Threading.Thread.Sleep(500);
			}

			throw new WebDriverException(string.Format("SendKeys('{0}') put text '{1}' into element", text, actual));
        }

        /// <summary>
        /// Submits this element to the web server.
        /// </summary>
        /// <remarks>If this current element is a form, or an element within a form,
        /// then this will be submitted to the web server. If this causes the current
        /// page to change, then this method will block until the new page is loaded.</remarks>
        public void Submit()
        {
            this.WrappedElement.Submit();
        }

        /// <summary>
        /// Clones the native element, setting <paramref name="nativeElement"/> as the core element.
        /// </summary>
        /// <param name="nativeElement">The native element.</param>
        internal void CloneNativeElement(IWebElement nativeElement)
        {
            this.Cache = true;
            this.cachedElement = nativeElement;
        }

        /// <summary>
        /// Updates the locators for the element.
        /// </summary>
        /// <param name="locators">The locators.</param>
        internal void UpdateLocators(IEnumerable<By> locators)
        {
            // Clear any existing locators
            this.cachedElement = null;
            this.bys.Clear();

            if (locators != null)
            {
                this.bys.AddRange(locators);
            }
        }
    }
}