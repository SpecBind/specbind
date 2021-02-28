// <copyright file="PlaybackSteps.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.CodedUI
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Playback Steps.
    /// </summary>
    [Binding]
    public class PlaybackSteps
    {
        /// <summary>
        /// Runs before executing each scenario.
        /// </summary>
        [BeforeScenario]
        public void Before()
        {
            if (!Playback.IsInitialized)
            {
                Playback.Initialize();
            }

            Playback.PlaybackSettings.LoggerOverrideState = HtmlLoggerState.AllActionSnapshot;
        }

        /// <summary>
        /// Runs after executing each scenario.
        /// </summary>
        [AfterScenario]
        public void After()
        {
            if (Playback.IsInitialized)
            {
                Playback.Cleanup();
            }
        }
    }
}
