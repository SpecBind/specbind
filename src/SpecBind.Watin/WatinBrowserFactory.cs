// <copyright file="WatinBrowserFactory.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Watin
{
	using System;
	using System.Threading;

	using SpecBind.BrowserSupport;

	using WatiN.Core;

	/// <summary>
	/// A browser factory for Watin.
	/// </summary>
	public class WatinBrowserFactory : BrowserFactory
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
			Lazy<Browser> browser = null;
			switch (browserType)
			{
				case BrowserType.IE:
					browser = new Lazy<Browser>(() => new IE(true), LazyThreadSafetyMode.None);
					break;
				case BrowserType.FireFox:
					browser = new Lazy<Browser>(() => new FireFox(), LazyThreadSafetyMode.None);
					break;
			}

			return new WatinBrowser(browser);
		}
	}
}