// <copyright file="GetListItemByIndexAction.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.Pages;

    /// <summary>
    /// An action that gets the list item by the requested index in the list.
    /// </summary>
    internal class GetListItemByIndexAction : ContextActionBase<GetListItemByIndexAction.ListItemByIndexContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetListItemByIndexAction" /> class.
        /// </summary>
        public GetListItemByIndexAction()
            : base(typeof(GetListItemByIndexAction).Name)
        {
        }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(ListItemByIndexContext actionContext)
        {
            var propertyData = this.ElementLocator.GetProperty(actionContext.PropertyName);

            if (!propertyData.IsList)
            {
                return ActionResult.Failure(new ElementExecuteException("Property '{0}' was found but is not a list element.", propertyData.Name));
            }

            var itemNumber = actionContext.ItemNumber;
            var item = propertyData.GetItemAtIndex(itemNumber - 1);

            return item == null
                       ? ActionResult.Failure(
                           new ElementExecuteException(
                             "Could not find item {0} on list '{1}'",
                             itemNumber,
                             propertyData.Name))
                       : ActionResult.Successful(item);
        }

        /// <summary>
        /// A context for the get list by index action.
        /// </summary>
        public class ListItemByIndexContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ListItemByIndexContext"/> class.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="itemNumber">The item number.</param>
            public ListItemByIndexContext(string propertyName, int itemNumber)
                : base(propertyName)
            {
                this.ItemNumber = itemNumber;
            }

            /// <summary>
            /// Gets the item number.
            /// </summary>
            /// <value>The item number.</value>
            public int ItemNumber { get; private set; }
        }
    }
}