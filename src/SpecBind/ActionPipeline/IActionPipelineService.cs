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
		/// <param name="page">The page.</param>
		/// <param name="action">The action.</param>
		/// <returns>The result of the action</returns>
		ActionResult PerformAction(IPage page, IAction action);
	}
}