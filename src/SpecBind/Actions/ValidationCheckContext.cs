// <copyright file="ValidationCheckContext.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;

    /// <summary>
    /// Class ValidationCheckContext.
    /// </summary>
    internal class ValidationCheckContext : ActionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationCheckContext"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="shouldExist">if set to <c>true</c> the element should exist.</param>
        public ValidationCheckContext(string propertyName, bool shouldExist)
            : base(propertyName)
        {
            this.ShouldExist = shouldExist;
        }

        /// <summary>
        /// Gets a value indicating whether the should exist or have a positive outcome.
        /// </summary>
        /// <value><c>true</c> if  it should should exist; otherwise, <c>false</c>.</value>
        public bool ShouldExist { get; private set; }
    }
}