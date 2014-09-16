// <copyright file="DismissDialogAction.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System;

    using SpecBind.ActionPipeline;
    using SpecBind.BrowserSupport;
    using SpecBind.Pages;

    /// <summary>
    /// An action that dismisses a dialog on the screen.
    /// </summary>
    internal class DismissDialogAction : ContextActionBase<DismissDialogAction.DismissDialogContext>
    {
        private readonly IBrowser browser;

        /// <summary>
        /// Initializes a new instance of the <see cref="DismissDialogAction" /> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public DismissDialogAction(IBrowser browser)
            : base(typeof(DismissDialogAction).Name)
        {
            this.browser = browser;
        }

        /// <summary>
        /// Executes the specified action using the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(DismissDialogContext context)
        {
            var action = ParseAction(context.ButtonName);
            var text = context.IsTextEntered ? context.Text ?? string.Empty : null;

            if (!action.HasValue)
            {
                var availableActions = string.Join(", ", Enum.GetNames(typeof(AlertBoxAction)));
                return ActionResult.Failure(
                    new ElementExecuteException(
                        "Could not translate '{0}' into a known dialog action. Available Actions: {1}",
                        context.ButtonName,
                        availableActions));
            }

            this.browser.DismissAlert(action.Value, text);
            
            return ActionResult.Successful();
        }

        /// <summary>
        /// Parses the action.
        /// </summary>
        /// <param name="buttonName">Name of the button.</param>
        /// <returns>The parsed action name.</returns>
        private static AlertBoxAction? ParseAction(string buttonName)
        {
            AlertBoxAction action;
            if (buttonName != null && Enum.TryParse(buttonName.Trim(), true, out action))
            {
                return action;
            }

            return null;
        }

        /// <summary>
        /// Class DismissDialogContext.
        /// </summary>
        public class DismissDialogContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DismissDialogContext"/> class.
            /// </summary>
            /// <param name="buttonName">Name of the button.</param>
            public DismissDialogContext(string buttonName)
                : base(null)
            {
                this.ButtonName = buttonName;
                this.IsTextEntered = false;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DismissDialogContext" /> class.
            /// </summary>
            /// <param name="buttonName">Name of the button.</param>
            /// <param name="text">The text.</param>
            public DismissDialogContext(string buttonName, string text)
                : base(null)
            {
                this.ButtonName = buttonName;
                this.IsTextEntered = true;
                this.Text = text;
            }

            /// <summary>
            /// Gets a value indicating whether this instance is text entered.
            /// </summary>
            /// <value><c>true</c> if this instance is text entered; otherwise, <c>false</c>.</value>
            public bool IsTextEntered { get; private set; }
            
            /// <summary>
            /// Gets the name of the button to choose.
            /// </summary>
            /// <value>The name of the button.</value>
            public string ButtonName { get; private set; }

            /// <summary>
            /// Gets the text to input.
            /// </summary>
            /// <value>The text.</value>
            public string Text { get; private set; }
        }
    }
}