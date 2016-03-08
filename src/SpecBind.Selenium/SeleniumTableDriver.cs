// <copyright file="SeleniumTableDriver.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using OpenQA.Selenium;

    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A class that abstracts the functionality of a table for UI tests.
    /// </summary>
    public class SeleniumTableDriver : SeleniumListElementWrapper<IWebElement, SeleniumTableDriver.RowWrapper>
    {
        private readonly Dictionary<int, string> cellLookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumTableDriver"/> class.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="browser">The browser.</param>
        public SeleniumTableDriver(IWebElement parentElement, IBrowser browser)
            : base(parentElement, browser)
        {
            this.cellLookup = new Dictionary<int, string>();
        }

        /// <summary>
        /// Builds the item collection.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <returns>The created item collection.</returns>
        protected override ReadOnlyCollection<IWebElement> BuildItemCollection(IWebElement parentElement)
        {
            var list = base.BuildItemCollection(parentElement);

            if (list == null || list.Count == 0)
            {
                return new ReadOnlyCollection<IWebElement>(new List<IWebElement>(0));
            }

            // Check first row for headers
            var headerCells = list.First().FindElements(By.TagName("th"));
            if (headerCells != null && headerCells.Count > 0)
            {
                for (var i = 0; i < headerCells.Count; i++)
                {
                    var cell = headerCells[i];
                    var headerName = cell.Text;
                    if (!string.IsNullOrWhiteSpace(headerName))
                    {
                        this.cellLookup.Add(i, headerName.ToLookupKey());
                    }
                }

                return list.Count > 1
                           ? list.Skip(1).ToList().AsReadOnly()
                           : new ReadOnlyCollection<IWebElement>(new List<IWebElement>(0));
            }

            return list;
        }

        /// <summary>
        /// Creates the child element.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="element">The element.</param>
        /// <returns>The created child element.</returns>
        protected override RowWrapper CreateChildElement(IBrowser browser, IWebElement parentElement, IWebElement element)
        {
            var rowWrapper = new RowWrapper(parentElement, this.cellLookup);
            rowWrapper.CloneNativeElement(element);

            return rowWrapper;
        }

        /// <summary>
        /// Gets the element locator.
        /// </summary>
        /// <returns>The locator to get the rows.</returns>
        protected override By GetElementLocator()
        {
            return By.TagName("tr");
        }

        /// <summary>
        /// A wrapper class for an individual row.
        /// </summary>
        public class RowWrapper : WebElement, IElementProvider
        {
            private readonly Dictionary<int, string> cellLookup;

            /// <summary>
            /// Initializes a new instance of the <see cref="WebElement" /> class.
            /// </summary>
            /// <param name="searchContext">The driver used to search for elements.</param>
            /// <param name="cellLookup">The cell lookup.</param>
            public RowWrapper(ISearchContext searchContext, Dictionary<int, string> cellLookup)
                : base(searchContext)
            {
                this.cellLookup = cellLookup;
            }

            /// <summary>
            /// Gets the properties.
            /// </summary>
            /// <returns>The properties list for the class.</returns>
            public IEnumerable<ElementDescription> GetElements()
            {
                var cells = (IList<IWebElement>)this.FindElements(By.TagName("td")) ?? new List<IWebElement>();

                for (var i = 0; i < cells.Count; i++)
                {
                    string cellName;
                    if (this.cellLookup.TryGetValue(i, out cellName))
                    {
                        var localCell = cells[i];
                        yield return new ElementDescription(cellName, typeof(IWebElement), localCell);
                    }
                }
            }
        }
    }
}