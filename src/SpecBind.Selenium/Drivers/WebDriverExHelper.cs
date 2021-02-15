// <copyright file="WebDriverExHelper.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using SpecBind.Helpers;
    using SpecBind.Selenium.Extensions;
    using SpecBind.Selenium.ProcessHelper;

    /// <summary>
    /// Web Driver Extended Helper
    /// </summary>
    public static class WebDriverExHelper
    {
        /// <summary>
        /// Gets the child process.
        /// </summary>
        /// <param name="parentProcessId">The parent process identifier.</param>
        /// <param name="browserProcessName">Name of the browser process.</param>
        /// <returns>The child process.</returns>
        public static Process GetChildProcess(int parentProcessId, string browserProcessName)
        {
            // get all the browser processes
            bool foundChildProcess = false;
            int retryCount = 1;
            int maxRetryCount = 5;
            List<Process> childProcesses = new List<Process>();
            while ((!foundChildProcess) && (retryCount <= maxRetryCount))
            {
                Process[] browserProcesses = Process.GetProcessesByName(browserProcessName);
                if (browserProcesses.Length == 0)
                {
                    throw new Exception($"Could not find any processes with name '{browserProcessName}'.");
                }

                foreach (Process browserProcess in browserProcesses)
                {
                    Logger.Log($"Found process '{browserProcess.Id}' with parent process '{ProcessHelper.GetParentProcess(browserProcess.Handle).Id}', main window title '{browserProcess.MainWindowTitle}', and main window handle '{browserProcess.MainWindowHandle}'.");
                }

                // Filter process by parent (driver pid)

                // maybe use selenium currentHandle to identify the correct browser, page title, etc.
                // in case if multiple browser are linked to single hub
                childProcesses = browserProcesses.FilterChildProcesses(parentProcessId).ToList();
                foundChildProcess = childProcesses.Count >= 1;
                if (!foundChildProcess)
                {
                    Thread.Sleep(1000);
                }

                retryCount++;
            }

            if (childProcesses.Count < 1)
            {
                throw new Exception($"Could not find any child processes of parent process '{parentProcessId}' with name '{browserProcessName}' after {maxRetryCount} tries.");
            }

            if (childProcesses.Count > 1)
            {
                throw new Exception($"Found more than one child process of parent process '{parentProcessId}' with name '{browserProcessName}'.");
            }

            return childProcesses.Single();
        }

        /// <summary>
        /// Gets the main browser window handle.
        /// </summary>
        /// <param name="parentProcessId">The parent process identifier.</param>
        /// <param name="browserProcessName">Name of the browser process.</param>
        /// <returns>The main browser window handle.</returns>
        public static string GetMainBrowserWindowHandle(int parentProcessId, string browserProcessName)
        {
            var childProcess = GetChildProcess(parentProcessId, browserProcessName);

            string mainWindowHandle = childProcess.WaitForMainWindow();

            Logger.Log($"Found child process '{childProcess.Id}' with main window title '{childProcess.MainWindowTitle}' and main window handle '{mainWindowHandle}'.");

            return mainWindowHandle;
        }
    }
}
