// <copyright file="IActionRepository.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
// <copyright file="IActionRepository.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.ActionPipeline
{
    using System.Collections.Generic;

    using SpecBind.Validation;

    /// <summary>
	/// Contains a cache of available actions, pre-actions and post-actions.
	/// </summary>
	public interface IActionRepository
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Creates the action.
        /// </summary>
        /// <typeparam name="TAction">The type of the action.</typeparam>
        /// <returns>The created action object.</returns>
        TAction CreateAction<TAction>();

        /// <summary>
        /// Gets the post-execute actions.
        /// </summary>
        /// <returns>An enumerable collection of actions.</returns>
        IEnumerable<IPostAction> GetPostActions();

        /// <summary>
        /// Gets the pre-execute actions.
        /// </summary>
        /// <returns>An enumerable collection of actions.</returns>
        IEnumerable<IPreAction> GetPreActions();

        /// <summary>
        /// Gets the locator actions.
        /// </summary>
        /// <returns>An enumerable collection of actions.</returns>
        IEnumerable<ILocatorAction> GetLocatorActions();

        /// <summary>
        /// Gets the comparison actions used to process various types.
        /// </summary>
        /// <returns>An enumerable collection of actions.</returns>
        IReadOnlyCollection<IValidationComparer> GetComparisonTypes();
    }
}