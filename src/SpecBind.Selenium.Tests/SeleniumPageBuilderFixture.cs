// <copyright file="SeleniumPageBuilderFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Tests
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    using SpecBind.BrowserSupport;
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
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var listItem = new Mock<IWebElement>(MockBehavior.Loose);
            listItem.Setup(l => l.Displayed).Returns(true);

            var itemList = new ReadOnlyCollection<IWebElement>(new[] { listItem.Object });

            // Setup list mock
            var listElement = new Mock<IWebElement>(MockBehavior.Strict);
            listElement.Setup(l => l.FindElements(By.TagName("LI"))).Returns(itemList);

            // Setup mock for list
            driver.Setup(d => d.FindElement(By.Id("ListDiv"))).Returns(listElement.Object);


            var pageFunc = new SeleniumPageBuilder().CreatePage(typeof(BuildPage));

            var pageObject = pageFunc(driver.Object, browser.Object, null);
            var page = pageObject as BuildPage;

            Assert.IsNotNull(page);

            Assert.IsNotNull(page.TestButton);
            AssertLocatorValue(page.TestButton, By.Id("MyControl"));

            Assert.IsNotNull(page.CombinedControl);
            AssertLocatorValue(page.CombinedControl, new ByChained(By.Id("Field1"), By.LinkText("The Button")));

            Assert.IsNotNull(page.UserName);
            AssertLocatorValue(page.UserName, By.Name("UserName"));

            // Nesting Test
            Assert.IsNotNull(page.MyDiv);
            AssertLocatorValue(page.MyDiv, By.ClassName("btn"));

            Assert.IsNotNull(page.MyDiv.InternalButton);
            AssertLocatorValue(page.MyDiv.InternalButton, By.Id("InternalItem"));

            //List Test
            Assert.IsNotNull(page.MyCollection);
            Assert.IsInstanceOfType(page.MyCollection, typeof(SeleniumListElementWrapper<IWebElement, ListItem>));

            var propertyList = (SeleniumListElementWrapper<IWebElement, ListItem>)page.MyCollection;
            Assert.IsNotNull(propertyList.Parent);
            AssertLocatorValue(propertyList.Parent, By.Id("ListDiv"));

            // Test First Element
            var element = propertyList.FirstOrDefault();

            Assert.IsNotNull(element);
            Assert.IsNotNull(element.MyTitle);
            AssertLocatorValue(element.MyTitle, By.Id("itemTitle"));

            listElement.VerifyAll();
            listItem.VerifyAll();
            driver.VerifyAll();
        }

        /// <summary>
        /// Tests the multiple constructor arguments. when looking for a driver and parent context.
        /// </summary>
        [TestMethod]
        public void TestMultipleConstructorArguments()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var listItem = new Mock<IWebElement>(MockBehavior.Loose);
            listItem.Setup(l => l.Displayed).Returns(true);

            var itemList = new ReadOnlyCollection<IWebElement>(new[] { listItem.Object });

            // Setup list mock
            var listElement = new Mock<IWebElement>(MockBehavior.Strict);
            listElement.Setup(l => l.FindElements(By.TagName("LI"))).Returns(itemList);

            // Setup mock for list
            driver.Setup(d => d.FindElement(By.Id("ListDiv"))).Returns(listElement.Object);


            var pageFunc = new SeleniumPageBuilder().CreatePage(typeof(NestedElementPage));

            var pageObject = pageFunc(driver.Object, browser.Object, null);
            var page = pageObject as NestedElementPage;

            Assert.IsNotNull(page);

            Assert.IsNotNull(page.FirstChild);
            AssertLocatorValue(page.FirstChild, By.Id("Test1"));

            Assert.IsNotNull(page.FirstChild.SecondChild);
            AssertLocatorValue(page.FirstChild.SecondChild, By.Id("Test2"));

            var element = page.FirstChild.SecondChild;
            Assert.IsNotNull(element.SearchContext);
            Assert.IsNotNull(element.Driver);

            Assert.AreSame(driver.Object, element.Driver);
            Assert.AreSame(page.FirstChild, element.SearchContext);
        }

        /// <summary>
        /// Tests the create page method with mixed attributes.
        /// </summary>
        [TestMethod]
        public void TestCreatePageWithNativeAttributes()
        {
            var driver = new Mock<IWebDriver>();
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var pageFunc = new SeleniumPageBuilder().CreatePage(typeof(NativeAttributePage));

            var pageObject = pageFunc(driver.Object, browser.Object, null);
            var page = pageObject as NativeAttributePage;

            Assert.IsNotNull(page);
            Assert.IsNotNull(page.NativeElement);
            AssertLocatorValue(page.NativeElement, By.Id("nativeElement"));
        }

        /// <summary>
        /// Tests the create page method with mixed attributes.
        /// </summary>
        [TestMethod]
        public void TestCreatePageWithCombinedNativeAndLocatorAttributes()
        {
            var driver = new Mock<IWebDriver>();
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var pageFunc = new SeleniumPageBuilder().CreatePage(typeof(NativeAttributePage));

            var pageObject = pageFunc(driver.Object, browser.Object, null);
            var page = pageObject as NativeAttributePage;

            Assert.IsNotNull(page);
            Assert.IsNotNull(page.CombinedElement);
            AssertLocatorValue(page.CombinedElement, new ByChained(By.Id("combined"), By.TagName("DIV")));
        }

        /// <summary>
        /// Tests the create page method with duplicate attributes.
        /// </summary>
        [TestMethod]
        public void TestCreatePageWithDuplicateNativeAndLocatorAttributes()
        {
            var driver = new Mock<IWebDriver>();
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var pageFunc = new SeleniumPageBuilder().CreatePage(typeof(NativeAttributePage));

            var pageObject = pageFunc(driver.Object, browser.Object, null);
            var page = pageObject as NativeAttributePage;

            Assert.IsNotNull(page);
            Assert.IsNotNull(page.DuplicateElement);
            AssertLocatorValue(page.DuplicateElement, By.Id("1234"));
        }

        /// <summary>
        /// Tests the create page method.
        /// </summary>
        [TestMethod]
        public void TestCreatePageWithNativeProperties()
        {
            var driver = new Mock<IWebDriver>(MockBehavior.Strict);
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var listItem = new Mock<IWebElement>(MockBehavior.Loose);
            listItem.Setup(l => l.Displayed).Returns(true);

            var itemList = new ReadOnlyCollection<IWebElement>(new[] { listItem.Object });

            // Setup list mock
            var listElement = new Mock<IWebElement>(MockBehavior.Strict);
            listElement.Setup(l => l.FindElements(By.TagName("LI"))).Returns(itemList);

            // Setup mock for list
            driver.Setup(d => d.FindElement(By.Id("ListDiv"))).Returns(listElement.Object);


            var pageFunc = new SeleniumPageBuilder().CreatePage(typeof(BuildPage));

            var pageObject = pageFunc(driver.Object, browser.Object, null);
            var page = pageObject as BuildPage;

            Assert.IsNotNull(page);

            Assert.IsNotNull(page.TestButton);
            AssertLocatorValue(page.TestButton, By.Id("MyControl"));

            Assert.IsNotNull(page.CombinedControl);
            AssertLocatorValue(page.CombinedControl, new ByChained(By.Id("Field1"), By.LinkText("The Button")));

            Assert.IsNotNull(page.UserName);
            AssertLocatorValue(page.UserName, By.Name("UserName"));

            // Image Url Testing
            Assert.IsNotNull(page.ImageElement);
            AssertLocatorValue(page.ImageElement, By.XPath("//img[@src='/myapp']"));

            // Link Url Testing
            Assert.IsNotNull(page.LinkElement);
            AssertLocatorValue(page.LinkElement, By.XPath("//a[@href='/myapp']"));

            // Link Area Url Testing
            Assert.IsNotNull(page.LinkAreaElement);
            AssertLocatorValue(page.LinkAreaElement, By.XPath("//area[@href='/myapp']"));

            // Image Alt Testing
            Assert.IsNotNull(page.AltImageElement);
            AssertLocatorValue(page.AltImageElement, By.XPath("//img[@alt='Alt Text']"));

            // Value Attribute Testing
            Assert.IsNotNull(page.ValueElement);
            AssertLocatorValue(page.ValueElement, By.XPath("//input[@value='something']"));

            // Title Attribute Testing
            Assert.IsNotNull(page.TitleElement);
            AssertLocatorValue(page.TitleElement, By.XPath("//p[@title='royal']"));

            // Type Attribute Testing
            Assert.IsNotNull(page.InputTypeElement);
            AssertLocatorValue(page.InputTypeElement, By.XPath("//input[@type='password']"));

            // Index Attribute Testing
            Assert.IsNotNull(page.IndexElement);
            AssertLocatorValue(page.IndexElement, By.XPath("//button[1]"));

            // Nesting Test
            Assert.IsNotNull(page.MyDiv);
            AssertLocatorValue(page.MyDiv, By.ClassName("btn"));

            Assert.IsNotNull(page.MyDiv.InternalButton);
            AssertLocatorValue(page.MyDiv.InternalButton, By.Id("InternalItem"));

            //List Test
            Assert.IsNotNull(page.MyCollection);
            Assert.IsInstanceOfType(page.MyCollection, typeof(SeleniumListElementWrapper<IWebElement, ListItem>));

            var propertyList = (SeleniumListElementWrapper<IWebElement, ListItem>)page.MyCollection;
            Assert.IsNotNull(propertyList.Parent);
            AssertLocatorValue(propertyList.Parent, By.Id("ListDiv"));

            // Test First Element
            var element = propertyList.FirstOrDefault();

            Assert.IsNotNull(element);
            Assert.IsNotNull(element.MyTitle);
            AssertLocatorValue(element.MyTitle, By.Id("itemTitle"));

            listElement.VerifyAll();
            listItem.VerifyAll();
            driver.VerifyAll();
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
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var page = pageFunc(driver.Object, browser.Object, null);

            Assert.IsNotNull(page);
            Assert.IsInstanceOfType(page, typeof(NoConstructorElement));
        }

        /// <summary>
        /// Tests the create page method with the browser in the constructor.
        /// </summary>
        [TestMethod]
        public void TestCreatePageWithBrowserArgument()
        {
            var driver = new Mock<IWebDriver>();
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var pageFunc = new SeleniumPageBuilder().CreatePage(typeof(BrowserDocument));

            var pageObject = pageFunc(driver.Object, browser.Object, null);
            var page = pageObject as BrowserDocument;

            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Browser);

            driver.VerifyAll();
            browser.VerifyAll();
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

        /// <summary>
        /// Asserts the locator value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="findBy">The find by.</param>
        [ExcludeFromCodeCoverage]
        private static void AssertLocatorValue(IWebElement element, By findBy)
        {
            var proxy = element as WebElement;
            if (proxy != null)
            {
                if (proxy.Locators.Any(l => l.ToString() == findBy.ToString()))
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
            [ElementLocator(Id = "MyControl")]
            public IWebElement TestButton { get; set; }

            /// <summary>
            /// Gets or sets the combined locator button.
            /// </summary>
            /// <value>
            /// The test button.
            /// </value>
            [ElementLocator(Id = "Field1", Text = "The Button")]
            public IWebElement CombinedControl { get; set; }

            /// <summary>
            /// Gets or sets my div.
            /// </summary>
            /// <value>
            /// My div.
            /// </value>
            [ElementLocator(Class = "btn")]
            public CustomDiv MyDiv { get; set; }

            /// <summary>
            /// Gets or sets the name of the user.
            /// </summary>
            /// <value>
            /// The name of the user.
            /// </value>
            [ElementLocator(Name = "UserName")]
            public IWebElement UserName { get; set; }

            /// <summary>
            /// Gets or sets the input type element.
            /// </summary>
            /// <value>The input type element.</value>
            [ElementLocator(Type = "password", TagName = "input")]
            public IWebElement InputTypeElement { get; set; }

            /// <summary>
            /// Gets or sets the name of the user.
            /// </summary>
            /// <value>
            /// The name of the user.
            /// </value>
            [ElementLocator(Alt = "Alt Text", TagName = "img")]
            public IWebElement AltImageElement { get; set; }

            /// <summary>
            /// Gets or sets the name of the user.
            /// </summary>
            /// <value>
            /// The name of the user.
            /// </value>
            [ElementLocator(Url = "/myapp", TagName = "img")]
            public IWebElement ImageElement { get; set; }

            /// <summary>
            /// Gets or sets my collection.
            /// </summary>
            /// <value>
            /// My collection.
            /// </value>
            [ElementLocator(Id = "ListDiv")]
            public IElementList<IWebElement, ListItem> MyCollection { get; set; }

            /// <summary>
            /// Gets or sets the link element.
            /// </summary>
            /// <value>The link element.</value>
            [ElementLocator(Url = "/myapp", TagName = "a")]
            public IWebElement LinkElement { get; set; }

            /// <summary>
            /// Gets or sets the link element.
            /// </summary>
            /// <value>The link element.</value>
            [ElementLocator(Url = "/myapp", TagName = "area")]
            public IWebElement LinkAreaElement { get; set; }

            /// <summary>
            /// Gets or sets the title element.
            /// </summary>
            /// <value>The title element.</value>
            [ElementLocator(Title = "royal", TagName = "p")]
            public IWebElement TitleElement { get; set; }

            /// <summary>
            /// Gets or sets the value element.
            /// </summary>
            /// <value>The value element.</value>
            [ElementLocator(Value = "something", TagName = "input")]
            public IWebElement ValueElement { get; set; }

            /// <summary>
            /// Gets or sets the index element.
            /// </summary>
            /// <value>The index element.</value>
            [ElementLocator(TagName = "button", Index = 2)]
            public IWebElement IndexElement { get; set; }
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
        public class ListItem : WebElement, IDataControl
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WebElement" /> class.
            /// </summary>
            /// <param name="searchContext">The driver used to search for elements.</param>
            protected internal ListItem(ISearchContext searchContext)
                : base(searchContext)
            {
            }

            /// <summary>
            /// Gets or sets my title.
            /// </summary>
            /// <value>
            /// My title.
            /// </value>
            [ElementLocator(Id = "itemTitle")]
            public IWebElement MyTitle { get; set; }

            /// <summary>
            /// Sets the value in the control.
            /// </summary>
            /// <param name="value">The value to set.</param>
            /// <exception cref="System.NotImplementedException">Not Implemented</exception>
            public void SetValue(string value)
            {
                throw new System.NotImplementedException();
            }
        }

        #endregion

        #region Class - NativeAttributePage

        /// <summary>
        /// A test class with native combined and duplicated attributes.
        /// </summary>
        public class NativeAttributePage
        {
            /// <summary>
            /// Gets or sets an element that has combined properties.
            /// </summary>
            /// <value>The element.</value>
            [ElementLocator(Id = "combined")]
            [FindsBy(How = How.TagName, Using = "DIV")]
            public IWebElement CombinedElement { get; set; }

            /// <summary>
            /// Gets or sets an element that has duplicate properties.
            /// </summary>
            /// <value>The element.</value>
            [ElementLocator(Id = "1234")]
            [FindsBy(How = How.Id, Using = "1234")]
            public IWebElement DuplicateElement { get; set; }

            /// <summary>
            /// Gets or sets an element that has only native properties.
            /// </summary>
            /// <value>The element.</value>
            [FindsBy(How = How.Id, Using = "nativeElement")]
            public IWebElement NativeElement { get; set; }
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

        #region Class - RequestsDriverInterface

        /// <summary>
        /// A nested element build page.
        /// </summary>
        public class NestedElementPage
        {
            /// <summary>
            /// Gets or sets the first child.
            /// </summary>
            /// <value>The first child.</value>
            [ElementLocator(Id = "Test1")]
            public NestedElementFirstChild FirstChild { get; set; }
        }

        /// <summary>
        /// A nested element build page that requests the first locator child.
        /// </summary>
        public class NestedElementFirstChild : WebElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WebElement" /> class.
            /// </summary>
            /// <param name="searchContext">The driver used to search for elements.</param>
            public NestedElementFirstChild(ISearchContext searchContext)
                : base(searchContext)
            {
            }

            /// <summary>
            /// Gets or sets the second child.
            /// </summary>
            /// <value>The second child.</value>
            [ElementLocator(Id = "Test2")]
            public RequestsDriverInterface SecondChild { get; set; }
        }

        /// <summary>
        /// A test class that requests the parent web driver interface
        /// </summary>
        public class RequestsDriverInterface : WebElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WebElement" /> class.
            /// </summary>
            /// <param name="driver">The driver.</param>
            /// <param name="searchContext">The driver used to search for elements.</param>
            public RequestsDriverInterface(IWebDriver driver, ISearchContext searchContext)
                : base(searchContext)
            {
                this.Driver = driver;
                this.SearchContext = searchContext;
            }

            /// <summary>
            /// Gets the driver.
            /// </summary>
            /// <value>The driver.</value>
            public IWebDriver Driver { get; private set; }

            /// <summary>
            /// Gets the search context.
            /// </summary>
            /// <value>The search context.</value>
            public ISearchContext SearchContext { get; private set; }
        }

        #endregion

        #region Class - BrowserDocument

        /// <summary>
        /// Class BrowserDocument.
        /// </summary>
        public class BrowserDocument
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BrowserDocument"/> class.
            /// </summary>
            /// <param name="browser">The browser.</param>
            public BrowserDocument(IBrowser browser)
            {
                this.Browser = browser;
            }

            /// <summary>
            /// Gets the browser.
            /// </summary>
            /// <value>The browser.</value>
            public IBrowser Browser { get; private set; }
        }

        #endregion
    }
}