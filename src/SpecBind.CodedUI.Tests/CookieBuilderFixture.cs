// <copyright file="CookieBuilderFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.CodedUI.Tests
{
    using System;
    using System.Text.RegularExpressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// A test fixture for the CookieBuilder class.
    /// </summary>
    [TestClass]
    public class CookieBuilderFixture
    {
        /// <summary>
        /// Tests cookie builder with an invalid character in the name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCreateCookieWithSemiColonInNameThrowsAnException()
        {
            try
            {
                CookieBuilder.CreateCookie("Test;Cookie", "Some Value", null, null, null, false);
            }
            catch (ArgumentException ex)
            {
                StringAssert.Contains(ex.Message, "Cookie name cannot contain ';'");
                Assert.AreEqual("name", ex.ParamName);
                throw;
            }
        }

        /// <summary>
        /// Tests cookie builder creates a basic default cookie
        /// </summary>
        [TestMethod]
        public void TestCreateCookieWithMinimalValueCreatesCorrectString()
        {
            var cookieString = CookieBuilder.CreateCookie("TestCookie", "Some Value", null, null, null, false);

            Assert.AreEqual(@"document.cookie = ""TestCookie=Some%20Value; path=/""", cookieString);
        }

        /// <summary>
        /// Tests cookie builder creates a basic default cookie
        /// </summary>
        [TestMethod]
        public void TestCreateCookieWithPathCreatesCorrectString()
        {
            var cookieString = CookieBuilder.CreateCookie("TestCookie", "Some Value", "/MyPath", null, null, false);

            Assert.AreEqual(@"document.cookie = ""TestCookie=Some%20Value; path=/MyPath""", cookieString);
        }

        /// <summary>
        /// Tests cookie builder creates a basic default cookie with an expiration date.
        /// </summary>
        [TestMethod]
        public void TestCreateCookieWithExpirationDateCreatesUtcExpirationString()
        {
            var cookieString = CookieBuilder.CreateCookie("TestCookie", "Some Value", "/MyPath", new DateTime(2015, 3, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false);

            Assert.AreEqual(@"document.cookie = ""TestCookie=Some%20Value; expires=Mon, 30 Mar 2015 00:00:00 GMT; path=/MyPath""", cookieString);
        }

        /// <summary>
        /// Tests cookie builder creates a basic default cookie with an minimum expiration date.
        /// </summary>
        [TestMethod]
        public void TestCreateCookieWithMinimumExpirationDateCreatesMinExpirationString()
        {
            var cookieString = CookieBuilder.CreateCookie("TestCookie", "Some Value", "/MyPath", DateTime.MinValue, null, false);

            Assert.AreEqual(@"document.cookie = ""TestCookie=Some%20Value; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/MyPath""", cookieString);
        }

        /// <summary>
        /// Tests cookie builder creates a basic default cookie with a maximum expiration date.
        /// </summary>
        [TestMethod]
        public void TestCreateCookieWithMaximumExpirationDateCreatesInfinityExpirationString()
        {
            var cookieString = CookieBuilder.CreateCookie("TestCookie", "Some Value", "/MyPath", DateTime.MaxValue, null, false);

            Assert.AreEqual(@"document.cookie = ""TestCookie=Some%20Value; expires=Fri, 31 Dec 9999 23:59:59 GMT; path=/MyPath""", cookieString);
        }

        /// <summary>
        /// Tests cookie builder creates a basic default cookie with a domain in the path.
        /// </summary>
        [TestMethod]
        public void TestCreateCookieWithDomainCreatesDomainCookie()
        {
            var cookieString = CookieBuilder.CreateCookie("TestCookie", "Some Value", "/MyPath", null, "www.mydomain.com", false);

            Assert.AreEqual(@"document.cookie = ""TestCookie=Some%20Value; domain=www.mydomain.com; path=/MyPath""", cookieString);
        }

        /// <summary>
        /// Tests cookie builder creates a basic default cookie with a domain that has a port number in the path.
        /// </summary>
        [TestMethod]
        public void TestCreateCookieWithDomainAndPortNumberCreatesDomainCookieWithoutPort()
        {
            var cookieString = CookieBuilder.CreateCookie("TestCookie", "Some Value", "/MyPath", null, "www.mydomain.com:8080", false);

            Assert.AreEqual(@"document.cookie = ""TestCookie=Some%20Value; domain=www.mydomain.com; path=/MyPath""", cookieString);
        }

        /// <summary>
        /// Tests cookie builder creates a basic default cookie with a domain in the path and secure setting.
        /// </summary>
        [TestMethod]
        public void TestCreateCookieWithDomainAndSecureCreatesSecureDomainCookie()
        {
            var cookieString = CookieBuilder.CreateCookie("TestCookie", "Some Value", "/MyPath", null, "www.mydomain.com", false);

            Assert.AreEqual(@"document.cookie = ""TestCookie=Some%20Value; domain=www.mydomain.com; path=/MyPath""", cookieString);
        }

        /// <summary>
        /// Tests cookie builder creates a cookie with all options defined.
        /// </summary>
        [TestMethod]
        public void TestCreateCookieWithAllParametersCreatesFullString()
        {
            var cookieString = CookieBuilder.CreateCookie("TestCookie", "Some Value", "/MyPath", new DateTime(2015, 3, 30, 0, 0, 0, 0, DateTimeKind.Utc), "www.mydomain.com", false);

            Assert.AreEqual(@"document.cookie = ""TestCookie=Some%20Value; expires=Mon, 30 Mar 2015 00:00:00 GMT; domain=www.mydomain.com; path=/MyPath""", cookieString);
        }

        [TestMethod]
        public void TestGetCookieValueCreatesAndCallsFunction()
        {
            var cookieString = CookieBuilder.GetCookieValue("TestCookie");
            var strippedString = Regex.Replace(cookieString, @"\s+", "");
            var expectedOutput = "function getCookie(name) {\n" + "  var value = \"; \" + document.cookie;\n"
                                 + "  var parts = value.split(\"; \" + name + \"=\");\n"
                                 + "  if (parts.length == 2) return parts.pop().split(\";\").shift();\n" + "}\n"
                                 + "getCookie('TestCookie')";
            var strippedOutput = Regex.Replace(expectedOutput, @"\s+", "");

            Assert.AreEqual(strippedOutput, strippedString);
        }
    }
}