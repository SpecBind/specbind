// <copyright file="SetCookiePreAction.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A pre-action that sets the cookie in the browser if defined.
    /// </summary>
    public class SetCookiePreAction : IPreAction
    {
        private readonly IBrowser browser;
        private readonly ILogger logger;
        private readonly IPageMapper pageMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetCookiePreAction" /> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="pageMapper">The page mapper.</param>
        public SetCookiePreAction(IBrowser browser, ILogger logger, IPageMapper pageMapper)
        {
            this.browser = browser;
            this.logger = logger;
            this.pageMapper = pageMapper;
        }

        /// <summary>
        /// Performs the pre-execute action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="context">The action context.</param>
        public void PerformPreAction(IAction action, ActionContext context)
        {
            var propertyName = context.PropertyName;
            var type = this.pageMapper.GetTypeFromName(propertyName);

            SetCookieAttribute attribute;
            if (type == null || !type.TryGetAttribute(out attribute))
            {
                return;
            }

            this.logger.Debug("Setting Cookie: {0}", attribute);
            this.browser.AddCookie(
                attribute.Name,
                attribute.Value,
                attribute.Path,
                attribute.Expires,
                attribute.Domain,
                attribute.IsSecure);
        }
    }
}