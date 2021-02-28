// <copyright file="IpHelper.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.ProcessHelper
{
    using System;
    using System.Net.NetworkInformation;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <see href="http://msdn2.microsoft.com/en-us/library/aa366073.aspx"/>
    /// </summary>
    internal static class IpHelper
    {
        /// <summary>
        /// The af inet
        /// </summary>
        public const int AfInet = 2;

        private const string DllName = "iphlpapi.dll";

        /// <summary>
        /// <see href="http://msdn2.microsoft.com/en-us/library/aa366386.aspx"/>
        /// </summary>
        public enum TcpTableType
        {
            /// <summary>
            /// A MIB_TCPTABLE table that contains all listening (receiving only) TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            BasicListener,

            /// <summary>
            /// A MIB_TCPTABLE table that contains all connected TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            BasicConnections,

            /// <summary>
            /// A MIB_TCPTABLE table that contains all TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            BasicAll,

            /// <summary>
            /// A MIB_TCPTABLE_OWNER_PID or MIB_TCP6TABLE_OWNER_PID that contains all listening (receiving only) TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            OwnerPidListener,

            /// <summary>
            /// A MIB_TCPTABLE_OWNER_PID or MIB_TCP6TABLE_OWNER_PID that structure that contains all connected TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            OwnerPidConnections,

            /// <summary>
            /// A MIB_TCPTABLE_OWNER_PID or MIB_TCP6TABLE_OWNER_PID structure that contains all TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            OwnerPidAll,

            /// <summary>
            /// A MIB_TCPTABLE_OWNER_MODULE or MIB_TCP6TABLE_OWNER_MODULE structure that contains all listening (receiving only) TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            OwnerModuleListener,

            /// <summary>
            /// A MIB_TCPTABLE_OWNER_MODULE or MIB_TCP6TABLE_OWNER_MODULE structure that contains all connected TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            OwnerModuleConnections,

            /// <summary>
            /// A MIB_TCPTABLE_OWNER_MODULE or MIB_TCP6TABLE_OWNER_MODULE structure that contains all TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            OwnerModuleAll,
        }

        /// <summary>
        ///   <see href="http://msdn2.microsoft.com/en-us/library/aa365928.aspx" />
        /// </summary>
        /// <param name="tcpTable">The TCP table.</param>
        /// <param name="tcpTableLength">Length of the TCP table.</param>
        /// <param name="sort">if set to <c>true</c> sorts the table.</param>
        /// <param name="ipVersion">The ip version.</param>
        /// <param name="tcpTableType">Type of the TCP table.</param>
        /// <param name="reserved">Reserved. This value must be zero.</param>
        /// <returns>If the call is successful, the value NO_ERROR is returned.
        /// If the function fails, the return value is one of the following error codes.
        /// ERROR_INSUFFICIENT_BUFFER
        /// An insufficient amount of space was allocated for the table. The size of the table is returned in the pdwSize parameter, and must be used in a subsequent call to this function in order to successfully retrieve the table.
        /// This error is also returned if the pTcpTable parameter is NULL.
        /// ERROR_INVALID_PARAMETER
        /// An invalid parameter was passed to the function.This error is returned if the TableClass parameter contains a value that is not defined in the TCP_TABLE_CLASS enumeration. </returns>
        [DllImport(IpHelper.DllName, SetLastError = true)]
        public static extern uint GetExtendedTcpTable(IntPtr tcpTable, ref int tcpTableLength, bool sort, int ipVersion, TcpTableType tcpTableType, int reserved);

        /// <summary>
        /// <see href="http://msdn2.microsoft.com/en-us/library/aa366921.aspx"/>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TcpTable
        {
            /// <summary>
            /// The length
            /// </summary>
            public uint Length;

            /// <summary>
            /// The row
            /// </summary>
            public TcpRow Row;
        }

        /// <summary>
        /// <see href="http://msdn2.microsoft.com/en-us/library/aa366913.aspx"/>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TcpRow
        {
            /// <summary>
            /// The state
            /// </summary>
            public TcpState State;

            /// <summary>
            /// The local addr
            /// </summary>
            public uint LocalAddr;

            /// <summary>
            /// The local port1
            /// </summary>
            public byte LocalPort1;

            /// <summary>
            /// The local port2
            /// </summary>
            public byte LocalPort2;

            /// <summary>
            /// The local port3
            /// </summary>
            public byte LocalPort3;

            /// <summary>
            /// The local port4
            /// </summary>
            public byte LocalPort4;

            /// <summary>
            /// The remote addr
            /// </summary>
            public uint RemoteAddr;

            /// <summary>
            /// The remote port1
            /// </summary>
            public byte RemotePort1;

            /// <summary>
            /// The remote port2
            /// </summary>
            public byte RemotePort2;

            /// <summary>
            /// The remote port3
            /// </summary>
            public byte RemotePort3;

            /// <summary>
            /// The remote port4
            /// </summary>
            public byte RemotePort4;

            /// <summary>
            /// The owning pid
            /// </summary>
            public int OwningPid;
        }
    }
}
