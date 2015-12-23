namespace SpecBind.CodedUI.Tests
{
	using System;

	using Microsoft.QualityTools.Testing.Fakes;
	using Microsoft.VisualStudio.TestTools.UITesting;
	using Microsoft.VisualStudio.TestTools.UITesting.Fakes;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using SpecBind.Actions;

	/// <summary>
	/// A test fixture for the CodedUIBrowser class
	/// </summary>
	[TestClass]
	public class CodedUIBrowserFixture
	{
		[TestMethod]
		public void TestClearCookies()
		{
			var logger = new Mock<ILogger>(MockBehavior.Loose);
			var browserWindow = new Mock<BrowserWindow>(MockBehavior.Strict);
			var browser = new CodedUIBrowser(new Lazy<BrowserWindow>(() => browserWindow.Object), logger.Object);

			using (ShimsContext.Create())
			{
				var clearCookiesCalled = false;
				var clearCacheCalled = false;

				ShimBrowserWindow.ClearCookies = () => clearCookiesCalled = true;
				ShimBrowserWindow.ClearCache = () => clearCacheCalled = true;

				browser.ClearCookies();

				Assert.IsTrue(clearCookiesCalled);
				Assert.IsTrue(clearCacheCalled);
			}
		}

		[TestMethod]
		public void TestCloseWhenDisposeIsTrue()
		{
			var logger = new Mock<ILogger>(MockBehavior.Loose);
			var browserWindow = new Mock<BrowserWindow>(MockBehavior.Strict);
			var browser = new Mock<CodedUIBrowser>(new Lazy<BrowserWindow>(() => browserWindow.Object), logger.Object){CallBase = true};

			browser.Object.Close(true);

			browser.Verify(b => b.Dispose());
		}

		[TestMethod]
		public void TestCloseWhenDisposeIsFalse()
		{
			var logger = new Mock<ILogger>(MockBehavior.Loose);
			var browserWindow = new Mock<BrowserWindow>(MockBehavior.Strict);
			var browser = new Mock<CodedUIBrowser>(new Lazy<BrowserWindow>(() => browserWindow.Object), logger.Object) { CallBase = true };

			browser.Object.Close(false);

			browser.Verify(b => b.Dispose(), Times.Never());
		}
	}
}
