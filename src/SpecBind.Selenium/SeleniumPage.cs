// <copyright file="SeleniumPage.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;
    using System.Threading;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;

    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// An implementation of <see cref="IPage"/> for the Selenium driver.
    /// </summary>
    public class SeleniumPage : PageBase<object, IWebElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumPage"/> class.
        /// </summary>
        /// <param name="nativePage">The native page.</param>
        public SeleniumPage(object nativePage)
            : base(nativePage.GetType(), nativePage)
        {
        }

        /// <summary>
        /// A delegate to set the ElementLocateTimeout.
        /// </summary>
        public Action<TimeSpan, Action> ExecuteWithElementLocateTimeout { get; set; }

        /// <summary>
        /// A delegate to set the ElementLocateTimeout.
        /// </summary>
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
        /// Checks to see if the element exists.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element exists, <c>false</c> otherwise.</returns>
        public override bool ElementNotExistsCheck(IWebElement element)
        {
            if (element == null) return true;

            return this.EvaluateWithElementLocateTimeout(
                new TimeSpan(),
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
            return FillPage;
        }

        /// <summary>
        /// Gets the element text.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The text of the element.</returns>
        public override string GetElementText(IWebElement element)
        {
            var tagName = element.TagName.ToLowerInvariant().Trim();
            switch (tagName)
            {
                case "select":
                    var selectElement = new SelectElement(element);
                    return selectElement.SelectedOption.Text;
                case "input":
                case "textarea":
                    // Special case for a checkbox control
                    if (string.Equals("checkbox", element.GetAttribute("type"), StringComparison.OrdinalIgnoreCase))
                    {
                        return element.Selected.ToString();
                    }

                    return element.GetAttribute("value");
                default:
                    return element.Text;
            }
        }

        /// <summary>
        /// Gets the page from element.
        /// </summary>
        /// <param name="element">The parent element.</param>
        /// <returns>The child page as a scope.</returns>
        public override IPage GetPageFromElement(IWebElement element)
        {
            return CreatePageFromElement(element);
        }

        private SeleniumPage CreatePageFromElement(IWebElement element)
        {
            return new SeleniumPage(element)
               {
                   ExecuteWithElementLocateTimeout = this.ExecuteWithElementLocateTimeout,
                   EvaluateWithElementLocateTimeout = this.EvaluateWithElementLocateTimeout
               };
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
        /// Clicks the element a given number of times.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="times">The number of times to click.</param>
        /// <returns><c>true</c> if the element is clicked, <c>false</c> otherwise.</returns>
        protected virtual bool ClickElement(IWebElement element, int times)
        {
            if (times < 1) return true;

            if (!this.WaitForElement(element, WaitConditions.NotMoving, timeout: null)) return false;
            if (!this.WaitForElement(element, WaitConditions.BecomesEnabled, timeout: null)) return false;

            // TODO: consider waiting between clicks, so that it's not interpreted as a double-click
            for (var i = 0; i < times; i++) element.Click();
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

            try
            {
                switch (waitCondition)
                {
                    case WaitConditions.BecomesNonExistent: // AKA NotExists
                        this.ExecuteWithElementLocateTimeout(
                            new TimeSpan(), 
                            () =>
                            {
                                try
                                {
                                    waiter.Until(e => !e.Displayed);
                                }
                                catch (NoSuchElementException) { return; }
                                catch (NotFoundException) { return; }
                                catch (ElementNotVisibleException) { return; }
                                catch (StaleElementReferenceException) { return; }
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
                                catch (NoSuchElementException) { return true; }
                                catch (NotFoundException) { return true; }
                                catch (ElementNotVisibleException) { return true; }
                                catch (StaleElementReferenceException) { return true; }
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
                        waiter.IgnoreExceptionTypes(typeof(ElementNotVisibleException), typeof(NotFoundException));
                        waiter.Until(e => e.Displayed);
                        break;
                    case WaitConditions.NotMoving:
                        waiter.IgnoreExceptionTypes(typeof(ElementNotVisibleException), typeof(NotFoundException));
                        waiter.Until(e => e.Displayed);
                        waiter.Until(e => !Moving(e));
                        break;
                    case WaitConditions.RemainsEnabled:
                        return this.DoesFullTimeoutElapse(waiter, e => !e.Enabled);
                    case WaitConditions.RemainsDisabled:
                        return this.DoesFullTimeoutElapse(waiter, e => e.Enabled);
                    case WaitConditions.RemainsExistent:
                        return this.DoesFullTimeoutElapse(waiter, e => !e.Displayed);
                }
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
            
            return true;
        }

        private bool DoesFullTimeoutElapse(DefaultWait<IWebElement> waiter, Func<IWebElement, bool> condition)
        {
            var startTime = DateTime.Now;
            waiter.Until(condition);
            var elapsed = DateTime.Now - startTime;
            return elapsed >= waiter.Timeout;
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
        }

        /// <summary>
        /// Fills the page.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="data">The data.</param>
        private static void FillPage(IWebElement element, string data)
        {
            // Respect the data control interface first.
            // ReSharper disable once SuspiciousTypeConversion.Global
            var dataControlElement = element as IDataControl;
            if (dataControlElement != null)
            {
                dataControlElement.SetValue(data);
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
                    
                    selectElement.SelectByText(data);
                    break;
                case "input":
                    // Special case for a checkbox control
                    var inputType = element.GetAttribute("type");
                    if (string.Equals("checkbox", inputType, StringComparison.OrdinalIgnoreCase))
                    {
                        bool checkValue;
                        if (bool.TryParse(data, out checkValue) && element.Selected != checkValue)
                        {
                            new SeleniumPage(element).ClickElement(element);
                        }
                        return;
                    }

                    if (string.Equals("radio", inputType, StringComparison.OrdinalIgnoreCase))
                    {
                        // Need to click twice to select the element.
                        new SeleniumPage(element).ClickElement(element, times: 2);
                        return;
                    }
                    
                    if (string.Equals("file", inputType, StringComparison.OrdinalIgnoreCase))
                    {
                        FileUploadHelper.UploadFile(data, element.SendKeys);
                        return;
                    }
                    goto default;
                default:
                    element.SendKeys(data);
                    break;
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
    }
}
