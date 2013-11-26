// <copyright file="ActionContext.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.ActionPipeline
{
    /// <summary>
    /// A class that defines the data that is needed to perform the action.
    /// </summary>
    public class ActionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionContext"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public ActionContext(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; private set; }
    }
}