// <copyright file="UriHelperWatinFixture.cs" company="Dan Piessens">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Watin.Tests
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using SpecBind.BrowserSupport;
	using SpecBind.Helpers;

	using WatiN.Core;

	/// <summary>
	///     A test fixture for URI helpers.
	/// </summary>
	[TestClass]
	public class UriHelperWatinFixture
	{
		/// <summary>
		///     Tests the get page URI method.
		/// </summary>
		[TestMethod]
		public void TestGetPageUriFromTypeWithPageAttribute()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(b => b.GetUriForPageType(typeof(PageAttributePage))).Returns("/testpage");

			var url = UriHelper.GetPageUri(browser.Object, typeof(PageAttributePage));

			Assert.AreEqual(url, "/testpage");
			browser.VerifyAll();
		}

		// ReSharper disable ClassNeverInstantiated.Local
		
		/// <summary>
		/// A test class for page attribute configurations.
		/// </summary>
		[Page(UrlRegex = "/testpage")]
		private class PageAttributePage : Page
			// ReSharper restore ClassNeverInstantiated.Local
		{
		}
	}
}