// <copyright file="ExceptionHelper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
    using System;

    /// <summary>
    /// A helper class to call methods that throw exceptions.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class ExceptionHelper
    {
        /// <summary>
        /// Calls the testAction method and runs any optionally post validation.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="testAction">The test action.</param>
        /// <param name="validationCheck">The validation check.</param>
        public static void SetupForException<TException>(Action testAction, Action<TException> validationCheck = null)
            where TException : Exception
        {
            try
            {
                testAction();
            }
            catch (TException ex)
            {
                validationCheck?.Invoke(ex);

                throw;
            }
        }
    }
}