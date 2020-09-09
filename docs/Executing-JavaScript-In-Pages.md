### Overview

For complicated scenarios it may become necessary to execute JavaScript within the page as a property is being accessed. It should be noted that while support is built in for this, it should be used with caution as this script and could make tests more brittle.

### Accessing the JavaScript Runner

Any page, child element or list element can have the *SpecBind.BrowserSupport.IBrowser* interface injected into the constructor and then stored as a field for use while the page is being used. From there a method named *ExecuteScript* can be used to run the script. That method accepts parameters that are parsed as JavaScript variables and can return an object. If the result it a string it will be returned, otherwise if it represents a DOM element then it will be wrapped with the appropriate element wrapper. An example of this in use is as follows:

```C#
using System;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting;
using SpecBind.Pages;
using SpecBind.BrowserSupport;

namespace My.Application
{
	[PageNavigation("/products")]
	public class ProductsPage:HtmlDocument
	{
		private readonly IBrowser browser;
		
		public ProductsPage(IBrowser browser, UITestControl parent) : base(parent)
		{
			this.browser = browser;
		}

		public string ObscureData
		{
			get
			{
				return (string)this.browser.ExecuteScript("return $('.obscure')[0]");
			}
		}
	}
}
```

In Selenium it would look similar:

```C#
using System;
using OpenQA.Selenium;
using SpecBind.Pages;
using SpecBind.BrowserSupport;

namespace My.Application
{
	[PageNavigation("/products")]
	public class ProductsPage
	{
		private readonly IBrowser browser;
		
		public ProductsPage(IBrowser browser)
		{
			this.browser = browser;
		}

		public string ObscureData
		{
			get
			{
				return (string)this.browser.ExecuteScript("return $('.obscure')[0]");
			}
		}
	}
}
``` 