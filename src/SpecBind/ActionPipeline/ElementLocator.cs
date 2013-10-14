// <copyright file="ElementLocator.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.ActionPipeline
{
	using System;
	using System.Collections.Generic;

	using SpecBind.Pages;

	/// <summary>
	/// An implementation of <see cref="IElementLocator"/> that uses the page model
	/// to locate the element but ties in plugins.
	/// </summary>
	internal class ElementLocator : IElementLocator
	{
		private readonly List<ILocatorAction> filterActions;
		private readonly IPage page;

		/// <summary>
		/// Initializes a new instance of the <see cref="ElementLocator" /> class.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="filterActions">The filter actions.</param>
		public ElementLocator(IPage page, IEnumerable<ILocatorAction> filterActions)
		{
			this.page = page;
			this.filterActions = new List<ILocatorAction>(filterActions);
		}

		/// <summary>
		/// Gets the element from the context.
		/// </summary>
		/// <param name="propertyName">The property name to locate.</param>
		/// <returns>The resulting property data.</returns>
		/// <exception cref="ElementExecuteException">Thrown when the element could not be found.</exception>
		public IPropertyData GetElement(string propertyName)
		{
			IPropertyData propertyData;
			if (!this.TryGetElement(propertyName, out propertyData))
			{
				throw GetElementNotFoundException(this.page, propertyName, f => f.IsElement);
			}

			return propertyData;
		}

		/// <summary>
		/// Gets the property from the context.
		/// </summary>
		/// <param name="propertyName">The property name to locate.</param>
		/// <returns>The resulting property data.</returns>
		public IPropertyData GetProperty(string propertyName)
		{
			IPropertyData propertyData;
			if (!this.TryGetProperty(propertyName, out propertyData))
			{
				throw GetElementNotFoundException(this.page, propertyName, v => true);
			}

			return propertyData;
		}

		/// <summary>
		/// Tries the get the element.
		/// </summary>
		/// <param name="propertyName">The key.</param>
		/// <param name="propertyData">The property data.</param>
		/// <returns><c>true</c> if the element exists; otherwise <c>false</c>.</returns>
		public bool TryGetElement(string propertyName, out IPropertyData propertyData)
		{
			foreach (var locatorAction in this.filterActions)
			{
				locatorAction.OnLocate(propertyName);
			}

			var result = this.page.TryGetElement(propertyName, out propertyData);

			foreach (var locatorAction in this.filterActions)
			{
				locatorAction.OnLocateComplete(propertyName, propertyData);
			}

			return result;
		}

		/// <summary>
		/// Tries the get the property.
		/// </summary>
		/// <param name="propertyName">The key.</param>
		/// <param name="propertyData">The property data.</param>
		/// <returns><c>true</c> if the element exists; otherwise <c>false</c>.</returns>
		public bool TryGetProperty(string propertyName, out IPropertyData propertyData)
		{
			foreach (var locatorAction in this.filterActions)
			{
				locatorAction.OnLocate(propertyName);
			}

			var result = this.page.TryGetProperty(propertyName, out propertyData);

			foreach (var locatorAction in this.filterActions)
			{
				locatorAction.OnLocateComplete(propertyName, propertyData);
			}

			return result;
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
	}
}