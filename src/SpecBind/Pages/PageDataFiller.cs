// <copyright file="PageDataFiller.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// A class that fills the target page object fields 
	/// </summary>
	public class PageDataFiller : IPageDataFiller
	{
		/// <summary>
		/// Clicks the link or button field.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the field.</param>
		public void ClickItem(IPage page, string fieldName)
		{
			IPropertyData propertyData;
			if (!page.TryGetElement(fieldName, out propertyData))
			{
				throw GetElementNotFoundException(page, fieldName, v => v.IsElement);
			}

			propertyData.ClickElement();
		}

		/// <summary>
		/// Fills the field.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="data">The data.</param>
		public void FillField(IPage page, string fieldName, string data)
		{
			IPropertyData propertyData;
			if (!page.TryGetElement(fieldName, out propertyData))
			{
				throw GetElementNotFoundException(page, fieldName, v => v.IsElement);
			}

			propertyData.FillData(data);
		}

		/// <summary>
		/// Gets the value of the currently asked for property.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns>The current value of the selected property.</returns>
		public string GetItemValue(IPage page, string fieldName)
		{
			IPropertyData propertyData;
			if (!page.TryGetProperty(fieldName, out propertyData))
			{
				throw GetElementNotFoundException(page, fieldName, v => true);
			}

			return propertyData.GetCurrentValue();
		}

		/// <summary>
		/// Validates that the given element exists on the page and is enabled.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="shouldExist">if set to <c>true</c> the element should exist; if <c>false</c> it should not.</param>
		public void ValidateEnabled(IPage page, string fieldName, bool shouldExist)
		{
			ValidateElementItem(page,
									 fieldName,
									 shouldExist,
									 e => e.CheckElementEnabled(),
									 "Element '{0}' is not enabled on the page {1} and should be enabled.",
									 "Element '{0}' is enabled on the page {1} and should not be enabled.");
		}

		/// <summary>
		/// Validates that the given element exists on the page.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="shouldExist">if set to <c>true</c> the element should exist; if <c>false</c> it should not.</param>
		public void ValidateExists(IPage page, string fieldName, bool shouldExist)
		{
			ValidateElementItem(page,
									 fieldName,
									 shouldExist,
									 e => e.CheckElementExists(),
			                         "Element '{0}' should exist on page {1} and does not.",
			                         "Element '{0}' exists on page {1} and should not.");
		}

		/// <summary>
		/// Gets the element property as a page interface.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns>
		/// The property as a page object.
		/// </returns>
		public IPage GetElementAsPage(IPage page, string fieldName)
		{
			IPropertyData propertyData;
			if (!page.TryGetElement(fieldName, out propertyData) || propertyData.IsList)
			{
				throw GetElementNotFoundException(page, fieldName, v => v.IsElement && !v.IsList);
			}

			var propertyPage = propertyData.GetItemAsPage();

			if (propertyPage == null)
			{
				throw new ElementExecuteException("Could not retrieve a page from property '{0}'", propertyData.Name);
			}

			return propertyPage;
		}

		/// <summary>
		/// Validates the item.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="validation">The validation.</param>
		/// <exception cref="ElementExecuteException">Value comparison of '{0}' failed</exception>
		public void ValidateItem(IPage page, ItemValidation validation)
		{
			IPropertyData propertyData;
			if (!page.TryGetProperty(validation.FieldName, out propertyData))
			{
				throw GetElementNotFoundException(page, validation.FieldName, v => true);
			}

			string actualValue;
			if (!propertyData.ValidateItem(validation, out actualValue))
			{
				throw new ElementExecuteException(
					"Value comparison of  field '{0}' failed. Rule: {1} Comparison Value: {2}, Actual Value: {3}",
					validation.FieldName,
					validation.ComparisonType,
					validation.ComparisonValue,
					actualValue);
			}
		}

		/// <summary>
		/// Validates the list of item follows the specified values.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the item.</param>
		/// <param name="compareType">Type of the compare.</param>
		/// <param name="validations">The validations.</param>
		/// <exception cref="ElementExecuteException">One or more comparisons failed.</exception>
		public void ValidateList(IPage page, string fieldName, ComparisonType compareType, ICollection<ItemValidation> validations)
		{
			IPropertyData propertyData;
			if (!page.TryGetProperty(fieldName, out propertyData) || !propertyData.IsList)
			{
				throw GetElementNotFoundException(page, fieldName, v => v.IsList);
			}

			if (!propertyData.ValidateList(compareType, validations))
			{
				throw new ElementExecuteException("Value comparison of failed, no items satisfied the checks.", fieldName);
			}
		}

		/// <summary>
		/// Gets the list item as a child page.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="itemNumber">The item number.</param>
		/// <returns>
		/// The child item as a page.
		/// </returns>
		public IPage GetListItem(IPage page, string fieldName, int itemNumber)
		{
			IPropertyData propertyData;
			if (!page.TryGetProperty(fieldName, out propertyData) || !propertyData.IsList)
			{
				throw GetElementNotFoundException(page, fieldName, v => v.IsList);
			}

			var item = propertyData.GetItemAtIndex(itemNumber - 1);

			if (item == null)
			{
				throw new ElementExecuteException("Could not find item {0} on list '{1}'", itemNumber, propertyData.Name);
			}

			return item;
		}

		/// <summary>
		/// Gets the element not found exception.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="filter">The filter.</param>
		/// <returns>
		/// The created exception.
		/// </returns>
		private static ElementExecuteException GetElementNotFoundException(IPage page, string fieldName, Func<IPropertyData, bool> filter = null)
		{
			string availableFields = null;

			if (filter != null)
			{
				var builder = new System.Text.StringBuilder(" Available Fields: ");
				builder.AppendLine();

				foreach (var field in page.GetPropertyNames(filter))
				{
					builder.AppendLine(field);
				}

				availableFields = builder.ToString();
			}

			return new ElementExecuteException("Could not locate property '{0}' on page {1}.{2}", fieldName, page.PageType.Name, availableFields);
		}

		/// <summary>
		/// Validates that the given element exists on the page.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="shouldExist">if set to <c>true</c> the element should exist; if <c>false</c> it should not.</param>
		/// <param name="elementFunc">The element function to check.</param>
		/// <param name="trueError">The true error message.</param>
		/// <param name="falseError">The false error message.</param>
		/// <exception cref="ElementExecuteException">Element '{0}' should exist on page {1} and does not.</exception>
		private static void ValidateElementItem(IPage page, string fieldName, bool shouldExist, Func<IPropertyData, bool> elementFunc, string trueError, string falseError)
		{
			IPropertyData propertyData;
			if (!page.TryGetElement(fieldName, out propertyData))
			{
				throw GetElementNotFoundException(page, fieldName,  v => v.IsElement);
			}

			var exists = elementFunc(propertyData);

			if (shouldExist && !exists)
			{
				throw new ElementExecuteException(trueError, propertyData.Name, page.PageType.Name);
			}

			if (!shouldExist && exists)
			{
				throw new ElementExecuteException(falseError, propertyData.Name, page.PageType.Name);
			}
		}
	}
}