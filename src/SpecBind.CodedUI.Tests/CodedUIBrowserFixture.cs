// <copyright file="CodedUIBrowserFixture.cs">
//    Copyright © 2016 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI.Tests
{
	using System;

	using Microsoft.VisualStudio.TestTools.UITesting;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using SpecBind.Actions;

	/// <summary>
	/// A test fixture for the CodedUIBrowser class
	/// </summary>
	[TestClass]
	public class CodedUIBrowserFixture
	{
        /// <summary>
        /// Tests the close method when dispose is true.
        /// </summary>
		[TestMethod]
		public void TestCloseWhenDisposeIsTrue()
		{
			var logger = new Mock<ILogger>(MockBehavior.Loose);
			var browserWindow = new Mock<BrowserWindow>(MockBehavior.Strict);
			var browser = new Mock<CodedUIBrowser>(new Lazy<BrowserWindow>(() => browserWindow.Object), logger.Object) { CallBase = true };

			browser.Object.Close(true);

			browser.Verify(b => b.Dispose());
		}

        /// <summary>
        /// Tests the close method when dispose is false.
        /// </summary>
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
