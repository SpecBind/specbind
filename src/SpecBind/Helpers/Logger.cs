// <copyright file="Logger.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Logger.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public static void Log(Exception ex)
        {
            Log(ex.ToString());
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Log(string message)
        {
            Trace.WriteLine($"[{DateTime.Now}] {message}");
        }
    }
}
