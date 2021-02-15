// <copyright file="ValidatePageParametersAction.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web;
    using SpecBind.ActionPipeline;
    using SpecBind.BrowserSupport;

    /// <summary>
    /// Validate Page Parameters Action
    /// </summary>
    public class ValidatePageParametersAction : ContextActionBase<ValidatePageParametersAction.ValidatePageParametersActionContext>
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatePageParametersAction" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ValidatePageParametersAction(
            ILogger logger)
            : base(typeof(ValidatePageParametersAction).Name)
        {
            this.logger = logger;
        }

        /// <summary>
        /// The type of page parameter validation action to be performed.
        /// </summary>
        public enum PageParameterValidationAction
        {
            /// <summary>
            /// Validates the specified page parameters are contained in the URL.
            /// </summary>
            Contains,

            /// <summary>
            /// Validates the specified page parameters are not contained in the URL.
            /// </summary>
            DoesNotContain
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The result of the action.</returns>
        protected override ActionResult Execute(ValidatePageParametersActionContext context)
        {
            Dictionary<string, string> expectedParameters = context.PageParameters;

            IBrowser browser = WebDriverSupport.CurrentBrowser;

            string url = browser.Url;
            this.logger.Debug($"Validating page parameters in URL: {url}");

            Uri uri = new Uri(url);

            NameValueCollection queryString = HttpUtility.ParseQueryString(uri.Query);
            IDictionary<string, string> actualParameters = queryString.AllKeys.ToDictionary(x => x, x => queryString[x]);

            foreach (string key in expectedParameters.Keys)
            {
                if (context.PageParameterValidationAction == PageParameterValidationAction.Contains)
                {
                    if (!actualParameters.ContainsKey(key))
                    {
                        return ActionResult.Failure(new Exception($"Parameter key '{key}' was not found in query string '{queryString.ToString()}'."));
                    }

                    if (expectedParameters[key] != actualParameters[key])
                    {
                        string[] errorMessage = new[]
                        {
                            $"Value of parameter key '{key}' does not match.",
                            $"Expected: <{expectedParameters[key]}>",
                            $"Actual: <{actualParameters[key]}>."
                        };

                        return ActionResult.Failure(new Exception(string.Join(Environment.NewLine, errorMessage)));
                    }
                }
                else
                {
                    if (actualParameters.ContainsKey(key))
                    {
                        return ActionResult.Failure(new Exception($"Parameter key '{key}' was found in query string '{queryString.ToString()}'."));
                    }
                }
            }

            return ActionResult.Successful();
        }

        /// <summary>
        /// Validate Page Parameters Action Context
        /// </summary>
        /// <seealso cref="SpecBind.ActionPipeline.ActionContext" />
        public class ValidatePageParametersActionContext : ActionContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValidatePageParametersActionContext" /> class.
            /// </summary>
            /// <param name="pageParameterValidationAction">The page parameter validation action.</param>
            /// <param name="pageParameters">The page parameters.</param>
            public ValidatePageParametersActionContext(PageParameterValidationAction pageParameterValidationAction, Dictionary<string, string> pageParameters)
                : base(null)
            {
                this.PageParameterValidationAction = pageParameterValidationAction;
                this.PageParameters = pageParameters;
            }

            /// <summary>
            /// Gets or sets the page parameter validation action.
            /// </summary>
            /// <value>
            /// The page parameter validation action.
            /// </value>
            internal PageParameterValidationAction PageParameterValidationAction { get; set; }

            /// <summary>
            /// Gets or sets the page parameters.
            /// </summary>
            /// <value>
            /// The page parameters.
            /// </value>
            internal Dictionary<string, string> PageParameters { get; set; }
        }
    }
}
