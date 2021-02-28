// <copyright file="NativeMethods.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Native Methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Sets the foreground window.
        /// </summary>
        /// <param name="hWnd">The window handle.</param>
        /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Query information about a process.
        /// </summary>
        /// <param name="processHandle">The process handle.</param>
        /// <param name="processInformationClass">The process information class.</param>
        /// <param name="processInformation">The process information.</param>
        /// <param name="processInformationLength">Length of the process information.</param>
        /// <param name="returnLength">Length of the return.</param>
        /// <returns>An NTSTATUS success or error code.</returns>
        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(
            IntPtr processHandle,
            int processInformationClass,
            ref PROCESS_BASIC_INFORMATION processInformation,
            int processInformationLength,
            out int returnLength);

        /// <summary>
        /// Basic Process Information
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_BASIC_INFORMATION
        {
            /// <summary>
            /// The exit code of the process. If the process has not exited, this is set to STILL_ACTIVE (259).
            /// </summary>
            public IntPtr ExitStatus;

            /// <summary>
            /// The address to the process environment block (PEB). Reserved for use by the operating system.
            /// </summary>
            public IntPtr PebBaseAddress;

            /// <summary>
            /// The affinity mask of the process.
            /// </summary>
            public IntPtr AffinityMask;

            /// <summary>
            /// The base priority level of the process.
            /// </summary>
            public IntPtr BasePriority;

            /// <summary>
            /// The unique process identifier
            /// </summary>
            public IntPtr UniqueProcessId;

            /// <summary>
            /// The parent process's unique process ID.
            /// </summary>
            public IntPtr InheritedFromUniqueProcessId;
        }
    }
}
