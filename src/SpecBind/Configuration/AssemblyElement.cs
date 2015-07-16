namespace SpecBind.Configuration
{
	using System.Configuration;

	/// <summary>
	/// The configuration element that handles an assembly configuration element.
	/// </summary>
	public class AssemblyElement : ConfigurationElement
	{
		private const string NameKey = @"name";

		/// <summary>
		/// Gets or sets the assembly's name.
		/// </summary>
		/// <value>The assembly's name.</value>
		[ConfigurationProperty(NameKey, IsRequired = true)]
		public string Name
		{
			get
			{
				return (string)this[NameKey];
			}
			set
			{
				this[NameKey] = value;
			}
		}
	}
}
