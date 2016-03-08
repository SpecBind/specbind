// <copyright file="ExpressionData.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// An smart structure for managing expressions and matching types.
    /// </summary>
    public class ExpressionData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionData" /> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="type">The type.</param>
        /// <param name="name">The name of the expression.</param>
        public ExpressionData(Expression expression, Type type, string name = null)
        {
            this.Expression = expression;
            this.Name = name;
            this.Type = type;
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Gets the name of the expression if supplied.
        /// </summary>
        /// <value>The name of the expression.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; private set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("Type: {0}{1}", this.Type.FullName, this.Name != null ? string.Format(", Name: {0}", this.Name) : string.Empty);
        }
    }
}