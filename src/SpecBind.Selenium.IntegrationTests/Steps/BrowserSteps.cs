// <copyright file="SeleniumBrowserFixture.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium.IntegrationTests.Steps
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;

    using SpecBind.BrowserSupport;

    using System.Collections.Generic;

    using TechTalk.SpecFlow;

    [Binding]
    public sealed class BrowserSteps
    {
        private readonly IBrowser browser;

        public BrowserSteps(IBrowser browser)
        {
            this.browser = browser;
        }

        [Then("I am at the url \"(.*)\"")]
        public void ThenIAmAtTheUrl(string url)
        {
            Assert.AreEqual(url, this.browser.Url);
        }

        [Then("I can get the browser logs")]
        public void ThenICanGetTheBrowserLogs()
        {
            SeleniumBrowser seleniumBrowser = this.browser as SeleniumBrowser;
            Assert.IsNotNull(seleniumBrowser);

            IReadOnlyCollection<LogEntry> collection = seleniumBrowser.GetBrowserLogs();

            Assert.IsNotNull(collection);
        }
    }
}
