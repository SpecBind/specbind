// <copyright file="BrowserType.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.BrowserSupport
{
    /// <summary>
    /// Enumerates the various supported browsers.
    /// </summary>
	public enum BrowserType
	{
		/// <summary>
		/// Internet Explorer
		/// </summary>
		IE = 1,
		
		/// <summary>
		/// FireFox browser
		/// </summary>
		FireFox = 2,

		/// <summary>
		/// Chrome Browser
		/// </summary>
		Chrome = 3,

        /// <summary>
        /// Safari Browser
        /// </summary>
        Safari = 4,

        /// <summary>
        /// Opera Browser
        /// </summary>
        Opera = 5,

        /// <summary>
        /// Android Emulator
        /// </summary>
        Android = 6,

        /// <summary>
        /// iOS iPhone Emulator
        /// </summary>
        // ReSharper disable once InconsistentNaming
        iPhone = 7,

        /// <summary>
        /// iOS iPad Emulator
        /// </summary>
        // ReSharper disable once InconsistentNaming
        iPad = 8,

        /// <summary>
        /// PhantomJS Browser
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PhantomJS = 9,
	}
}