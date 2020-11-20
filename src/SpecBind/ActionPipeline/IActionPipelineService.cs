// <copyright file="IActionPipelineService.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.ActionPipeline
{
    using SpecBind.Pages;

    /// <summary>
    /// Represents the action pipeline that performs actions.
    /// </summary>
    public interface IActionPipelineService
    {
        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <typeparam name="TAction">The type of the action that inherits from <see cref="IAction"/>.</typeparam>
        /// <param name="page">The page.</param>
        /// <param name="context">The context.</param>
        /// <returns>The result of the action.</returns>
        ActionResult PerformAction<TAction>(IPage page, ActionContext context)
            where TAction : IAction;

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="action">The action.</param>
        /// <param name="context">The context.</param>
        /// <returns>The result of the action.</returns>
	    ActionResult PerformAction(IPage page, IAction action, ActionContext context);
    }
}