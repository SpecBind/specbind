namespace SpecBind.Selenium.Tests
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OpenQA.Selenium;

    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the locator builder class.
    /// </summary>
    [TestClass]
    public class LocatorBuilderFixture
    {
        [TestMethod]
        public void TestAttributeWithIdReturnsIdLocator()
        {
            var attribute = new ElementLocatorAttribute { Id = "MyId" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.Id("MyId"), item);
        }

        [TestMethod]
        public void TestAttributeWithNameReturnsNameLocator()
        {
            var attribute = new ElementLocatorAttribute { Name = "MyName" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.Name("MyName"), item);
        }

        [TestMethod]
        public void TestAttributeWithCssClassReturnsCssClassLocator()
        {
            var attribute = new ElementLocatorAttribute { Class = ".something" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.ClassName(".something"), item);
        }

        [TestMethod]
        public void TestAttributeWithLinkTextReturnsLinkTextLocator()
        {
            var attribute = new ElementLocatorAttribute { Text = "Hello World" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.LinkText("Hello World"), item);
        }

        [TestMethod]
        public void TestAttributeWithTagNameReturnsTagLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.TagName("input"), item);
        }

        [TestMethod]
        public void TestAttributeWithTagNameAndTypeReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Type = "email"};

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//input[@type='email']"), item);
        }

        [TestMethod]
        public void TestAttributeWithTagNameAndTitleReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Title = "Page" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//input[@title='Page']"), item);
        }

        [TestMethod]
        public void TestAttributeWithTagNameAndValueReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Value = "test" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//input[@value='test']"), item);
        }

        [TestMethod]
        public void TestAttributeWithTagNameAndAltReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Alt = "test" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//input[@alt='test']"), item);
        }

        [TestMethod]
        public void TestAttributeWithImageTagAndUrlReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "img", Url = "myimage.png" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//img[@src='myimage.png']"), item);
        }

        [TestMethod]
        public void TestAttributeWithLinkTagAndUrlReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "a", Url = "mylink.htm" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//a[@href='mylink.htm']"), item);
        }

        [TestMethod]
        public void TestAttributeWithLinkAreaTagAndUrlReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "area", Url = "mylink.htm" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//area[@href='mylink.htm']"), item);
        }

        [TestMethod]
        public void TestAttributeWithTagNameAndIndexReturnsXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Index = 1};

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//input[0]"), item);
        }

        [TestMethod]
        public void TestAttributeWithTagNameAndTypeAndIndexReturnsComplexXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Type = "email", Index = 1 };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("(//input[@type='email'])[0]"), item);
        }

        [TestMethod]
        public void TestAttributeWithTagNameAndTypeAndTitleReturnsCompundXPathLocator()
        {
            var attribute = new ElementLocatorAttribute { TagName = "input", Type = "email", Title = "my title" };

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(1, resultList.Count);
            var item = resultList.First();
            Assert.AreEqual(By.XPath("//input[@title='my title' and @type='email']"), item);
        }

        [TestMethod]
        public void TestAttributeWithIdAndTagNameReturnsTwoLocators()
        {
            var attribute = new ElementLocatorAttribute { Id = "MyId", TagName = "a"};

            var resultList = LocatorBuilder.GetElementLocators(attribute);

            Assert.AreEqual(2, resultList.Count);
            
            var item = resultList.First();
            Assert.AreEqual(By.Id("MyId"), item);

            var item2 = resultList.Last();
            Assert.AreEqual(By.TagName("a"), item2);
        }

        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestAttributeWithNoTagNameAndPropertyThrowsAnException()
        {
            var attribute = new ElementLocatorAttribute { Type = "submit"};

            LocatorBuilder.GetElementLocators(attribute);
        }
    }
}
