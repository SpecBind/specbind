// <copyright file="MockBrowserFactory.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Tests.Support
{
    using System;
    using Moq;

    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Configuration;

    /// <summary>
    /// A Mock Browser Factory used for testing.
    /// </summary>
    public class MockBrowserFactory : BrowserFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockBrowserFactory"/> class.
        /// </summary>
        public MockBrowserFactory()
            : base(new BrowserFactoryConfiguration())
        {
        }

        /// <summary>
        /// Gets the browser.
        /// </summary>
        /// <value>The browser.</value>
        public Mock<IBrowser> BrowserMock { get; private set; }

        /// <summary>
        /// Resets the driver.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public override void ResetDriver(IBrowser browser)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the browser.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>A browser object.</returns>
        protected override IBrowser CreateBrowser(ILogger logger)
        {
            this.BrowserMock = new Mock<IBrowser>();
            return this.BrowserMock.Object;
        }
    }
}