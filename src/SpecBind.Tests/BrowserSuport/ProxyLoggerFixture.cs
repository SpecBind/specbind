// <copyright file="ProxyLoggerFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.BrowserSuport
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.BrowserSupport;

    using TechTalk.SpecFlow.Tracing;

    /// <summary>
    /// A test fixture for the proxy logger
    /// </summary>
    [TestClass]
    public class ProxyLoggerFixture
    {
        /// <summary>
        /// Tests the log debug method writes to trace listener.
        /// </summary>
        [TestMethod]
        public void TestLogDebugWritesToTraceListener()
        {
            var traceListener = new Mock<ITraceListener>(MockBehavior.Strict);
            traceListener.Setup(t => t.WriteTestOutput("SpecBind Debug: Hello World!"));

            var proxyLogger = new ProxyLogger(traceListener.Object);

            proxyLogger.Debug("Hello {0}", "World!");

            traceListener.VerifyAll();
        }

        /// <summary>
        /// Tests the log information method writes to trace listener.
        /// </summary>
        [TestMethod]
        public void TestLogInfoWritesToTraceListener()
        {
            var traceListener = new Mock<ITraceListener>(MockBehavior.Strict);
            traceListener.Setup(t => t.WriteTestOutput("SpecBind Info: Hello World!"));

            var proxyLogger = new ProxyLogger(traceListener.Object);

            proxyLogger.Info("Hello {0}", "World!");

            traceListener.VerifyAll();
        }
    }
}
