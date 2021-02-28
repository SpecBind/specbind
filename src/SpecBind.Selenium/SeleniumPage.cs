// <copyright file="SeleniumPage.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Support.Extensions;
    using OpenQA.Selenium.Support.UI;

    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using SpecBind.Selenium.Drivers;

    /// <summary>
    /// An implementation of <see cref="IPage"/> for the Selenium driver.
    /// </summary>
    public class SeleniumPage : PageBase<object, IWebElement>
    {
        private readonly IWebDriver webDriver;
        private readonly bool focusWindowBeforeClicking;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumPage" /> class.
        /// </summary>
        /// <param name="nativePage">The native page.</param>
        /// <param name="webDriver">The web driver.</param>
        public SeleniumPage(object nativePage, IWebDriver webDriver)
            : base(nativePage.GetType(), nativePage)
        {
            this.webDriver = webDriver;

            var configSection = SettingHelper.GetConfigurationSection();
            if (configSection != null)
            {
                this.focusWindowBeforeClicking = configSection.Application.FocusWindowBeforeClicking;
            }
        }

        /// <summary>
        /// Gets or sets a delegate to set the ElementLocateTimeout.
        /// </summary>
        /// <value>
        /// A delegate to set the ElementLocateTimeout.
        /// </value>
        public Action<TimeSpan, Action> ExecuteWithElementLocateTimeout { get; set; }

        /// <summary>
        /// Gets or sets a delegate to set the ElementLocateTimeout.
        /// </summary>
        /// <value>
        /// A delegate to set the ElementLocateTimeout.
        /// </value>
        public Func<TimeSpan, Func<bool>, bool> EvaluateWithElementLocateTimeout { get; set; }

        /// <summary>
        /// Checks to see if the element is enabled.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element is enabled, <c>false</c> otherwise.</returns>
        public override bool ElementEnabledCheck(IWebElement element)
        {
            return CheckElementState(e => e.Displayed && e.Enabled, element);
        }

        /// <summary>
        /// Checks to see if the element exists.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element exists, <c>false</c> otherwise.</returns>
        public override bool ElementExistsCheck(IWebElement element)
        {
            return CheckElementState(e => e.Displayed, element);
        }

        /// <summary>
        /// Checks to see if the element doesn't exists.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element doesn't exists, <c>false</c> otherwise.</returns>
        public override bool ElementNotExistsCheck(IWebElement element)
        {
            if (element == null)
            {
                return true;
            }

            return this.EvaluateWithElementLocateTimeout(
                default,
                () => CheckElementState(e => !e.Displayed, element, stateIfNotFound: true));
        }

        /// <summary>
        /// Gets the element attribute value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>The attribute value.</returns>
        public override string GetElementAttributeValue(IWebElement element, string attributeName)
        {
            return element.GetAttribute(attributeName);
        }

        /// <inheritdoc/>
        public override void SetElementAttributeValue(IWebElement element, string attributeName, string value)
        {
            this.webDriver.ExecuteJavaScript("arguments[0].setAttribute(arguments[1], arguments[2])", element, attributeName, value);
        }

        /// <summary>
        /// Gets the clears method.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>
        ///  The function used to clear the data.
        /// </returns>
        public override Action<IWebElement> GetClearMethod(Type propertyType)
        {
            return ClearPage;
        }

        /// <summary>
        /// Gets the page fill method.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>The action to fill the page.</returns>
        public override Action<IWebElement, string> GetPageFillMethod(Type propertyType)
        {
            return this.FillPage;
        }

        /// <summary>
        /// Gets the element options for multi-select or list options.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The element's options if supported, otherwise <c>null</c>.</returns>
        public override IList<ComboBoxItem> GetElementOptions(IWebElement element)
        {
            if (element is IComboBoxDataControl)
            {
                IComboBoxDataControl dataControlElement = element as IComboBoxDataControl;
                return dataControlElement.GetElementOptions();
            }

            var tagName = element.TagName.ToLowerInvariant().Trim();
            switch (tagName)
            {
                case "select":
                    var selectElement = new SelectElement(element);
                    return selectElement.Options
                        .Select(option => new ComboBoxItem { Value = option.GetAttribute("value"), Text = option.GetAttribute("text") })
                        .ToList();
            }

            return null;
        }

        /// <summary>
        /// Gets the element text.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The text of the element.</returns>
        public override string GetElementText(IWebElement element)
        {
            if (element is IDataControl)
            {
                IDataControl dataControlElement = element as IDataControl;
                return dataControlElement.GetText();
            }

            var tagName = element.TagName.ToLowerInvariant().Trim();
            switch (tagName)
            {
                case "select":
                    var selectElement = new SelectElement(element);
                    return selectElement.SelectedOption.Text;
                case "input":
                case "textarea":
                    // Special case for a checkbox control
                    if (string.Equals("checkbox", element.GetAttribute("type"), StringComparison.OrdinalIgnoreCase)
                        || string.Equals("radio", element.GetAttribute("type"), StringComparison.OrdinalIgnoreCase))
                    {
                        return element.Selected.ToString();
                    }

                    return element.GetAttribute("value");
                default:
                    return element.Text;
            }
        }

        /// <inheritdoc />
        public override void Highlight(IWebElement element)
        {
            var orig = element.GetAttribute("style");
            this.SetAttribute(element, "style", "border: 5px solid red;");
            Thread.Sleep(TimeSpan.FromSeconds(1.5));
            this.SetAttribute(element, "style", orig);
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        /// <param name="element">The element.</param>
        public override void ClearCache(IWebElement element)
        {
            (element as WebElement).ClearCache();
        }

        /// <summary>
        /// Gets the page from element.
        /// </summary>
        /// <param name="element">The parent element.</param>
        /// <returns>The child page as a scope.</returns>
        public override IPage GetPageFromElement(IWebElement element)
        {
            return this.CreatePageFromElement(element);
        }

        /// <summary>
        /// Clicks the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element is clicked, <c>false</c> otherwise.</returns>
        public override bool ClickElement(IWebElement element)
        {
            return this.ClickElement(element, times: 1);
        }

        /// <summary>
        /// Moves the mouse over the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the mouse moved over the element, <c>false</c> otherwise.</returns>
        public override bool MouseOverElement(IWebElement element)
        {
            if (!this.WaitForElement(element, WaitConditions.NotMoving, timeout: null))
            {
                return false;
            }

            if (!this.WaitForElement(element, WaitConditions.BecomesEnabled, timeout: null))
            {
                return false;
            }

            Actions action = new Actions(this.webDriver);
            action.MoveToElement(element).Perform();

            return true;
        }

        /// <summary>
        /// Double-clicks the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element is double-clicked, <c>false</c> otherwise.</returns>
        public override bool DoubleClickElement(IWebElement element)
        {
            return this.ClickElement(element, times: 2);
        }

        /// <summary>
        /// Right-clicks the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element is right-clicked, <c>false</c> otherwise.</returns>
        public override bool RightClickElement(IWebElement element)
        {
            if (!this.WaitForElement(element, WaitConditions.NotMoving, timeout: null))
            {
                return false;
            }

            if (!this.WaitForElement(element, WaitConditions.BecomesEnabled, timeout: null))
            {
                return false;
            }

            if (this.focusWindowBeforeClicking)
            {
                string currentWindowHandle = this.webDriver.CurrentWindowHandle;
                int currentWindowHandleParsed;
                if (int.TryParse(currentWindowHandle, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out currentWindowHandleParsed))
                {
                    int currentWindowHandleInt = Convert.ToInt32(currentWindowHandle, 16);
                    if (!NativeMethods.SetForegroundWindow(new IntPtr(currentWindowHandleInt)))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(), $"Failed to set window with handle {currentWindowHandle} as the foreground window.");
                    }
                }
            }

            WindowsDriverEx driver = this.webDriver as WindowsDriverEx;

#pragma warning disable CS0618 // Type or member is obsolete
            driver.Mouse.ContextClick((element as WebElement).Coordinates);
#pragma warning restore CS0618 // Type or member is obsolete

            return true;
        }

        /// <summary>
        /// Waits for the element to meet a certain condition.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="waitCondition">The wait condition.</param>
        /// <param name="timeout">The timeout to wait before failing.</param>
        /// <returns><c>true</c> if the condition is met, <c>false</c> otherwise.</returns>
        public override bool WaitForElement(IWebElement element, WaitConditions waitCondition, TimeSpan? timeout)
        {
            var waiter = new DefaultWait<IWebElement>(element);
            waiter.Timeout = timeout.GetValueOrDefault(waiter.Timeout);

            switch (waitCondition)
            {
                case WaitConditions.BecomesNonExistent: // AKA NotExists
                    this.ExecuteWithElementLocateTimeout(
                        default,
                        () =>
                        {
                            try
                            {
                                waiter.Until(e => !e.Displayed);
                            }
                            catch (NoSuchElementException)
                            {
                            }
                            catch (NotFoundException)
                            {
                            }
                            catch (ElementNotVisibleException)
                            {
                            }
                            catch (StaleElementReferenceException)
                            {
                            }
                            catch (WebDriverException)
                            {
                                // An element command failed because the referenced element is no longer attached to the DOM.
                            }
                        });
                    break;
                case WaitConditions.RemainsNonExistent:
                    return this.EvaluateWithElementLocateTimeout(
                        waiter.Timeout,
                        () =>
                        {
                            try
                            {
                                return this.DoesFullTimeoutElapse(waiter, e => e.Displayed);
                            }
                            catch (NoSuchElementException)
                            {
                                return true;
                            }
                            catch (NotFoundException)
                            {
                                return true;
                            }
                            catch (ElementNotVisibleException)
                            {
                                return true;
                            }
                            catch (StaleElementReferenceException)
                            {
                                return true;
                            }
                        });
                case WaitConditions.BecomesEnabled: // AKA Enabled
                    waiter.IgnoreExceptionTypes(typeof(ElementNotVisibleException), typeof(NotFoundException));
                    waiter.Until(e => e.Enabled);
                    break;
                case WaitConditions.BecomesDisabled: // AKA NotEnabled
                    waiter.IgnoreExceptionTypes(typeof(ElementNotVisibleException), typeof(NotFoundException));
                    waiter.Until(e => !e.Enabled);
                    break;
                case WaitConditions.BecomesExistent: // AKA Exists
                    waiter.IgnoreExceptionTypes(typeof(ElementNotVisibleException), typeof(NotFoundException) /*, typeof(StaleElementReferenceException)*/);
                    waiter.Until(e => e.Displayed);
                    break;
                case WaitConditions.NotMoving:
                    waiter.IgnoreExceptionTypes(typeof(ElementNotVisibleException), typeof(NotFoundException));
                    waiter.Until(e => e.Displayed);
                    waiter.Until(e => !this.Moving(e));
                    break;
                case WaitConditions.RemainsEnabled:
                    return this.DoesFullTimeoutElapse(waiter, e => !e.Enabled);
                case WaitConditions.RemainsDisabled:
                    return this.DoesFullTimeoutElapse(waiter, e => e.Enabled);
                case WaitConditions.RemainsExistent:
                    return this.DoesFullTimeoutElapse(waiter, e => !e.Displayed);
            }

            return true;
        }

        /// <summary>
        /// Clicks the element a given number of times.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="times">The number of times to click.</param>
        /// <returns><c>true</c> if the element is clicked, <c>false</c> otherwise.</returns>
        protected virtual bool ClickElement(IWebElement element, int times)
        {
            if (times < 1)
            {
                return true;
            }

            if (!this.WaitForElement(element, WaitConditions.NotMoving, timeout: null))
            {
                return false;
            }

            if (!this.WaitForElement(element, WaitConditions.BecomesEnabled, timeout: null))
            {
                return false;
            }

            if (times == 2)
            {
                new Actions(this.webDriver).DoubleClick(element).Perform();

                return true;
            }

            // TODO: consider waiting between clicks, so that it's not interpreted as a double-click
            for (var i = 0; i < times; i++)
            {
                if (this.focusWindowBeforeClicking)
                {
                    string currentWindowHandle = this.webDriver.CurrentWindowHandle;
                    int currentWindowHandleParsed;
                    if (int.TryParse(currentWindowHandle, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out currentWindowHandleParsed))
                    {
                        int currentWindowHandleInt = Convert.ToInt32(currentWindowHandle, 16);
                        if (!NativeMethods.SetForegroundWindow(new IntPtr(currentWindowHandleInt)))
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error(), $"Failed to set window with handle {currentWindowHandle} as the foreground window.");
                        }
                    }
                }

                element.Click();
            }

            return true;
        }

        /// <summary>
        /// Determines if an element is currently moving (e.g. due to animation).
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element's Location is changing, <c>false</c> otherwise.</returns>
        protected virtual bool Moving(IWebElement element)
        {
            var firstLocation = element.Location;
            Thread.Sleep(200);
            var secondLocation = element.Location;
            var moved = !secondLocation.Equals(firstLocation);
            return moved;
        }

        /// <summary>
        /// Checks to see if the property type is supported.
        /// </summary>
        /// <param name="type">The type being checked.</param>
        /// <returns><c>true</c> if the type is supported, <c>false</c> otherwise.</returns>
        protected override bool SupportedPropertyType(Type type)
        {
            return typeof(IWebElement).IsAssignableFrom(type) || typeof(string).IsAssignableFrom(type);
        }

        /// <summary>
        /// Checks to see if the current type matches the base type of the system to not reflect base properties.
        /// </summary>
        /// <param name="propertyInfo">Type of the page.</param>
        /// <returns><c>true</c> if the type is the base class, otherwise <c>false</c>.</returns>
        protected override bool TypeIsNotBaseClass(PropertyInfo propertyInfo)
        {
            return true;
        }

        /// <summary>
        /// Checks the state of the element.
        /// </summary>
        /// <param name="checkFunc">The check function.</param>
        /// <param name="element">The element.</param>
        /// <param name="stateIfNotFound">The result to assume if the element cannot be found.  Defaults to <c>false</c>.</param>
        /// <returns>The result of the check.</returns>
        private static bool CheckElementState(Func<IWebElement, bool> checkFunc, IWebElement element, bool stateIfNotFound = false)
        {
            try
            {
                return checkFunc(element);
            }
            catch (NoSuchElementException)
            {
                return stateIfNotFound;
            }
            catch (NotFoundException)
            {
                return stateIfNotFound;
            }
            catch (ElementNotVisibleException)
            {
                return stateIfNotFound;
            }
            catch (StaleElementReferenceException)
            {
                return stateIfNotFound;
            }
            catch (InvalidOperationException)
            {
                // An element could not be located on the page using the given search parameters.
                return stateIfNotFound;
            }
        }

        /// <summary>
        /// Clears the page.
        /// </summary>
        /// <param name="element">The element.</param>
        private static void ClearPage(IWebElement element)
        {
            element.Clear();
        }

        /// <summary>
        /// Fills the page.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="data">The data.</param>
        private void FillPage(IWebElement element, string data)
        {
            string text = data
                .Replace("{SPACE}", " ")
                .Replace("{RIGHT}", Keys.Right)
                .Replace("{NEWLINE}", Environment.NewLine);

            // Respect the data control interface first.
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (element is IDataControl)
            {
                IDataControl dataControlElement = element as IDataControl;
                dataControlElement.SetValue(text);
                return;
            }

            var tagName = element.TagName.ToLowerInvariant().Trim();
            switch (tagName)
            {
                case "select":
                    var selectElement = new SelectElement(element);
                    if (selectElement.IsMultiple)
                    {
                        selectElement.DeselectAll();
                    }

                    selectElement.SelectByText(text);
                    break;
                case "input":
                case "controltype.checkbox":
                case "controltype.radiobutton":
                    // Special case for a checkbox control
                    var inputType = element.GetAttribute("type");
                    if (string.Equals("checkbox", inputType, StringComparison.OrdinalIgnoreCase)
                        || (tagName == "controltype.checkbox")
                        || (tagName == "controltype.radiobutton"))
                    {
                        if (bool.TryParse(data, out bool checkValue) && element.Selected != checkValue)
                        {
                            new SeleniumPage(element, this.webDriver).ClickElement(element);
                        }

                        return;
                    }

                    if (string.Equals("radio", inputType, StringComparison.OrdinalIgnoreCase))
                    {
                        // Need to click twice to select the element.
                        new SeleniumPage(element, this.webDriver).ClickElement(element, 2);
                        return;
                    }

                    if (string.Equals("file", inputType, StringComparison.OrdinalIgnoreCase))
                    {
                        FileUploadHelper.UploadFile(text, element.SendKeys);
                        return;
                    }

                    goto default;
                default:
                    element.SendKeys(text);
                    break;
            }
        }

        /// <summary>
        /// Creates the page from the given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The appropriate page object.</returns>
        private SeleniumPage CreatePageFromElement(IWebElement element)
        {
            return new SeleniumPage(element, this.webDriver)
            {
                ExecuteWithElementLocateTimeout = this.ExecuteWithElementLocateTimeout,
                EvaluateWithElementLocateTimeout = this.EvaluateWithElementLocateTimeout
            };
        }

        /// <summary>
        /// Checks for the full timeout to have elapsed.
        /// </summary>
        /// <param name="waiter">The waiter.</param>
        /// <param name="condition">The condition.</param>
        /// <returns><c>true</c> if complete; otherwise <c>false</c></returns>
        private bool DoesFullTimeoutElapse(DefaultWait<IWebElement> waiter, Func<IWebElement, bool> condition)
        {
            var startTime = DateTime.Now;
            waiter.Until(condition);
            var elapsed = DateTime.Now - startTime;
            return elapsed >= waiter.Timeout;
        }

        /// <summary>
        /// Sets the attribute on the element via the web driver.
        /// </summary>
        /// <param name="element">The element being manipulated.</param>
        /// <param name="attributeName">The name of the attribute being modified.</param>
        /// <param name="value">The value of the parameter.</param>
        private void SetAttribute(IWebElement element, string attributeName, string value)
        {
            try
            {
                this.webDriver.ExecuteJavaScript("arguments[0].setAttribute(arguments[1], arguments[2])", element, attributeName, value);
            }
            catch (WebDriverException)
            {
                // This is a support function, ignore not supported for now
            }
        }
    }
}