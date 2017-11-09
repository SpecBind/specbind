// <copyright file="PageBuilderBaseFixture.cs">
//    Copyright © 2014 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the common operation of the PageBuilderBase class.
    /// </summary>
    [TestClass]
    public class PageBuilderBaseFixture
    {
        /// <summary>
        /// A dummy element for a parent context
        /// </summary>
        public interface IParentContext
        {
        }

        /// <summary>
        /// A dummy element interfaces
        /// </summary>
        public interface ITestElement
        {
        }

        /// <summary>
        /// Test set property is discovered and can be set correctly.
        /// </summary>
        [TestMethod]
        public void TestDiscoverSetStringProperty()
        {
            var parent = new Mock<IParentContext>(MockBehavior.Strict);
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            var uriHelper = new Mock<IUriHelper>(MockBehavior.Strict);
            var lazyUriHelper = new Lazy<IUriHelper>(() => uriHelper.Object);

            var pageFunc = new PageBuilderTestProxy(lazyUriHelper).CreatePage(typeof(PropertyPage));

            var pageObject = pageFunc(parent.Object, browser.Object, lazyUriHelper, null);
            var page = pageObject as PropertyPage;

            Assert.IsNotNull(page);
            Assert.AreSame(parent.Object, page.ParentContext);
        }

        /// <summary>
        /// A test page to see if property setting works.
        /// </summary>
        public class PropertyPage
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PropertyPage"/> class.
            /// </summary>
            /// <param name="parentContext">The parent context.</param>
            public PropertyPage(IParentContext parentContext)
            {
                this.ParentContext = parentContext;
            }

            /// <summary>
            /// Gets the parent context.
            /// </summary>
            /// <value>The parent context.</value>
            public IParentContext ParentContext { get; private set; }

            /// <summary>
            /// Gets or sets the password.
            /// </summary>
            /// <value>The password.</value>
            public string Password { get; set; }
        }

        /// <summary>
        /// A test class that uses the correct page builder methods.
        /// </summary>
        public class PageBuilderTestProxy : PageBuilderBase<IParentContext, object, ITestElement>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PageBuilderTestProxy"/> class.
            /// </summary>
            /// <param name="uriHelper">The URI helper.</param>
            public PageBuilderTestProxy(Lazy<IUriHelper> uriHelper)
                : base(uriHelper)
            {
            }

            /// <summary>
            /// Creates the page.
            /// </summary>
            /// <param name="pageType">Type of the page.</param>
            /// <returns>The created page class.</returns>
            public Func<IParentContext, IBrowser, Lazy<IUriHelper>, Action<object>, object> CreatePage(Type pageType)
            {
                return this.CreateElementInternal(pageType);
            }

            /// <summary>
            /// Assigns the element attributes.
            /// </summary>
            /// <param name="control">The control.</param>
            /// <param name="attribute">The attribute.</param>
            /// <param name="nativeAttributes">The native attributes.</param>
            protected override void AssignElementAttributes(ITestElement control, ElementLocatorAttribute attribute, object[] nativeAttributes)
            {
            }

            /// <summary>
            /// Gets the type of the element collection.
            /// </summary>
            /// <returns>The type that should be used as the collection.</returns>
            protected override Type GetElementCollectionType()
            {
                return null;
            }

            /// <summary>
            /// Gets the type of the table driver.
            /// </summary>
            /// <returns>The table driver type.</returns>
            protected override Type GetTableDriverType()
            {
                return null;
            }
        }
    }
}
