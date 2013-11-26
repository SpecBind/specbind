// <copyright file="ContextActionBase.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System;

    using SpecBind.ActionPipeline;

    /// <summary>
    /// A base class for actions that converts the context to the desired override type.
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public abstract class ContextActionBase<TContext> : ActionBase
        where TContext : ActionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionBase" /> class.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        protected ContextActionBase(string actionName)
            : base(actionName)
        {
        }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The result of the action.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the context could not be converted to the desired type.</exception>
        public override ActionResult Execute(ActionContext actionContext)
        {
            var newContext = actionContext as TContext;
            if (newContext == null)
            {
                throw new InvalidOperationException(
                    string.Format("Action context could not be converted to type '{0}'", typeof(TContext).FullName));
            }

            return this.Execute(newContext);
        }

        /// <summary>
        /// Executes the action using the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The result of the action.</returns>
        protected abstract ActionResult Execute(TContext context);
    }
}