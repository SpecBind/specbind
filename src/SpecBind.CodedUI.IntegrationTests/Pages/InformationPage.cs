// <copyright file="InformationPage.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI.IntegrationTests.Pages
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

    using SpecBind.Pages;

    /// <summary>
    /// The information test page.
    /// </summary>
    [PageNavigation("/Home/NewInfo")]
    public class InformationPage : HtmlDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InformationPage" /> class by using the provided parent control.
        /// </summary>
        /// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
        public InformationPage(UITestControl parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Gets or sets the alert button.
        /// </summary>
        /// <value>The alert button.</value>
        [ElementLocator(Id = "alertButton")]
        public HtmlButton AlertBox { get; set; }

        /// <summary>
        /// Gets or sets the confirm button.
        /// </summary>
        /// <value>The confirm button.</value>
        [ElementLocator(Id = "confirmButton")]
        public HtmlButton ConfirmBox { get; set; }

        /// <summary>
        /// Gets or sets the prompt button.
        /// </summary>
        /// <value>The prompt button.</value>
        [ElementLocator(Id = "promptButton")]
        public HtmlButton PromptExample { get; set; }

        /// <summary>
        /// Gets or sets the result text.
        /// </summary>
        /// <value>The result text.</value>
        [ElementLocator(Id = "resultText")]
        public HtmlDiv Result { get; set; }
    }
}