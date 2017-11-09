// <copyright file="ActionPipelineService.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.ActionPipeline
{
    using System;
	using System.Collections.Generic;
	using System.Linq;

    using BoDi;
    using BrowserSupport;
	using SpecBind.Pages;

	/// <summary>
	/// A class that manages actions that should be taken during parts of the process
	/// </summary>
	/// <remarks>
	/// The pipeline works as follows:
	/// 1. Populate the element locater, this is an extension of this class and can this call actions.
	/// 2. Perform any pre actions (current use is unknown but may be helpful)
	/// 3. Perform the main action and acquire the result
	/// 4. Perform any post actions (current use is unknown but may be helpful)
	/// 5. Return result
	/// </remarks>
	internal class ActionPipelineService : IActionPipelineService
	{
        private readonly IObjectContainer objectContainer;

		private readonly IActionRepository actionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionPipelineService" /> class.
        /// </summary>
        /// <param name="objectContainer">The object container.</param>
        /// <param name="actionRepository">The action repository.</param>
        public ActionPipelineService(IObjectContainer objectContainer, IActionRepository actionRepository)
        {
            this.objectContainer = objectContainer;
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
            // Initialize the browser if not already initialized
            if (!this.objectContainer.IsRegistered<IBrowser>())
            {
                WebDriverSupport.InitializeBrowser(this.objectContainer);
            }

            // Initialize the action repository if not already initialized
            if (!this.actionRepository.IsInitialized)
            {
                this.actionRepository.Initialize();
            }

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
			var locater = this.CreateElementLocater(page);
			action.ElementLocator = locater;

			var result = this.PerformPreAction(action, context);

		    if (result != null)
		    {
		        return result;
		    }

			try
			{
				result = action.Execute(context);
			}
			catch (Exception ex)
			{
				result = ActionResult.Failure(ex);
			}

            this.PerformPostAction(action, context, result);

			return result;
		}

		/// <summary>
		/// Creates the element locater.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <returns>The element locater interface.</returns>
		private IElementLocator CreateElementLocater(IPage page)
		{
			var filterActions = this.actionRepository.GetLocatorActions();
			return new ElementLocator(page, filterActions);
		}

        /// <summary>
        /// Performs any actions ahead of the actual action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="context">The action context.</param>
        /// <returns>If successful <c>null</c>, otherwise an ActionResult with the error.</returns>
	    private ActionResult PerformPreAction(IAction action, ActionContext context)
        {
            var exceptions = new List<Exception>();

			foreach (var preAction in this.actionRepository.GetPreActions())
			{
                try
                {
                    preAction.PerformPreAction(action, context);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
			}

            switch (exceptions.Count)
            {
                case 0:
                    return null;
                case 1:
                    return ActionResult.Failure(exceptions.First());
                default:
                    return ActionResult.Failure(new AggregateException(exceptions));
            }
        }

        /// <summary>
        /// Performs any actions after the actual action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="context">The action context.</param>
        /// <param name="result">The result.</param>
	    private void PerformPostAction(IAction action, ActionContext context, ActionResult result)
		{
			foreach (var postAction in this.actionRepository.GetPostActions())
			{
				postAction.PerformPostAction(action, context, result);
			}
		}
    }
}