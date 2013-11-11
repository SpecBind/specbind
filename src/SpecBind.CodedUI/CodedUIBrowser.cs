// <copyright file="CodedUIBrowser.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	
	using Microsoft.VisualStudio.TestTools.UITesting;
	using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

	using SpecBind.BrowserSupport;
	using SpecBind.Helpers;
	using SpecBind.Pages;

	/// <summary>
	/// An IBrowser implementation for Coded UI.
	/// </summary>
	// ReSharper disable once InconsistentNaming
    public class CodedUIBrowser : BrowserBase, IDisposable
	{
		private readonly Dictionary<Type, Func<UITestControl, Action<HtmlControl>, HtmlDocument>> pageCache;
		private readonly Lazy<Dictionary<string, Func<UITestControl, HtmlFrame>>> frameCache;
		private readonly Lazy<BrowserWindow> window;
		
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="CodedUIBrowser" /> class.
		/// </summary>
		/// <param name="browserWindow">The browser window.</param>
		public CodedUIBrowser(Lazy<BrowserWindow> browserWindow)
		{
			this.frameCache = new Lazy<Dictionary<string, Func<UITestControl, HtmlFrame>>>(GetFrameCache);
			this.window = browserWindow;
			this.pageCache = new Dictionary<Type, Func<UITestControl, Action<HtmlControl>, HtmlDocument>>();
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="CodedUIBrowser" /> class.
		/// </summary>
		~CodedUIBrowser()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// Gets the type of the base page.
		/// </summary>
		/// <value>
		/// The type of the base page.
		/// </value>
		public override Type BasePageType
		{
			get
			{
				return typeof(HtmlDocument);
			}
		}

		/// <summary>
		/// Closes this instance.
		/// </summary>
        public override void Close()
		{
			if (this.window.IsValueCreated)
			{
				this.window.Value.Close();
			}
		}

		/// <summary>
		/// Navigates the browser to the given <paramref name="url" />.
		/// </summary>
		/// <param name="url">The URL specified as a well formed Uri.</param>
        public override void GoTo(Uri url)
		{
			this.window.Value.NavigateToUrl(url);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || this.disposed)
			{
				return;
			}

			if (this.window.IsValueCreated)
			{
				this.window.Value.Dispose();
			}
			this.disposed = true;
		}

        /// <summary>
        /// Gets the native page location.
        /// </summary>
        /// <param name="page">The page interface.</param>
        /// <returns>A collection of URIs to validate.</returns>
	    protected override IList<string> GetNativePageLocation(IPage page)
        {
            var localWindow = this.window.Value;

	        var pageList = new List<string>
	                           {
	                               localWindow.Uri.ToString()
	                           };

            if (page != null)
            {
                var nativePage = page.GetNativePage<HtmlDocument>();
                if (nativePage != null && nativePage.FrameDocument && nativePage.AbsolutePath != null)
                {
                    pageList.Add(nativePage.AbsolutePath);
                }
            }

            return pageList;
	    }

        /// <summary>
        /// Creates the native page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="verifyPageValidity">if set to <c>true</c> verify the page validity.</param>
        /// <returns>The created page object.</returns>
	    protected override IPage CreateNativePage(Type pageType, bool verifyPageValidity)
	    {
            var nativePage = this.CreateNativePage(pageType);

            if (verifyPageValidity)
            {
                nativePage.Find();
            }

            return new CodedUIPage<HtmlDocument>(nativePage);
	    }

	    /// <summary>
		/// Creates the native page.
		/// </summary>
		/// <param name="pageType">Type of the page.</param>
		/// <returns>The internal document.</returns>
		private HtmlDocument CreateNativePage(Type pageType)
		{
			Func<UITestControl, Action<HtmlControl>, HtmlDocument> function;
			if (!this.pageCache.TryGetValue(pageType, out function))
			{
                function = PageBuilder<UITestControl, HtmlDocument>.CreateElement(pageType);
				this.pageCache.Add(pageType, function);
			}

			UITestControl parentElement = this.window.Value;

			// Check to see if a frames reference exists
			var isFrameDocument = false;
			PageNavigationAttribute navigationAttribute;
			if (pageType.TryGetAttribute(out navigationAttribute) && !string.IsNullOrWhiteSpace(navigationAttribute.FrameName))
			{
				Func<UITestControl, HtmlFrame> frameFunction;
				if (!this.frameCache.Value.TryGetValue(navigationAttribute.FrameName, out frameFunction))
				{
					throw new PageNavigationException("Cannot locate frame with ID '{0}' for page '{1}'", navigationAttribute.FrameName, pageType.Name);
				}

				parentElement = frameFunction(parentElement);
				isFrameDocument = true;

				if (parentElement == null)
				{
					throw new PageNavigationException(
						"Cannot load frame with ID '{0}' for page '{1}'. The property that matched the frame did not return a parent document.",
						navigationAttribute.FrameName,
						pageType.Name);
				}

				
			}

			var documentElement = function(parentElement, null);

			if (isFrameDocument)
			{
				// Set properties that are relevant to the frame.
				documentElement.SearchProperties[HtmlDocument.PropertyNames.FrameDocument] = "True";
				documentElement.SearchProperties[HtmlDocument.PropertyNames.RedirectingPage] = "False";
			}

			return documentElement;
		}

		/// <summary>
		/// Creates the frame cache from the currently loaded types in the project.
		/// </summary>
		/// <returns>The created frame cache.</returns>
		private static Dictionary<string, Func<UITestControl, HtmlFrame>> GetFrameCache()
		{
			var frames = new Dictionary<string, Func<UITestControl, HtmlFrame>>(StringComparer.OrdinalIgnoreCase);

			foreach (var frameType in GetFrameTypes())
			{
				// Check the properties for ones that can produce a frame.
				foreach (var property in frameType.GetProperties()
												  .Where(p => typeof(HtmlFrame).IsAssignableFrom(p.PropertyType) && p.CanRead && !frames.ContainsKey(p.Name)))
				{
                    frames.Add(property.Name, PageBuilder<UITestControl, HtmlFrame>.CreateFrameLocator(frameType, property));
				}
			}

			return frames;
		}

		/// <summary>
		/// Gets the user defined type of class that defines the frame structure.
		/// </summary>
		/// <returns>Any matching types that are the given definition of the frame.</returns>
		private static IEnumerable<Type> GetFrameTypes()
		{
			var frameTypes = new List<Type>();

			try
			{
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					try
					{
						var types = assembly.GetExportedTypes();
						foreach (var type in types)
						{
							try
							{
								if (typeof(UITestControl).IsAssignableFrom(type) && type.GetAttribute<FrameMapAttribute>() != null)
								{
									frameTypes.Add(type);
								}
							}
							catch (SystemException)
							{
							}
						}
					}
					catch (SystemException)
					{
					}
				}
			}
			catch (SystemException)
			{
			}

			return frameTypes;
		}
	}
}