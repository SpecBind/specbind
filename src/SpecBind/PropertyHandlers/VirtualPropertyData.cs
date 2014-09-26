// <copyright file="VirtualPropertyData.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.PropertyHandlers
{
    using System;

    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
    /// A property type that is an accessor on an element.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    internal class VirtualPropertyData<TElement> : PropertyDataBase<TElement>
    {
        private readonly string attributeName;
        private readonly Func<IPage, Func<TElement, bool>, bool> handler;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualPropertyData{TElement}" /> class.
        /// </summary>
        /// <param name="elementHandler">The element page handler.</param>
        /// <param name="name">The name.</param>
        /// <param name="handler">The element handler.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        public VirtualPropertyData(IPageElementHandler<TElement> elementHandler, string name, Func<IPage, Func<TElement, bool>, bool> handler, string attributeName)
            : base(elementHandler, name, typeof(string))
        {
            this.attributeName = attributeName;
            this.handler = handler;
        }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        /// <returns>The current value from the element.</returns>
        public override string GetCurrentValue()
        {
            string propertyValue = null;

            this.handler(this.ElementHandler,
                e =>
                    {
                        propertyValue = this.ElementHandler.GetElementAttributeValue(e, this.attributeName);
                        return true;
                    });

            return propertyValue;
        }

        /// <summary>
        /// Validates the item.
        /// </summary>
        /// <param name="validation">The validation.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <returns><c>true</c> if the validation is successful, <c>false</c> otherwise.</returns>
        public override bool ValidateItem(ItemValidation validation, out string actualValue)
        {
            actualValue = this.GetCurrentValue();
            return validation.Compare(this, actualValue);
        }
    }
}