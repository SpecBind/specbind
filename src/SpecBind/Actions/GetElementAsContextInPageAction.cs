// <copyright file="GetElementAsContextInPageAction.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// An action that returns a property as a new context in the page.
    /// </summary>
    public class GetElementAsContextInPageAction : ContextActionBase<GetElementAsContextInPageAction.GetElementAsContextInPageActionContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetElementAsContextInPageAction"/> class.
        /// </summary>
        public GetElementAsContextInPageAction()
            : base(typeof(GetElementAsContextInPageAction).Name)
        {
        }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(GetElementAsContextInPageActionContext actionContext)
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

        /// <summary>
        /// Get Element As Context In Page Action Context.
        /// </summary>
        /// <seealso cref="SpecBind.ActionPipeline.ActionContext" />
        public class GetElementAsContextInPageActionContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetElementAsContextInPageActionContext" /> class.
            /// </summary>
            /// <param name="page">The page.</param>
            /// <param name="propertyName">Name of the property.</param>
            public GetElementAsContextInPageActionContext(IPage page, string propertyName)
                : base(propertyName)
            {
                this.Page = page;
            }

            /// <summary>
            /// Gets the page.
            /// </summary>
            /// <value>The page.</value>
            public IPage Page { get; private set; }
        }
    }
}
