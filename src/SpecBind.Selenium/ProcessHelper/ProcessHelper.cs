// <copyright file="ProcessHelper.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.ProcessHelper
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using OpenQA.Selenium;
    using static SpecBind.Selenium.NativeMethods;
    using static SpecBind.Selenium.ProcessHelper.ManagedIpHelper;

    /// <summary>
    /// Process Helper
    /// </summary>
    internal static class ProcessHelper
    {
        /// <summary>
        /// Filters the child processes.
        /// </summary>
        /// <param name="processes">The processes.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <returns>The child processes.</returns>
        public static IEnumerable<Process> FilterChildProcesses(this IEnumerable<Process> processes, int parentId)
        {
            return processes.Where(p => GetParentProcess(p.Handle).Id == parentId);
        }

        /// <summary>
        /// Gets the PID from port.
        /// </summary>
        /// <param name="hubLocalPort">The hub local port.</param>
        /// <returns>The OID.</returns>
        public static int GetPidFromPort(int hubLocalPort)
        {
            TcpRow tcpRow = GetExtendedTcpTable(true).FirstOrDefault(x => x.LocalEndPoint.Port == hubLocalPort);

            if (tcpRow != null)
            {
                return tcpRow.ProcessId;
            }

            throw new NotFoundException(string.Format("Driver hub port {0} not found", hubLocalPort));
        }

        /// <summary>
        /// Gets the parent process of specified process.
        /// </summary>
        /// <param name="id">The process id.</param>
        /// <returns>An instance of the Process class.</returns>
        internal static Process GetParentProcess(int id)
        {
            Process process = Process.GetProcessById(id);
            return GetParentProcess(process.Handle);
        }

        /// <summary>
        /// Gets the parent process of a specified process.
        /// </summary>
        /// <param name="handle">The process handle.</param>
        /// <returns>An instance of the Process class.</returns>
        internal static Process GetParentProcess(IntPtr handle)
        {
            PROCESS_BASIC_INFORMATION pbi = default(PROCESS_BASIC_INFORMATION);
            int status = NativeMethods.NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out int returnLength);
            if (status != 0)
            {
                throw new Win32Exception(status);
            }

            try
            {
                return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
            }
            catch (ArgumentException)
            {
                // not found
                return null;
            }
        }
    }
}
