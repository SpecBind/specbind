// <copyright file="CodedUIPage.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI
{
	using System;
	using System.CodeDom;
	using System.Drawing;
	using System.Linq;
    using System.Threading;
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
            var milliseconds = (int)timeout.GetValueOrDefault(TimeSpan.FromSeconds(10)).TotalMilliseconds;
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
                case WaitConditions.NotMoving:
                    element.WaitForControlExist(milliseconds);
                    return element.WaitForControlCondition(e => !Moving(e), milliseconds);
            }

            return true;
        }

	    /// <summary>
        /// Checks to see if the element is enabled.
        /// </summary>
		/// <param name="element">The element.</param>
		/// <returns><c>true</c> if the element is enabled; otherwise <c>false</c></returns>
		public override bool ElementEnabledCheck(HtmlControl element)
		{
			return element.Exists && element.Enabled;
		}

		/// <summary>
        /// Checks to see if the element exists.
        /// Waits the appropriate timeout if necessary.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns><c>true</c> if the element exists; otherwise <c>false</c></returns>
        public override bool ElementExistsCheck(HtmlControl element)
        {
            if (element.Exists)
            {
                return true;
            }

            element.Find();
            return true;
        }

        /// <summary>
        /// Checks to see if the element doesn't exist.
        /// Unlike ElementExistsCheck, this doesn't let the web driver wait first for the element to exist.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element doesn't exist; otherwise <c>false</c></returns>
        public override bool ElementNotExistsCheck(HtmlControl element)
        {
            if (element == null)
            {
                return true;
            }

            return !element.Exists;
		}

        /// <summary>
        /// Gets the element attribute value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>The element attribute value.</returns>
	    public override string GetElementAttributeValue(HtmlControl element, string attributeName)
        {
            var value = element.GetProperty(attributeName);
            return value != null ? value.ToString() : string.Empty;
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
		    this.WaitForElement(element, WaitConditions.NotMoving, timeout: null);

			Point point;
			if (element.TryGetClickablePoint(out point))
			{
				element.EnsureClickable();
			}
            
			Mouse.Click(element);
			return true;
		}

        /// <summary>
        /// Gets the clear method for the control.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>
        /// The function used to clear the data.
        /// </returns>
		public override Action<HtmlControl> GetClearMethod(Type propertyType)
		{
			var fillMethod = this.GetPageFillMethod(propertyType);
			return c => fillMethod(c, string.Empty);
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

                        if (string.IsNullOrEmpty(s))
                        {
                            return;
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

        /// <summary>
        /// Determines if an element is currently moving (e.g. due to animation).
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the element's Location is changing, <c>false</c> otherwise.</returns>
        protected virtual bool Moving(UITestControl element)
        {
            var firstLeft = element.Left;
            var firstTop = element.Top;
            Thread.Sleep(200);
            var secondLeft = element.Left;
            var secondTop = element.Top;
            var moved = !(secondLeft.Equals(firstLeft) && secondTop.Equals(firstTop));
            return moved;
        }
	}
}