// <copyright file="ListElementWrapper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// A wrapper class for lists of elements.
	/// </summary>
	/// <typeparam name="TElement">The type of the parent element.</typeparam>
	/// <typeparam name="TChildElement">The type of the child element.</typeparam>
	public abstract class ListElementWrapper<TElement, TChildElement> : IElementList<TElement, TChildElement>
		where TElement : class where TChildElement : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListElementWrapper{T, TChildElement}" /> class.
		/// </summary>
		/// <param name="parentElement">The parent element.</param>
		protected ListElementWrapper(TElement parentElement)
		{
			this.Parent = parentElement;
		}

		#region IEnumerator<TChildItem> Implementation

		/// <summary>
		/// Gets the parent element.
		/// </summary>
		/// <value>
		/// The parent element.
		/// </value>
		public TElement Parent { get; private set; }

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator for the list.</returns>
		public IEnumerator<TChildElement> GetEnumerator()
		{
			return new ElementEnumerator(this);
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Creates the element.
		/// </summary>
		/// <param name="parentElement">The parent element.</param>
		/// <param name="index">The index.</param>
		/// <returns>
		/// The child element.
		/// </returns>
		protected abstract TChildElement CreateElement(TElement parentElement, int index);

		/// <summary>
		/// Checks to see if the element actually exists according to the DOM.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns><c>true</c> if the element exists; otherwise <c>false</c>.</returns>
		protected abstract bool ElementExists(TChildElement element);

		/// <summary>
		/// Tries the get child element.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="childElement">The child element.</param>
		/// <returns><c>true</c> if the element exists; otherwise <c>false</c>.</returns>
		private bool TryGetChildElement(int index, out TChildElement childElement)
		{
			childElement = default(TChildElement);
			var element = this.CreateElement(this.Parent, index);
			if (!Equals(element, default(TChildElement)) && this.ElementExists(element))
			{
				childElement = element;
				return true;
			}

			return false;
		}

		#endregion

		#region Enumerator Implementation

		/// <summary>
		/// The enumerator for the element.
		/// </summary>
		private class ElementEnumerator : IEnumerator<TChildElement>
		{
			private readonly ListElementWrapper<TElement, TChildElement> parent;
			private int index;

			/// <summary>
			/// Initializes a new instance of the <see cref="ElementEnumerator" /> class.
			/// </summary>
			/// <param name="parent">The child element builder.</param>
			public ElementEnumerator(ListElementWrapper<TElement, TChildElement> parent)
			{
				this.parent = parent;
			}

			/// <summary>
			/// Gets the element in the collection at the current position of the enumerator.
			/// </summary>
			/// <returns>
			/// The element in the collection at the current position of the enumerator.
			/// </returns>
			public TChildElement Current { get; private set; }

			/// <summary>
			/// Gets the element in the collection at the current position of the enumerator.
			/// </summary>
			/// <returns>
			/// The element in the collection at the current position of the enumerator.
			/// </returns>
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public void Dispose()
			{
			}

			/// <summary>
			/// Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns>
			/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
			/// </returns>
			public bool MoveNext()
			{
				this.index++;

				TChildElement childElement;
				if (this.parent.TryGetChildElement(this.index, out childElement))
				{
					this.Current = childElement;
					return true;
				}

				this.Current = default(TChildElement);
				return false;
			}

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first element in the collection.
			/// </summary>
			public void Reset()
			{
				this.index = 0;
				this.Current = default(TChildElement);
			}
		}

		#endregion
	}
}