// <copyright file="SeleniumBrowser.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;

    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using SpecBind.Selenium.Drivers;
    using Cookie = System.Net.Cookie;

    /// <summary>
    /// A web browser level wrapper for selenium
    /// </summary>
    public class SeleniumBrowser : SeleniumBase
    {
        private bool switchedContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumBrowser" /> class.
        /// </summary>
        /// <param name="driver">The browser driver as a lazy object.</param>
        /// <param name="logger">The logger.</param>
        public SeleniumBrowser(Lazy<IWebDriverEx> driver, ILogger logger)
            : base(driver, logger)
        {
        }

        /// <summary>
        /// Gets the main browser window handle.
        /// </summary>
        /// <returns>The main browser window handle.</returns>
        public string GetMainBrowserWindowHandle()
        {
            return this.Driver.GetMainBrowserWindowHandle();
        }

        /// <summary>
        /// Adds the cookie to the browser.
        /// </summary>
        /// <param name="name">The cookie name.</param>
        /// <param name="value">The cookie value.</param>
        /// <param name="path">The path.</param>
        /// <param name="expireDateTime">The expiration date time.</param>
        /// <param name="domain">The cookie domain.</param>
        /// <param name="secure">if set to <c>true</c> the cookie is secure.</param>
        public override void AddCookie(
            string name,
            string value,
            string path,
            DateTime? expireDateTime,
            string domain,
            bool secure)
        {
            var localDriver = this.Driver;
            var cookieContainer = localDriver.Manage().Cookies;

            var currentCookie = cookieContainer.GetCookieNamed(name);
            if (currentCookie != null)
            {
                cookieContainer.DeleteCookieNamed(name);
            }

            cookieContainer.AddCookie(new OpenQA.Selenium.Cookie(name, value, domain, path, expireDateTime));
        }

        /// <summary>
        /// Get a cookie from the browser
        /// </summary>
        /// <param name="name">The name of the cookie</param>
        /// <returns>The cookie (if exists)</returns>
        public override Cookie GetCookie(string name)
        {
            var localDriver = this.Driver;
            var cookieContainer = localDriver.Manage().Cookies;
            var seleniumCookie = cookieContainer.GetCookieNamed(name);

            if (seleniumCookie != null)
            {
                var cookie = new Cookie()
                {
                    Name = seleniumCookie.Name,
                    Domain = seleniumCookie.Domain,
                    HttpOnly = seleniumCookie.IsHttpOnly,
                    Expires = seleniumCookie.Expiry.GetValueOrDefault(),
                    Path = seleniumCookie.Path,
                    Value = seleniumCookie.Value,
                    Secure = seleniumCookie.Secure
                };

                return cookie;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Clear all browser cookies
        /// </summary>
        public override void ClearCookies()
        {
            var localDriver = this.Driver;
            if (!(localDriver is WindowsDriverEx))
            {
                IOptions options = localDriver.Manage();
                ICookieJar cookieJar = options.Cookies;
                cookieJar.DeleteAllCookies();
            }
        }

        /// <summary>
        /// Clears the URL.
        /// </summary>
        public override void ClearUrl()
        {
            this.Driver.Url = "about:blank";
        }

        /// <summary>
        /// Determines whether the URL can be retrieved.
        /// </summary>
        /// <returns><c>true</c> if the URL can be retrieved; otherwise, <c>false</c>.</returns>
        public override bool CanGetUrl()
        {
            if (!this.IsCreated)
            {
                return false;
            }

            return !this.IsAlertDialogDisplayed();
        }

        /// <summary>
        /// Dismisses the alert.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="text">The text to enter.</param>
        public override void DismissAlert(AlertBoxAction action, string text)
        {
            var alert = this.Driver.SwitchTo().Alert();

            if (text != null)
            {
                alert.SendKeys(text);
            }

            switch (action)
            {
                case AlertBoxAction.Cancel:
                case AlertBoxAction.Close:
                case AlertBoxAction.Ignore:
                case AlertBoxAction.No:
                    alert.Dismiss();
                    break;
                case AlertBoxAction.Ok:
                case AlertBoxAction.Retry:
                case AlertBoxAction.Yes:
                    alert.Accept();
                    break;
            }
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script to execute.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The result of the script if needed.</returns>
        public override object ExecuteScript(string script, params object[] args)
        {
            var javascriptExecutor = this.Driver as IJavaScriptExecutor;

            if (javascriptExecutor == null)
            {
                return null;
            }

            var result = javascriptExecutor.ExecuteScript(script, args);

            var webElement = result as IWebElement;
            if (webElement == null)
            {
                return result;
            }

            var proxy = new WebElement(webElement);
            proxy.CloneNativeElement(webElement);
            return proxy;
        }

        /// <summary>
        /// Navigates the browser to the given <paramref name="url" />.
        /// </summary>
        /// <param name="url">The URL specified as a well formed Uri.</param>
        public override void GoTo(Uri url)
        {
            this.Driver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Goes back to the previous page using the browser's back button.
        /// </summary>
        public override void GoBack()
        {
            this.Driver.Navigate().Back();
        }

        /// <summary>
        /// Switches to the window with the specified page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <returns>
        /// The page object.
        /// </returns>
        public override IPage SwitchToWindow(Type pageType)
        {
            IPage page = this.Page(pageType);
            string actualPath;
            string expectedPath;

            ReadOnlyCollection<string> windowHandles = this.Driver.WindowHandles;
            int count = windowHandles.Count;
            for (int i = 0; i < count; i++)
            {
                string windowHandle = windowHandles[i];

                this.Driver.SwitchTo().Window(windowHandle);
                this.Logger.Debug($"Found browser window [{i + 1}/{count}] with title '{this.Driver.Title}' and url '{this.Driver.Url}'.");

                if (this.CheckIsOnPage(page.PageType, page, out actualPath, out expectedPath))
                {
                    return page;
                }
            }

            return null;
        }

        /// <summary>
        /// Refreshes the current page.
        /// </summary>
        public override void Refresh()
        {
            this.Driver.Navigate().Refresh();
        }

        /// <summary>
        /// Gets the native page location.
        /// </summary>
        /// <param name="page">The page interface.</param>
        /// <returns>A collection of URIs to validate.</returns>
        protected override IList<string> GetNativePageLocation(IPage page)
        {
            var currentLocation = this.Driver.Url;
            return new List<string> { currentLocation };
        }

        /// <summary>
        /// Creates the native page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="verifyPageValidity">if set to <c>true</c> verify the page validity.</param>
        /// <returns>The created page object.</returns>
        protected override IPage CreateNativePage(Type pageType, bool verifyPageValidity)
        {
            var webDriver = this.Driver;

            // Check to see if a frames reference exists, and switch if needed
            PageNavigationAttribute navigationAttribute;
            if (pageType.TryGetAttribute(out navigationAttribute)
                && !string.IsNullOrWhiteSpace(navigationAttribute.FrameName))
            {
                webDriver.SwitchTo().Frame(navigationAttribute.FrameName);
                this.switchedContext = true;
            }
            else if (this.switchedContext)
            {
                webDriver.SwitchTo().DefaultContent();
                this.switchedContext = false;
            }

            return this.CreateNativePage(pageType);
        }

        /// <summary>
        /// Determines whether the alert dialog is displayed.
        /// </summary>
        /// <returns><c>true</c> if the alert dialog is displayed; otherwise, <c>false</c>.</returns>
        private bool IsAlertDialogDisplayed()
        {
            if (!this.IsCreated)
            {
                return false;
            }

            IAlert alert = ExpectedConditions.AlertIsPresent().Invoke(this.Driver);
            return alert != null;
        }
    }
}