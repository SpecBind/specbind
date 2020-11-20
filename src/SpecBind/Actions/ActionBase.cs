﻿// <copyright file="ActionBase.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System;
    using SpecBind.ActionPipeline;

    /// <summary>
    /// A base class for an action in the pipeline.
    /// </summary>
    public abstract class ActionBase : IAction
    {
        private readonly string actionName;

        /// <summary>
        /// Initializes the ActionBase type.
        /// </summary>
        static ActionBase()
        {
            DefaultTimeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionBase"/> class.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        protected ActionBase(string actionName)
        {
            this.actionName = actionName;
        }

        /// <summary>
        /// Gets or sets the default timeout to wait.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public static TimeSpan DefaultTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to keep retrying validations, until the timeout.
        /// </summary>
        public static bool RetryValidationUntilTimeout { get; set; }

        /// <summary>
        /// Gets the action name.
        /// </summary>
        /// <value>The action name.</value>
        public string Name
        {
            get { return this.actionName; }
        }

        /// <summary>
        /// Gets or sets the element locator.
        /// </summary>
        /// <value>The element locator.</value>
        public IElementLocator ElementLocator { protected get; set; }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
	    public abstract ActionResult Execute(ActionContext actionContext);
    }
}