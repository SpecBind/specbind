// <copyright file="CodedUIPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI
{
	using System;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Input;

	using Microsoft.VisualStudio.TestTools.UITest.Extension;
	using Microsoft.VisualStudio.TestTools.UITesting;
	using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

	using SpecBind.Actions;
	using SpecBind.Helpers;
	using SpecBind.Pages;

	/// <summary>
	/// An implementation of a page for the code base.
	/// </summary>
	/// <typeparam name="TDocument">The type of the document.</typeparam>
	// ReSharper disable once InconsistentNaming
	public class CodedUIPage<TDocument> : PageBase<TDocument, HtmlControl>
		where TDocument : class
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CodedUIPage{TDocument}"/> class.
		/// </summary>
		/// <param name="page">
		/// The page.
		/// </param>
		public CodedUIPage(TDocument page)
			: base(page.GetType(), page)
		{
		}

		#endregion

		/// <summary>
		/// Highlights this instance.
		/// </summary>
		public override void Highlight()
		{
			this.GetNativePage<HtmlControl>().DrawHighlight();
		}

        /// <summary>
        /// Highlights the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
	    public override void Highlight(HtmlControl element)
	    {
	        element.DrawHighlight();
	    }

        /// <summary>
        /// Waits for element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="waitCondition">The wait condition.</param>
        /// <param name="timeout">The timeout to wait before failing.</param>
        /// <returns><c>true</c> if the condition is met, <c>false</c> otherwise.</returns>
	    public override bool WaitForElement(HtmlControl element, WaitConditions waitCondition, TimeSpan? timeout)
        {
            var milliseconds = (int)timeout.GetValueOrDefault(TimeSpan.FromMilliseconds(500)).TotalMilliseconds;
            switch (waitCondition)
            {
                case WaitConditions.Exists:
                    return element.WaitForControlExist(milliseconds);
                case WaitConditions.NotExists:
                    return element.WaitForControlNotExist(milliseconds);
                case WaitConditions.Enabled:
                    return element.WaitForControlCondition(e => e.Enabled, milliseconds);
                case WaitConditions.NotEnabled:
                    return element.WaitForControlCondition(e => !e.Enabled, milliseconds);
            }

            return true;
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
		    var comboBoxElement = element as HtmlComboBox;
		    if (comboBoxElement != null)
		    {
		        return comboBoxElement.SelectedItem;
		    }

		    var checkBoxElement = element as HtmlCheckBox;
            if (checkBoxElement != null)
            {
                return checkBoxElement.Checked ? "true" : "false";
            }

		    var inputElement = element as HtmlEdit;
            if (inputElement != null)
            {
                return inputElement.Text;
            }
            
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
			// Respect the data control interface first.
			if (propertyType.GetInterfaces().Any(i => i == typeof(IDataControl)))
			{
				// ReSharper disable SuspiciousTypeConversion.Global
				return (control, s) => ((IDataControl)control).SetValue(s);
				// ReSharper restore SuspiciousTypeConversion.Global
			}

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

						try
						{
							Keyboard.SendKeys(control, s, ModifierKeys.None);
						}
						catch (PlaybackFailureException ex)
						{
							if (ex.Message.Contains("SendKeys"))
							{
								// Fallback strategy of setting the text directly if sendkeys doesn't work
								editControl.Text = s;
							}
							else
							{
								throw;
							}
						}
					};
			}

			if (propertyType == typeof(HtmlComboBox))
			{
				return (control, s) =>
					{
						var combo = (HtmlComboBox)control;
                        combo.SetFocus();
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
				return (control, s) =>
				    {
				        var inputControl = (HtmlFileInput)control;
				        FileUploadHelper.UploadFile(s, path => inputControl.FileName = path);
				    };
			}

            if (propertyType == typeof(HtmlCustom))
            {
                return (control, s) => Keyboard.SendKeys(control, s, ModifierKeys.None);
            }

			return null;
		}
	}
}