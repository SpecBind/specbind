// <copyright file="IElementList.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
	using System.Collections.Generic;

	/// <summary>
	/// An interface that represents a list of child elements.
	/// </summary>
	/// <typeparam name="TElement">The type of the parent element.</typeparam>
	/// <typeparam name="TChildElement">The type of the child element.</typeparam>
	public interface IElementList<out TElement, out TChildElement> : IEnumerable<TChildElement>
	{
		/// <summary>
		/// Gets the parent element.
		/// </summary>
		/// <value>
		/// The parent element.
		/// </value>
		TElement Parent { get; }
	}
}