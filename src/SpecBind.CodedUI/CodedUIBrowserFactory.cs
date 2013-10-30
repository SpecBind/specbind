// <copyright file="CodedUIBrowserFactory.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI
{
	using System;
	using System.Configuration;
	using System.Threading;

	using Microsoft.VisualStudio.TestTools.UITesting;

	using SpecBind.BrowserSupport;
	using SpecBind.Configuration;

    /// <summary>
	/// A browser factory class for Coded UI tests.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public class CodedUIBrowserFactory : BrowserFactory
    {
        /// <summary>
        /// Creates the browser.
        /// </summary>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="browserFactoryConfiguration">The browser factory configuration.</param>
        /// <returns>A browser object.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the browser type is not supported.</exception>
        protected override IBrowser CreateBrowser(BrowserType browserType, BrowserFactoryConfigurationElement browserFactoryConfiguration)
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

            Playback.PlaybackSettings.SearchTimeout = (int)browserFactoryConfiguration.ElementLocateTimeout.TotalSeconds;
            Playback.PlaybackSettings.WaitForReadyTimeout = (int)browserFactoryConfiguration.PageLoadTimeout.TotalSeconds;

			var launchAction = new Func<BrowserWindow>(() =>
				{
					//Switch key if needed.
					if (browserKey != null)
					{
						BrowserWindow.CurrentBrowser = browserKey;
					}

                    return BrowserWindow.Launch();
				});

			var browser = new Lazy<BrowserWindow>(launchAction, LazyThreadSafetyMode.None);
			return new CodedUIBrowser(browser);
		}
    }
}
