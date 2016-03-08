// <copyright file="BasicValidationChecksActionBase.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// A base class for the basic checks like items existing and are enabled.
    /// </summary>
    internal abstract class BasicValidationChecksActionBase : ContextActionBase<ValidationCheckContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicValidationChecksActionBase" /> class.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        protected BasicValidationChecksActionBase(string actionName)
            : base(actionName)
        {
        }

        /// <summary>
        /// Gets the false error message.
        /// </summary>
        /// <value>The false error message.</value>
        protected abstract string FalseErrorMessage { get; }

        /// <summary>
        /// Gets the true error message.
        /// </summary>
        /// <value>The true error message.</value>
        protected abstract string TrueErrorMessage { get; }

        /// <summary>
        /// Checks the element.
        /// </summary>
        /// <param name="propertyData">The property data.</param>
        /// <returns><c>true</c> if the element passes the check, <c>false</c> otherwise.</returns>
        protected abstract bool CheckElement(IPropertyData propertyData);

        /// <summary>
        /// Executes the specified action using the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(ValidationCheckContext context)
        {
            var propertyData = this.ElementLocator.GetElement(context.PropertyName);

            var shouldExist = context.ShouldExist;
            var exists = this.CheckElement(propertyData);

            if (shouldExist && !exists)
            {
                return ActionResult.Failure(new ElementExecuteException(this.TrueErrorMessage, propertyData.Name));
            }

            if (!shouldExist && exists)
            {
                return ActionResult.Failure(new ElementExecuteException(this.FalseErrorMessage, propertyData.Name));
            }

            return ActionResult.Successful();
        }
    }
}