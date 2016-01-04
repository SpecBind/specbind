// <copyright file="ElementPropertyData.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.PropertyHandlers
{
    using System;

    using SpecBind.Actions;
    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
    /// The property data for a given property.
    /// </summary>
    /// <typeparam name="TElement">The propertyValue of the element.</typeparam>
    internal class ElementPropertyData<TElement> : PropertyDataBase<TElement>
    {
        private readonly Func<IPage, Func<TElement, bool>, bool> elementAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDataBase{TElement}" /> class.
        /// </summary>
        /// <param name="elementHandler">The element handler.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="elementAction">The element action.</param>
        public ElementPropertyData(IPageElementHandler<TElement> elementHandler, string name, Type propertyType, Func<IPage, Func<TElement, bool>, bool> elementAction)
            : base(elementHandler, name, propertyType)
        {
            this.elementAction = elementAction;
            this.IsElement = true;
        }

        /// <summary>
        /// Clicks the element that this property represents.
        /// </summary>
        public override void ClickElement()
        {
            this.ThrowIfElementDoesNotExist();
            var success = this.elementAction(this.ElementHandler, this.ElementHandler.ClickElement);

            if (!success)
            {
                throw new ElementExecuteException("Click Action for property '{0}' failed!", this.Name);
            }
        }

        /// <summary>
        /// Checks to see if the element exists.
        /// </summary>
        /// <returns><c>true</c> if the element exists.</returns>
        public override bool CheckElementEnabled()
        {
            return this.elementAction(this.ElementHandler, this.ElementHandler.ElementEnabledCheck);
        }

        /// <summary>
        /// Checks to see if the element exists.
        /// </summary>
        /// <returns><c>true</c> if the element exists.</returns>
        public override bool CheckElementExists()
        {
            return this.elementAction(this.ElementHandler, this.ElementHandler.ElementExistsCheck);
        }

	    /// <summary>
	    /// Clears the data for the element that this property represents.
	    /// </summary>
	    public override void ClearData()
        {
            this.ThrowIfElementDoesNotExist();

            var clearMethod = this.ElementHandler.GetClearMethod(this.PropertyType);
            if (clearMethod == null)
            {
                throw new ElementExecuteException(
                    "Cannot find input handler for property '{0}' on page {1}. Element propertyValue was: {2}",
                    this.Name,
                    this.ElementHandler.PageType.Name,
                    this.PropertyType.Name);
            }

            this.elementAction(
                this.ElementHandler, 
                e =>
                    {
                        clearMethod(e);
                        return true;
                    });
        }

        /// <summary>
        /// Checks to see if the element does not exist.
        /// Unlike ElementExistsCheck() this, doesn't let the web driver wait first for the element to exist.
        /// </summary>
        /// <returns><c>true</c> if the element exists.</returns>
        public override bool CheckElementNotExists()
        {
            return this.elementAction(this.ElementHandler, this.ElementHandler.ElementNotExistsCheck);
        }

        /// <summary>
        /// Fills the data.
        /// </summary>
        /// <param name="data">The data.</param>
        public override void FillData(string data)
        {
            this.ThrowIfElementDoesNotExist();

            var fillMethod = this.ElementHandler.GetPageFillMethod(this.PropertyType);
            if (fillMethod == null)
            {
                throw new ElementExecuteException(
                    "Cannot find input handler for property '{0}' on page {1}. Element propertyValue was: {2}",
                    this.Name,
                    this.ElementHandler.PageType.Name,
                    this.PropertyType.Name);
            }

            this.elementAction(
                this.ElementHandler, 
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
        public override string GetCurrentValue()
        {
            string fieldValue = null;

            this.ThrowIfElementDoesNotExist();
            this.elementAction(this.ElementHandler,
                prop =>
                {
                    fieldValue = this.ElementHandler.GetElementText(prop);
                    return true;
                });

            return fieldValue;
        }

        /// <summary>
        /// Gets the item as page.
        /// </summary>
        /// <returns>
        /// The item as a page.
        /// </returns>
        public override IPage GetItemAsPage()
        {
            var item = default(TElement);
            var findItem = new Func<TElement, bool>(
                prop =>
                    {
                        item = prop;
                        return true;
                    });

            this.elementAction(this.ElementHandler, findItem);
            return this.ElementHandler.GetPageFromElement(item);
        }

        /// <summary>
        /// Highlights this instance.
        /// </summary>
        public override void Highlight()
        {
            this.elementAction(
                this.ElementHandler,
                e =>
                    {
                        this.ElementHandler.Highlight(e);
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

            if (validation.CheckElementExistence)
            {
                this.ThrowIfElementDoesNotExist();
            }

            var result = this.elementAction(this.ElementHandler, compareWrapper);

            actualValue = realValue;
            return result;
        }
        
        /// <summary>
        /// Waits for the element condition to be met.
        /// </summary>
        /// <param name="waitCondition">The wait condition.</param>
        /// <param name="timeout">The timeout to wait before failing.</param>
        /// <returns><c>true</c> if the condition is met, <c>false</c> otherwise.</returns>
        public override bool WaitForElementCondition(WaitConditions waitCondition, TimeSpan? timeout)
        {
            return this.elementAction(this.ElementHandler, o => this.ElementHandler.WaitForElement(o, waitCondition, timeout));
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
                    this.ElementHandler.PageType.Name);
            }
        }
    }
}