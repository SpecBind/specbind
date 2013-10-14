// <copyright file="IPreAction.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.ActionPipeline
{
	/// <summary>
	/// An extension that can interact with an action before it occurs.
	/// </summary>
	public interface IPreAction
	{
		/// <summary>
		/// Performs the pre-execute action.
		/// </summary>
		/// <param name="action">The action.</param>
		void PerformPreAction(IAction action);
	}
}