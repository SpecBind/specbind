// <copyright file="WatinPage.cs">
// Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
// <summary>
//   The watin page implementation of <see cref="IPage" />.
// </summary>
namespace SpecBind.Watin
{
	using System;
	using System.Threading;

	using SpecBind.Pages;

	using WatiN.Core;
	using WatiN.Core.Exceptions;

	/// <summary>
	/// The watin page implementation of <see cref="IPage"/>.
	/// </summary>
	public class WatinPage : PageBase<Page, Element>
	{
		#region Fields

		private readonly Page page;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="WatinPage"/> class.
		/// </summary>
		/// <param name="page">
		/// The page.
		/// </param>
		public WatinPage(Page page)
			: base(page.GetType())
		{
			this.page = page;
		}

		#endregion

		#region Public Properties

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Gets the native page object.
		/// </summary>
		/// <typeparam name="TPage">The type of the page.</typeparam>
		/// <returns>
		/// The native page object.
		/// </returns>
		public override TPage GetNativePage<TPage>()
		{
			return this.page as TPage;
		}

		/// <summary>
		/// Elements the enabled check.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns><c>true</c> if it is enabled.</returns>
		public override bool ElementEnabledCheck(Element element)
		{
			return element.Exists && element.Enabled;
		}

		/// <summary>
		/// Gets the element exists check function.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>
		/// True if the element exists; otherwise false.
		/// </returns>
		public override bool ElementExistsCheck(Element element)
		{
			return element.Exists;
		}

		/// <summary>
		/// Gets the page from element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns></returns>
		/// <exception cref="System.NotSupportedException">Watin doesn't support list checking currently</exception>
		public override IPage GetPageFromElement(Element element)
		{
			throw new NotSupportedException("Watin doesn't support list checking currently");
		}

		/// <summary>
		/// Gets the click element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>
		/// True if the element exists; otherwise false.
		/// </returns>
		public override bool ClickElement(Element element)
		{
			element.Click();
			return true;
		}

		/// <summary>
		/// Gets the page fill method.
		/// </summary>
		/// <param name="propertyType">Type of the property.</param>
		/// <returns>
		/// The page fill method.
		/// </returns>
		public override Action<Element, string> GetPageFillMethod(Type propertyType)
		{
			if (propertyType == typeof(TextField))
			{
				return (o, s) =>
				{
					o.Focus();
					((TextField)o).TypeText(s);
					o.Blur();
				};
			}

			if (propertyType == typeof(CheckBox))
			{
				return (o, s) =>
				{
					bool boolVal;
					if (bool.TryParse(s, out boolVal))
					{
						((CheckBox)o).Checked = boolVal;
					}
				};
			}

			if (propertyType == typeof(SelectList))
			{
				return (o, s) =>
				{
					var selectList = (SelectList)o;
					selectList.Focus();
					try
					{
						selectList.Select(s);
					}
					catch (SelectListItemNotFoundException)
					{
						selectList.SelectByValue(s);
					}

					selectList.DomContainer.Eval(string.Format("$({0}).change();", selectList.GetJavascriptElementReference()));
					Thread.Sleep(TimeSpan.FromMilliseconds(500));
					selectList.Blur();
				};
			}

			if (propertyType == typeof(RadioButton))
			{
				return (o, s) =>
				{
					bool boolVal;
					if (bool.TryParse(s, out boolVal))
					{
						((RadioButton)o).Checked = boolVal;
					}
				};
			}

			return null;
		}

		/// <summary>
		/// Gets the element text.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>The element text.</returns>
		public override string GetElementText(Element element)
		{
			return element.Text;
		}

		#endregion
	}
}