// <copyright file="PageHistoryException.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.ActionPipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pages;

    /// <summary>
    /// Page History Exception.
    /// </summary>
    /// <seealso cref="System.ApplicationException" />
    public class PageHistoryException : ApplicationException
    {
#pragma warning disable SA1118 // Parameter must not span multiple lines
        /// <summary>
        /// Initializes a new instance of the <see cref="PageHistoryException" /> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="pageHistory">The page history.</param>
        public PageHistoryException(string propertyName, Dictionary<Type, IPage> pageHistory)
                    : base(string.Join(Environment.NewLine, new[]
                    {
                        $"A property with the name '{propertyName}' was not found in any of the displayed pages:",
                        string.Join(Environment.NewLine, pageHistory.Select(x => x.Key.Name))
                    }))
        {
        }
#pragma warning restore SA1118 // Parameter must not span multiple lines
    }
}
