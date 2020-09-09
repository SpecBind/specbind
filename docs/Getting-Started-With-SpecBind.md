## Getting to your web application
The first step in working with SpecBind and web applications is letting the application know where the root URL of the site is. This is done in the app.config file of the project, under an element named *application* and an attribute named _startUrl_. If you were using CodedUI as your driver, your configuration section would look something like this:

```xml
<specBind>
    <application startUrl="http://something.com/MyApplication" />
    <browserFactory provider="SpecBind.CodedUI.CodedUIBrowserFactory, SpecBind.CodedUI" browserType="Chrome" />
</specBind>
```

## Step Overview

One of the key features of SpecBind is it features a common set of steps that can be used to describe actions you take in validating a web application. These steps are outlined by their functionality in this guide. One thing to note is how they work with the Gherkin language. For any **Given** steps the step is in the past tense. This is because it assumes the action is a prerequisite and has already occurred. The **When** and **Then** steps are in the present tense since they are what the test indicates should occur. In this guide, both commands will be outlined so that the syntax is know, but only the present tense will be used for examples. 

See the following topics for more information about available steps and actions:

* [Navigation](Navigation-Steps.md)
* [Waiting For a Page To Load](Waiting-Steps.md)
* [Entering Data](Entering-Data-Steps.md)
* [Clicking Items](Clicking-Items-Steps.md)
* [Verifying Items](Verifying-Steps.md)
* [Saving and Reusing Data With Tokens](Using-Tokens.md)


