// <copyright file="PageBuilderFixture.cs">
//    Copyright © 2018 Rami Abughazaleh  All rights reserved.
// </copyright>

namespace SpecBind.CodedUI.Tests
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Pages;

    [CodedUITest]
    public class PageMapperFixture
    {
        [TestMethod]
        public void Initialize_WithHtmlDocumentType_Succeeds()
        {
            var mapper = new PageMapper();
            mapper.Initialize(typeof(HtmlDocument));

            Assert.AreEqual(3, mapper.MapCount);
        }
    }
}
