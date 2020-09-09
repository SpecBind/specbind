## What is SpecBind?

SpecBind is an extension to SpecFlow that uses common language steps to allow a user to define interaction with an application. Unlike conventional SpecFlow where the user needs to define how each step definition interacts with the host system, SpecBind uses conventions and a thin page model to minimize coding efforts and maximize flexibility when defining the link between specifications and working sites.

## Initial Setup

Getting SpecBind installed is a simple process:

1. Go to http://www.specflow.org and follow the instructions for installing the Visual Studio Extension
2. Choose the driver type you want to use to interact with the host system:
	* Coded UI
	* Selenium

3. Create a new test project in Visual Studio, Make it a Coded UI project if you are using that.
4. Install the NuGet package for SpecBind

  * [Coded UI](https://www.nuget.org/packages/SpecBind.CodedUI)	``PM\> Install-Package SpecBind.CodedUI``
  * [Selenium](https://www.nuget.org/packages/SpecBind.Selenium)``PM\> Install-Package SpecBind.Selenium``

And you're done! 

## Step Overview

One of the key features of SpecBind is it features a common set of steps that can be used to describe actions you take in validating a web application. These steps are outlined by their functionality in this guide. One thing to note is how they work with the Gherkin language. For any **Given** steps the step is in the past tense. This is because it assumes the action is a prerequisite and has already occurred. The **When** and **Then** steps are in the present tense since they are what the test indicates should occur. In this guide, both commands will be outlined so that the syntax is know, but only the present tense will be used for examples. 

## Next Steps

Follow the [Getting Started](Getting-Started-With-SpecBind.md) section to begin creating tests, or [Documentation](Documentation.md) for in detailed information on SpecBind.

## Table of Contents

### Initial Setup
* [Getting Started With SpecBind](Getting-Started-With-SpecBind.md)
* [Configuration](Configuration.md)
* [Debugging](Debugging.md)

### Understanding Available Steps
* [Navigation Steps](Navigation-Steps.md)
* [Waiting Steps](Waiting-Steps.md)
* [Entering Data Steps](Entering-Data-Steps.md)
* [Clicking Items Steps](Clicking-Items-Steps.md)
* [Verifying Steps](Verifying-Steps.md)
* [Using Tokens](Using-Tokens.md)

### Developing Page Binding Code
* [Page Navigation Binding](Page-Navigation-Binding.md)
* [Page Model Properties - Coded UI](Page-Model-Properties.md)
* [Page Model Properties - Selenium](Page-Model-Properties-Selenium.md)
* [Working With Lists](Working-With-Lists.md)
* [Dialog Pages](Dialog-Pages.md)
* [Frames](Frames.md)
* [Executing JavaScript In Pages](Executing-JavaScript-In-Pages.md)
* [Custom Validation Rules](Custom-Validation-Rules.md)