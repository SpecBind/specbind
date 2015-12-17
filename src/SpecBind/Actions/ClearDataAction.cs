// <copyright file="ClearDataAction.cs">
//    Copyright © 2015 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// An action that clears data from a field.
    /// </summary>
    internal class ClearDataAction : ContextActionBase<ClearDataAction.ClearDataContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClearDataAction" /> class.
        /// </summary>
        public ClearDataAction()
            : base(typeof(ClearDataAction).Name)
        {
        }

        /// <summary>
        /// Executes the specified action using the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(ClearDataContext context)
        {
            // First look for an element
            IPropertyData item;
            if (!this.ElementLocator.TryGetElement(context.PropertyName, out item))
            {
                // Try to get a property and check to make sure it's a string for now
                item = this.ElementLocator.GetProperty(context.PropertyName);
            }

            item.ClearData();

            return ActionResult.Successful();
        }

        /// <summary>
        /// An extended context class to pass clear data.
        /// </summary>
        public class ClearDataContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ClearDataContext"/> class.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            public ClearDataContext(string propertyName)
                : base(propertyName)
            {
            }
        }
    }
}
