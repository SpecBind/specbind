// <copyright file="TestBase.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Tests.Support
{
	using System;

	using SpecBind.Pages;

	/// <summary>
	/// A test base class.
	/// </summary>
	public class TestBase : PageBase<BasePageClass, BaseElement>
	{
		private readonly InheritedClass item;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestBase" /> class.
		/// </summary>
		public TestBase()
			: base(typeof(InheritedClass))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TestBase" /> class.
		/// </summary>
		/// <param name="item">The item.</param>
		public TestBase(InheritedClass item)
			: this()
		{
			this.item = item;
		}

		/// <summary>
		/// Gets the native page.
		/// </summary>
		/// <typeparam name="TPage">The type of the page.</typeparam>
		/// <returns>The native page object.</returns>
		public override TPage GetNativePage<TPage>()
		{
			return this.item as TPage;
		}

		/// <summary>
		/// Elements the enabled check.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>Success of the call.</returns>
		public override bool ElementEnabledCheck(BaseElement element)
		{
			return true;
		}

		/// <summary>
		/// Elements the exists check.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>Success of the call.</returns>
		public override bool ElementExistsCheck(BaseElement element)
		{
			return true;
		}

		/// <summary>
		/// Gets the element text.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>Success of the call.</returns>
		public override string GetElementText(BaseElement element)
		{
			return null;
		}

		/// <summary>
		/// Gets the page from element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>The page interface.</returns>
		public override IPage GetPageFromElement(BaseElement element)
		{
			return null;
		}

		/// <summary>
		/// Clicks the element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>Success of the call.</returns>
		public override bool ClickElement(BaseElement element)
		{
			return true;
		}

		/// <summary>
		/// Gets the page fill method.
		/// </summary>
		/// <param name="propertyType">Type of the property.</param>
		/// <returns>Success of the call.</returns>
		public override Action<BaseElement, string> GetPageFillMethod(Type propertyType)
		{
			return null;
		}
	}
}