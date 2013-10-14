// <copyright file="ButtonClickAction.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
	using SpecBind.ActionPipeline;

	/// <summary>
	/// An action that performs a button click
	/// </summary>
	public class ButtonClickAction : ActionBase
	{
		private readonly string propertyName;

		/// <summary>
		/// Initializes a new instance of the <see cref="ButtonClickAction" /> class.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		public ButtonClickAction(string propertyName) : base("Item Click")
		{
			this.propertyName = propertyName;
		}

		/// <summary>
		/// Executes this instance action.
		/// </summary>
		/// <returns>The result of the action.</returns>
		public override ActionResult Execute()
		{
			var propertyData = this.ElementLocator.GetElement(this.propertyName);
			propertyData.ClickElement();

			return ActionResult.Successful();
		}
	}
}