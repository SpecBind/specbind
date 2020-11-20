// <copyright file="PageExtensionsFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.BrowserSupport;
    using SpecBind.Pages;

    /// <summary>
    /// A unit test fixture for the <see cref="BrowserExtensions"/> class.
    /// </summary>
    [TestClass]
    public class PageExtensionsFixture
    {
        /// <summary>
        /// Tests the ensure on page method to ensure the page exists.
        /// </summary>
        [TestMethod]
        public void TestEnsureOnPage()
        {
            var page = new Mock<IPage>(MockBehavior.Strict);

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Page(typeof(MyPage))).Returns(page.Object);
            browser.Setup(b => b.EnsureOnPage(page.Object));

            browser.Object.EnsureOnPage<MyPage>();

            browser.VerifyAll();
            page.VerifyAll();
        }

        /// <summary>
        /// Tests the GoToPage method to ensure the page exists.
        /// </summary>
        [TestMethod]
        public void TestGoToPage()
        {
            var page = new Mock<IPage>(MockBehavior.Strict);

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.GoToPage(typeof(MyPage), null)).Returns(page.Object);

            browser.Object.GoToPage<MyPage>();

            browser.VerifyAll();
            page.VerifyAll();
        }

        #region Class - MyPage

        /// <summary>
        /// A non-named page class
        /// </summary>
        [PageNavigation("/mypage")]
        public class MyPage
        {
        }

        #endregion
    }
}