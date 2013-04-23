// <copyright file="PageMapperFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using SpecBind.Pages;
	using SpecBind.Tests.Support;

	/// <summary>
	/// A test fixture for the <see cref="PageMapper" /> class.
	/// </summary>
	[TestClass]
	public class PageMapperFixture
	{
		/// <summary>
		/// Tests to ensure a normal type will not be mapped.
		/// </summary>
		[TestMethod]
		public void TestMapAssemblyTypesWithNormalType()
		{
			var mapper = new PageMapper();
			mapper.MapAssemblyTypes(new[] { typeof(string) }, typeof(TestBase));

			Assert.AreEqual(0, mapper.MapCount);
		}

		/// <summary>
		/// Tests to ensure a page class will be mapped by name.
		/// </summary>
		[TestMethod]
		public void TestMapAssemblyTypesWithNonPrefixedPageName()
		{
			var mapper = new PageMapper();
			mapper.MapAssemblyTypes(new[] { typeof(NoName) }, typeof(TestBase));

			var type = mapper.GetTypeFromName("noname");

			Assert.IsNotNull(type);
			Assert.AreEqual(1, mapper.MapCount);
		}

		/// <summary>
		/// Tests to ensure a page class will be mapped by name removing page from it.
		/// </summary>
		[TestMethod]
		public void TestMapAssemblyTypesWithPrefixedPageName()
		{
			var mapper = new PageMapper();
			mapper.MapAssemblyTypes(new[] { typeof(MyPage) }, typeof(TestBase));

			var type = mapper.GetTypeFromName("my");
			var shouldNotExist = mapper.GetTypeFromName("mypage");

			Assert.IsNotNull(type);
			Assert.IsNull(shouldNotExist);
			Assert.AreEqual(1, mapper.MapCount);
		}

		/// <summary>
		/// Tests to ensure a page class will be mapped by name and any aliases.
		/// </summary>
		[TestMethod]
		public void TestMapAssemblyTypesWithPrefixedPageNameAndAlias()
		{
			var mapper = new PageMapper();
			mapper.MapAssemblyTypes(new[] { typeof(AliasPage) }, typeof(TestBase));

			var type = mapper.GetTypeFromName("alias");
			var aliasType = mapper.GetTypeFromName("another item");

			Assert.IsNotNull(type);
			Assert.IsNotNull(aliasType);
			Assert.AreEqual(2, mapper.MapCount);
		}

		/// <summary>
		/// Tests to ensure the GetTypeFromName method return null if the string is null or empty.
		/// </summary>
		[TestMethod]
		public void TestGetTypeFromNameNullString()
		{
			var mapper = new PageMapper();
			var nullType = mapper.GetTypeFromName(null);
			var emptyType = mapper.GetTypeFromName(string.Empty);
			var whitespaceType = mapper.GetTypeFromName("    ");

			Assert.IsNull(nullType);
			Assert.IsNull(emptyType);
			Assert.IsNull(whitespaceType);
		}

		#region Class - NoName

		/// <summary>
		/// A non-named page class
		/// </summary>
		private class NoName : TestBase
		{
		}

		#endregion

		#region Class - MyPage

		/// <summary>
		/// A non-named page class
		/// </summary>
		private class MyPage : TestBase
		{
		}

		#endregion

		#region Class - AliasPage

		/// <summary>
		/// A non-named page class
		/// </summary>
		[PageAlias("AnotherItem")]
		private class AliasPage : TestBase
		{
		}

		#endregion
	}
}