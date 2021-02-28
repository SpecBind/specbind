// <copyright file="ManagedIpHelper.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.ProcessHelper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Managed IP Helper
    /// </summary>
    internal static class ManagedIpHelper
    {
        /// <summary>
        /// Gets the extended TCP table.
        /// </summary>
        /// <param name="sorted">if set to <c>true</c> sorts the table.</param>
        /// <returns>The TCP table.</returns>
        public static TcpTable GetExtendedTcpTable(bool sorted)
        {
            List<TcpRow> tcpRows = new List<TcpRow>();

            IntPtr tcpTable = IntPtr.Zero;
            int tcpTableLength = 0;

            if (IpHelper.GetExtendedTcpTable(tcpTable, ref tcpTableLength, sorted, IpHelper.AfInet, IpHelper.TcpTableType.OwnerPidAll, 0) != 0)
            {
                try
                {
                    tcpTable = Marshal.AllocHGlobal(tcpTableLength);
                    if (IpHelper.GetExtendedTcpTable(tcpTable, ref tcpTableLength, true, IpHelper.AfInet, IpHelper.TcpTableType.OwnerPidAll, 0) == 0)
                    {
                        IpHelper.TcpTable table = (IpHelper.TcpTable)Marshal.PtrToStructure(tcpTable, typeof(IpHelper.TcpTable));

                        IntPtr rowPtr = (IntPtr)((long)tcpTable + Marshal.SizeOf(table.Length));
                        for (int i = 0; i < table.Length; ++i)
                        {
                            tcpRows.Add(new TcpRow((IpHelper.TcpRow)Marshal.PtrToStructure(rowPtr, typeof(IpHelper.TcpRow))));
                            rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(typeof(IpHelper.TcpRow)));
                        }
                    }
                }
                finally
                {
                    if (tcpTable != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(tcpTable);
                    }
                }
            }

            return new TcpTable(tcpRows);
        }

        /// <summary>
        /// TCP Table
        /// </summary>
        public class TcpTable : IEnumerable<TcpRow>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TcpTable"/> class.
            /// </summary>
            /// <param name="tcpRows">The TCP rows.</param>
            public TcpTable(IEnumerable<TcpRow> tcpRows)
            {
                this.Rows = tcpRows;
            }

            /// <summary>
            /// Gets the rows.
            /// </summary>
            /// <value>
            /// The rows.
            /// </value>
            public IEnumerable<TcpRow> Rows { get; }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// An enumerator that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator<TcpRow> GetEnumerator()
            {
                return this.Rows.GetEnumerator();
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
            /// </returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.Rows.GetEnumerator();
            }
        }

        /// <summary>
        /// TCP Row
        /// </summary>
        public class TcpRow
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TcpRow"/> class.
            /// </summary>
            /// <param name="tcpRow">The TCP row.</param>
            public TcpRow(IpHelper.TcpRow tcpRow)
            {
                this.State = tcpRow.State;
                this.ProcessId = tcpRow.OwningPid;

                int localPort = (tcpRow.LocalPort1 << 8) + tcpRow.LocalPort2 + (tcpRow.LocalPort3 << 24) + (tcpRow.LocalPort4 << 16);
                long localAddress = tcpRow.LocalAddr;
                this.LocalEndPoint = new IPEndPoint(localAddress, localPort);

                int remotePort = (tcpRow.RemotePort1 << 8) + tcpRow.RemotePort2 + (tcpRow.RemotePort3 << 24) + (tcpRow.RemotePort4 << 16);
                long remoteAddress = tcpRow.RemoteAddr;
                this.RemoteEndPoint = new IPEndPoint(remoteAddress, remotePort);
            }

            /// <summary>
            /// Gets the local end point.
            /// </summary>
            /// <value>
            /// The local end point.
            /// </value>
            public IPEndPoint LocalEndPoint { get; }

            /// <summary>
            /// Gets the remote end point.
            /// </summary>
            /// <value>
            /// The remote end point.
            /// </value>
            public IPEndPoint RemoteEndPoint { get; }

            /// <summary>
            /// Gets the state.
            /// </summary>
            /// <value>
            /// The state.
            /// </value>
            public TcpState State { get; }

            /// <summary>
            /// Gets the process identifier.
            /// </summary>
            /// <value>
            /// The process identifier.
            /// </value>
            public int ProcessId { get; }
        }
    }
}
