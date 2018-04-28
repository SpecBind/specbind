﻿// <copyright file="SeleniumBrowser.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using OpenQA.Selenium;

    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    using Cookie = System.Net.Cookie;

    /// <summary>
    /// A web browser level wrapper for selenium
    /// </summary>
    public class SeleniumBrowser : BrowserBase
    {
        private readonly Lazy<IWebDriver> driver;

        private readonly SeleniumPageBuilder pageBuilder;

        private readonly Dictionary<Type, Func<IWebDriver, IBrowser, Lazy<IUriHelper>, Action<object>, object>> pageCache;

        private bool switchedContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumBrowser" /> class.
        /// </summary>
        /// <param name="driver">The browser driver as a lazy object.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="uriHelper">The URI helper.</param>
        public SeleniumBrowser(Lazy<IWebDriver> driver, ILogger logger, Lazy<IUriHelper> uriHelper)
            : base(logger, uriHelper)
        {
            // TODO: create timeouts structure, pass it through this constructor, so we know what the default timeouts are.
            this.driver = driver;

            this.pageBuilder = new SeleniumPageBuilder(this.UriHelper);
            this.pageCache = new Dictionary<Type, Func<IWebDriver, IBrowser, Lazy<IUriHelper>, Action<object>, object>>();
        }

        /// <summary>
        /// Gets the type of the base page.
        /// </summary>
        /// <value>The type of the base page.</value>
        public override Type BasePageType => null;

        /// <summary>
        /// Gets the url of the current page.
        /// </summary>
        /// <value>
        /// The url of the base page.
        /// </value>
        public override string Url => this.driver.Value.Url;

        /// <summary>
        /// Gets the current driver to enable the user to do custom steps if necessary
        /// </summary>
        public IWebDriver Driver => this.driver.Value;

        /// <summary>
        /// Gets a value indicating whether or not the browser is created.
        /// </summary>
        public override bool IsCreated => this.driver.IsValueCreated;

        /// <summary>
        /// Finalizes an instance of the <see cref="SeleniumBrowser" /> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        ~SeleniumBrowser()
        {
            this.Dispose(false);
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
            var localDriver = this.driver.Value;
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
            var localDriver = this.driver.Value;
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
            var localDriver = this.driver.Value;
            localDriver.Manage().Cookies.DeleteAllCookies();
        }

        /// <summary>
        /// Clears the URL.
        /// </summary>
        public override void ClearUrl()
        {
            this.driver.Value.Url = "about:blank";
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close()
        {
            if (this.driver.IsValueCreated)
            {
                this.driver.Value.Close();
            }
        }

        /// <summary>
        /// Dismisses the alert.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="text">The text to enter.</param>
        public override void DismissAlert(AlertBoxAction action, string text)
        {
            var alert = this.driver.Value.SwitchTo().Alert();

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
            var javascriptExecutor = this.driver.Value as IJavaScriptExecutor;

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
            this.driver.Value.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Takes the screenshot from the native browser.
        /// </summary>
        /// <param name="imageFolder">The image folder.</param>
        /// <param name="fileNameBase">The file name base.</param>
        /// <returns>The complete file path if created; otherwise <c>null</c>.</returns>
        public override string TakeScreenshot(string imageFolder, string fileNameBase)
        {
            var localDriver = this.driver.Value;
            var takesScreenshot = localDriver as ITakesScreenshot;

            if (takesScreenshot == null)
            {
                return null;
            }

            try
            {
                var fullPath = Path.Combine(imageFolder, $"{fileNameBase}.jpg");

                var screenshot = takesScreenshot.GetScreenshot();
                screenshot.SaveAsFile(fullPath, ScreenshotImageFormat.Jpeg);

                return fullPath;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Save the html from the native browser.
        /// </summary>
        /// <param name="destinationFolder">The destination folder.</param>
        /// <param name="fileNameBase">The file name base.</param>
        /// <returns>The complete file path if created; otherwise <c>null</c>.</returns>
        public override string SaveHtml(string destinationFolder, string fileNameBase)
        {
            var localDriver = this.driver.Value;
            try
            {
                var fullPath = Path.Combine(destinationFolder, $"{fileNameBase}.html");
                using (var writer = File.CreateText(fullPath))
                {
                    writer.Write(localDriver.PageSource);
                }

                return fullPath;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the browser logs.
        /// </summary>
        /// <returns>A collection of log entries.</returns>
        public IReadOnlyCollection<LogEntry> GetBrowserLogs()
        {
            return this.driver.Value.Manage().Logs.GetLog(LogType.Browser);
        }

        /// <summary>
        /// Gets the native page location.
        /// </summary>
        /// <param name="page">The page interface.</param>
        /// <returns>A collection of URIs to validate.</returns>
        protected override IList<string> GetNativePageLocation(IPage page)
        {
            var currentLocation = this.driver.Value.Url;
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
            var webDriver = this.driver.Value;

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

            Func<IWebDriver, IBrowser, Lazy<IUriHelper>, Action<object>, object> pageBuildMethod;
            if (!this.pageCache.TryGetValue(pageType, out pageBuildMethod))
            {
                pageBuildMethod = this.pageBuilder.CreatePage(pageType);
                this.pageCache.Add(pageType, pageBuildMethod);
            }

            var nativePage = pageBuildMethod(webDriver, this, this.UriHelper, null);

            return new SeleniumPage(nativePage, webDriver)
                   {
                       ExecuteWithElementLocateTimeout = this.ExecuteWithElementLocateTimeout,
                       EvaluateWithElementLocateTimeout =
                           this.EvaluateWithElementLocateTimeout
                   };
        }

        /// <summary>
        /// Releases windows and driver specific resources. This method is already protected by the base instance.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void DisposeWindow(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (this.driver.IsValueCreated)
            {
                var localDriver = this.driver.Value;
                localDriver.Quit();
                localDriver.Dispose();
            }
        }

        /// <summary>
        /// Evaluates the with element locate timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="work">The work.</param>
        /// <returns><c>true</c> if the element is located; otherwise <c>false</c>.</returns>
        private bool EvaluateWithElementLocateTimeout(TimeSpan timeout, Func<bool> work)
        {
            var timeoutManager = this.driver.Value.Manage().Timeouts();

            try
            {
                timeoutManager.ImplicitWait = timeout;
                return work();
            }
            finally
            {
                timeoutManager.ImplicitWait = ActionBase.DefaultTimeout;
            }
        }

        /// <summary>
        /// Executes the with element locate timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="work">The work.</param>
        private void ExecuteWithElementLocateTimeout(TimeSpan timeout, Action work)
        {
            var timeoutManager = this.driver.Value.Manage().Timeouts();

            try
            {
                timeoutManager.ImplicitWait = timeout;
                work();
            }
            finally
            {
                timeoutManager.ImplicitWait = ActionBase.DefaultTimeout;
            }
        }
    }
}