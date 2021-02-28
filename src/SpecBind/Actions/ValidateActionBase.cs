// <copyright file="ValidateActionBase.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using System;
    using SpecBind.ActionPipeline;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
    /// A convenience class for validation actions that may support retrying until a given timeout.
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public abstract class ValidateActionBase<TContext> : ContextActionBase<TContext>
        where TContext : ActionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateActionBase{TContext}"/> class.
        /// </summary>
        /// <param name="concreteClassName">The name of the implementing class.</param>
        public ValidateActionBase(string concreteClassName) : base(concreteClassName)
        {
        }

        /// <summary>
        /// Perform standard handling around user-supplied core validation logic.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="arg">The argument to pass to the validator.</param>
        /// <param name="validator">The underlying validation method.</param>
        /// <remarks>If RetryValidationUntilTimeout is set, the validator will be called
        /// until it returns <c>true</c> or the timeout expires.</remarks>
        protected void DoValidate<T>(T arg, Func<T, bool> validator)
        {
            if (!RetryValidationUntilTimeout)
            {
                validator(arg);
                return;
            }

            int attemptsCompleted = 0;
            try
            {
                var waiter = new Waiter<T>(DefaultTimeout);
                waiter.WaitFor(arg, e =>
                    {
                        bool result = validator(e);
                        attemptsCompleted++;
                        if (!result)
                        {
                            LogDebug(() => "    (expected condition not yet met)");
                        }

                        return result;
                    });
            }
            catch (TimeoutException)
            {
                if (attemptsCompleted == 0)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="validation">The validation.</param>
        /// <param name="itemResult">The item result.</param>
        /// <returns><c>true</c> if the validation is successful, <c>false</c> otherwise.</returns>
        protected bool ValidateProperty(ItemValidation validation, ValidationItemResult itemResult)
        {
            IPropertyData propertyData;
            if (!this.ElementLocator.TryGetProperty(validation.FieldName, out propertyData))
            {
                itemResult.NoteMissingProperty(validation);
                return false;
            }

            string actualValue = null;
            bool? successful = null;

            this.DoValidate<IPropertyData>(propertyData, e =>
            {
                successful = e.ValidateItem(validation, out actualValue);
                return successful.Value;
            });

            itemResult.NoteValidationResult(validation, successful.Value, actualValue);
            return successful.Value;
        }

        private static void LogDebug(Func<string> messageGenerator)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine(messageGenerator());
        }
    }
}