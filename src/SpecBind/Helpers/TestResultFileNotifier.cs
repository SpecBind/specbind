// <copyright file="TestResultFileNotifier.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
    using System;

    /// <summary>
    /// Test Result File Notifier.
    /// </summary>
    public class TestResultFileNotifier
    {
        /// <summary>
        /// Occurs when a test result file was created.
        /// </summary>
        public event EventHandler<TestResultFileCreatedEventArgs> TestResultFileCreated;

        /// <summary>
        /// Adds the test result file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void AddTestResultFile(string filePath)
        {
            TestResultFileCreatedEventArgs args = new TestResultFileCreatedEventArgs
            {
                TestResultFilePath = filePath
            };

            this.OnTestResultFileCreated(args);
        }

        /// <summary>
        /// Handles the <see cref="E:TestResultFileCreated" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TestResultFileCreatedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTestResultFileCreated(TestResultFileCreatedEventArgs e)
        {
            this.TestResultFileCreated?.Invoke(this, e);
        }
    }
}
