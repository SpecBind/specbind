// <copyright file="IAction.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.ActionPipeline
{
    /// <summary>
    /// Represents an action in the system.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Gets the action name.
        /// </summary>
        /// <value>The action name.</value>
        string Name { get; }

        /// <summary>
        /// Sets the element locator.
        /// </summary>
        /// <value>The element locator.</value>
        IElementLocator ElementLocator { set; }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
	    ActionResult Execute(ActionContext actionContext);
    }
}