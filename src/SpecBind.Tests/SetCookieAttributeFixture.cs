// <copyright file="SetCookieAttributeFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the SetCookieAttribute class.
    /// </summary>
    [TestClass]
    public class SetCookieAttributeFixture
    {
        /// <summary>
        /// Tests that the default constructor sets name and value.
        /// </summary>
        [TestMethod]
        public void TestDefaultConstructorSetsNameAndValue()
        {
            var attribute = new SetCookieAttribute("TestName", "TestValue");

            Assert.AreEqual("TestName", attribute.Name);
            Assert.AreEqual("TestValue", attribute.Value);
            Assert.AreEqual("/", attribute.Path);
        }

        /// <summary>
        /// Tests that the set of the path value overrides the default value.
        /// </summary>
        [TestMethod]
        public void TestSetPathOverridesDefaultValue()
        {
            var attribute = new SetCookieAttribute("TestName", "TestValue") { Path = "/MyDomain" };

            Assert.AreEqual("TestName", attribute.Name);
            Assert.AreEqual("TestValue", attribute.Value);
            Assert.AreEqual("/MyDomain", attribute.Path);
        }

        /// <summary>
        /// Tests that the set of the path value overrides the default value.
        /// </summary>
        [TestMethod]
        public void TestToStringReturnsValues()
        {
            var attribute = new SetCookieAttribute("TestName", "TestValue") { Path = "/MyDomain" };

            var result = attribute.ToString();

            StringAssert.Contains(result, "Name: TestName");
            StringAssert.Contains(result, "Value: TestValue");
            StringAssert.Contains(result, "Path: /MyDomain");
        }
    }
}
