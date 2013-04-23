// <copyright file="CodedUIBrowserFactory.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI
{
	using System;
	using System.Threading;

	using Microsoft.VisualStudio.TestTools.UITesting;

	using SpecBind.BrowserSupport;

	/// <summary>
	/// A browser factory class for Coded UI tests.
	/// </summary>
	public class CodedUIBrowserFactory : BrowserFactory
    {
		/// <summary>
		/// Creates the browser.
		/// </summary>
		/// <param name="browserType">Type of the browser.</param>
		/// <returns>
		/// A browser object.
		/// </returns>
		protected override IBrowser CreateBrowser(BrowserType browserType)
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
			}

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
