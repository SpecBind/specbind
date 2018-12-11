// <copyright file="SeleniumPhantomJSDriver.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using Configuration;
    using OpenQA.Selenium;
    using OpenQA.Selenium.PhantomJS;
    using System.IO;

    /// <summary>
    /// Selenium PhantomJS Driver.
    /// </summary>
    /// <seealso cref="SpecBind.Selenium.Drivers.SeleniumDriverBase" />
    internal class SeleniumPhantomJSDriver : SeleniumDriverBase
    {
        private const string PhantomjsExe = "phantomjs.exe";

        /// <summary>
        /// Creates the web driver from the specified browser factory configuration.
        /// </summary>
        /// <returns>The configured web driver.</returns>
        protected override IWebDriver CreateLocalDriver(BrowserFactoryConfiguration browserFactoryConfiguration)
        {
            var phantomJsDriverService = PhantomJSDriverService.CreateDefaultService();
            phantomJsDriverService.HideCommandPromptWindow = true;
            return new PhantomJSDriver(phantomJsDriverService);
        }

        /// <summary>
        /// Downloads the driver to the specified path.
        /// </summary>
        /// <param name="driverPath">The driver path.</param>
        protected override void Download(string driverPath)
        {
            const string FileName = "phantomjs-2.0.0-windows.zip";

            DownloadAndExtractZip("https://bitbucket.org/ariya/phantomjs/downloads", driverPath, FileName);

            // Move the phantomjs.exe out of the unzipped folder
            var unzippedFolder = Path.Combine(driverPath, Path.GetFileNameWithoutExtension(FileName), "bin");
            File.Move(Path.Combine(unzippedFolder, PhantomjsExe), Path.Combine(driverPath, PhantomjsExe));
            Directory.Delete(unzippedFolder, true);
        }

        /// <summary>
        /// Creates the driver options.
        /// </summary>
        /// <returns>The driver options.</returns>
        protected override DriverOptions CreateRemoteDriverOptions(BrowserFactoryConfiguration browserFactoryConfiguration)
        {
            return new PhantomJSOptions();
        }
    }
}
