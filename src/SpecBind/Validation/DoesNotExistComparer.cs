// <copyright file="DoesNotExistComparer.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Validation
{
    using SpecBind.Pages;

    /// <summary>
    /// A validation comparer to see if something does not exist.
    /// </summary>
    public class DoesNotExistComparer : ValidationComparerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoesNotExistComparer"/> class.
        /// </summary>
        public DoesNotExistComparer()
            : base("doesnotexist")
        {
        }

        /// <summary>
        /// Gets a value indicating whether this validation requires a field value.
        /// </summary>
        /// <value><c>true</c> if a field value is required; otherwise, <c>false</c>.</value>
        public override bool RequiresFieldValue
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Determines wither the element should be checked for existence.
        /// </summary>
        /// <value><c>true</c> if the element should be checked; otherwise, <c>false</c>.</value>
        public override bool ShouldCheckElementExistence
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Compares the values using the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <returns><c>true</c> if the comparison passes, <c>false</c> otherwise.</returns>
        public override bool Compare(IPropertyData property, string expectedValue, string actualValue)
        {
            bool parsedValue;
            return (expectedValue != null && bool.TryParse(expectedValue, out parsedValue) && !parsedValue)
                       ? property.CheckElementExists()
                       : !property.CheckElementExists();
        }
    }
}