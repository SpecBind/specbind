Selecting items on a page are one of the most common actions on a page is one of the most common actions performed. To click on an item use the following step:

| Verb  | Action                     |
|-------|----------------------------|
| Given | I chose \<property name\>  |
| When  | I choose \<property name\> |

The \<property name\> argument is used to map to the [property](Page-Model-Properties.md) on the page to select. Most elements in the frameworks are clickable so this should be supported with most element types. 

As with other property locators, SpecBind normalizes the name so that you can make the step more readable. For instance if your property name is "FirstName" you can enter the value "First Name" in the step and it will locate the correct property.
 
**If this results in navigation to a new page or display of a inline dialog, you must use the [Ensure Step](Navigation-Steps.md) step next to set the correct context.**

#### Example ####

```Cucumber
When I choose Checkout
``` 

### Browser Dialogs

At times browsers may raise a JavaScript alert to notify the user of something. These alerts are simple and generally have an accept and dismiss button and optionally a text entry field. There are two steps available to dismissing an alert. The first is for an alert that does not contain a text field:

| Verb  | Action                     |
|-------|----------------------------|
| Given | I saw an alert box and selected \<button name\>  |
| When  | I see an alert box and select \<button name\> |

```Cucumber
When I see an alert box and select Ok
``` 

In this case button name can be one of Ok, Cancel, Yes, No, Retry, Ignore. If text needs to be entered the following can be used:

| Verb  | Action                     |
|-------|----------------------------|
| Given | I saw an alert box, entered "\<text\>" and selected \<button name\>  |
| When  | I see an alert box, enter "\<text\>" and select \<button name\> |

```Cucumber
When I see an alert box, entered "Hello!" and select Ok
``` 

In this case \<text\> is what you want to put in the input box and should be surrounded in quotes.

### Hovering Over an Element

If an element is only visible when you hover over it, you can use the following steps to hover on an item to get that element to appear. It should be noted that this assumes the browser driver does not manipulate the screen further after the step completes. You should use an "[I see](Verifying-Steps)" step to validate the item that is now visible.

| Verb  | Action                           |
|-------|----------------------------------|
| Given | I hovered over \<element name\>  |
| When  | I hover over \<element name\>    |

```Cucumber
When I hover over OK
``` 