// <copyright file="ComparisonTestBase.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Tests.Validation
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.Pages;
    using SpecBind.Validation;

    /// <summary>
    /// A test class base for item comparison classes.
    /// </summary>
    /// <typeparam name="T">The type of the comparison.</typeparam>
    public abstract class ComparisonTestBase<T>
        where T : IValidationComparer, new()
    {
        /// <summary>
        /// Runs the item compare test.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="isTrue">if set to <c>true</c> the result should be true.</param>
        /// <param name="propertyData">The property data.</param>
        protected static void RunItemCompareTest(string expectedValue, string actualValue, bool isTrue, Mock<IPropertyData> propertyData = null)
        {
            propertyData = propertyData ?? new Mock<IPropertyData>(MockBehavior.Strict);

            var validation = new T();

            var result = validation.Compare(propertyData.Object, expectedValue, actualValue);

            Assert.AreEqual(isTrue, result, "Test: {0}, Actual: {1}", expectedValue, actualValue);

            propertyData.VerifyAll();
        }

        /// <summary>
        /// Runs the comparison expecting an exception.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="propertyData">The property data.</param>
        protected static void CheckNotSupported(string expectedValue, string actualValue, Mock<IPropertyData> propertyData = null)
        {
            propertyData = propertyData ?? new Mock<IPropertyData>(MockBehavior.Strict);

            var validation = new T();

            try
            {
                validation.Compare(propertyData.Object, expectedValue, actualValue);
            }
            catch (NotSupportedException)
            {
                propertyData.VerifyAll();
                throw;
            }
        }
    }
}