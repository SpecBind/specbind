// <copyright file="ProcessExtensions.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// Process Extensions.
    /// </summary>
    public static class ProcessExtensions
    {
        /// <summary>
        /// Waits for the main window.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <returns>The handle to the main window.</returns>
        public static string WaitForMainWindow(this Process process)
        {
            // refresh process to guarantee that we'll retrieve the current handle
            process.Refresh();

            // TODO: use a timeout value
            while (((!process.HasExited)
                && (process.MainWindowHandle == IntPtr.Zero))
                || string.IsNullOrEmpty(process.MainWindowTitle))
            {
                Thread.Sleep(100);
                process.Refresh();
            }

            if (process.HasExited)
            {
                return null;
            }

            // convert main window handle to hex
            return process.MainWindowHandle.ToHex();
        }
    }
}
