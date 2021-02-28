// <copyright file="InformationPage.cs" company="">
//     Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium.IntegrationTests.Pages
{
    using OpenQA.Selenium;

    using SpecBind.Pages;

    /// <summary>
    /// The information test page.
    /// </summary>
    [PageNavigation("/Home/NewInfo")]
    public class InformationPage
    {
        /// <summary>
        /// Gets or sets the alert button.
        /// </summary>
        /// <value>The alert button.</value>
        [ElementLocator(Id = "alertButton")]
        public IWebElement AlertBox { get; set; }

        /// <summary>
        /// Gets or sets the confirm button.
        /// </summary>
        /// <value>The confirm button.</value>
        [ElementLocator(Id = "confirmButton")]
        public IWebElement ConfirmBox { get; set; }

        /// <summary>
        /// Gets or sets the prompt button.
        /// </summary>
        /// <value>The prompt button.</value>
        [ElementLocator(Id = "promptButton")]
        public IWebElement PromptExample { get; set; }

        /// <summary>
        /// Gets or sets the result text.
        /// </summary>
        /// <value>The result text.</value>
        [ElementLocator(Id = "resultText")]
        public IWebElement Result { get; set; }
    }
}