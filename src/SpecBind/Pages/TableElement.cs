// <copyright file="TableElement.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// An element on the page that represents a table
    /// </summary>
    /// <typeparam name="TCell">The type of the element that represents a cell.</typeparam>
    public class TableElement<TCell> : IEnumerable<TCell>
    {
        private IEnumerable<TCell> enumerator;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<TCell> GetEnumerator()
        {
            return this.enumerator != null ? this.enumerator.GetEnumerator() : new List<TCell>(0).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Sets the driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        internal void SetDriver(IEnumerable<TCell> driver)
        {
            this.enumerator = driver;
        }
    }
}