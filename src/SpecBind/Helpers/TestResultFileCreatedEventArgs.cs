// <copyright file="TestResultFileCreatedEventArgs.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
    /// <summary>
    /// Test Result File Created Event Arguments.
    /// </summary>
    public class TestResultFileCreatedEventArgs
    {
        /// <summary>
        /// Gets or sets the test result file path.
        /// </summary>
        /// <value>The test result file path.</value>
        public string TestResultFilePath { get; set; }
    }
}