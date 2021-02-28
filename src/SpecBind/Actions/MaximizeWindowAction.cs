// <copyright file="MaximizeWindowAction.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.BrowserSupport;
    using SpecBind.Pages;

    /// <summary>
    /// Maximize Window Action
    /// </summary>
    public class MaximizeWindowAction : ActionBase
    {
        private readonly ILogger logger;
        private readonly IPageMapper pageMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaximizeWindowAction" /> class.
        /// </summary>
        /// <param name="pageMapper">The page mapper.</param>
        /// <param name="logger">The logger.</param>
        public MaximizeWindowAction(IPageMapper pageMapper, ILogger logger)
            : base(typeof(MaximizeWindowAction).Name)
        {
            this.pageMapper = pageMapper;
            this.logger = logger;
        }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>
        /// The result of the action.
        /// </returns>
        public override ActionResult Execute(ActionContext actionContext)
        {
            IBrowser browser = WebDriverSupport.CurrentBrowser;

            browser.Maximize();

            return ActionResult.Successful();
        }
    }
}
