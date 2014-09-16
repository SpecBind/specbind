// <copyright file="ActionRepository.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.ActionPipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BoDi;

    using SpecBind.Validation;

    /// <summary>
	/// The action repository for plugins in the pipeline.
	/// </summary>
	internal class ActionRepository : IActionRepository
	{
        private readonly IObjectContainer objectContainer;

        private readonly List<IPreAction> preActions;
        private readonly List<IPostAction> postActions;
        private readonly List<ILocatorAction> locatorActions;
        private readonly List<IValidationComparer> validationComparisons;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRepository" /> class.
        /// </summary>
        /// <param name="objectContainer">The object container.</param>
        public ActionRepository(IObjectContainer objectContainer)
	    {
            this.objectContainer = objectContainer;

            this.preActions = new List<IPreAction>(5);
            this.postActions = new List<IPostAction>(5);
            this.locatorActions = new List<ILocatorAction>(5);

            this.validationComparisons = new List<IValidationComparer>(10);
	    }

        /// <summary>
        /// Creates the action.
        /// </summary>
        /// <typeparam name="TAction">The type of the action.</typeparam>
        /// <returns>The created action object.</returns>
        public TAction CreateAction<TAction>()
        {
            return this.CreateItem<TAction>(typeof(TAction));
        }

	    /// <summary>
		/// Gets the post-execute actions.
		/// </summary>
		/// <returns>An enumerable collection of actions.</returns>
		public IEnumerable<IPostAction> GetPostActions()
		{
			return this.postActions.AsReadOnly();
		}

		/// <summary>
		/// Gets the pre-execute actions.
		/// </summary>
		/// <returns>An enumerable collection of actions.</returns>
		public IEnumerable<IPreAction> GetPreActions()
		{
			return this.preActions.AsReadOnly();
		}

        /// <summary>
        /// Gets the comparison actions used to process various types.
        /// </summary>
        /// <returns>An enumerable collection of actions.</returns>
        public IReadOnlyCollection<IValidationComparer> GetComparisonTypes()
        {
            return this.validationComparisons.AsReadOnly();
        }

		/// <summary>
		/// Gets the locator actions.
		/// </summary>
		/// <returns>An enumerable collection of actions.</returns>
		public IEnumerable<ILocatorAction> GetLocatorActions()
		{
			return this.locatorActions.AsReadOnly();
		}

        /// <summary>
        /// Initializes this instance.
        /// </summary>
	    public void Initialize()
        {
            // Get all items from the current assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && !a.GlobalAssemblyCache);
            foreach (var asmType in assemblies.SelectMany(a => a.GetExportedTypes()).Where(t => !t.IsAbstract && !t.IsInterface))
            {
                this.RegisterType(asmType);
            }
	    }

        /// <summary>
        /// Registers the type in the pipeline.
        /// </summary>
        /// <param name="type">The type to register in the pipeline.</param>
        public void RegisterType(Type type)
        {
            if (typeof(IPreAction).IsAssignableFrom(type))
            {
                this.preActions.Add(this.CreateItem<IPreAction>(type));
            }

            if (typeof(IPostAction).IsAssignableFrom(type))
            {
                this.postActions.Add(this.CreateItem<IPostAction>(type));
            }

            if (typeof(ILocatorAction).IsAssignableFrom(type))
            {
                this.locatorActions.Add(this.CreateItem<ILocatorAction>(type));
            }

            if (typeof(IValidationComparer).IsAssignableFrom(type))
            {
                this.validationComparisons.Add(this.CreateItem<IValidationComparer>(type));
            }
        }

        /// <summary>
        /// Creates the item through the DI container.
        /// </summary>
        /// <typeparam name="T">The type of the created item.</typeparam>
        /// <param name="concreteType">Type of the concrete.</param>
        /// <returns>The created item.</returns>
        private T CreateItem<T>(Type concreteType)
        {
            return (T)this.objectContainer.Resolve(concreteType);
        }
	}
}