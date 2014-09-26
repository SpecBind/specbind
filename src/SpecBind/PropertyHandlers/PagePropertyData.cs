// <copyright file="PagePropertyData.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.PropertyHandlers
{
    using System;

    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
    /// The property data for a given non-element property.
    /// </summary>
    /// <typeparam name="TElement">The propertyValue of the element.</typeparam>
    internal class PagePropertyData<TElement> : PropertyDataBase<TElement>
    {
        private readonly Func<IPage, Func<object, bool>, bool> action;
        private readonly Action<IPage, object> setAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDataBase{TElement}" /> class.
        /// </summary>
        /// <param name="elementHandler">The element handler.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="action">The action.</param>
        /// <param name="setAction">The set action.</param>
        public PagePropertyData(IPageElementHandler<TElement> elementHandler, string name, Type propertyType, Func<IPage, Func<object, bool>, bool> action, Action<IPage, object> setAction)
            : base(elementHandler, name, propertyType)
        {
            this.IsElement = false;
            this.IsList = false;
            this.action = action;
            this.setAction = setAction;
        }

        /// <summary>
        /// Fills the data.
        /// </summary>
        /// <param name="data">The data.</param>
        public override void FillData(string data)
        {
            // Support only string property filling for now
            if (typeof(string).IsAssignableFrom(this.PropertyType) && this.setAction != null)
            {
                this.setAction(this.ElementHandler, data);
            }
            else
            {
                throw new ElementExecuteException("Only string properties are supported today. Property Type: {0}", this.PropertyType);
            }
        }

        /// <summary>
        /// Gets the current value of the property.
        /// </summary>
        /// <returns>The current value as a string.</returns>
        public override string GetCurrentValue()
        {
            string fieldValue = null;
            this.action(this.ElementHandler,
                    o =>
                        {
                            fieldValue = o.ToString();
                            return true;
                        });

            return fieldValue;
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

            var result = this.action(this.ElementHandler, o => this.ComparePropertyValue(o, validation, out realValue));

            actualValue = realValue;
            return result;
        }
    }
}