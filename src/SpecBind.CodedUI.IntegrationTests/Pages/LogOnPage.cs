// <copyright file="LogOnPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.CodedUI.IntegrationTests.Pages
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

    using SpecBind.Pages;

    /// <summary>
    /// The main home page model
    /// </summary>
    [PageNavigation("/Account/LogOn")]
    public class LogOnPage : HtmlDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogOnPage" /> class by using the provided parent control.
        /// </summary>
        /// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
        public LogOnPage(UITestControl parent) : base(parent)
        {
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [ElementLocator(Id = "Password", Type = "PASSWORD")]
        public HtmlEdit Password { get; set; }
    }
}