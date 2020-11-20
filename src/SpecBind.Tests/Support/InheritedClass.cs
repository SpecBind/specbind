// <copyright file="InheritedClass.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Tests.Support
{
    using System;

    /// <summary>
	/// A sample inherited class.
	/// </summary>
	public class InheritedClass : BasePageClass
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the button.
        /// </summary>
        /// <value>
        /// The button.
        /// </value>
        public BaseElement Button { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>The date of birth.</value>
	    public DateTime DateOfBirth { get; set; }
    }
}