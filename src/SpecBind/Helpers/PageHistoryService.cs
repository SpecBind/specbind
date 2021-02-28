// <copyright file="PageHistoryService.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BoDi;
    using SpecBind.Actions;
    using SpecBind.Pages;

    /// <summary>
    /// Page History Service.
    /// </summary>
    public class PageHistoryService
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageHistoryService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public PageHistoryService(ILogger logger)
        {
            this.logger = logger;
            this.PageHistory = new Dictionary<Type, IPage>();
        }

        /// <summary>
        /// Gets or sets the page history.
        /// </summary>
        /// <value>The page history.</value>
        internal Dictionary<Type, IPage> PageHistory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPage"/> with the specified page type.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <returns>The page.</returns>
        public IPage this[Type pageType]
        {
            get
            {
                return this.PageHistory[pageType];
            }
        }

        /// <summary>
        /// Determines whether the specified page is in the history.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns><c>true</c> if the specified page is in the history; otherwise, <c>false</c>.</returns>
        public bool Contains(IPage page)
        {
            return this.Contains(page.PageType);
        }

        /// <summary>
        /// Determines whether the specified page is in the history.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <returns><c>true</c> if the specified page is in the history; otherwise, <c>false</c>.</returns>
        public bool Contains(Type pageType)
        {
            return this.PageHistory.ContainsKey(pageType);
        }

        /// <summary>
        /// Adds the specified page to the history.
        /// </summary>
        /// <param name="page">The page.</param>
        public void Add(IPage page)
        {
            this.PageHistory.Add(page.PageType, page);
        }

        /// <summary>
        /// Removes the specified page from the history.
        /// </summary>
        /// <param name="page">The page.</param>
        public void Remove(IPage page)
        {
            this.PageHistory.Remove(page.PageType);
        }

        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <returns>The page.</returns>
        public IPage GetCurrentPage()
        {
            return this.PageHistory.Last().Value;
        }

        /// <summary>
        /// Gets the page history service.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The page history service.</returns>
        internal static PageHistoryService GetPageHistoryService(IObjectContainer container)
        {
            var configSection = SettingHelper.GetConfigurationSection();
            if (configSection == null || configSection.PageHistoryService == null || string.IsNullOrWhiteSpace(configSection.PageHistoryService.Provider))
            {
                return null;
            }

            var type = Type.GetType(configSection.PageHistoryService.Provider, AssemblyLoader.OnAssemblyCheck, AssemblyLoader.OnGetType);
            if (type == null || !typeof(PageHistoryService).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(string.Format("Could not load type: {0}. Make sure this is fully qualified and the assembly exists. Also ensure the base type is PageHistoryService", configSection.PageHistoryService.Provider));
            }

            var pageHistoryService = (PageHistoryService)container.Resolve(type);

            return pageHistoryService;
        }

        /// <summary>
        /// Finds the page containing the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The page.</returns>
        internal IPage FindPageContainingProperty(string propertyName)
        {
            return this.FindPageContainingProperty(propertyName, false, false, false);
        }

        /// <summary>
        /// Finds the page with the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The page.</returns>
        internal IPage FindPage(string propertyName)
        {
            return this.FindPageContainingProperty(propertyName, true, true, true);
        }

        private IPage FindPageContainingProperty(
            string propertyName,
            bool returnItemAsPage,
            bool returnPageIfFound,
            bool removeOtherPagesFromHistory)
        {
            // try to find the property name starting with the current page
            IPage foundPage = null;
            int pageIndex = this.PageHistory.Count - 1;
            while (pageIndex >= 0)
            {
                KeyValuePair<Type, IPage> page = this.PageHistory.ElementAt(pageIndex);

                this.logger.Debug($"Looking in page '{page.Key.Name}' for property with name '{propertyName}'...");

                IPropertyData propertyData;
                if (page.Value.TryGetProperty(propertyName, out propertyData))
                {
                    this.logger.Debug($"Property of type '{propertyData.PropertyType.Name}' found.");

                    if (returnItemAsPage)
                    {
                        foundPage = propertyData.GetItemAsPage();
                    }
                    else
                    {
                        foundPage = page.Value;
                    }

                    break;
                }

                if (returnPageIfFound && (page.Key.Name.ToLower() == propertyName))
                {
                    foundPage = page.Value;
                    break;
                }

                pageIndex--;
            }

            if ((foundPage != null) && removeOtherPagesFromHistory)
            {
                // if a page was found, remove the pages from history where the property name wasn't found
                while (pageIndex < this.PageHistory.Count - 1)
                {
                    KeyValuePair<Type, IPage> page = this.PageHistory.Last();
                    if (foundPage.PageType == page.Value.PageType)
                    {
                        break;
                    }

                    this.logger.Debug($"Removing page '{page.Key.Name}' from history...");

                    this.PageHistory.Remove(page.Key);
                }
            }

            return foundPage;
        }
    }
}
