// <copyright file="AttributeExtensionsFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Tests.Helpers
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the attribute extensions class.
    /// </summary>
    [TestClass]
    public class AttributeExtensionsFixture
    {
        /// <summary>
        /// Tests the GetAttribute method when the attribute exists.
        /// </summary>
        [TestMethod]
        public void TestGetAttributeWhenAttributeExistsReturnsAttribute()
        {
            var result = typeof(AttributeExists).GetAttribute<PageNavigationAttribute>();

            Assert.IsNotNull(result);
            Assert.AreEqual("/foo", result.Url);
        }

        /// <summary>
        /// Tests the GetAttribute method when the attribute does not exists.
        /// </summary>
        [TestMethod]
        public void TestGetAttributeWhenAttributedDoesNotExistReturnsNull()
        {
            var result = typeof(object).GetAttribute<PageNavigationAttribute>();

            Assert.IsNull(result);
        }

        /// <summary>
        /// Tests the TryGetAttribute method when the attribute exists.
        /// </summary>
        [TestMethod]
        public void TestTryGetAttributeWhenAttributeExistsReturnsAttribute()
        {
            PageNavigationAttribute attribute;
            var result = typeof(AttributeExists).TryGetAttribute(out attribute);

            Assert.AreEqual(true, result);
            Assert.IsNotNull(attribute);
            Assert.AreEqual("/foo", attribute.Url);
        }

        /// <summary>
        /// Tests the TryGetAttribute method when the attribute does not exists.
        /// </summary>
        [TestMethod]
        public void TestGetTryAttributeWhenAttributedDoesNotExistReturnsNull()
        {
            PageNavigationAttribute attribute;
            var result = typeof(object).TryGetAttribute<PageNavigationAttribute>(out attribute);

            Assert.AreEqual(false, result);
            Assert.IsNull(attribute);
        }

        /// <summary>
        /// A test class that contains the attribute
        /// </summary>
        [PageNavigation("/foo")]
        private class AttributeExists
        {
        }
    }
}