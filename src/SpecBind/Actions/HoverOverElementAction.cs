namespace SpecBind.Actions
{
	using System;
	using SpecBind.ActionPipeline;

	/// <summary>
	/// An action that performs a hover over an element
	/// </summary>
	internal class HoverOverElementAction : ActionBase
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonClickAction" /> class.
        /// </summary>
		public HoverOverElementAction()
			: base(typeof(HoverOverElementAction).Name)
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
            propertyData.WaitForElementCondition(WaitConditions.NotMoving, timeout: null);
            propertyData.WaitForElementCondition(WaitConditions.BecomesEnabled, timeout: null);

			try
			{
				propertyData.ClickElement();
			}
			catch (Exception ex)
			{
				if (ex.Message.Contains("Element is not clickable at point"))
				{
					// Starting with Selenium WebDriver 2.48, we get this is if a popup opens on hover.
					// Since we're just hovering and don't actually want to click the element, swallow this exception.
					return ActionResult.Successful();
				}

				throw;
			}

			return ActionResult.Successful();
		}
	}
}