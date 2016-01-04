// <copyright file="ListPropertyData.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.PropertyHandlers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using SpecBind.Pages;
	using SpecBind.Validation;

	/// <summary>
	/// The property data for property that represents a list.
	/// </summary>
	/// <typeparam name="TElement">The propertyValue of the element.</typeparam>
	internal class ListPropertyData<TElement> : PropertyDataBase<TElement>
	{
		private readonly Func<IPage, Func<object, bool>, bool> action;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyDataBase{TElement}" /> class.
		/// </summary>
		/// <param name="elementHandler">The element handler.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="propertyType">Type of the property.</param>
		/// <param name="action">The action used to get the property.</param>
		public ListPropertyData(IPageElementHandler<TElement> elementHandler, string name, Type propertyType, Func<IPage, Func<object, bool>, bool> action)
			: base(elementHandler, name, propertyType)
		{
			this.IsList = true;
			this.action = action;
		}

		#region Public Methods

		/// <summary>
		/// Gets the index of the item at.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>
		/// The item as an <see cref="IPage" /> item; otherwise <c>null</c>.
		/// </returns>
		public override IPage GetItemAtIndex(int index)
		{
			var item = default(TElement);
			var findItem = new Func<object, bool>(prop =>
				{
					var list = (IEnumerable<TElement>)prop;
					item = list.ElementAtOrDefault(index);

					return !Equals(item, default(TElement));
				});

			var success = this.action(this.ElementHandler, findItem);
			return success ? this.ElementHandler.GetPageFromElement(item) : null;
		}

		/// <summary>
		/// Validates the list.
		/// </summary>
		/// <param name="validations">The validations.</param>
		/// <returns>The validation result including checks performed.</returns>
		public override Tuple<IPage, ValidationResult> FindItemInList(ICollection<ItemValidation> validations)
		{
			var validationResult = new ValidationResult(validations);

			var item = default(TElement);
			this.action(this.ElementHandler,
				propertyValue =>
					{
						var list = ((IEnumerable<TElement>)propertyValue).ToList();
						item = list.FirstOrDefault(i => this.CheckItem(i, validations, validationResult));
						return true;
					});

			var page = !Equals(item, default(TElement)) ? this.ElementHandler.GetPageFromElement(item) : null;
			return new Tuple<IPage, ValidationResult>(page, validationResult);
		}

		/// <summary>
		/// Validates the item or property matches the expected expression.
		/// </summary>
		/// <param name="validation">The validation item.</param>
		/// <param name="actualValue">The actual value if validation fails.</param>
		/// <returns>
		///   <c>true</c> if the items are valid; otherwise <c>false</c>.
		/// </returns>
		public override bool ValidateItem(ItemValidation validation, out string actualValue)
		{
			string realValue = null;
			var compareWrapper = new Func<TElement, bool>(
				e =>
					{
						var text = this.GetElementText(validation, e);

						realValue = text;
						return validation.Compare(this, text);
					});

			var result = this.action(
					this.ElementHandler,
					o =>
					{
						var list = (IElementList<TElement, TElement>)o;
						return compareWrapper(list.Parent);
					});
			
			actualValue = realValue;
			return result;
		}


		/// <summary>
		/// Validates the list.
		/// </summary>
		/// <param name="compareType">Type of the compare.</param>
		/// <param name="validations">The validations.</param>
		/// <returns>The validation result including checks performed.</returns>
		public override ValidationResult ValidateList(ComparisonType compareType, ICollection<ItemValidation> validations)
		{
			var validationResult = new ValidationResult(validations);
			var isValid = this.action(this.ElementHandler, o => this.ValidateListInternal(o, compareType, validations, validationResult));

			validationResult.IsValid = isValid;

			return validationResult;
		}

		/// <summary>
		/// Validates the list row count.
		/// </summary>
		/// <param name="comparisonType">Type of the comparison.</param>
		/// <param name="expectedRowCount">The expected row count.</param>
		/// <returns>A tuple indicating if the results were successful and the actual row count.</returns>
		public override Tuple<bool, int> ValidateListRowCount(NumericComparisonType comparisonType, int expectedRowCount)
		{
			var actualRowCount = 0;
			var isValid = this.action(this.ElementHandler,
				e =>
					{
						var rowCount = ((IEnumerable<TElement>)e).Count();
						actualRowCount = rowCount;

						switch (comparisonType)
						{
							case NumericComparisonType.Equals:
								return rowCount == expectedRowCount;
							case NumericComparisonType.GreaterThanEquals:
								return rowCount >= expectedRowCount;
							case NumericComparisonType.LessThanEquals:
								return rowCount <= expectedRowCount;
							default:
								return false;
						}
					});

			return new Tuple<bool, int>(isValid, actualRowCount);
		}

		#endregion

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
			var page = this.ElementHandler.GetPageFromElement(element);
			
			var validationItemResult = new ValidationItemResult();
			validationResult.CheckedItems.Add(validationItemResult);

			var result = true;
			foreach (var itemValidation in validations)
			{
				IPropertyData property;
				if (!page.TryGetProperty(itemValidation.FieldName, out property))
				{
					if (itemValidation.Comparer is DoesNotContainComparer)
					{
						result = true;
						validationItemResult.NoteValidationResult(itemValidation, successful: true, actualValue: null);
						continue;
					}

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