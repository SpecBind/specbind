// <copyright file="NullLogger.cs">
//    Copyright © 2014 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.BrowserSupport
{
    using SpecBind.Actions;

    /// <summary>
    /// A logger class that does nothing.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class NullLogger : ILogger
    {
        /// <summary>
        /// Logs the debug level message.
        /// </summary>
        /// <param name="format">The format for the message.</param>
        /// <param name="args">The arguments for the message.</param>
        public void Debug(string format, params object[] args)
        {
        }

        /// <summary>
        /// Logs the information level message.
        /// </summary>
        /// <param name="format">The format for the message.</param>
        /// <param name="args">The arguments for the message.</param>
        public void Info(string format, params object[] args)
        {
        }
    }
}