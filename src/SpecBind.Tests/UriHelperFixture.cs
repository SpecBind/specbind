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

			var url = UriHelper.GetQualifiedPageUri(browser.Object, typeof(NavigationAttributePage));

			Assert.AreEqual(url, new Uri("http://localhost:2222/root"));

			browser.VerifyAll();
		}

        /// <summary>
        ///     Tests the get page URI method with a page type with no slash.
        /// </summary>
        [TestMethod]
        public void TestGetQualifiedPageUriFromPageTypeWithNoSlash()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

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

            var regex = UriHelper.GetQualifiedPageUriRegex(browser.Object, typeof(NavigationAttributePage));

            Assert.AreEqual(true, regex.IsMatch("http://localhost:2222/root"));
            Assert.AreEqual(true, regex.IsMatch("http://localhost:2222/root/something123"));
            
            browser.VerifyAll();
        }

        /// <summary>
        ///     Tests the get page URI method.
        /// </summary>
        [TestMethod]
        public void TestGetQualifiedPageUriRegexAttributeDoesNotContainASlash()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var regex = UriHelper.GetQualifiedPageUriRegex(browser.Object, typeof(NavigationWithNoSlashAttributePage));

            Assert.AreEqual(true, regex.IsMatch("http://localhost:2222/root"));
            Assert.AreEqual(true, regex.IsMatch("http://localhost:2222/root/something123"));
            
            browser.VerifyAll();
        }

        /// <summary>
        ///     Tests the get page URI method.
        /// </summary>
        [TestMethod]
        public void TestGetQualifiedPageUriRegexWithRegexInNavigation()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var regex = UriHelper.GetQualifiedPageUriRegex(browser.Object, typeof(NavigationWithRegexAttributePage));

            Assert.AreEqual(true, regex.IsMatch("http://localhost:2222/root/223"));
            
            browser.VerifyAll();
        }

        /// <summary>
		///     Tests the get Navigate To method.
		/// </summary>
		[TestMethod]
		public void TestNavigateTo()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(b => b.GoTo(It.Is<Uri>(u => u.ToString() == "http://localhost:2222/subpath/1")));

			browser.Object.NavigateTo("subpath/1");
			
			browser.VerifyAll();
		}

		/// <summary>
		///     Tests the get Navigate To method with a given type.
		/// </summary>
		[TestMethod]
		public void TestNavigateToWithType()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(b => b.GoTo(It.Is<Uri>(u => u.ToString() == "http://localhost:2222/root")));

			var path = browser.Object.NavigateTo<NavigationAttributePage>();

			Assert.AreEqual("http://localhost:2222/root", path);

			browser.VerifyAll();
		}

		/// <summary>
		///     Tests the get Navigate To method with a given type in non-generic form.
		/// </summary>
		[TestMethod]
		public void TestNavigateToWithNonGenericType()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);
			browser.Setup(b => b.GoTo(It.Is<Uri>(u => u.ToString() == "http://localhost:2222/root")));

			var path = browser.Object.NavigateTo(typeof(NavigationAttributePage));

			Assert.AreEqual("http://localhost:2222/root", path);

			browser.VerifyAll();
		}

		/// <summary>
		///     Tests the get page URI method.
		/// </summary>
		[TestMethod]
		public void TestGetPageUriFromTypeWithNavigationAttribute()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			var url = UriHelper.GetPageUri(browser.Object, typeof(NavigationAttributePage));

			Assert.AreEqual(url, "/root");

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

			var url = UriHelper.FillPageUri(
				browser.Object, typeof(NavigationAttributePage), new Dictionary<string, string> { { "Id", "1" } });

			Assert.AreEqual("/root/1", url);

			browser.VerifyAll();
		}

		/// <summary>
		///     Tests the get page URI method with a null dictionary.
		/// </summary>
		[TestMethod]
		public void TestFillPageUriNullDictionary()
		{
			var browser = new Mock<IBrowser>(MockBehavior.Strict);

			var url = UriHelper.FillPageUri(
				browser.Object, typeof(NavigationAttributePage), null);

			Assert.AreEqual("/root/{id}", url);

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

			var url = UriHelper.FillPageUri(
				browser.Object, typeof(InvalidPage), null);

			Assert.AreEqual("/testpage", url);

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
        // ReSharper restore ClassNeverInstantiated.Local
	}
}