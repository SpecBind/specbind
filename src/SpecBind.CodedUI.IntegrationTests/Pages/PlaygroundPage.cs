// <copyright file="PlaygroundPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.CodedUI.IntegrationTests.Pages
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

    using SpecBind.Pages;

    /// <summary>
    /// A test page for debugging scenarios
    /// </summary>
    [PageNavigation("/home/landing")]
    [PageAlias("EchoSearch")]
    public class PlaygroundPage : HtmlDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlDocument" /> class by using the provided parent control.
        /// </summary>
        /// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
        public PlaygroundPage(UITestControl parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        [ElementLocator(Id = "country")]
        public HtmlComboBox Country { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        [ElementLocator(Id = "end-date-facade")]
        public HtmlEdit EndDate { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        [ElementLocator(Id = "start-date-facade")]
        public HtmlEdit StartDate { get; set; }
    }
}