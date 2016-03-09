// <copyright file="ActionPipelineService.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.ActionPipeline
{
	using System;

	using SpecBind.Helpers;
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
		private readonly IActionRepository actionRepository;

        /// <summary>
        /// Initializes the <see cref="ActionPipelineService"/> class.
        /// </summary>
        static ActionPipelineService()
		{
			var configSection = SettingHelper.GetConfigurationSection();
			ConfiguredActionRetryLimit = configSection.Application.ActionRetryLimit;
		}

        /// <summary>
		/// Initializes a new instance of the <see cref="ActionPipelineService"/> class.
		/// </summary>
		/// <param name="actionRepository">The action repository.</param>
		public ActionPipelineService(IActionRepository actionRepository)
        {
            this.actionRepository = actionRepository;
            this.ActionRetryLimit = ConfiguredActionRetryLimit;
        }

        /// <summary>
        /// Gets or sets the number of times to retry a failed action.
        /// </summary>
        public int ActionRetryLimit { get; set; }

        /// <summary>
        /// Gets or sets the configured action retry limit.
        /// </summary>
        /// <value>
        /// The configured action retry limit.
        /// </value>
        protected internal static int ConfiguredActionRetryLimit { get; set; }

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
			var locater = this.CreateElementLocater(page);
			action.ElementLocator = locater;

			this.PerformPreAction(action, context);

			ActionResult result = null;

			int tries = 0;
			do
			{
				if (tries++ > 0)
				{
					System.Threading.Thread.Sleep(1000);
				}

			    try
			    {
				    result = action.Execute(context);
                        if (result.Success)
                        {
                            break;
                        }

			    }
			    catch (Exception ex)
			    {
				    result = ActionResult.Failure(ex);
			    }
			}
			while (tries <= this.ActionRetryLimit);

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
	    private void PerformPreAction(IAction action, ActionContext context)
		{
			foreach (var preAction in this.actionRepository.GetPreActions())
			{
				preAction.PerformPreAction(action, context);
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