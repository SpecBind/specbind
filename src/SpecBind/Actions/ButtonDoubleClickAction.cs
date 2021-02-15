// <copyright file="ButtonDoubleClickAction.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.Helpers;

    /// <summary>
    /// An action that performs a button double-click
    /// </summary>
    internal class ButtonDoubleClickAction : ActionBase
    {
        /// <summary>
        /// Initializes static members of the <see cref="ButtonDoubleClickAction"/> class.
        /// </summary>
        static ButtonDoubleClickAction()
        {
            var configSection = SettingHelper.GetConfigurationSection();
            WaitForStillElementBeforeClicking = configSection.Application.WaitForStillElementBeforeClicking;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonDoubleClickAction" /> class.
        /// </summary>
        public ButtonDoubleClickAction()
            : base(typeof(ButtonDoubleClickAction).Name)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to wait for the element to exist before clicking.
        /// </summary>
        /// <value>
        /// <c>true</c> if [wait for still element before clicking]; otherwise, <c>false</c>.
        /// </value>
        protected internal static bool WaitForStillElementBeforeClicking { get; set; }

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

            propertyData.DoubleClickElement();
            return ActionResult.Successful();
        }
    }
}
