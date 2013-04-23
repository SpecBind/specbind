// <copyright file="CodedUIListElementWrapper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI
{
	using System;
	using System.Globalization;

	using Microsoft.VisualStudio.TestTools.UITesting;
	using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

	using SpecBind.Pages;

	/// <summary>
	/// An implementation of <see cref="ListElementWrapper{TElement,TChildElement}"/> for Coded UI.
	/// </summary>
	/// <typeparam name="TElement">The type of the element.</typeparam>
	/// <typeparam name="TChildElement">The type of the child element.</typeparam>
	public class CodedUIListElementWrapper<TElement, TChildElement> : ListElementWrapper<TElement, TChildElement>
		where TElement : UITestControl
		where TChildElement : HtmlControl
	{
		private readonly Func<TElement, Action<TChildElement>, TChildElement> builderFunc;

		/// <summary>
		/// Initializes a new instance of the <see cref="CodedUIListElementWrapper{TElement, TChildElement}" /> class.
		/// </summary>
		/// <param name="parentElement">The parent element.</param>
		public CodedUIListElementWrapper(TElement parentElement)
			: base(parentElement)
		{
			this.builderFunc = PageBuilder.CreateElement<TElement, TChildElement>(typeof(TChildElement));
			this.ValidateElementExists = true;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to validate the element exists.
		/// </summary>
		/// <value>
		/// <c>true</c> if the class should validate element exists; otherwise, <c>false</c>.
		/// </value>
		public bool ValidateElementExists { get; set; }

		/// <summary>
		/// Creates the element.
		/// </summary>
		/// <param name="parentElement">The parent element.</param>
		/// <param name="index">The index.</param>
		/// <returns>The newly created element.</returns>
		protected override TChildElement CreateElement(TElement parentElement, int index)
		{
			return this.builderFunc(this.Parent,
				e => e.FilterProperties[HtmlControl.PropertyNames.TagInstance] = index.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Elements the exists.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns><c>true</c> if the element exists.</returns>
		protected override bool ElementExists(TChildElement element)
		{
			return !this.ValidateElementExists || element.Exists;
		}
	}
}