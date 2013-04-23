// <copyright file="IActiveCheck.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Pages
{
	/// <summary>
	/// An interface that can be applied to a page to allow it to wait for it to become active.
	/// </summary>
	public interface IActiveCheck
	{
		/// <summary>
		/// Waits for the page to become active based on a property.
		/// </summary>
		void WaitForActive();
	}
}