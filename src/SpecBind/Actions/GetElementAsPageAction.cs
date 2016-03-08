// <copyright file="GetElementAsPageAction.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// An action that returns a property as a new page child scope.
    /// </summary>
    internal class GetElementAsPageAction : ActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetElementAsPageAction"/> class.
        /// </summary>
        public GetElementAsPageAction()
            : base(typeof(GetElementAsPageAction).Name)
        {
        }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
        public override ActionResult Execute(ActionContext actionContext)
        {
            var propertyData = this.ElementLocator.GetElement(actionContext.PropertyName);

            if (propertyData.IsList)
            {
                return ActionResult.Failure(
                        new ElementExecuteException(
                            "Property '{0}' was located but is a list element which cannot be a sub-page.",
                            propertyData.Name));
            }

            var propertyPage = propertyData.GetItemAsPage();

            return propertyPage == null
                       ? ActionResult.Failure(new ElementExecuteException("Could not retrieve a page from property '{0}'", propertyData.Name))
                       : ActionResult.Successful(propertyPage);
        }
    }
}