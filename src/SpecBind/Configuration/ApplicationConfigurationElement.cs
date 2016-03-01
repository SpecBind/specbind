// <copyright file="ApplicationConfigurationElement.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Configuration
{
	using System.Configuration;

	/// <summary>
	/// A configuration element that handles settings for the application.
	/// </summary>
	public class ApplicationConfigurationElement : ConfigurationElement
	{
		private const string StartUrlElement = @"startUrl";
		private const string ExcludedAssembliesElement = @"excludedAssemblies";
		private const string ActionRetryLimitElement = @"actionRetryLimit";
		private const string WaitForStillElementBeforeClickingElement = @"waitForStillElementBeforeClicking";

		/// <summary>
		/// Gets or sets the application's start URL setting.
		/// </summary>
		/// <value>The application's start URL setting.</value>
		[ConfigurationProperty(StartUrlElement, DefaultValue = null, IsRequired = false)]
		public string StartUrl
		{
			get
			{
				return (string)this[StartUrlElement];
			}

			set
			{
				this[StartUrlElement] = value;
			}
		}

		/// <summary>
		/// Gets or sets the application's retry limit for actions in steps (defaults to 0).
		/// </summary>
		/// <value>The retry limit.</value>
		/// <remarks>
		/// Waits for 1 second between retries.
		/// Experimental; use for working around general async flakiness.
		/// </remarks>
		[ConfigurationProperty(ActionRetryLimitElement, DefaultValue = 0, IsRequired = false)]
		public int ActionRetryLimit
		{
			get
			{
				return (int)this[ActionRetryLimitElement];
			}

			set
			{
				this[ActionRetryLimitElement] = value;
			}
		}

		/// <summary>
		/// Gets or sets whether or not to wait for an element to stop moving before clicking on it (defaults to false).
		/// </summary>
		/// <value>Whether or not to wait.</value>
		/// <remarks>
		/// Waits 200ms between element position measures.
		/// Resolves issues where animation moves a button for a while, when it first becomes available.
		/// Without this workaround, the click simply ends up sent to a nearby element or the page, usually doing nothing
		/// and causing the next step verifying navigation or other click effect to timeout.
		/// </remarks>
		[ConfigurationProperty(WaitForStillElementBeforeClickingElement, DefaultValue = false, IsRequired = false)]
		public bool WaitForStillElementBeforeClicking
		{
			get
			{
				return (bool)this[WaitForStillElementBeforeClickingElement];
			}

			set
			{
				this[WaitForStillElementBeforeClickingElement] = value;
			}
		}

		/// <summary>
		/// Gets the list of excluded assemblies, if any are supplied.
		/// </summary>
		/// <value>The list of excluded assemblies.</value>
		[ConfigurationProperty(ExcludedAssembliesElement, IsRequired = false)]
		[ConfigurationCollection(typeof(AssemblyElement))]
		public AssemblyCollection ExcludedAssemblies
		{
			get { return (AssemblyCollection)base[ExcludedAssembliesElement]; }
		}
	}
}