### Property Mapping - Selenium

Once you have created your [page](Page-Navigation-Binding.md), the next step is to define fields or other items on the page that need to be accessed. This is done by adding properties to the page class. The property name is used by SpecBind to match to the requested field name in the step. The system attempts to normalize language as shown in the following example (note these don't make linguistic sense just shown as an example):

Property Name: **RoomName**

Matching Field Names in Scenario:

* room name
* Room Name
* The room name
* A room name
* room a name

Properties can all be represented as public get/set automatic properties, since the model will populate the fields when the page class is created. The other piece of data that is needed is the link between the actual HTML element and the field. This is done with the *ElementLocator* attribute. This attribute allows you to define HTML element attributes that the system can use to locate the item within its search scope. The most common locators are *Id* and *Name*, corresponding to the respective attributes on an HTML element. The table below defines what locator attributes are available. It is recommended that you use Id or Name as a primary locator and other attributes as further filters if ambiguity exists.

|Attribute Name|Description|Applicable HTML Elements|
|--------------|-----------|------------------------|
| Alt | Matches by the alternate text on an element | Image, Hyperlink, Requires TagName |
| Class | Matches by the HTML class name on the element | All |
| CssSelector | Matches by a CSS selector tied to the element | All |
| Id | Matches by the Id attribute of an element| All |
| Index | Matches by the index of the item if it is in a sequence | All |
| Name | Matches by the name attribute of an input element | Input elements |
| TagName | Matches by the name of the tag i.e. \<li\> = "li" | Encouraged on most controls that don't search by ID or Name |
| Text | Matches by the inner text of the element | All, Requires TagName |
| Title | Not Supported | - |
| Type | Matches by the type attribute of an HTML input control | Input elements, Requires TagName |
| Url | Matches by the URL of the element | Image, Hyperlink, Requires TagName |
| XPath | Matches complex or custom elements by their XPath lookup | All, **New 2.0** |

Selenium also provided a similar attribute named *FindByAttribute* in their support assembly. SpecBind respects the existence of these tags to support pages created before SpecBind and will remove any duplicate locators.

All framework properties for Selenium are represented as *IWebElement* items. Once these have been completed a property in your class would look something like the following sample:

```C#
using System;
using OpenQA.Selenium;
using SpecBind.Pages;

namespace My.Application
{
	[PageNavigation("/products")]
	public class ProductsPage
	{
		[ElementLocator(Id = "submit")]
		public IWebElement Submit { get; set; }
	}
}
```

## Nested Properties

In some cases you may need to nest into a set of HTML elements to get to a certain property. This is accomplished fairly easily using a nested class. The nested class would inherit from the HTML data type of the top level element you need to match and you can then define properties within that nested class that match the child element. Let's say you have a page who's validation error pane HTML looks like this:

```HTML
<div id="validation-errors">
  <ul class="errorList">
   <li class="error">This went really wrong</li>
  </ul>
</div>
```

A page model to get to the first error in this list would look like:


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
		[ElementLocator(Id = "validation-errors")]
		public ErrorPanelDiv ErrorsPanel { get; set; }

		public class ErrorPanelDiv : WebElement
		{
			public ErrorPanelDiv(ISearchContext parent):base(parent)
			{
			}
			
			[ElementLocator(TagName = "LI", Index = 1)]
			public IWebElement ErrorItem { get; set; }
		}
	}
}
```

This way you can access the error item as a nested element. In some cases you may event want to access this as a top level item during validation. To do this, simply create a read-only property in the class that gets the element and returns the string text to validate. For instance if in the example above I want to get the content of the error item and validate it as a property named "Login Error" I would add the following property to my class:

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
		public string LoginError
		{
			get
			{
				return this.ErrorsPanel.ErrorItem.Text;
			}
		}

		[ElementLocator(Id = "validation-errors")]
		public ErrorPanelDiv ErrorsPanel { get; set; }

		public class ErrorPanelDiv : WebElement
		{
			public ErrorPanelDiv(ISearchContext parent):base(parent)
			{
			}
			
			[ElementLocator(TagName = "LI", Index = 1)]
			public IWebElement ErrorItem { get; set; }
		}
	}
}
```

## Accessing Element Attributes

If you want to validate a property of an element, you don't need to create a property that accesses it. Instead you can add a _VirtualProperty_ attribute on the element. It requires the HTML attribute you wish to access returned as a string and the new name of the property. Note this property can only be used for validation. 

``` C#

    [ElementLocator(Id = "loginLink")]
    [VirtualProperty(Attribute="href", Name="LogOnUrl")]
    public IWebElement LogOn { get; set; }
```