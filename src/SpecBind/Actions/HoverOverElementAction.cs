namespace SpecBind.Actions
{
	using System;
	using SpecBind.ActionPipeline;
	using SpecBind.Helpers;

	/// <summary>
	/// An action that performs a hover over an element
	/// </summary>
	internal class HoverOverElementAction : ActionBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HoverOverElementAction" /> class.
		/// </summary>
		public HoverOverElementAction()
			: base(typeof(HoverOverElementAction).Name)
		{
		}

		protected internal static bool WaitForStillElementBeforeClicking { get; set; }

		static HoverOverElementAction()
		{
			var configSection = SettingHelper.GetConfigurationSection();
			WaitForStillElementBeforeClicking = configSection.Application.WaitForStillElementBeforeClicking;
		}

		/// <summary>
		/// Executes this instance action.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		/// <returns>The result of the action.</returns>
		public override ActionResult Execute(ActionContext actionContext)
		{
			var propertyData = this.ElementLocator.GetElement(actionContext.PropertyName);

			if (WaitForStillElementBeforeClicking)
			{
				propertyData.WaitForElementCondition(WaitConditions.NotMoving, timeout: null);
				propertyData.WaitForElementCondition(WaitConditions.BecomesEnabled, timeout: null);
			}

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