// <copyright file="ValidateElementEnabledAction.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using SpecBind.Pages;

    /// <summary>
    /// An action that validates that the element exists.
    /// </summary>
    internal class ValidateElementEnabledAction : BasicValidationChecksActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateElementEnabledAction" /> class.
        /// </summary>
        public ValidateElementEnabledAction()
            : base(typeof(ValidateElementEnabledAction).Name)
        {
        }

        /// <summary>
        /// Gets the false error message.
        /// </summary>
        /// <value>The false error message.</value>
        protected override string FalseErrorMessage
        {
            get { return "Element '{0}' is enabled on the page and should not be enabled."; }
        }

        /// <summary>
        /// Gets the true error message.
        /// </summary>
        /// <value>The true error message.</value>
        protected override string TrueErrorMessage
        {
            get { return "Element '{0}' is not enabled on the page and should be enabled."; }
        }

        /// <summary>
        /// Checks the element.
        /// </summary>
        /// <param name="propertyData">The property data.</param>
        /// <returns><c>true</c> if the element exists, <c>false</c> otherwise.</returns>
        protected override bool CheckElement(IPropertyData propertyData)
        {
            return propertyData.CheckElementEnabled();
        }
    }
}