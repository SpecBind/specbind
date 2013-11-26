// <copyright file="ButtonClickAction.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
	using SpecBind.ActionPipeline;

	/// <summary>
	/// An action that performs a button click
	/// </summary>
	internal class ButtonClickAction : ActionBase
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonClickAction" /> class.
        /// </summary>
		public ButtonClickAction() : base("Item Click")
		{
		}

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
	    public override ActionResult Execute(ActionContext actionContext)
		{
			var propertyData = this.ElementLocator.GetElement(actionContext.PropertyName);
			propertyData.ClickElement();

			return ActionResult.Successful();
		}
	}
}