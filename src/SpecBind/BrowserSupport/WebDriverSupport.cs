// <copyright file="WebDriverSupport.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.BrowserSupport
{
	using System;

	using BoDi;

	using SpecBind.Helpers;
	using SpecBind.Pages;

	using TechTalk.SpecFlow;

	/// <summary>
	/// A hooks support class for the web driver.
	/// </summary>
	[Binding]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class WebDriverSupport
	{
		private readonly IObjectContainer objectContainer;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebDriverSupport" /> class.
		/// </summary>
		/// <param name="objectContainer">The object container.</param>
		public WebDriverSupport(IObjectContainer objectContainer)
		{
			this.objectContainer = objectContainer;
		}

		/// <summary>
		/// Initializes the page mapper at the start of the test run.
		/// </summary>
		[BeforeScenario]
		public void InitializeDriver()
		{
			var factory = BrowserFactory.GetBrowserFactory();
			var browser = factory.GetBrowser();
			this.objectContainer.RegisterInstanceAs(browser);

			var mapper = new PageMapper();
			mapper.Initialize(browser.BasePageType);
			this.objectContainer.RegisterInstanceAs<IPageMapper>(mapper);

			this.objectContainer.RegisterInstanceAs<IPageDataFiller>(new PageDataFiller());
			this.objectContainer.RegisterInstanceAs<IScenarioContextHelper>(new ScenarioContextHelper());
			this.objectContainer.RegisterInstanceAs(TokenManager.Current);
		}

		/// <summary>
		/// Tears down the web driver.
		/// </summary>
		[AfterScenario]
		public void TearDownWebDriver()
		{
			var browser = this.objectContainer.Resolve<IBrowser>();
			browser.Close();

// ReSharper disable SuspiciousTypeConversion.Global
			var dispoable = browser as IDisposable;
// ReSharper restore SuspiciousTypeConversion.Global
			if (dispoable != null)
			{
				dispoable.Dispose();
			}
		}
	}
}