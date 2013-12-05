// <copyright file="PropertyData.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using SpecBind.Actions;

    /// <summary>
	/// The property data for a given property.
	/// </summary>
	/// <typeparam name="TElement">The propertyValue of the element.</typeparam>
	internal class PropertyData<TElement> : IPropertyData
	{
		private readonly IPageElementHandler<TElement> elementHandler;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyData{TElement}" /> class.
		/// </summary>
		/// <param name="elementHandler">The element handler.</param>
		public PropertyData(IPageElementHandler<TElement> elementHandler)
		{
			this.elementHandler = elementHandler;
		}

		#region Public Properties

		/// <summary>
		///     Gets or sets a value indicating whether this instance represents a page element.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is a page element; otherwise, <c>false</c>.
		/// </value>
		public bool IsElement { get; internal set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is a list.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is a list; otherwise, <c>false</c>.
		/// </value>
		public bool IsList { get; internal set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; internal set; }

		/// <summary>
		///     Gets or sets the propertyValue of the property.
		/// </summary>
		/// <value>
		///     The propertyValue of the property.
		/// </value>
		public Type PropertyType { get; internal set; }

		#endregion

		#region Properties

		/// <summary>
		///     Gets or sets the call action.
		/// </summary>
		/// <value>
		///     The call action.
		/// </value>
		internal Func<IPage, Func<object, bool>, bool> Action { private get; set; }

		/// <summary>
		///     Gets or sets the element action.
		/// </summary>
		/// <value>
		///     The call action.
		/// </value>
		internal Func<IPage, Func<TElement, bool>, bool> ElementAction { private get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Clicks the element that this property represents.
		/// </summary>
		public void ClickElement()
		{
			this.ThrowIfElementDoesNotExist();
			var success = this.ElementAction(this.elementHandler, this.elementHandler.ClickElement);

			if (!success)
			{
				throw new ElementExecuteException("Click Action for property '{0}' failed!", this.Name);
			}
		}

		/// <summary>
		/// Checks to see if the element exists.
		/// </summary>
		/// <returns><c>true</c> if the element exists.</returns>
		public bool CheckElementEnabled()
		{
			return this.ElementAction(this.elementHandler, this.elementHandler.ElementEnabledCheck);
		}

		/// <summary>
		/// Checks to see if the element exists.
		/// </summary>
		/// <returns><c>true</c> if the element exists.</returns>
		public bool CheckElementExists()
		{
			return this.ElementAction(this.elementHandler, this.elementHandler.ElementExistsCheck);
		}

		/// <summary>
		/// Fills the data.
		/// </summary>
		/// <param name="data">The data.</param>
		public void FillData(string data)
		{
			this.ThrowIfElementDoesNotExist();

			var fillMethod = this.elementHandler.GetPageFillMethod(this.PropertyType);
			if (fillMethod == null)
			{
				throw new ElementExecuteException(
					"Cannot find input handler for property '{0}' on page {1}. Element propertyValue was: {2}",
					this.Name,
					this.elementHandler.PageType.Name,
					this.PropertyType.Name);
			}

			this.ElementAction(
				this.elementHandler, 
				e =>
				{
					fillMethod(e, data);
					return true;
				});
		}

		/// <summary>
		/// Gets the current value of the property.
		/// </summary>
		/// <returns>The current value as a string.</returns>
		public string GetCurrentValue()
		{
			string fieldValue = null;
			if (this.IsElement)
			{
				this.ThrowIfElementDoesNotExist();
				this.ElementAction(this.elementHandler,
					prop =>
					{
						fieldValue = this.elementHandler.GetElementText(prop);
						return true;
					});
			}
			else if (this.IsList)
			{
				throw new NotSupportedException("Cannot get the current value of a list property.");
			}
			else
			{
				this.Action(this.elementHandler,
					o =>
					{
						fieldValue = o.ToString();
						return true;
					});
			}

			return fieldValue;
		}

		/// <summary>
		/// Gets the index of the item at.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>
		/// The item as an <see cref="IPage" /> item; otherwise <c>null</c>.
		/// </returns>
		public IPage GetItemAtIndex(int index)
		{
			var item = default(TElement);
			var findItem = new Func<object, bool>(prop =>
				{
					var list = (IEnumerable<TElement>)prop;
					item = list.ElementAtOrDefault(index);

					return !Equals(item, default(TElement));
				});

			var success = this.Action(this.elementHandler, findItem);
			return success ? this.elementHandler.GetPageFromElement(item) : null;
		}

		/// <summary>
		/// Gets the item as page.
		/// </summary>
		/// <returns>
		/// The item as a page.
		/// </returns>
		public IPage GetItemAsPage()
		{
			var item = default(TElement);
			var findItem = new Func<TElement, bool>(
				prop =>
					{
						item = prop;
						return true;
					});

			this.ElementAction(this.elementHandler, findItem);
			return this.elementHandler.GetPageFromElement(item);
		}

        /// <summary>
        /// Highlights this instance.
        /// </summary>
	    public void Highlight()
        {
            this.ElementAction(
                this.elementHandler,
                e =>
                    {
                        this.elementHandler.Highlight(e);
                        return true;
                    });
        }

	    /// <summary>
		/// Validates the item or property matches the expected expression.
		/// </summary>
		/// <param name="validation">The validation item.</param>
		/// <param name="actualValue">The actual value if validation fails.</param>
		/// <returns>
		///   <c>true</c> if the items are valid; otherwise <c>false</c>.
		/// </returns>
		public bool ValidateItem(ItemValidation validation, out string actualValue)
		{
			string realValue = null;
			var compareWrapper = new Func<TElement, bool>(
					e =>
					{
						var text = this.elementHandler.GetElementText(e);
						realValue = text;
						return validation.Compare(this, text);
					});

			bool result;
			if (this.IsElement)
			{
				this.ThrowIfElementDoesNotExist();
				result = this.ElementAction(this.elementHandler, compareWrapper);
			}
			else if (this.IsList)
			{
				result = this.Action(
					this.elementHandler,
					o =>
						{
							var list = (IElementList<TElement, TElement>)o;
							return compareWrapper(list.Parent);
						});
			}
			else
			{
				result = this.Action(this.elementHandler, o => this.ComparePropertyValue(o, validation, out realValue));
			}

			actualValue = realValue;
			return result;
		}


		/// <summary>
		/// Validates the list.
		/// </summary>
		/// <param name="compareType">Type of the compare.</param>
		/// <param name="validations">The validations.</param>
		/// <returns>The validation result including checks performed.</returns>
		public ValidationResult ValidateList(ComparisonType compareType, ICollection<ItemValidation> validations)
		{
			var validationResult = new ValidationResult(validations);
			var isValid = this.Action(this.elementHandler, o => this.ValidateListInternal(o, compareType, validations, validationResult));

			validationResult.IsValid = isValid;

			return validationResult;
		}

        /// <summary>
        /// Waits for the element condition to be met.
        /// </summary>
        /// <param name="waitCondition">The wait condition.</param>
        /// <param name="timeout">The timeout to wait before failing.</param>
        /// <returns><c>true</c> if the condition is met, <c>false</c> otherwise.</returns>
        public bool WaitForElementCondition(WaitConditions waitCondition, TimeSpan? timeout)
        {
            return this.ElementAction(this.elementHandler, o => this.elementHandler.WaitForElement(o, waitCondition, timeout));
        }

        #endregion

		#region Methods

		#endregion

		/// <summary>
		/// Compares the property value.
		/// </summary>
		/// <param name="propertyValue">The property value.</param>
		/// <param name="validation">The validation.</param>
		/// <param name="actualValue">The actual value.</param>
		/// <returns>
		///   <c>true</c> if the comparison is valid.
		/// </returns>
		private bool ComparePropertyValue(object propertyValue, ItemValidation validation, out string actualValue)
		{
			var stringItems = propertyValue as IEnumerable<string>;
			if (stringItems != null)
			{
				var list = stringItems.ToList();
				actualValue = string.Join(",", list);
				return list.Any(s => validation.Compare(this, s));
			}

			actualValue = propertyValue != null ? propertyValue.ToString() : null;
			return validation.Compare(this, actualValue);
		}

		/// <summary>
		/// Check to make sure the element exists on the page.
		/// </summary>
		/// <exception cref="ElementExecuteException">Thrown if the element does not exist.</exception>
		private void ThrowIfElementDoesNotExist()
		{
			if (!this.CheckElementExists())
			{
				throw new ElementExecuteException(
					"Element mapped to property '{0}' does not exist on page {1}.",
					this.Name,
					this.elementHandler.PageType.Name);
			}
		}

		/// <summary>
		/// Validates the list.
		/// </summary>
		/// <param name="propertyValue">The property value.</param>
		/// <param name="compareType">Type of the compare.</param>
		/// <param name="validations">The validations.</param>
		/// <param name="validationResult">The validation tracker.</param>
		/// <returns>The list of validations.</returns>
		private bool ValidateListInternal(object propertyValue, ComparisonType compareType, IEnumerable<ItemValidation> validations, ValidationResult validationResult)
		{
			var list = ((IEnumerable<TElement>)propertyValue).ToList();

			validationResult.ItemCount = list.Count;

			switch (compareType)
			{
				case ComparisonType.Equals:
					return list.All(element => this.CheckItem(element, validations, validationResult));

				case ComparisonType.Contains:
					return list.Any(element => this.CheckItem(element, validations, validationResult));

				case ComparisonType.StartsWith:
					var firstItem = list.FirstOrDefault();
					return (!Equals(firstItem, default(TElement))) && this.CheckItem(firstItem, validations, validationResult);

				case ComparisonType.EndsWith:
					var lastItem = list.LastOrDefault();
					return (!Equals(lastItem, default(TElement))) && this.CheckItem(lastItem, validations, validationResult);

				case ComparisonType.DoesNotContain:
				case ComparisonType.DoesNotEqual:
					return list.All(element => !this.CheckItem(element, validations, validationResult));

				default:
					return false;
			}
		}

		/// <summary>
		/// Checks the item.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="validations">The validations.</param>
		/// <param name="validationResult">The validation tracker.</param>
		/// <returns><c>true</c> if the item is valid; otherwise <c>false</c>.</returns>
		private bool CheckItem(TElement element, IEnumerable<ItemValidation> validations, ValidationResult validationResult)
		{
			var page = this.elementHandler.GetPageFromElement(element);
			
			var validationItemResult = new ValidationItemResult();
			validationResult.CheckedItems.Add(validationItemResult);

			var result = true;
			foreach (var itemValidation in validations)
			{
				IPropertyData property;
				if (!page.TryGetProperty(itemValidation.FieldName, out property))
				{
					validationItemResult.NoteMissingProperty(itemValidation);
					result = false;
					continue;
				}

				string actualValue;
				var successful = true;
				if (!property.ValidateItem(itemValidation, out actualValue))
				{
					successful = false;
					result = false;
				}

				validationItemResult.NoteValidationResult(itemValidation, successful, actualValue);
			}

			return result;
		}
	}
}