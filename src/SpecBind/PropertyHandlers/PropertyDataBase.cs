// <copyright file="PropertyDataBase.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.PropertyHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SpecBind.Actions;
    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
    /// A base class to define all the properties.
    /// </summary>
    /// <typeparam name="TElement">The type of the t element.</typeparam>
    internal abstract class PropertyDataBase<TElement> : IPropertyData
    {
        private readonly IPageElementHandler<TElement> elementHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDataBase{TElement}" /> class.
        /// </summary>
        /// <param name="elementHandler">The element handler.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        protected PropertyDataBase(IPageElementHandler<TElement> elementHandler, string name, Type propertyType)
        {
            this.elementHandler = elementHandler;
            this.Name = name;
            this.PropertyType = propertyType;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance represents a page element.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is a page element; otherwise, <c>false</c>.
        /// </value>
        public bool IsElement { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a list.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is a list; otherwise, <c>false</c>.
        /// </value>
        public bool IsList { get; protected set; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        public Type PropertyType { get; private set; }

        /// <summary>
        /// Gets the element handler.
        /// </summary>
        /// <value>The element handler.</value>
        protected IPageElementHandler<TElement> ElementHandler
        {
            get
            {
                return this.elementHandler;
            }
        }

        /// <summary>
        /// Clears the data for the element that this property represents.
        /// </summary>
        public virtual void ClearData()
        {
            throw this.CreateNotSupportedException("Clearing an element");
        }

        /// <summary>
        /// Clicks the element that this property represents.
        /// </summary>
        public virtual void ClickElement()
        {
            throw this.CreateNotSupportedException("Clicking an element");
        }

        /// <summary>
        /// Checks to see if the element exists.
        /// </summary>
        /// <returns><c>true</c> if the element exists.</returns>
        public virtual bool CheckElementEnabled()
        {
            throw this.CreateNotSupportedException("Checking for an element being enabled");
        }

        /// <summary>
        /// Checks to see if the element exists.
        /// </summary>
        /// <returns><c>true</c> if the element exists.</returns>
        public virtual bool CheckElementExists()
        {
            throw this.CreateNotSupportedException("Checking for an element existing");
        }

        /// <summary>
        /// Checks to see if the element does not exist.
        /// </summary>
        /// <returns><c>true</c> if the element exists.</returns>
        public virtual bool CheckElementNotExists()
        {
            throw this.CreateNotSupportedException("Checking for an element not existing");
        }

        /// <summary>
        /// Fills the data.
        /// </summary>
        /// <param name="data">The data.</param>
        public virtual void FillData(string data)
        {
            throw this.CreateNotSupportedException("Filling in data");
        }

        /// <summary>
        /// Gets the current value of the property.
        /// </summary>
        /// <returns>The current value as a string.</returns>
        public virtual string GetCurrentValue()
        {
            throw this.CreateNotSupportedException("Getting the current value");
        }

        /// <summary>
        /// Gets the index of the item at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        /// The item as an <see cref="IPage" /> item; otherwise <c>null</c>.
        /// </returns>
        public virtual IPage GetItemAtIndex(int index)
        {
            throw this.CreateNotSupportedException("Getting an item at a given index");
        }

        /// <summary>
        /// Gets the item as page.
        /// </summary>
        /// <returns>
        /// The item as a page.
        /// </returns>
        public virtual IPage GetItemAsPage()
        {
            throw this.CreateNotSupportedException("Getting a property as a page item");
        }

        /// <summary>
        /// Validates the list.
        /// </summary>
        /// <param name="validations">The validations.</param>
        /// <returns>The validation result including checks performed.</returns>
        public virtual Tuple<IPage, ValidationResult> FindItemInList(ICollection<ItemValidation> validations)
        {
            throw this.CreateNotSupportedException("Finding an item in a list");
        }

        /// <summary>
        /// Highlights this instance.
        /// </summary>
        public virtual void Highlight()
        {
        }

        /// <summary>
        /// Validates the item or property matches the expected expression.
        /// </summary>
        /// <param name="validation">The validation item.</param>
        /// <param name="actualValue">The actual value if validation fails.</param>
        /// <returns>
        ///   <c>true</c> if the items are valid; otherwise <c>false</c>.
        /// </returns>
        public abstract bool ValidateItem(ItemValidation validation, out string actualValue);

        /// <summary>
        /// Validates the list.
        /// </summary>
        /// <param name="compareType">Type of the compare.</param>
        /// <param name="validations">The validations.</param>
        /// <returns>The validation result including checks performed.</returns>
        public virtual ValidationResult ValidateList(ComparisonType compareType, ICollection<ItemValidation> validations)
        {
            throw this.CreateNotSupportedException("Validating a list");
        }

        /// <summary>
        /// Validates the list row count.
        /// </summary>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <param name="expectedRowCount">The expected row count.</param>
        /// <returns>A tuple indicating if the results were successful and the actual row count.</returns>
        public virtual Tuple<bool, int> ValidateListRowCount(NumericComparisonType comparisonType, int expectedRowCount)
        {
            throw this.CreateNotSupportedException("Validating a list row count");
        }

        /// <summary>
        /// Waits for the element condition to be met.
        /// </summary>
        /// <param name="waitCondition">The wait condition.</param>
        /// <param name="timeout">The timeout to wait before failing.</param>
        /// <returns><c>true</c> if the condition is met, <c>false</c> otherwise.</returns>
        public virtual bool WaitForElementCondition(WaitConditions waitCondition, TimeSpan? timeout)
        {
            throw this.CreateNotSupportedException("Waiting for an element");
        }

        /// <summary>
        /// Compares the property value.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="validation">The validation.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <returns>
        ///   <c>true</c> if the comparison is valid.
        /// </returns>
        protected bool ComparePropertyValue(object propertyValue, ItemValidation validation, out string actualValue)
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
        /// Gets the element text.
        /// </summary>
        /// <param name="validation">The validation.</param>
        /// <param name="element">The element.</param>
        /// <returns>The cleaned text from the element.</returns>
        protected string GetElementText(ItemValidation validation, TElement element)
        {
            if (!validation.RequiresFieldValue)
            {
                return null;
            }

            var text = this.elementHandler.GetElementText(element);

            // Trim whitespace from text since the tables in SpecFlow will anyway.
            if (text != null)
            {
                text = text.Trim();
                text = text.Replace(Environment.NewLine, " ");
            }

            return text;
        }

        /// <summary>
        /// Creates the not supported exception.
        /// </summary>
        /// <param name="operationName">Name of the operation.</param>
        /// <returns>The created exception.</returns>
        private Exception CreateNotSupportedException(string operationName)
        {
            return new NotSupportedException(string.Format("{0} is not suppported by property type '{1}'", operationName, this.GetType().Name));
        }
    }
}