// <copyright file="UriHelperFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
	using System;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using SpecBind.BrowserSupport;
	using SpecBind.Helpers;
	using SpecBind.Pages;
	using SpecBind.Tests.Support;

	using System.Collections.Generic;

	/// <summary>
	///     A test fixture for URI helpers.
	/// </summary>
	[TestClass]
	public class UriHelperFixture
	{
		/// <summary>
		///     Tests the get page URI method.
		/// </summary>
		[TestMethod]
		public void TestGetQualifiedPageUri()
		{
            UriHelper.BaseUri = new Uri("http://localhost:2222/");
			var url = UriHelper.GetQualifiedPageUri("subpath/1");

			Assert.AreEqual(url, "http://localhost:2222/subpath/1");
		}

		/// <summary>
		///     Tests the get page URI method with a page type.
		/// </summary>
		[TestMethod]
		public void TestGetQualifiedPageUriFromPageType()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);

            UriHelper.BaseUri = new Uri("http://localhost:2222/");
			var url = UriHelper.GetQualifiedPageUri(browser.Object, typeof(NavigationAttributePage));

			Assert.AreEqual(url, new Uri("http://localhost:2222/root"));

			browser.VerifyAll();
		}

        /// <summary>
		///     Tests the get page URI method with a page type.
		/// </summary>
		[TestMethod]
		public void TestGetQualifiedPageUriFromPageTypeWithAbslouteUriAttribute()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);

            UriHelper.BaseUri = new Uri("http://localhost:2222/");
			var url = UriHelper.GetQualifiedPageUri(browser.Object, typeof(AbsoluteUriPage));

            Assert.AreEqual(url, new Uri("http://www.atestsite.com/subpath"));

			browser.VerifyAll();
		}
        
        /// <summary>
        ///     Tests the get page URI method with a page type and a longer hostname.
        /// </summary>
        [TestMethod]
        public void TestGetDottedHostnameQualifiedPageUriFromPageType()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            UriHelper.BaseUri = new Uri("http://myapp.qa8.somedomain.com/");
            var url = UriHelper.GetQualifiedPageUri(browser.Object, typeof(NavigationAttributePage));

            Assert.AreEqual(url, new Uri("http://myapp.qa8.somedomain.com/root"));

            browser.VerifyAll();
        }

        /// <summary>
        ///     Tests the get page URI method.
        /// </summary>
        [TestMethod]
        public void TestGetQualifiedPageUriWithBaseVirtualDirectory()
        {
            UriHelper.BaseUri = new Uri("http://localhost/MyVirtualDir");
            var url = UriHelper.GetQualifiedPageUri("subpath/1");

            Assert.AreEqual("http://localhost/MyVirtualDir/subpath/1", url.ToString());
        }

        /// <summary>
        ///     Tests the get page URI method with a page type with no slash.
        /// </summary>
        [TestMethod]
        public void TestGetQualifiedPageUriFromPageTypeWithNoSlash()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            UriHelper.BaseUri = new Uri("http://localhost:2222/");
            var url = UriHelper.GetQualifiedPageUri(browser.Object, typeof(NavigationWithNoSlashAttributePage));

            Assert.AreEqual(url, new Uri("http://localhost:2222/root"));

            browser.VerifyAll();
        }

        /// <summary>
        ///     Tests the get page URI method.
        /// </summary>
        [TestMethod]
        public void TestGetQualifiedPageUriRegex()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            UriHelper.BaseUri = new Uri("http://localhost:2222/");
            var regex = UriHelper.GetQualifiedPageUriRegex(browser.Object, typeof(NavigationAttributePage));

            Assert.AreEqual(true, regex.IsMatch("http://localhost:2222/root"));
            Assert.AreEqual(true, regex.IsMatch("http://localhost:2222/root/something123"));
            
            browser.VerifyAll();
        }

        /// <summary>
        ///     Tests the get page URI method with parameters
        /// </summary>
        [TestMethod]
        public void TestGetQualifiedPageUriRegexWithParameters()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            UriHelper.BaseUri = new Uri("http://localhost:2222/");
            var regex = UriHelper.GetQualifiedPageUriRegex(browser.Object, typeof(NavigationWithParamtersPage));

            Assert.AreEqual(true, regex.IsMatch("http://localhost:2222/root?q=parameter"));

            browser.VerifyAll();
        }


        /// <summary>
        ///     Tests the get page URI method.
        /// </summary>
        [TestMethod]
        public void TestGetQualifiedPageUriRegexAttributeDoesNotContainASlash()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            UriHelper.BaseUri = new Uri("http://localhost:2222/");
            var regex = UriHelper.GetQualifiedPageUriRegex(browser.Object, typeof(NavigationWithNoSlashAttributePage));

            Assert.AreEqual(true, regex.IsMatch("http://localhost:2222/root"));
            Assert.AreEqual(true, regex.IsMatch("http://localhost:2222/root/something123"));
            
            browser.VerifyAll();
        }

        /// <summary>
        /// Tests the get page URI method.
        /// </summary>
        [TestMethod]
        public void TestGetQualifiedPageUriWithAbsoluteNavigationPath()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            UriHelper.BaseUri = new Uri("http://localhost:2222/");
            var regex = UriHelper.GetQualifiedPageUriRegex(browser.Object, typeof(AbsoluteUriPage));

            Assert.AreEqual(true, regex.IsMatch("http://www.atestsite.com/subpath"));
            
            browser.VerifyAll();
        }

        /// <summary>
        ///     Tests the get page URI method.
        /// </summary>
        [TestMethod]
        public void TestGetQualifiedPageUriRegexWithRegexInNavigation()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            UriHelper.BaseUri = new Uri("http://localhost:2222/");
            var regex = UriHelper.GetQualifiedPageUriRegex(browser.Object, typeof(NavigationWithRegexAttributePage));

            Assert.AreEqual(true, regex.IsMatch("http://localhost:2222/root/223"));
            
            browser.VerifyAll();
        }

        /// <summary>
		///     Tests the get page URI method.
		/// </summary>
		[TestMethod]
		public void TestGetPageUriFromTypeWithNavigationAttribute()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);

            UriHelper.BaseUri = new Uri("http://localhost:2222/");
            var url = UriHelper.GetPageUri(browser.Object, typeof(NavigationAttributePage));

            Assert.AreEqual("/root", url);

			browser.VerifyAll();
		}

		/// <summary>
		///     Tests the get page URI method with a type that doesn't contain an attribute.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(PageNavigationException))]
		public void TestGetPageUriFromTypeWithInvalidNavigationAttribute()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(b => b.GetUriForPageType(typeof(InvalidPage))).Returns((string)null);

			try
			{
                UriHelper.BaseUri = new Uri("http://localhost:2222/");
				UriHelper.GetPageUri(browser.Object, typeof(InvalidPage));
			}
			catch (PageNavigationException ex)
			{
				StringAssert.Contains(ex.Message, "InvalidPage");

				browser.VerifyAll();
				throw;
			}
		}

		/// <summary>
		///     Tests the get page URI method.
		/// </summary>
		[TestMethod]
		public void TestFillPageUri()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);

            UriHelper.BaseUri = new Uri("http://localhost:2222/");
			var url = UriHelper.FillPageUri(
				browser.Object, typeof(NavigationAttributePage), new Dictionary<string, string> { { "Id", "1" } });

            Assert.AreEqual("http://localhost:2222/root/1", url);

			browser.VerifyAll();
		}

		/// <summary>
		///     Tests the get page URI method with a null dictionary.
		/// </summary>
		[TestMethod]
		public void TestFillPageUriNullDictionary()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);

            UriHelper.BaseUri = new Uri("http://localhost:2222/");
			var url = UriHelper.FillPageUri(
				browser.Object, typeof(NavigationAttributePage), null);

            Assert.AreEqual("http://localhost:2222/root/{id}", url);

			browser.VerifyAll();
		}

		/// <summary>
		///     Tests the get page URI method with a null dictionary.
		/// </summary>
		[TestMethod]
		public void TestFillPageUriNoAttribute()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(b => b.GetUriForPageType(typeof(InvalidPage))).Returns("/testpage");

            UriHelper.BaseUri = new Uri("http://localhost:2222/");
			var url = UriHelper.FillPageUri(
				browser.Object, typeof(InvalidPage), null);

            Assert.AreEqual("http://localhost:2222/testpage", url);

			browser.VerifyAll();
		}

		/// <summary>
		///     Tests the FillPageUri method when a template is specified and AbsoluteUri is true.
		///     When these conditions are met, the base Uri should be ignored.
		/// </summary>
		[TestMethod]
		public void TestFillPageUriWithTemplateAndAbsoluteUri()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			// Setting base uri here should not matter as it will get ignored since AbsoluteUri = true for this test
			UriHelper.BaseUri = new Uri("http://localhost:2222/");
			
			var url = UriHelper.FillPageUri(
				browser.Object, typeof(NavigationWithTemplateAndAbsoluteUri), new Dictionary<string, string> { { "param", "1" } });

			Assert.AreEqual("http://root?q=1", url);

			browser.VerifyAll();
		}


        // ReSharper disable ClassNeverInstantiated.Local

		/// <summary>
		/// A test class for invalid configurations.
		/// </summary>
		private class InvalidPage : TestBase
		{
		}

		/// <summary>
		/// A test class for navigation page attribute configurations.
		/// </summary>
		[PageNavigation("/root", UrlTemplate = "/root/{id}")]
		private class NavigationAttributePage : TestBase
		{
		}

        /// <summary>
        /// A test class for navigation with a URL template and fixed URL.
        /// </summary>
		[PageNavigation("http://root", UrlTemplate = "http://root?q={param}", IsAbsoluteUrl = true)]
		private class NavigationWithTemplateAndAbsoluteUri : TestBase
		{
		}

	    /// <summary>
	    /// A test class for navigation page with parameters
	    /// </summary>
	    [PageNavigation("/root?q=parameter")]
	    private class NavigationWithParamtersPage : TestBase
	    {
	    }

        /// <summary>
        /// A test class for navigation page attribute with regex in it configurations.
        /// </summary>
        [PageNavigation("/root/[0-9]+")]
        private class NavigationWithRegexAttributePage : TestBase
        {
        }

        /// <summary>
        /// A test class for navigation page attribute configurations.
        /// </summary>
        [PageNavigation("root")]
        private class NavigationWithNoSlashAttributePage : TestBase
        {
        }

        /// <summary>
        /// A test class for navigation page attribute configurations, an absolute path.
        /// </summary>
        [PageNavigation("http://www.atestsite.com/subpath", IsAbsoluteUrl = true)]
        private class AbsoluteUriPage : TestBase
        {
        }
        // ReSharper restore ClassNeverInstantiated.Local
	}
}