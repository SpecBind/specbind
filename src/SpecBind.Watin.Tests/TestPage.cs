// <copyright file="TestPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Watin.Tests
{
	using System.Collections.Generic;

	using SpecBind.Pages;

	using WatiN.Core;

	/// <summary>
	///     The test page class.
	/// </summary>
	[PageNavigation("/testpage")]
	public class TestPage : Page
	{
		/// <summary>
		/// Returns true if the current document represents this page (has the correct Url, etc.).
		/// The actual check(s) is done by the protected method <see cref="M:WatiN.Core.Page.VerifyDocumentProperties(WatiN.Core.Document,WatiN.Core.Page.ErrorReporter)" />
		/// </summary>
		public override bool IsCurrentDocument
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		///     Gets or sets the display text.
		/// </summary>
		/// <value>
		///     The display text.
		/// </value>
		public string DisplayText { get; set; }

		/// <summary>
		///     Gets or sets the display area.
		/// </summary>
		/// <value>
		///     The display area.
		/// </value>
		public Div DisplayArea { get; set; }

		/// <summary>
		///     Gets or sets the error list.
		/// </summary>
		/// <value>
		///     The error list.
		/// </value>
		public List<string> ErrorList { get; set; }

		/// <summary>
		///     Gets or sets the name.
		/// </summary>
		/// <value>
		///     The name.
		/// </value>
		public TextField Name { get; set; }

		/// <summary>
		///     Gets or sets the is person.
		/// </summary>
		/// <value>
		///     The is person.
		/// </value>
		public CheckBox IsPerson { get; set; }

		/// <summary>
		///     Gets or sets the option1.
		/// </summary>
		/// <value>
		///     The option1.
		/// </value>
		public RadioButton Option1 { get; set; }

		/// <summary>
		///     Gets or sets the items.
		/// </summary>
		/// <value>
		///     The items.
		/// </value>
		public SelectList Items { get; set; }
	}
}