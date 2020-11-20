// <copyright file="IPostAction.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.ActionPipeline
{
    /// <summary>
    /// An extension that can interact with an action after it occurs.
    /// </summary>
    public interface IPostAction
    {
        /// <summary>
        /// Performs the post-execute action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="context">The action context.</param>
        /// <param name="result">The result.</param>
	    void PerformPostAction(IAction action, ActionContext context, ActionResult result);
    }
}