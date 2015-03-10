// <copyright file="SetCookiePreAction.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Actions
{
    using System;

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
            if (!(action is PageNavigationAction))
            {
                return;
            }

            this.logger.Debug("Navigation action detected, checking for cookie...");

            var propertyName = context.PropertyName;
            var type = this.pageMapper.GetTypeFromName(propertyName);

            SetCookieAttribute attribute;
            if (type == null || !type.TryGetAttribute(out attribute))
            {
                return;
            }

            // Convert the date attribute
            DateTime? expires = null;

            var expireString = attribute.Expires;
            if (!string.IsNullOrWhiteSpace(expireString))
            {
                if (string.Equals("DateTime.MinValue", expireString, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.logger.Debug("Setting cookie expiration to minimum value");
                    expires = DateTime.MinValue;
                }
                else if (string.Equals("DateTime.MaxValue", expireString, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.logger.Debug("Setting cookie expiration to maximum value");
                    expires = DateTime.MaxValue;
                }
                else
                {
                    DateTime expireParse;
                    int numericParse;
                    if (DateTime.TryParse(expireString, out expireParse))
                    {
                        this.logger.Debug("Setting cookie expiration to: {0}", expireParse);
                        expires = expireParse;
                    }
                    else if (int.TryParse(expireString, out numericParse))
                    {
                        expires = DateTime.Now.AddSeconds(numericParse);
                        this.logger.Debug("Setting cooking expiration to now plus {0} seconds. Date: {1}", numericParse, expires);
                    }
                    else
                    {
                        this.logger.Info("Cannot parse cookie date: {0}", expireString);
                    }
                }
            }

            this.logger.Debug("Setting Cookie: {0}", attribute);
            this.browser.AddCookie(
                attribute.Name,
                attribute.Value,
                attribute.Path,
                expires,
                attribute.Domain,
                attribute.IsSecure);
        }
    }
}