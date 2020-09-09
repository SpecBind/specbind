### Overview

In SpecBind, the glue that links page steps with the underlying CodedUI framework is a page model that is represented in code. As discussed in other sections, pages are instantiated with [[navigation steps|Navigation Steps]] that are either an explicit navigation action or the result of a link click or page submission. 

The life cycle of this is as follows:

1. User calls the Navigate step with a page name specified (i.e. home)
2. The framework looks for a class that inherits from the proper driver base class and matches the name. (i.e. home -> HomePage)
3. The page is instantiated and validated against the URL defined by the PageNavigation attribute.
4. The page is ready for use as the context for further actions.

### Page Construction

Most pages follow some simple construction rules that must be observered:

* Pages must be non-abstract classes and inherit from the appropriate base class
   * For CodedUI this is *Microsoft.VisualStudio.TestTools.UITesting.HtmlControls.HtmlControl*
   * For Selenium there is no page base class requrement
* Page class names must end in Page. So the "home" page class would be named *HomePage*
* For CodedUI pages must have a single constructor that has a single parameter of type *Microsoft.VisualStudio.TestTools.UITesting.UITestControl*. This parameter should be passed to the base class constructor.
* Pages should contain a *PageNavigation* attribute that defines the URL match for the page. More on this in the [[Linking Navigation|Page-Navigation-Binding#linking-navigation]] section.

A sample page using CodedUI as a driver would look like:

```C#
using System;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting;
using SpecBind.Pages;

namespace My.Application
{
	[PageNavigation("/products")]
	public class ProductsPage:HtmlDocument
	{
		public ProductsPage(UITestControl parent) : base(parent)
		{
		}
	}
}
```

In Selenium it would look like:

```C#
using System;
using OpenQA.Selenium;
using SpecBind.Pages;

namespace My.Application
{
	[PageNavigation("/products")]
	public class ProductsPage
	{
	}
}
```

### Aliasing Pages

In some cases you may have a page where the class name does not match the name in the steps, or multiple names may refer to the same model. In this case you can use the *PageAlias* attribute to specify a different name the page should map to. The following example shows how the "Products" page maps to an alias of "Product List" page.

```C#
using System;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting;
using SpecBind.Pages;

namespace My.Application
{
	[PageNavigation("/products")]
	[PageAlias("Product List")]
	public class ProductsPage:HtmlDocument
	{
		public ProductsPage(UITestControl parent) : base(parent)
		{
		}
	}
}
```
In Selenium it would look like:

```C#
using System;
using OpenQA.Selenium;
using SpecBind.Pages;

namespace My.Application
{
	[PageNavigation("/products")]
	[PageAlias("Product List")]
	public class ProductsPage
	{
	}
}
```

### Working with Pages Outside Your Site

In cases such as Single Sign On or testing integration into 3rd party applications, you may find the need to manipulate pages that are outside your site. This requires a simple modification to the *PageNavigation* attribute to indicate that you are navigating to a fixed URL. To achieve this, change the link string to an absolute URL like "www.mysite.com/login" and add a parameter IsAbsoluteUrl = true in the attribute. The example below demonstrates this.

```C#
using System;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting;
using SpecBind.Pages;

namespace My.Application
{
	[PageNavigation("http://www.mysite.com/login", IsAbsoluteUrl = true)]
	public class ExternalLogin:HtmlDocument
	{
		public ExternalLogin(UITestControl parent) : base(parent)
		{
		}
	}
}
```

In Selenium it would look like:

```C#
using System;
using OpenQA.Selenium;
using SpecBind.Pages;

namespace My.Application
{
	[PageNavigation("http://www.mysite.com/login", IsAbsoluteUrl = true)]
	public class ExternalLogin
	{
	}
}
```

### Linking Navigation

Most navigation and matching is performed through the *PageNavigation* attribute. The first and default argument of this attribute is the sub-portion of the URL to match. This is formed by combining the base URL that you define in the [[configuration|Configuration]] section with this argument, then using it as a "starts with" comparison with the actual browser URL (for [[frames|Frames]] this is the frame URL). Since this is a starts with comparison, a sample URL of (http://mysite.com/products/1) would succeed with the *PageNavigation* attribute was set as "/products". This argument can also contain regular expressions so an argument of "/products/[0-9]+".

SpecBind also uses this navigation argument to compose the URL to navigate to on a navigation step. Most of the time this matches the validation URL and no further work is needed. If you do need to specify regular expressions to match the URL, you may need to also pass arguments into the created URL on a navigation command. For this a second optional argument named *UrlTemplate* can be used. For this specify a standard .NET format string, that will be filled in by the table used in the [[navigation step|Navigation Steps]]. A sample attribute value for the URL (http://mysite.com/products/1) would be "/products/{Id}".

### Setting Cookies

** New with SpecBind 1.5 **
For some sites it may become necessary to manually inject a cookie into the browser before the navigation is performed. This may be to suppress a dialog, set a preference or disable an ad. To do this an attribute exists named *SetCookie* that requires the cookie name and value. **HttpOnly is not supported since the cookie is injected by the browser** It also supports the following options:

|Name|Type|Description|
|----|----|-----------|
| Name | String | The cookie name |
| Value | String | The cookie contents (will be encoded automatically)|
| Path | String | The path under the root to set the cookie, defaults to "/" |
| Domain | String | The domain of the cookie (generally must be your site) |
| IsSecure | Boolean | Only passes the cookie over https |
| Expires | String | A representation of when the cookie expires, see below |

The *Expires* property supports several types of values

| Type | Example | Action |
|-------|---------|--------|
| Infinate Cookie | DateTime.MaxValue | Sets a cookie that never expires |
| Remove Cookie | DateTime.MinValue | Removes a currently set cookie |
| Fixed Date | 12/22/2015 12\:00\:00 AM | A fixed point in time with the code |
| Relative to Now (in seconds) | 120 | Sets a value in seconds relative to the time the test is run|

An example is posted below of how a typical cookie is set and expires in 60 minutes.

```C#
using System;
using OpenQA.Selenium;
using SpecBind.Pages;

namespace My.Application
{
	[PageNavigation("http://www.mysite.com/login", IsAbsoluteUrl = true)]
    [SetCookie("MyCookie", "some value", Expires = "3600")]
	public class ExternalLogin
	{
	}
}
```

### Post Navigation Hooks

In some cases it may become necessary to perform a particular action on a page after it has been navigated to. Examples my be to disable animations or set some JavaScript value to indicate testing. If this is necessary, there's an action pipeline method that can be created to handle this. Simply create the class below in your project and use the _IPage_ object provided to get properties on the page or use the _GetNativePage<>()_ method to get the actual page class you created. This can be useful if you want to use a base class model and invoke the page on each call. The example also demonstrates how you can access the TokenManager inside the hook.

```C#
namespace My.Application
{
    using System.Collections.Generic;

    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    public class TestPostNavigateHook : NavigationPostAction
    {
        private readonly ITokenManager tokenManager;

        public TestPostNavigateHook(ITokenManager tokenManager)
        {
            this.tokenManager = tokenManager;
        }

        protected override void OnPageNavigate(IPage page, PageNavigationAction.PageAction actionType, IDictionary<string, string> pageArguments)
        {
			// Do any page manipulation here
            // The PageAction enum can be used to determine if this was a navigation action or ensure action

            this.tokenManager.SetToken("NavigatedPageSuccess", page.PageType.Name);
        }
    }
}
```