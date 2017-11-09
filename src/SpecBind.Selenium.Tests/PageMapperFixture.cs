// <copyright file="SeleniumBrowserFixture.cs">
//    Copyright © 2018 Rami Abughazaleh.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Pages;

    [TestClass]
    public class PageMapperFixture
    {
        [TestMethod]
        public void Initialize_WithNull_Succeeds()
        {
            var mapper = new PageMapper();
            mapper.Initialize(null);

            Assert.AreNotEqual(0, mapper.MapCount);
        }
    }
}
