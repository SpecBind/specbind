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
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<TCell> GetEnumerator()
        {
            return null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}