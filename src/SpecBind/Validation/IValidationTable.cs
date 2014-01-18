// <copyright file="IValidationTable.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Validation
{
    /// <summary>
    /// An interface that indicates that an action has a validation table as part of it.
    /// </summary>
    public interface IValidationTable
    {
        /// <summary>
        /// Gets the validation table.
        /// </summary>
        /// <value>The validation table.</value>
        ValidationTable ValidationTable { get; }
    }
}