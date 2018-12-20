// <copyright file="SeleniumBase.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using OpenQA.Selenium;
    using Pages;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;

    /// <summary>
    /// Base class for a Selenium application.
    /// </summary>
    /// <seealso cref="SpecBind.BrowserSupport.BrowserBase" />
    public abstract class SeleniumBase : BrowserBase
    {
        private Lazy<IWebDriver> driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumBase" /> class.
        /// </summary>
        /// <param name="driver">The browser driver as a lazy object.</param>
        /// <param name="logger">The logger.</param>
        public SeleniumBase(Lazy<IWebDriver> driver, ILogger logger)
            : base(logger)
        {
            // TODO: create timeouts structure, pass it through this constructor, so we know what the default timeouts are.
            this.driver = driver;

            this.PageBuilder = new SeleniumPageBuilder();
            this.PageCache = new Dictionary<Type, Func<IWebDriver, IBrowser, Action<object>, object>>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SeleniumBase"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        ~SeleniumBase()
        {
            this.Dispose(false);
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
        /// Gets a value indicating whether or not the browser is created.
        /// </summary>
        public override bool IsCreated => this.driver.IsValueCreated;

        /// <summary>
        /// Gets the driver.
        /// </summary>
        /// <value>The driver.</value>
        public IWebDriver Driver => this.driver.Value;

        /// <summary>
        /// Gets or sets the page builder.
        /// </summary>
        /// <value>The page builder.</value>
        protected SeleniumPageBuilder PageBuilder { get; set; }

        /// <summary>
        /// Gets or sets the page cache.
        /// </summary>
        /// <value>The page cache.</value>
        protected Dictionary<Type, Func<IWebDriver, IBrowser, Action<object>, object>> PageCache { get; set; }

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
            var localDriver = this.Driver;
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
        /// Updates the driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        internal void UpdateDriver(Lazy<IWebDriver> driver)
        {
            this.driver = driver;
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

            if (this.IsCreated)
            {
                var localDriver = this.Driver;
                localDriver.Quit();
                localDriver.Dispose();
            }
        }

        /// <summary>
        /// Creates the native page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <returns>The created page object.</returns>
        protected IPage CreateNativePage(Type pageType)
        {
            Func<IWebDriver, IBrowser, Action<object>, object> pageBuildMethod;
            if (!this.PageCache.TryGetValue(pageType, out pageBuildMethod))
            {
                pageBuildMethod = this.PageBuilder.CreatePage(pageType);
                this.PageCache.Add(pageType, pageBuildMethod);
            }

            var nativePage = pageBuildMethod(this.Driver, this, null);

            return new SeleniumPage(nativePage, this.Driver)
            {
                ExecuteWithElementLocateTimeout = this.ExecuteWithElementLocateTimeout,
                EvaluateWithElementLocateTimeout =
                    this.EvaluateWithElementLocateTimeout
            };
        }

        /// <summary>
        /// Evaluates the with element locate timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="work">The work.</param>
        /// <returns><c>true</c> if the element is located; otherwise <c>false</c>.</returns>
        private bool EvaluateWithElementLocateTimeout(TimeSpan timeout, Func<bool> work)
        {
            var timeoutManager = this.Driver.Manage().Timeouts();

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
            var timeoutManager = this.Driver.Manage().Timeouts();

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
