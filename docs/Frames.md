### Overview

Many legacy sites use frames to divide regions of the application. The challenge then becomes how to ask for a page without needed to nest the frame element in each page. This is done with a simple class that maps the frame structure.

### Coded UI Approach

The following sections how to map frames with the CodedUI driver.

#### Constructing the Frame Map

Most frames are in a top level element called a frameset. Each internal element is a frame, that most likely contains an ID. To Mimic this we create a class that contains a special attribute named *FrameMap*. This attributes indicates to the framework that the site is composed of frames. This class then contains properties (for CodedUI these are of type HtmlFrame) that map to each frame in the page. A sample class that contains a menu and main frame looks as follows:

```C#
	[FrameMap]
	[ElementLocator(Id = "fSet1", TagName = "FRAMESET")]
	public class FrameStructureElement : HtmlCustom
	{
		public FrameStructureElement(UITestControl parent) : base(parent)
		{
		}

		[ElementLocator(Id = "Menu")]
		public HtmlFrame MenuFrame { get; set; }

		[ElementLocator(Id = "Main")]
		public HtmlFrame MainFrame { get; set; }
	}
``` 

#### Assigning Pages To Frames

Once the frame map has been setup, you can use a property named *FrameName* on the *PageNavigation* attribute. The string value here should match the property name of the frame assigned in the frame map. So to place a page in the main frame, you would set the *FrameName* property to "MainFrame" as shown in the example below. It is suggested that you create a constants file for this in the application to allow for consistency.

```C#
using System;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting;
using SpecBind.Pages;

namespace My.Application
{
	[PageNavigation("/products", FrameName = "MainFrame")]
	public class ProductsPage:HtmlDocument
	{
		public ProductsPage(UITestControl parent) : base(parent)
		{
		}
	}
}
```

### Selenium Approach

The following sections how to map frames with the Selenium driver. Unlike CodedUI you don't need to create a frame map. You only need to assign pages to frames.

#### Assigning Pages To Frames

To indicate what frame a page belongs in you use a property named *FrameName* on the *PageNavigation* attribute. The string value here should match the name of the the frame on the page. Note that this is an exact match to the visual element so it is suggested that you create a constants file for this in the application to allow for consistency.

```C#
using System;
using OpenQA.Selenium;
using SpecBind.Pages;

namespace My.Application
{
	[PageNavigation("/products", FrameName = "uiFrame")]
	public class ProductsPage
	{
	}
}
```