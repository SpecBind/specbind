// <copyright file="ElementExecuteException.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
    using System;

    /// <summary>
    /// An exception that indicates a failure occurred while executing a step element.
    /// </summary>
    [Serializable]
    public class ElementExecuteException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementExecuteException" /> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public ElementExecuteException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementExecuteException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public ElementExecuteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}