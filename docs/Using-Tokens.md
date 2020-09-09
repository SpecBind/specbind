At points in your scenario you may want to gather dynamic data that you use at a later date for [navigation](Navigation-Steps.md), [data entry](Entering-Data-Steps.md) or additional [verification](Verifying-Steps.md). One common example of this is you create a new record and wish to validate that deep linking with that record's ID works. You don't know the ID at the time you write the scenario so you gather it during the scenario and use that token value later to make sure it matches the one you gathered.

### Gathering Data With Tokens ###

Data gathering typically occurs during an [input](Entering-Data-Steps.md) step. Instead of specifying a value you specify a curly brace "\{" followed by the name of the token without spaces or special characters, followed by a closing curly brace "\}". The colon "\:" that follows is the value section of the token. You can put any static data you need here. This may also come from a scenario hook. An example of this in a SpecFlow table would be:

```Cucumber
When I enter data
| Field | Value     |
| ID    | {MyToken:Some Data} |
```

In some cases you may want to enter random data to test items like string lengths or fuzz testing. To do this some special values exist for the value portion of the token. If you have other needs for random data let us know! Here are some examples:

| Token | Description | Optional Value |
|-------|-------------|----------------|
| \{MyToken:randomstring:10\} | Generate a random string value | The maximum length of the string to generate (default max length is 30) |
| \{MyToken:randomguid\} | Generate a random GUID value | None |
| \{MyToken:randomint\} | Generate a random integer value | None |

There are also situations where you may want to set the token value from a given field at a particular point in time. For this a step exists: 

| Verb | Action |
|------|--------|
| Given, When, Then | I set token \<token name\> with the value of \<property name\> |

Similar to other actions *token name* corresponds with the name of the token you want to set, and *property name* indicates the property you want to gather it from. The follow example shows a way to set a token named "MyToken" with the value of the "Customer ID" field.

```Cucumber
When I set token MyToken with the value of Customer ID
```

### Special Characters ###
In some cases you may need to enter data that matches the format of a token `{{TAB}}`. To do so, simply double the curly braces and the content will be treated as entered.

### Verifying Data With Tokens ###

Similarly, when you need check this value during [verification](Verifying-Steps.md), you can use a token to specify the value you wish to check. It is applied to the rule in the same way as a static value would be applied. The same token syntax applies to parameters in [navigation](Navigation-Steps.md) steps.

```Cucumber
When I see
| Field | Rule   | Value     |
| ID    | Equals | {MyToken} | 
```

### Verifying Token Values ###

At points in the process you may want to validate that a token has a given value. There is a step in SpecBind that allows you to do this. 

*Introduced in version 1.4*

| Verb | Action |
|------|--------|
| Given, When, Then | I ensure token \<token name\> matches rule \<rule\> with value \<check value\> |

The *token name* variable is the name of the token. *rule* is the comparison you want to use against the *check value*. In most cases this is _equals_, but you can use any comparison step used in [verification](Verifying Steps.md).


### Getting or Setting Token Values In Code ###

In some cases you may need custom steps or scenario hooks that setup or tear down data and set token values in these areas of code. For this, an interface exists named *SpecBind.Helpers.ITokenManager* that you can request in the constructor of your step. The example below shows two custom steps that get and set a token value. To learn more about injecting dependencies into your steps see [Context Injection](https://github.com/techtalk/SpecFlow/wiki/Context-Injection) in SpecFlow documentation. 

Note that this interface contains two methods: *GetToken* and *GetTokenByKey*. *GetToken* is used to to pass in a value that is potentially a token but could be a static value, checks to see if it is and returns the token value. If the value is not a token, the original value is returned. *GetTokenByKey* expects only a token key and will return the value if located or null.

```C#
using System;
using TechTalk.SpecFlow;
using SpecBind.Helpers;

namespace MyApplication.MySteps
{
    [Binding]
    public class MyCustomSteps
    {
	private const string TokenId = "WidgetToken"; 

	private readonly ITokenManager tokenManager;
		
	public MyCustomSteps(ITokenManager tokenManager)
	{
            this.tokenManager = tokenManager;
	}

    	[Given("I create a new widget")]
    	public void GivenICreateANewWidget()
    	{
            int myNewWidgetId = 1;
	
            // Do some database stuff here
            this.tokenManager.SetToken(TokenId, myNewWidgetId);
    	}

    	[Then("I remove the new widget")]
    	public void ThenIRemoveTheNewWidget()
    	{
            int myNewWidgetId = this.tokenManager.GetTokenByKey(TokenId);	
            // Do some database stuff here to remove the widget
    	}
    }
}
```