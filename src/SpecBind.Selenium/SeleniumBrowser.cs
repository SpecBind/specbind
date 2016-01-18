// <copyright file="SeleniumBrowser.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Threading;

    using OpenQA.Selenium;

    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A web browser level wrapper for selenium
    /// </summary>
    public class SeleniumBrowser : BrowserBase
    {
        private readonly Lazy<IWebDriver> driver;
        private readonly SeleniumPageBuilder pageBuilder;
        private readonly Dictionary<Type, Func<IWebDriver, IBrowser, Action<object>, object>> pageCache;

        private bool disposed;
        private bool switchedContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumBrowser" /> class.
        /// </summary>
        /// <param name="driver">The browser driver as a lazy object.</param>
        /// <param name="logger">The logger.</param>
        public SeleniumBrowser(Lazy<IWebDriver> driver, ILogger logger) : base(logger)
        {
            // TODO: create timeouts structure, pass it through this constructor, so we know what the default timeouts are.
            this.driver = driver;

            this.pageBuilder = new SeleniumPageBuilder();
            this.pageCache = new Dictionary<Type, Func<IWebDriver, IBrowser, Action<object>, object>>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SeleniumBrowser" /> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        ~SeleniumBrowser()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the type of the base page.
        /// </summary>
        /// <value>The type of the base page.</value>
        public override Type BasePageType
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the url of the current page.
        /// </summary>
        /// <value>
        /// The url of the base page.
        /// </value>
        public override string Url
        {
            get
            {
                return this.driver.Value.Url;
            }
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
        public override void AddCookie(string name, string value, string path, DateTime? expireDateTime, string domain, bool secure)
        {
            var localDriver = this.driver.Value;
            var cookieContainer = localDriver.Manage().Cookies;

            var currentCookie = cookieContainer.GetCookieNamed(name);
            if (currentCookie != null)
            {
                cookieContainer.DeleteCookieNamed(name);
            }

            cookieContainer.AddCookie(new Cookie(name, value, domain, path, expireDateTime));
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
        /// Closes the instance and optionally dispose of all resources
        /// </summary>
        /// <param name="dispose">Whether or not resources should get disposed</param>
        public override void Close(bool dispose)
        {
            this.Close();
            if (dispose)
            {
                this.Dispose();
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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
                var fullPath = Path.Combine(imageFolder, string.Format("{0}.jpg", fileNameBase));
                
                var screenshot = takesScreenshot.GetScreenshot();
                screenshot.SaveAsFile(fullPath, ImageFormat.Jpeg);

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
                var fullPath = Path.Combine(destinationFolder, string.Format("{0}.html", fileNameBase));
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
            if (pageType.TryGetAttribute(out navigationAttribute) && !string.IsNullOrWhiteSpace(navigationAttribute.FrameName))
            {
                webDriver.SwitchTo().Frame(navigationAttribute.FrameName);
                this.switchedContext = true;
            }
            else if (this.switchedContext)
            {
                webDriver.SwitchTo().DefaultContent();
                this.switchedContext = false;
            }

            Func<IWebDriver, IBrowser, Action<object>, object> pageBuildMethod;
            if (!this.pageCache.TryGetValue(pageType, out pageBuildMethod))
            {
                pageBuildMethod = pageBuilder.CreatePage(pageType);
                this.pageCache.Add(pageType, pageBuildMethod);
            }

            var nativePage = pageBuildMethod(webDriver, this, null);

            return new SeleniumPage(nativePage)
               {
                   ExecuteWithElementLocateTimeout = this.ExecuteWithElementLocateTimeout,
                   EvaluateWithElementLocateTimeout =  this.EvaluateWithElementLocateTimeout
               };
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || this.disposed)
            {
                return;
            }

            if (this.driver.IsValueCreated)
            {
                var localDriver = this.driver.Value;
                localDriver.Quit();
                localDriver.Dispose();
            }
            this.disposed = true;
        }

        /// <summary>
        /// Evaluates the with element locate timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="work">The work.</param>
        /// <returns><c>true</c> if the element is located; otherwise <c>false</c>.</returns>
        private bool EvaluateWithElementLocateTimeout(TimeSpan timeout, Func<bool> work)
        {
            TimeSpan originalTimeout = WaitForElementAction.DefaultTimeout;
            var timeoutManager = this.driver.Value.Manage().Timeouts();

            try
            {
                timeoutManager.ImplicitlyWait(timeout);
                return work();
            }
            finally
            {
                timeoutManager.ImplicitlyWait(originalTimeout);
            }
        }

        /// <summary>
        /// Executes the with element locate timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="work">The work.</param>
        private void ExecuteWithElementLocateTimeout(TimeSpan timeout, Action work)
        {
            TimeSpan originalTimeout = WaitForElementAction.DefaultTimeout;
            var timeoutManager = this.driver.Value.Manage().Timeouts();

            try
            {
                timeoutManager.ImplicitlyWait(timeout);
                work();
            }
            finally
            {
                timeoutManager.ImplicitlyWait(originalTimeout);
            }
        }
    }
}