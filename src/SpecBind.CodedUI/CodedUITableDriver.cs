// <copyright file="CodedUITableDriver.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A table driver for Coded UI cells.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class CodedUITableDriver : CodedUIListElementWrapper<HtmlTable, CodedUITableDriver.RowWrapper>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodedUITableDriver"/> class.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="webBrowser">The web browser.</param>
        public CodedUITableDriver(HtmlTable parentElement, IBrowser webBrowser)
            : base(parentElement, webBrowser)
        {
        }

        /// <summary>
        /// Elements the exists.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="expectedIndex">The expected index.</param>
        /// <returns><c>true</c> if the element exists.</returns>
        protected override bool ElementExists(RowWrapper element, int expectedIndex)
        {
            return !this.ValidateElementExists || element.Exists;
        }

        /// <summary>
        /// Fetches the element list.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="browser">The browser.</param>
        /// <returns>The created list of items.</returns>
        protected override List<RowWrapper> FetchElementList(HtmlTable parentElement, IBrowser browser)
        {
            var rows = parentElement.Rows;
            if (rows != null && rows.Count > 0)
            {
                return rows.Cast<HtmlRow>()
                           .Where(c => c.ControlType != ControlType.RowHeader && 
                                      (c.Cells != null && !c.Cells.Any(f => f is HtmlHeaderCell)))
                           .Select(r =>
                                    {
                                        var control = new RowWrapper(parentElement);
                                        control.CopyFrom(r);
                                        return control;
                                    })
                            .OrderBy(r => r.RowIndex)
                            .ToList();
            }

            return new List<RowWrapper>(0);
        }

        /// <summary>
        /// A wrapper class for a row to return needed cells.
        /// </summary>
        public class RowWrapper : HtmlRow, IElementProvider
        {
            private readonly HtmlTable parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="HtmlRow" /> class by using the provided parent control.
            /// </summary>
            /// <param name="parent">The <see cref="UITestControl" /> that contains this control.</param>
            public RowWrapper(UITestControl parent)
                : base(parent)
            {
                this.parent = parent as HtmlTable;
            }

            /// <summary>
            /// Gets the properties.
            /// </summary>
            /// <returns>The properties list for the class.</returns>
            public IEnumerable<ElementDescription> GetElements()
            {
                var cellLookup = GetCellLookup();

                foreach (var cell in this.Cells.Cast<HtmlCell>())
                {
                    string cellName;
                    if (cellLookup.TryGetValue(cell.ColumnIndex, out cellName))
                    {
                        var localCell = cell;
                        yield return new ElementDescription(cellName, typeof(HtmlCell), localCell);
                    }
                }
            }

            /// <summary>
            /// Gets the cell lookup.
            /// </summary>
            /// <returns>The lookup dictionary to map indexes to cells.</returns>
            private Dictionary<int, string> GetCellLookup()
            {
                var cellLookup = new Dictionary<int, string>();

                var rows = this.parent.Rows;
                if (rows != null && rows.Count > 0)
                {
                    var htmlControl = rows[0] as HtmlControl;
                    if (htmlControl != null)
                    {
                        foreach (var child in htmlControl.GetChildren().OfType<HtmlHeaderCell>().Where(c => c.InnerText != null))
                        {
                            cellLookup.Add(child.ColumnIndex, child.InnerText.ToLookupKey());
                        }
                    }
                }

                return cellLookup;
            }
        }
    }
}