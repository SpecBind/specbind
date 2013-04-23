// <copyright file="StringLookupExtensionsFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using SpecBind.Helpers;

	/// <summary>
	///     A test fixture for the <see cref="StringLookupExtensions" /> class.
	/// </summary>
	[TestClass]
	public class StringLookupExtensionsFixture
	{
		/// <summary>
		/// Tests the ToLookupKey method with null and empty strings.
		/// </summary>
		[TestMethod]
		public void TestToLookupKeyNullStrings()
		{
			var nullResult = ((string)null).ToLookupKey();
			var emptyResult = string.Empty.ToLookupKey();
			var whitespaceResult = "   ".ToLookupKey();

			Assert.AreEqual(string.Empty, nullResult);
			Assert.AreEqual(string.Empty, emptyResult);
			Assert.AreEqual(string.Empty, whitespaceResult);
		}

		/// <summary>
		/// Tests the ToLookupKey method with A string that has no whitespace.
		/// </summary>
		[TestMethod]
		public void TestToLookupKeyNoWhitespace()
		{
			var result = "HelloWorld1".ToLookupKey();

			Assert.AreEqual("helloworld1", result);
		}

		/// <summary>
		/// Tests the ToLookupKey method with A string that has whitespace.
		/// </summary>
		[TestMethod]
		public void TestToLookupKeyWithWhitespace()
		{
			var result = "Hello World 1".ToLookupKey();

			Assert.AreEqual("helloworld1", result);
		}

		/// <summary>
		/// Tests the ToLookupKey method with a string that contains A in it.
		/// </summary>
		[TestMethod]
		public void TestToLookupKeyWithSingularFiller()
		{
			var resultUpper = "Eat A Cookie".ToLookupKey();
			var resultLower = "Eat a Cookie".ToLookupKey();

			var resultAnUpper = "Eat An Cookie".ToLookupKey();
			var resultAnLower = "Eat an Cookie".ToLookupKey();

			Assert.AreEqual("eatcookie", resultUpper);
			Assert.AreEqual("eatcookie", resultLower);

			Assert.AreEqual("eatcookie", resultAnUpper);
			Assert.AreEqual("eatcookie", resultAnLower);
		}

		/// <summary>
		/// Tests the ToLookupKey method with a string that contains the in it.
		/// </summary>
		[TestMethod]
		public void TestToLookupKeyWithTitleFiller()
		{
			var resultUpper = "The Cookie".ToLookupKey();
			var resultLower = "the Cookie".ToLookupKey();

			Assert.AreEqual("cookie", resultUpper);
			Assert.AreEqual("cookie", resultLower);
		}

		/// <summary>
		/// Tests the ToLookupKey method with special characters.
		/// </summary>
		[TestMethod]
		public void TestToLookupKeyWithSpecialCharacters()
		{
			var result = "Hello_World 1!".ToLookupKey();

			Assert.AreEqual("helloworld1", result);
		}

		/// <summary>
		/// Tests the NormalizedEquals method with null and empty strings.
		/// </summary>
		[TestMethod]
		public void TestNormalizedEqualsNullStrings()
		{
			var nullResult = ((string)null).NormalizedEquals(null);
			var emptyResult = string.Empty.NormalizedEquals(string.Empty);
			
			Assert.AreEqual(true, nullResult);
			Assert.AreEqual(true, emptyResult);
		}

		/// <summary>
		/// Tests the NormalizedEquals method with special characters.
		/// </summary>
		[TestMethod]
		public void TestNormalizedEqualsWithSpecialCharacters()
		{
			var result = "Hello_World 1!".NormalizedEquals("helloworld1");

			Assert.AreEqual(true, result);
		}
	}
}