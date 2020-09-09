### Overview

Most applications consist of lists of elements. These can be grids, tables, or just repeated list of items. The one thing these have in common is a HTML structure that has a single parent element and a repeated list of child elements similar to the following:

```HTML
<div id="productList" class="products">
	<div id="productItem">
		<div id="productName">Product 1</div>
		<!-- more content goes here -->
	</div>
	<div id="productItem">
		<div id="productName">Product 2</div>
		<!-- more content goes here -->
	</div>
</div>
```

### Constructing The List

To construct a list, a special interface exists named *SpecBind.Pages.IElementList\<TParent,TChild\>*. The *TParent* generic type is the element type of the top level element. The *TChild* generic type is the element type of each individual list item. This will generally be a custom nested class to allow access to each field. If we were to represent a list that matched the HTML structure above in Coded UI it would look like:

```C#
using System;
using SpecBind.Pages;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting;

namespace My.Application
{
	[PageNavigation("/products")]
	public class ProductsPage:HtmlDocument
	{
		public ProductsPage(UITestControl parent) : base(parent)
		{
		}

		[ElementLocator(Id = "productList")]
		public IElementList<HtmlDiv, ProductItemDiv> ErrorsPanel { get; set; }

		[ElementLocator(Id = "productItem")]
		public class ProductItemDiv : HtmlDiv
		{
			public ProductItemDiv(UITestControl parent):base(parent)
			{
			}
			
			[ElementLocator(Id = "productName")]
			public HtmlDiv ProductName { get; set; }
		}
	}
}
```

In Selenium the page would look similar:


```C#
using System;
using OpenQA.Selenium;
using SpecBind.Pages;
using SpecBind.Selenium;

namespace My.Application
{
	[PageNavigation("/products")]
	public class ProductsPage
	{
		[ElementLocator(Id = "productList")]
		public IElementList<IWebElement, ProductItemDiv> ErrorsPanel { get; set; }

		[ElementLocator(Id = "productItem")]
		public class ProductItemDiv : WebElement
		{
			public ProductItemDiv(ISearchContext parent):base(parent)
			{
			}
			
			[ElementLocator(Id = "productName")]
			public IWebElement ProductName { get; set; }
		}
	}
}
```

Note that the element locator attribute on the parent property matches the ID of the list element and that the first generic type matches the list container data type, which in this case is a DIV. The second type is a class that represents which exposes properties of each individual list item. This can be any item needed for selection, data entry or validation.