// <copyright file="PageBuilderFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.CodedUI.Tests
{
	using System;
	
	using Microsoft.VisualStudio.TestTools.UITesting;
	using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using SpecBind.BrowserSupport;
	using SpecBind.Pages;

	/// <summary>
	/// A test fixture for the PageBuilder class.
	/// </summary>
	[CodedUITest]
	public class PageBuilderFixture
	{
		/// <summary>
		/// Tests the create page method.
		/// </summary>
		[TestMethod]
		public void TestCreatePage()
		{
		    var window = new BrowserWindow();
            var pageFunc = PageBuilder<BrowserWindow, HtmlDocument>.CreateElement(typeof(BuildPage));

		    var pageObject = pageFunc(window, null, null);
            var page = pageObject as BuildPage;

			Assert.IsNotNull(page);
			Assert.AreEqual("/builds", page.FilterProperties[HtmlDocument.PropertyNames.AbsolutePath]);
			Assert.AreEqual("http://localhost:2222/builds", page.FilterProperties[HtmlDocument.PropertyNames.PageUrl]);

			Assert.IsNotNull(page.TestButton);
			Assert.AreEqual("MyControl", page.TestButton.SearchProperties[HtmlControl.PropertyNames.Id]);
			Assert.AreEqual("The Button", page.TestButton.FilterProperties[HtmlButton.PropertyNames.DisplayText]);

			Assert.IsNotNull(page.UserName);
			Assert.AreEqual("UserName", page.UserName.SearchProperties[UITestControl.PropertyNames.Name]);
			Assert.AreEqual("Bob", page.UserName.FilterProperties[HtmlEdit.PropertyNames.Text]);

			Assert.IsNotNull(page.Image);
			Assert.AreEqual("The Image", page.Image.FilterProperties[HtmlImage.PropertyNames.Alt]);
			Assert.AreEqual("http://myimage", page.Image.FilterProperties[HtmlImage.PropertyNames.Src]);

			Assert.IsNotNull(page.Hyperlink);
			Assert.AreEqual("The Hyperlink", page.Hyperlink.FilterProperties[HtmlHyperlink.PropertyNames.Alt]);
			Assert.AreEqual("http://myHyperlink", page.Hyperlink.FilterProperties[HtmlHyperlink.PropertyNames.Href]);

			Assert.IsNotNull(page.HyperlinkArea);
			Assert.AreEqual("The Hyperlink Area", page.HyperlinkArea.FilterProperties[HtmlAreaHyperlink.PropertyNames.Alt]);
			Assert.AreEqual("http://myHyperlinkArea", page.HyperlinkArea.FilterProperties[HtmlAreaHyperlink.PropertyNames.Href]);

			// Nesting Test
			Assert.IsNotNull(page.MyDiv);
			Assert.AreEqual("MyDiv", page.MyDiv.SearchProperties[HtmlControl.PropertyNames.Id]);
			Assert.AreEqual("The Div!", page.MyDiv.FilterProperties[HtmlControl.PropertyNames.InnerText]);
			Assert.AreEqual("btn", page.MyDiv.FilterProperties[UITestControl.PropertyNames.ClassName]);
			Assert.AreEqual("The Main Div", page.MyDiv.FilterProperties[HtmlControl.PropertyNames.Title]);

			Assert.IsNotNull(page.MyDiv.InternalButton);
			Assert.AreEqual("InternalItem", page.MyDiv.InternalButton.SearchProperties[HtmlControl.PropertyNames.Id]);

			//List Test
			Assert.IsNotNull(page.MyCollection);
			Assert.IsInstanceOfType(page.MyCollection, typeof(CodedUIListElementWrapper<HtmlDiv, ListItem>));

			var propertyList = (CodedUIListElementWrapper<HtmlDiv, ListItem>)page.MyCollection;
			Assert.IsNotNull(propertyList.Parent);
			Assert.AreEqual("ListDiv", propertyList.Parent.SearchProperties[HtmlControl.PropertyNames.Id]);

            // Table Test
            Assert.IsNotNull(page.MyTable);
		}

		/// <summary>
		/// Tests the missing argument constructor scenario.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestMissingArgumentConstructor()
		{
			try
			{

                PageBuilder<BrowserWindow, HtmlDocument>.CreateElement(typeof(NoConstructorElement));
			}
			catch (InvalidOperationException ex)
			{
				Assert.AreEqual(
					"Constructor on type 'NoConstructorElement' must have a single argument of type UITestControl.",
					ex.Message);
				throw;
			}
		}

		/// <summary>
		/// Tests the type of the generic.
		/// </summary>
		[TestMethod]
		public void TestGenericType()
		{
			var baseType = typeof(IElementList<HtmlDiv, HtmlCustom>);
			var concreteType = typeof(CodedUIListElementWrapper<,>).MakeGenericType(baseType.GetGenericArguments());

			Assert.IsTrue(baseType.IsGenericType, "Not a generic type");
			Assert.IsTrue(typeof(IElementList<,>).IsAssignableFrom(baseType.GetGenericTypeDefinition()));
			Assert.AreEqual(typeof(CodedUIListElementWrapper<HtmlDiv, HtmlCustom>), concreteType);
		}

		/// <summary>
		/// Tests the frame document creation.
		/// </summary>
		[TestMethod]
		public void TestFrameDocument()
		{
			var docType = typeof(MasterDocument);
			var property = docType.GetProperty("FrameNavigation");

			var window = new BrowserWindow();
            var pageFunc = PageBuilder<BrowserWindow, HtmlControl>.CreateFrameLocator(docType, property);
			var page = pageFunc(window);

			Assert.IsNotNull(page);
			Assert.IsInstanceOfType(page, typeof(HtmlFrame));
			Assert.AreEqual("1234", page.SearchProperties[HtmlControl.PropertyNames.Id]);
		}

        /// <summary>
        /// Tests the create page method with the browser in the constructor.
        /// </summary>
        [TestMethod]
        public void TestCreatePageWithBrowserArgument()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            var window = new BrowserWindow();
            var pageFunc = PageBuilder<BrowserWindow, HtmlDocument>.CreateElement(typeof(BrowserDocument));

            var pageObject = pageFunc(window, browser.Object, null);
            var page = pageObject as BrowserDocument;

            Assert.IsNotNull(page);
            Assert.AreSame(browser.Object, page.Browser);
            Assert.AreSame(window, page.Parent);

            browser.VerifyAll();
        }

		#region Class - FrameDocument

		/// <summary>
		/// A test class for seeing if frames will resolve.
		/// </summary>
		public class MasterDocument : HtmlCustom
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="T:Microsoft.VisualStudio.TestTools.UITesting.HtmlControls.HtmlDocument" /> class by using the provided parent control.
			/// </summary>
			/// <param name="parent">The <see cref="T:Microsoft.VisualStudio.TestTools.UITesting.UITestControl" /> that contains this control.</param>
			public MasterDocument(UITestControl parent)
				: base(parent)
			{
			}

			/// <summary>
			/// Gets or sets the frameNavigation.
			/// </summary>
			/// <value>The frame1.</value>
			[ElementLocator(Id = "1234")]
			public HtmlFrame FrameNavigation { get; set; }
		}

		#endregion

		#region Class - BuildPage

		/// <summary>
		/// A test class for the page builder
		/// </summary>
		[PageNavigation("/builds")]
		public class BuildPage : HtmlDocument
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="BuildPage" /> class.
			/// </summary>
			/// <param name="parent">The parent.</param>
			public BuildPage(UITestControl parent)
				: base(parent)
			{
			}

			/// <summary>
			/// Gets or sets the test button.
			/// </summary>
			/// <value>
			/// The test button.
			/// </value>
			[ElementLocator(Id = "MyControl", Text = "The Button")]
			public HtmlButton TestButton { get; set; }

			/// <summary>
			/// Gets or sets my div.
			/// </summary>
			/// <value>
			/// My div.
			/// </value>
			[ElementLocator(Id = "MyDiv", Class = "btn", Text = "The Div!", Title = "The Main Div")]
			public CustomDiv MyDiv { get; set; }

			/// <summary>
			/// Gets or sets the name of the user.
			/// </summary>
			/// <value>
			/// The name of the user.
			/// </value>
			[ElementLocator(Name = "UserName", Text = "Bob")]
			public HtmlEdit UserName { get; set; }

			/// <summary>
			/// Gets or sets the image.
			/// </summary>
			/// <value>
			/// The image.
			/// </value>
			[ElementLocator(Alt = "The Image", Url = "http://myimage")]
			public HtmlImage Image { get; set; }

			/// <summary>
			/// Gets or sets the hyperlink.
			/// </summary>
			/// <value>
			/// The hyperlink.
			/// </value>
			[ElementLocator(Alt = "The Hyperlink", Url = "http://myHyperlink")]
			public HtmlHyperlink Hyperlink { get; set; }

			/// <summary>
			/// Gets or sets the hyperlink.
			/// </summary>
			/// <value>
			/// The hyperlink.
			/// </value>
			[ElementLocator(Alt = "The Hyperlink Area", Url = "http://myHyperlinkArea")]
			public HtmlAreaHyperlink HyperlinkArea { get; set; }

			/// <summary>
			/// Gets or sets my collection.
			/// </summary>
			/// <value>
			/// My collection.
			/// </value>
			[ElementLocator(Id = "ListDiv")]
			public IElementList<HtmlDiv, ListItem> MyCollection { get; set; }

            /// <summary>
            /// Gets or sets the driver.
            /// </summary>
            /// <value>The driver.</value>
            [ElementLocator(Id = "Table")]
            public CodedUITableDriver MyTable { get; set; }
		}

		/// <summary>
		/// A custom div element.
		/// </summary>
		public class CustomDiv : HtmlDiv
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="CustomDiv" /> class.
			/// </summary>
			/// <param name="parent">The parent.</param>
			public CustomDiv(UITestControl parent)
				: base(parent)
			{
			}

			/// <summary>
			/// Gets or sets the test button.
			/// </summary>
			/// <value>
			/// The test button.
			/// </value>
			[ElementLocator(Id = "InternalItem")]
			public HtmlButton InternalButton { get; set; }
		}

		/// <summary>
		/// An inner list item.
		/// </summary>
		[ElementLocator(TagName = "LI")]
		public class ListItem : HtmlCustom
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ListItem" /> class.
			/// </summary>
			/// <param name="parent">The parent.</param>
			public ListItem(UITestControl parent)
				: base(parent)
			{
			}

			/// <summary>
			/// Gets or sets my title.
			/// </summary>
			/// <value>
			/// My title.
			/// </value>
			[ElementLocator(Id = "itemTitle")]
			public HtmlDiv MyTitle { get; set; }
		}

		#endregion

		#region Class - NoConstructorElement

		/// <summary>
		/// An invalid class that has no constructor.
		/// </summary>
		public class NoConstructorElement : HtmlDocument
		{
		}

		#endregion

        #region Class - BrowserDocument

        /// <summary>
        /// Class BrowserDocument.
        /// </summary>
        public class BrowserDocument : HtmlDocument
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BrowserDocument"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="browser">The browser.</param>
            public BrowserDocument(UITestControl parent, IBrowser browser)
                : base(parent)
            {
                this.Parent = parent;
                this.Browser = browser;
            }

            /// <summary>
            /// Gets the parent.
            /// </summary>
            /// <value>The parent.</value>
            public UITestControl Parent { get; private set; }

            /// <summary>
            /// Gets the browser.
            /// </summary>
            /// <value>The browser.</value>
            public IBrowser Browser { get; private set; }
        }

        #endregion
	}
}