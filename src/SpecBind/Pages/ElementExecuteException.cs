// <copyright file="ElementExecuteException.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
	using System;

	/// <summary>
	/// An exception that indicates a failure occurred while executing a step element.
	/// </summary>
	public class ElementExecuteException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ElementExecuteException" /> class.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public ElementExecuteException(string format, params object[] args)
			: base(string.Format(format, args))
		{
		}
	}
}