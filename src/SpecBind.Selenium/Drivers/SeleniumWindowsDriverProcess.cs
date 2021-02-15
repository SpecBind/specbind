// <copyright file="SeleniumWindowsDriverProcess.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Drivers
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using Alphaleonis.Win32.Filesystem;
    using Helpers;

    /// <summary>
    /// Selenium Windows Driver Process.
    /// </summary>
    public class SeleniumWindowsDriverProcess
    {
        private readonly string exeFilePath;
        private readonly Uri uri;
        private readonly bool saveLog;
        private readonly string logFilePath;
        private readonly TestResultFileNotifier testResultFileNotifier;

        private string previouslyReceivedMessage = null;
        private bool newLinePreviouslyWritten;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumWindowsDriverProcess" /> class.
        /// </summary>
        /// <param name="exeFilePath">The windows application driver executable file path.</param>
        /// <param name="uri">The windows application driver URI.</param>
        /// <param name="saveLog">if set to <c>true</c> [save log].</param>
        /// <param name="logFilePath">The log file path.</param>
        /// <param name="testResultFileNotifier">The test result file notifier.</param>
        public SeleniumWindowsDriverProcess(
            string exeFilePath,
            Uri uri,
            bool saveLog,
            string logFilePath,
            TestResultFileNotifier testResultFileNotifier)
        {
            this.exeFilePath = exeFilePath;
            this.uri = uri;
            this.saveLog = saveLog;
            this.logFilePath = logFilePath;
            this.testResultFileNotifier = testResultFileNotifier;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
        public bool IsRunning
        {
            get
            {
                return (this.Process != null)
                    && (!this.Process.HasExited);
            }
        }

        private Process Process { get; set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        internal void Start()
        {
            if (this.IsRunning)
            {
                return;
            }

            // the /forcequit parameter is specified to force kill the given application process when the window refuses to close
            string commandLineArguments = $"{this.uri.Host} {this.uri.Port}{this.uri.PathAndQuery} /forcequit";

            ProcessStartInfo processStartInfo = new ProcessStartInfo(
                this.exeFilePath,
                commandLineArguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,

                // redirect the standard input stream to avoid WinAppDriver from exiting immediately after launching
                // See https://github.com/Microsoft/WinAppDriver/issues/588#issuecomment-586811140
                RedirectStandardInput = true,
                RedirectStandardOutput = this.saveLog,
                RedirectStandardError = this.saveLog,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            Logger.Log($"Launching \"{processStartInfo.FileName}\" {processStartInfo.Arguments}");

            this.Process = new Process()
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = this.saveLog
            };

            if (this.saveLog)
            {
                this.Process.ErrorDataReceived += this.OnDataReceived;
                this.Process.OutputDataReceived += this.OnDataReceived;
            }

            this.Process.Start();

            if (this.saveLog)
            {
                this.Process.BeginOutputReadLine();
                this.Process.BeginErrorReadLine();
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        internal void Stop()
        {
            if ((this.Process != null) && this.IsRunning)
            {
                Logger.Log($"Stopping process \"{this.Process.StartInfo.FileName}\".");

                this.Process.Kill();

                this.Process.WaitForExit();

                this.AddTestResultFiles();
            }
        }

        /// <summary>
        /// Waits for the process to exit.
        /// </summary>
        internal void WaitForExit()
        {
            if ((this.Process != null) && this.IsRunning)
            {
                Logger.Log($"Waiting for process \"{this.Process.StartInfo.FileName}\" to exit.");

                this.Process.WaitForExit();
            }
        }

        /// <summary>
        /// Adds the test result files.
        /// </summary>
        internal void AddTestResultFiles()
        {
            if ((!this.saveLog)
                || string.IsNullOrEmpty(this.logFilePath)
                || (!File.Exists(this.logFilePath)))
            {
                return;
            }

            lock (this.logFilePath)
            {
                this.testResultFileNotifier?.AddTestResultFile(this.logFilePath);
            }
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            string utf8Message = e.Data.TrimStart('\0');

            if ((this.previouslyReceivedMessage == null)
                && (utf8Message == string.Empty)
                && !this.newLinePreviouslyWritten)
            {
                this.Log(string.Empty);
                this.previouslyReceivedMessage = utf8Message;
                this.newLinePreviouslyWritten = true;
                return;
            }
            else if ((this.previouslyReceivedMessage == string.Empty)
                && (utf8Message == string.Empty))
            {
                // do not write output
                this.previouslyReceivedMessage = null;
                return;
            }
            else if (utf8Message == string.Empty)
            {
                // do not write output
                this.previouslyReceivedMessage = utf8Message;
                return;
            }

            this.previouslyReceivedMessage = utf8Message;
            this.newLinePreviouslyWritten = false;

            byte[] bytes = Encoding.UTF8.GetBytes(utf8Message);
            string unicodeMessage = Encoding.Unicode.GetString(bytes);

            this.Log(unicodeMessage);
        }

        private void Log(string unicodeMessage)
        {
            Logger.Log(unicodeMessage);
            this.WriteToLogFile(unicodeMessage);
        }

        private void WriteToLogFile(string unicodeMessage)
        {
            lock (this.logFilePath)
            {
                File.AppendAllText(this.logFilePath, unicodeMessage + Environment.NewLine, Encoding.Unicode);
            }
        }
    }
}
