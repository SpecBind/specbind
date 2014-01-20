// <copyright file="ProxyLogger.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.BrowserSupport
{
    using SpecBind.Actions;

    using TechTalk.SpecFlow.Tracing;

    /// <summary>
    /// A logger class that proxies values to the SpecFlow infrastructure.
    /// </summary>
    internal class ProxyLogger : ILogger
    {
        private readonly ITraceListener traceListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyLogger"/> class.
        /// </summary>
        /// <param name="traceListener">The trace listener.</param>
        public ProxyLogger(ITraceListener traceListener)
        {
            this.traceListener = traceListener;
        }

        /// <summary>
        /// Logs the debug level message.
        /// </summary>
        /// <param name="format">The format for the message.</param>
        /// <param name="args">The arguments for the message.</param>
        public void Debug(string format, params object[] args)
        {
            this.traceListener.WriteTestOutput("SpecBind Debug: {0}", (object)string.Format(format, args));
        }

        /// <summary>
        /// Logs the information level message.
        /// </summary>
        /// <param name="format">The format for the message.</param>
        /// <param name="args">The arguments for the message.</param>
        public void Info(string format, params object[] args)
        {
            this.traceListener.WriteTestOutput("SpecBind Info: {0}", (object)string.Format(format, args));
        }
    }
}