// <copyright file="HighlightPreAction.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A pre-action that highlights elements when enabled.
    /// </summary>
    public sealed class HighlightPreAction : ILocatorAction
    {
        /// <summary>
        /// The highlight mode tag constant.
        /// </summary>
        public const string HighlightMode = "Highlight";

        private readonly IScenarioContextHelper contextHelper;
        private readonly ISettingHelper settingHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightPreAction" /> class.
        /// </summary>
        /// <param name="contextHelper">The context helper.</param>
        /// <param name="settingHelper">The setting helper.</param>
        public HighlightPreAction(IScenarioContextHelper contextHelper, ISettingHelper settingHelper)
        {
            this.contextHelper = contextHelper;
            this.settingHelper = settingHelper;
        }

        /// <summary>
        /// Called when an element is about to be located.
        /// </summary>
        /// <param name="key">The element key.</param>
        public void OnLocate(string key)
        {
            // Pre-locate isn't used here
        }

        /// <summary>
        /// Called when an element is completed.
        /// </summary>
        /// <param name="key">The element key.</param>
        /// <param name="item">The item if located; otherwise <c>null</c>.</param>
        public void OnLocateComplete(string key, IPropertyData item)
        {
            if (item == null || !this.HighlightModeEnabled())
            {
                return;
            }

            item.Highlight();
        }

        /// <summary>
        /// Checks to see if highlighting mode is enabled
        /// </summary>
        /// <returns><c>true</c> if the mode is enabled; otherwise <c>false</c>.</returns>
        private bool HighlightModeEnabled()
        {
            return this.settingHelper.HighlightModeEnabled
                   || this.contextHelper.FeatureContainsTag(HighlightMode)
                   || this.contextHelper.ContainsTag(HighlightMode);
        }
    }
}