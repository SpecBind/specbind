// <copyright file="PageBuilderContext.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
    /// <summary>
    /// A class that holds the current context of variables and arguments used to construct items on the page.
    /// </summary>
    public class PageBuilderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageBuilderContext" /> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="document">The document.</param>
        public PageBuilderContext(ExpressionData browser, ExpressionData parentElement, ExpressionData document)
        {
            this.Browser = browser;
            this.Document = document;
            this.ParentElement = parentElement;
        }

        /// <summary>
        /// Gets the browser expression data.
        /// </summary>
        /// <value>The document expression data.</value>
        public ExpressionData Browser { get; private set; }

        /// <summary>
        /// Gets the document expression data.
        /// </summary>
        /// <value>The document expression data.</value>
        public ExpressionData Document { get; private set; }

        /// <summary>
        /// Gets the root locator expression data.
        /// </summary>
        /// <value>The root locator expression data.</value>
        public ExpressionData RootLocator { get; private set; }

        /// <summary>
        /// Gets the parent element expression data.
        /// </summary>
        /// <value>The parent element expression data.</value>
        public ExpressionData ParentElement { get; private set; }

        /// <summary>
        /// Gets or sets the current property element being built.
        /// </summary>
        /// <value>The current property element.</value>
        public ExpressionData CurrentElement { get; set; }

        /// <summary>
        /// Creates the child context.
        /// </summary>
        /// <param name="childContext">The new child context element.</param>
        /// <returns>The created child context.</returns>
        public PageBuilderContext CreateChildContext(ExpressionData childContext)
        {
            return new PageBuilderContext(this.Browser, this.Document, childContext)
                       {
                           CurrentElement = null,
                           RootLocator = this.RootLocator ?? this.ParentElement
                       };
        }
    }
}