// <copyright file="CodedUIBrowserFactory.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI
{
	using System;
	using System.Threading;

	using Microsoft.VisualStudio.TestTools.UITest.Extension;
	using Microsoft.VisualStudio.TestTools.UITesting;

	using SpecBind.Actions;
	using SpecBind.BrowserSupport;
	using SpecBind.Configuration;

	using BrowserFactory = SpecBind.BrowserSupport.BrowserFactory;

    /// <summary>
	/// A browser factory class for Coded UI tests.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public class CodedUIBrowserFactory : BrowserFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodedUIBrowserFactory"/> class.
        /// </summary>
        public CodedUIBrowserFactory()
            : base(false)
        {
        }

        /// <summary>
        /// Creates the browser.
        /// </summary>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A browser object.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the browser type is not supported.</exception>
        protected override IBrowser CreateBrowser(BrowserType browserType, BrowserFactoryConfigurationElement browserFactoryConfiguration, ILogger logger)
		{
			string browserKey = null;
			switch (browserType)
			{
				case BrowserType.FireFox:
					browserKey = "firefox";
					break;
				case BrowserType.Chrome:
					browserKey = "chrome";
					break;
                case BrowserType.IE:
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Browser type '{0}' is not supported in Coded UI.", browserType));
			}

            Playback.PlaybackSettings.SmartMatchOptions = SmartMatchOptions.Control;
            Playback.PlaybackSettings.SearchTimeout = (int)browserFactoryConfiguration.ElementLocateTimeout.TotalMilliseconds;
            Playback.PlaybackSettings.WaitForReadyTimeout = (int)browserFactoryConfiguration.PageLoadTimeout.TotalMilliseconds;

			var launchAction = new Func<BrowserWindow>(() =>
				{
					//Switch key if needed.
					if (browserKey != null)
					{
						BrowserWindow.CurrentBrowser = browserKey;
					}

                    var window = BrowserWindow.Launch();

				    if (browserFactoryConfiguration.EnsureCleanSession)
				    {
				        BrowserWindow.ClearCache();
                        BrowserWindow.ClearCookies();
				    }

				    return window;
				});

			var browser = new Lazy<BrowserWindow>(launchAction, LazyThreadSafetyMode.None);
			return new CodedUIBrowser(browser, logger);
		}
    }
}
