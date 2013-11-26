// <copyright file="ActionPipelineService.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.ActionPipeline
{
	using System;
    
    using SpecBind.Pages;

	/// <summary>
	/// A class that manages actions that should be taken during parts of the process
	/// </summary>
	/// <remarks>
	/// The pipeline works as follows:
	/// 1. Populate the element locator, this is an extension of this class and can this call actions.
	/// 2. Perform any pre actions (current use is unknown but may be helpful)
	/// 3. Perform the main action and acquire the result
	/// 4. Perform any post actions (current use is unknown but may be helpful)
	/// 5. Return result
	/// </remarks>
	internal class ActionPipelineService : IActionPipelineService
	{
		private readonly IActionRepository actionRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionPipelineService"/> class.
		/// </summary>
		/// <param name="actionRepository">The action repository.</param>
		public ActionPipelineService(IActionRepository actionRepository)
		{
			this.actionRepository = actionRepository;
		}

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <typeparam name="TAction">The type of the action.</typeparam>
        /// <param name="page">The page.</param>
        /// <param name="context">The context.</param>
        /// <returns>The result of the action.</returns>
	    public ActionResult PerformAction<TAction>(IPage page, ActionContext context) 
            where TAction : IAction
        {
            var action = this.actionRepository.CreateAction<TAction>();
            return this.PerformAction(page, action, context);
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="action">The action.</param>
        /// <param name="context">The context.</param>
        /// <returns>The result of the action</returns>
	    public ActionResult PerformAction(IPage page, IAction action, ActionContext context)
		{
			var locator = this.CreateElementLocator(page);
			action.ElementLocator = locator;

			this.PerformPreAction(action);

			ActionResult result;
			try
			{
				result = action.Execute(context);
			}
			catch (Exception ex)
			{
				result = ActionResult.Failure(ex);
			}

			this.PerformPostAction(action, result);

			return result;
		}

		/// <summary>
		/// Creates the element locator.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <returns>The element locator interface.</returns>
		private IElementLocator CreateElementLocator(IPage page)
		{
			var filterActions = this.actionRepository.GetLocatorActions();
			return new ElementLocator(page, filterActions);
		}

		/// <summary>
		/// Performs any actions ahead of the actual action.
		/// </summary>
		/// <param name="action">The action.</param>
		private void PerformPreAction(IAction action)
		{
			foreach (var preAction in this.actionRepository.GetPreActions())
			{
				preAction.PerformPreAction(action);
			}
		}

		/// <summary>
		/// Performs any actions after the actual action.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="result">The result.</param>
		private void PerformPostAction(IAction action, ActionResult result)
		{
			foreach (var postAction in this.actionRepository.GetPostActions())
			{
				postAction.PerformPostAction(action, result);
			}
		}
	}
}