// <copyright file="SeleniumPageBuilderFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Tests
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using OpenQA.Selenium;

    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the PageBuilder class.
    /// </summary>
    [TestClass]
    public class SeleniumPageBuilderFixture
    {
        /// <summary>
        /// Tests the create page method.
        /// </summary>
        [TestMethod]
        public void TestCreatePage()
        {
            var driver = new Mock<IWebDriver>();
            var pageFunc = new SeleniumPageBuilder().CreatePage(typeof(BuildPage));

            var pageObject = pageFunc(driver.Object, null);
            var page = pageObject as BuildPage;

            Assert.IsNotNull(page);
            
            Assert.IsNotNull(page.TestButton);
            AssertLocatorValue(page.TestButton, By.Id("MyControl"));
            //Assert.AreEqual("The Button", page.TestButton.FilterProperties[HtmlButton.PropertyNames.DisplayText]);

            Assert.IsNotNull(page.UserName);
            AssertLocatorValue(page.UserName, By.Name("UserName"));
            AssertLocatorValue(page.UserName, By.LinkText("Bob"));
            
            Assert.IsNotNull(page.Image);
            //Assert.AreEqual("The Image", page.Image.FilterProperties[HtmlImage.PropertyNames.Alt]);
            //Assert.AreEqual("http://myimage", page.Image.FilterProperties[HtmlImage.PropertyNames.Src]);

            Assert.IsNotNull(page.Hyperlink);
            //Assert.AreEqual("The Hyperlink", page.Hyperlink.FilterProperties[HtmlHyperlink.PropertyNames.Alt]);
            //Assert.AreEqual("http://myHyperlink", page.Hyperlink.FilterProperties[HtmlHyperlink.PropertyNames.Href]);

            Assert.IsNotNull(page.HyperlinkArea);
            //Assert.AreEqual("The Hyperlink Area", page.HyperlinkArea.FilterProperties[HtmlAreaHyperlink.PropertyNames.Alt]);
            //Assert.AreEqual("http://myHyperlinkArea", page.HyperlinkArea.FilterProperties[HtmlAreaHyperlink.PropertyNames.Href]);

            // Nesting Test
            Assert.IsNotNull(page.MyDiv);
            AssertLocatorValue(page.MyDiv, By.Id("MyDiv"));
            AssertLocatorValue(page.MyDiv, By.ClassName("btn"));
            
            Assert.IsNotNull(page.MyDiv.InternalButton);
            AssertLocatorValue(page.MyDiv.InternalButton, By.Id("InternalItem"));
            
            //List Test
            Assert.IsNotNull(page.MyCollection);
            Assert.IsInstanceOfType(page.MyCollection, typeof(SeleniumListElementWrapper<IWebElement, ListItem>));

            var propertyList = (SeleniumListElementWrapper<IWebElement, ListItem>)page.MyCollection;
            Assert.IsNotNull(propertyList.Parent);
            AssertLocatorValue(propertyList.Parent, By.Id("ListDiv"));
            
            //Disable validation for test
            propertyList.ValidateElementExists = false;

            // Test First Element
            var element = propertyList.FirstOrDefault();
			
            //Assert.IsNotNull(element);
            //Assert.AreEqual("LI", element.SearchProperties[HtmlControl.PropertyNames.TagName]);
            //Assert.AreEqual("1", element.FilterProperties[HtmlControl.PropertyNames.TagInstance]);

            //Assert.IsNotNull(element.MyTitle);
            //AssertLocatorValue(element.MyTitle, By.Id("itemTitle"));
        }

        /// <summary>
        /// Tests the constructor scenario where there is no argument.
        /// </summary>
        [TestMethod]
        public void TestMissingArgumentConstructor()
        {
            var builder = new SeleniumPageBuilder();

            var pageFunc = builder.CreatePage(typeof(NoConstructorElement));

            Assert.IsNotNull(pageFunc);

            var driver = new Mock<IWebDriver>();
            var page = pageFunc(driver.Object, null);

            Assert.IsNotNull(page);
            Assert.IsInstanceOfType(page, typeof(NoConstructorElement));
        }

        /// <summary>
        /// Tests the type of the generic.
        /// </summary>
        [TestMethod]
        public void TestGenericType()
        {
            var baseType = typeof(IElementList<IWebElement, IWebElement>);
            var concreteType = typeof(SeleniumListElementWrapper<,>).MakeGenericType(baseType.GetGenericArguments());

            Assert.IsTrue(baseType.IsGenericType, "Not a generic type");
            Assert.IsTrue(typeof(IElementList<,>).IsAssignableFrom(baseType.GetGenericTypeDefinition()));
            Assert.AreEqual(typeof(SeleniumListElementWrapper<IWebElement, IWebElement>), concreteType);
        }

        ///// <summary>
        ///// Tests the frame document creation. Save for when frames are supported.
        ///// </summary>
        //[TestMethod]
        //public void TestFrameDocument()
        //{
        //    var docType = typeof(MasterDocument);
        //    var property = docType.GetProperty("FrameNavigation");

        //    var window = new BrowserWindow();
        //    var pageFunc = PageBuilder<BrowserWindow, HtmlControl>.CreateFrameLocator(docType, property);
        //    var page = pageFunc(window);

        //    Assert.IsNotNull(page);
        //    Assert.IsInstanceOfType(page, typeof(HtmlFrame));
        //    Assert.AreEqual("1234", page.SearchProperties[HtmlControl.PropertyNames.Id]);
        //}

        /// <summary>
        /// Asserts the locator value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="findBy">The find by.</param>
        private static void AssertLocatorValue(IWebElement element, By findBy)
        {
            var proxy = element as WebElement;
            if (proxy != null)
            {
                if (proxy.Locators.Any(l => l == findBy))
                {
                    return;
                }

                var properties = proxy.Locators.Count > 0 
                    ? string.Join(", ", proxy.Locators) 
                    : "NONE";
                Assert.Fail(
                    "Element should have contained property '{0}' but did not. Available Properties: {1}", 
                    findBy, 
                    properties);
            }

            Assert.Fail("Instance of this class cannot be inspected.");
        }

        #region Class - FrameDocument

        /// <summary>
        /// A test class for seeing if frames will resolve.
        /// </summary>
        public class MasterDocument
        {
            /// <summary>
            /// Gets or sets the frameNavigation.
            /// </summary>
            /// <value>The frame1.</value>
            [ElementLocator(Id = "1234")]
            public IWebElement FrameNavigation { get; set; }
        }

        #endregion

        #region Class - BuildPage

        /// <summary>
        /// A test class for the page builder
        /// </summary>
        [PageNavigation("/builds")]
        public class BuildPage
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BuildPage" /> class.
            /// </summary>
            /// <param name="driver">The driver.</param>
            public BuildPage(IWebDriver driver)
            {
                this.Driver = driver;
            }

            /// <summary>
            /// Gets the driver.
            /// </summary>
            /// <value>The driver.</value>
            public IWebDriver Driver { get; private set; }

            /// <summary>
            /// Gets or sets the test button.
            /// </summary>
            /// <value>
            /// The test button.
            /// </value>
            [ElementLocator(Id = "MyControl", Text = "The Button")]
            public IWebElement TestButton { get; set; }

            /// <summary>
            /// Gets or sets my div.
            /// </summary>
            /// <value>
            /// My div.
            /// </value>
            [ElementLocator(Id = "MyDiv", Class = "btn")]
            public CustomDiv MyDiv { get; set; }

            /// <summary>
            /// Gets or sets the name of the user.
            /// </summary>
            /// <value>
            /// The name of the user.
            /// </value>
            [ElementLocator(Name = "UserName", Text = "Bob")]
            public IWebElement UserName { get; set; }

            /// <summary>
            /// Gets or sets the image.
            /// </summary>
            /// <value>
            /// The image.
            /// </value>
            [ElementLocator(Alt = "The Image", Url = "http://myimage")]
            public IWebElement Image { get; set; }

            /// <summary>
            /// Gets or sets the hyperlink.
            /// </summary>
            /// <value>
            /// The hyperlink.
            /// </value>
            [ElementLocator(Alt = "The Hyperlink", Url = "http://myHyperlink")]
            public IWebElement Hyperlink { get; set; }

            /// <summary>
            /// Gets or sets the hyperlink.
            /// </summary>
            /// <value>
            /// The hyperlink.
            /// </value>
            [ElementLocator(Alt = "The Hyperlink Area", Url = "http://myHyperlinkArea")]
            public IWebElement HyperlinkArea { get; set; }

            /// <summary>
            /// Gets or sets my collection.
            /// </summary>
            /// <value>
            /// My collection.
            /// </value>
            [ElementLocator(Id = "ListDiv")]
            public IElementList<IWebElement, ListItem> MyCollection { get; set; }
        }

        /// <summary>
        /// A custom div element.
        /// </summary>
        public class CustomDiv : WebElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WebElement" /> class.
            /// </summary>
            /// <param name="searchContext">The driver used to search for elements.</param>
            public CustomDiv(ISearchContext searchContext)
                : base(searchContext)
            {
            }

            /// <summary>
            /// Gets or sets the test button.
            /// </summary>
            /// <value>
            /// The test button.
            /// </value>
            [ElementLocator(Id = "InternalItem")]
            public IWebElement InternalButton { get; set; }
        }

        /// <summary>
        /// An inner list item.
        /// </summary>
        [ElementLocator(TagName = "LI")]
        public class ListItem
        {
            /// <summary>
            /// Gets or sets my title.
            /// </summary>
            /// <value>
            /// My title.
            /// </value>
            [ElementLocator(Id = "itemTitle")]
            public IWebElement MyTitle { get; set; }
        }

        #endregion

        #region Class - NoConstructorElement

        /// <summary>
        /// An invalid class that has no constructor.
        /// </summary>
        public class NoConstructorElement
        {
        }

        #endregion
    }
}