// <copyright file="CodedUIPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI
{
	using System;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Windows.Input;

	using Microsoft.VisualStudio.TestTools.UITesting;
	using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

	using SpecBind.Helpers;
	using SpecBind.Pages;

	/// <summary>
	/// An implementation of a page for the code base.
	/// </summary>
	/// <typeparam name="TDocument">The type of the document.</typeparam>
	public class CodedUIPage<TDocument> : PageBase<TDocument, HtmlControl>
		where TDocument : class
	{
		#region Fields

		private readonly TDocument page;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CodedUIPage{TDocument}"/> class.
		/// </summary>
		/// <param name="page">
		/// The page.
		/// </param>
		public CodedUIPage(TDocument page)
			: base(page.GetType())
		{
			this.page = page;
		}

		#endregion

		/// <summary>
		/// Gets the native page.
		/// </summary>
		/// <typeparam name="TPage">The type of the page.</typeparam>
		/// <returns>The wrapped page object.</returns>
		public override TPage GetNativePage<TPage>()
		{
			return this.page as TPage;
		}

		/// <summary>
		/// Highlights this instance.
		/// </summary>
		public override void Highlight()
		{
			this.GetNativePage<HtmlControl>().DrawHighlight();
		}

		/// <summary>
		/// Elements the enabled check.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns><c>true</c> if the element is enabled; otherwise <c>false</c></returns>
		public override bool ElementEnabledCheck(HtmlControl element)
		{
			return element.Exists && element.Enabled;
		}

		/// <summary>
		/// Elements the exists check.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns><c>true</c> if the element exists; otherwise <c>false</c></returns>
		public override bool ElementExistsCheck(HtmlControl element)
		{
			var exists = element.Exists;
			if (exists)
			{
				return true;
			}

			element.Find();
			return true;
		}

		/// <summary>
		/// Gets the element text.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>The element text.</returns>
		public override string GetElementText(HtmlControl element)
		{
			return element.InnerText;
		}

		/// <summary>
		/// Gets the page from element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>The page interface.</returns>
		public override IPage GetPageFromElement(HtmlControl element)
		{
			return new CodedUIPage<HtmlControl>(element);
		}

		/// <summary>
		/// Clicks the element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns><c>true</c> unless there is an error.</returns>
		public override bool ClickElement(HtmlControl element)
		{
			Point point;
			if (element.TryGetClickablePoint(out point))
			{
				element.EnsureClickable();
			}

			Mouse.Click(element);
			return true;
		}

		/// <summary>
		/// Gets the page fill method.
		/// </summary>
		/// <param name="propertyType">Type of the property.</param>
		/// <returns>The action used to fill the page field.</returns>
		public override Action<HtmlControl, string> GetPageFillMethod(Type propertyType)
		{
			if (propertyType == typeof(HtmlTextArea))
			{
				return (control, s) => ((HtmlTextArea)control).Text = s;
			}

			if (propertyType == typeof(HtmlEdit))
			{
				return (control, s) =>
					{
						var editControl = (HtmlEdit)control;

						if (editControl.IsPassword)
						{
							editControl.Password = Playback.EncryptText(s);
							return;
						}

						if (!string.IsNullOrEmpty(editControl.Text))
						{
							editControl.Text = string.Empty;
						}

						Keyboard.SendKeys(control, s, ModifierKeys.None);
					};
			}

			if (propertyType == typeof(HtmlComboBox))
			{
				return (control, s) =>
					{
						var combo = (HtmlComboBox)control;
						combo.SelectedItem = s;
					};
			}

			if (propertyType == typeof(HtmlRadioButton))
			{
				return (control, s) =>
				{
					var radioButton = (HtmlRadioButton)control;
					bool boolValue;
					if (bool.TryParse(s, out boolValue))
					{
						radioButton.Selected = boolValue;
					}
				};
			}

			if (propertyType == typeof(HtmlCheckBox))
			{
				return (control, s) =>
				{
					var radioButton = (HtmlCheckBox)control;
					bool boolValue;
					if (bool.TryParse(s, out boolValue))
					{
						radioButton.Checked = boolValue;
					}
				};
			}

			if (propertyType == typeof(HtmlFileInput))
			{
				return EnterFileInput;
			}

			// Fallback to get any other controls
			if (propertyType.GetInterfaces().Any(i => i == typeof(IDataControl)))
			{
				// ReSharper disable SuspiciousTypeConversion.Global
				return (control, s) => ((IDataControl)control).SetValue(s);
				// ReSharper restore SuspiciousTypeConversion.Global
			}

			return null;
		}

		/// <summary>
		/// Enters the file input.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <param name="data">The data.</param>
		private static void EnterFileInput(HtmlControl control, string data)
		{
			var locatorName = Path.GetFileNameWithoutExtension(data);
			var fileBytes = ResourceLocator.GetResource(locatorName);
			if (fileBytes == null)
			{
				throw new ElementExecuteException(
					"Could not locate file resource: '{0}'. Registered Resources: '{1}'. Make sure your resource file is public and it is a binary resource.",
					locatorName,
					ResourceLocator.GetResourceNames());
			}

			//Create a temporary path for it.
			var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), data);

			try
			{
				File.WriteAllBytes(path, fileBytes);

				var inputControl = (HtmlFileInput)control;
				inputControl.FileName = path;
			}
			finally
			{
				if (File.Exists(path))
				{
					File.Delete(path);
					
					var parent = Path.GetDirectoryName(path);
					if (parent != null)
					{
						Directory.Delete(parent);
					}
				}
			}
		}
	}
}