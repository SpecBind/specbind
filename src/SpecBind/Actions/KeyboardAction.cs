// <copyright file="KeyboardAction.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System;
    using BrowserSupport;
    using SpecBind.ActionPipeline;

    /// <summary>
    /// Keyboard Action.
    /// </summary>
    internal class KeyboardAction : ContextActionBase<KeyboardAction.KeyboardActionContext>
    {
        private readonly IBrowser browser;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardAction" /> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public KeyboardAction(IBrowser browser)
            : base(typeof(KeyboardAction).Name)
        {
            this.browser = browser;
        }

        /// <summary>
        /// Keyboard send type
        /// </summary>
        public enum KeyboardSendType
        {
            /// <summary>
            /// Default press
            /// </summary>
            Default = 0,

            /// <summary>
            /// Press keys
            /// </summary>
            Press = 1,

            /// <summary>
            /// Release keys
            /// </summary>
            Release = 2
        }

        /// <summary>
        /// Executes the specified action using the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(KeyboardActionContext context)
        {
            switch (context.KeyboardSendType)
            {
                case KeyboardSendType.Default:
                    this.browser.SendKeys(context.Data);
                    break;
                case KeyboardSendType.Press:
                    this.browser.PressKeys(context.Data);
                    break;
                case KeyboardSendType.Release:
                    this.browser.ReleaseKeys(context.Data);
                    break;
                default:
                    throw new NotImplementedException(context.KeyboardSendType.ToString());
            }

            return ActionResult.Successful();
        }

        /// <summary>
        /// An extended context class to pass keyboard data.
        /// </summary>
        public class KeyboardActionContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="KeyboardActionContext" /> class.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="keyboardSendType">Type of the keyboard send.</param>
            public KeyboardActionContext(string data, KeyboardSendType keyboardSendType)
                : base(null)
            {
                this.Data = data;
                this.KeyboardSendType = keyboardSendType;
            }

            /// <summary>
            /// Gets the data that should be entered.
            /// </summary>
            /// <value>The data that should be entered.</value>
            public string Data { get; private set; }

            /// <summary>
            /// Gets the type of the keyboard send.
            /// </summary>
            /// <value>
            /// The type of the keyboard send.
            /// </value>
            public KeyboardSendType KeyboardSendType { get; private set; }
        }
    }
}
