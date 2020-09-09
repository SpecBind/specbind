### Property Mapping - Coded UI

Once you have created your [[page|Page Navigation Binding]], the next step is to define fields or other items on the page that need to be accessed. This is done by adding properties to the page class. The property name is used by SpecBind to match to the requested field name in the step. The system attempts to normalize language as shown in the following example (note these don't make linguistic sense just shown as an example):

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
| Alt | Matches by the alternate text on an element | Image, Hyperlink |
| Class | Matches by the HTML class name on the element | All |
| CssSelector | *Not supported in CodedUI currently* | None |
| Id | Matches by the Id attribute of an element| All |
| Index | Matches by the index of the item if it is in a sequence | All |
| Name | Matches by the name attribute of an input element | Input elements |
| TagName | Matches by the name of the tag i.e. \<li\> = "li" | Should only be used with HtmlCustom controls |
| Text | Matches by the inner text of the element | All |
| Title | Matches by the window title of a contained HTML control | All |
| Type | Matches by the type attribute of an HTML input control | Input elements |
| Url | Matches by the URL of the element | Image, Hyperlink |

The last piece is performed by the data type of the property, which is the native framework class that represents the field type. For in depth details see the the driver's native documentation, however a limited list of items is provided below to provide a basic mapping between HTML elements and properties

| Name | HTML Element | CodedUI Class | Required Locator Attributes |
|------|--------------|---------------|-----------------------------|
| Button | \<button\> | HtmlButton | None |
| Check Box | \<checkbox\> | HtmlCheckBox | None |
| Combo Box | \<select\> | HtmlComboBox | None |
| Div | \<div\> | HtmlDiv | None |
| File Input | \<input type="FileInput"/> | HtmlFileInput | None |
| Hyperlink | \<a\> | HtmlHyperlink | None |
| Input Submit | \<input type="submit"\> | HtmlInputButton | Type="submit" |
| Password | \<input type="password"\> | HtmlEdit | Type="PASSWORD" |
| Span | \<span\> | HtmlSpan | None |
| Text Area | \<textarea\> | HtmlTextArea | None |
| Textbox | \<input\> | HtmlEdit | None |

Once these have been completed a property in your class would look something like the following sample:

```C#
using System;
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

		[ElementLocator(Id = "submit")]
		public HtmlButton Submit { get; set; }
	}
}
```

## Accessing Element Attributes

If you want to validate a property of an element, you don't need to create a property that accesses it. Instead you can add a _PropertyAccess_ attribute on the element. It requires the property you wish to access returned as a string and the new name of the property. Note this property can only be used for validation. 

``` C#

    [ElementLocator(Id = "loginLink")]
    [PropertyAccess(Attribute="Hyperlink" Name="LogOnUrl")]
    public HtmlButton LogOn { get; set; }
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

		[ElementLocator(Id = "validation-errors")]
		public ErrorPanelDiv ErrorsPanel { get; set; }

		public class ErrorPanelDiv : HtmlDiv
		{
			public ErrorPanelDiv(UITestControl parent):base(parent)
			{
			}
			
			[ElementLocator(TagName = "LI", Index = 1)]
			public HtmlCustom ErrorItem { get; set; }
		}
	}
}
```

This way you can access the error item as a nested element. In some cases you may event want to access this as a top level item during validation. To do this, simply create a read-only property in the class that gets the element and returns the string text to validate. For instance if in the example above I want to get the content of the error item and validate it as a property named "Login Error" I would add the following property to my class:

```C#
using System;
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

		public string LoginError
		{
			get
			{
				return this.ErrorsPanel.ErrorItem.InnerText;
			}
		}

		[ElementLocator(Id = "validation-errors")]
		public ErrorPanelDiv ErrorsPanel { get; set; }

		public class ErrorPanelDiv : HtmlDiv
		{
			public ErrorPanelDiv(UITestControl parent):base(parent)
			{
			}
			
			[ElementLocator(TagName = "LI", Index = 1)]
			public HtmlCustom ErrorItem { get; set; }
		}
	}
}
```