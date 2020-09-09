The core behavior of SpecBind can be configured through .NET configuration files. The configuration has to be placed in a file called *App.config*.

## Default Configuration

The NuGet packages that come with SpecBind will create a default configuration file entry that contains most of the primary information needed to run.
The following sample demonstrates what the SpecBind section will contain after installing the CodedUI package. 

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="specBind" type="SpecBind.Configuration.ConfigurationSectionHandler, SpecBind"/>
  </configSections>
  <specBind>
    <application startUrl="{application URL}" />
    <browserFactory provider="SpecBind.CodedUI.CodedUIBrowserFactory, SpecBind.CodedUI" browserType="IE" />
  </specBind>
</configuration>
```

The following example shows all possible configuration options with their default values (the config section definition has been omitted for better readability).

```xml
<specBind>
    <application startUrl="{application URL}" retryValidationUntilTimeout="false" waitForStillElementBeforeClicking="false">
        <excludedAssemblies>
            <add name="MyCoolApp, Version=1.2.3.0, Culture=neutral, PublicKeyToken=null" />
        </excludedAssemblies>
    </application>
    <browserFactory provider="{provider assembly name}" 
                    browserType="IE"
				    elementLocateTimeout="00:00:30"
                    ensureCleanSession="false"
					pageLoadTimeout="00:00:30"
                    reuseBrowser="false"
                    createScreenshotAfterScenario="true"
                    validateWebDriver="true"
                    waitForPendingAjaxCallsVia="none" >
		<settings>
			<add name="mySetting" value="something" />
		</settings>
    </browserFactory>
</specBind>
```
### Configuration Elements
#### `<application>`
This section can be used to define specific settings for the application to use when it begins testing.

<table>
    <tr>
        <th>Attribute</th>
        <th>Value</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>startUrl</td>
        <td>A browser URL (http://myapp.com)</td>
        <td>The base address of the application being tested.
            This will be combined with the application page addresses 
            for navigation and validation. This can also be specified with an envrionment variable named <b>APPLICATION_START_URL</b>.</td>
    </tr>
    <tr>
        <td>retryValidationUntilTimeout</td>
        <td>true/false (false)</td>
        <td>Indicates whether or not validation actions should retry until the standard ElementLocateTimeout.</td>
    </tr>
    <tr>
        <td>waitForStillElementBeforeClicking</td>
        <td>true/false (false)</td>
        <td>Indicates whether or not to wait for an element to stop moving before clicking on it.</td>
    </tr>
    <tr>
        <td>excludedAssemblies</td>
        <td>A collection of named assemblies (see example)</td>
        <td>A list of assembly names that are excluded from the search for steps.</td>
    </tr>
</table>

#### `<browserFactory>`
This section can be used to define which back-end provider is being used to link the steps to the application being tested. 

<table>
    <tr>
        <th>Attribute</th>
        <th>Value</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>provider</td>
        <td>The assembly name of the provider.</td>
        <td>The fully qualified assembly type of the provider. This should be installed by the NuGet package.</td>
    </tr>
	<tr>
        <td>browserType</td>
        <td>The type of browser to start for web testing. (IE)</td>
        <td>The ID of the browser type to start. See the settings section below for supported browsers.</td>
    </tr>
	<tr>
        <td>elementLocateTimeout</td>
        <td>A time span to wait for an element to appear implicitly. (00:00:30)</td>
        <td>The drivers have implicit wait times for locating elements. Change this to speed up or slow down search times.</td>
    </tr>
    <tr>
        <td>ensureCleanSession</td>
        <td>true/false (false)</td>
        <td>Indicates whether the session cache and cookies should be cleared before starting.</td>
    </tr>
	<tr>
        <td>pageLoadTimeout</td>
        <td>A time span to wait for a page to complete loading. (00:00:30)</td>
        <td>The driver waits for a page load complete, change this if your application needs more time to load a page.</td>
    </tr>
    <tr>
        <td>reuseBrowser</td>
        <td>true/false (false)</td>
        <td>Indicates whether or not the same browser should be reused during tests.</td>
    </tr>
    <tr>
        <td>createScreenshotAfterScenario</td>
        <td>true/false (false)</td>
        <td>Indicates whether or not to create a screenshot of the final step on successful scenarios. <b>New in 2.0</b></td>
    </tr>
	<tr>
        <td>settings</td>
        <td>Name/Value elements that define custom settings for the driver to pick up.</td>
        <td>See the section below for supported custom settings for a driver.</td>
    </tr>
    <tr>
        <td>validateWebDriver</td>
        <td>true/false (true)</td>
        <td>Indicates whether the web driver should be validated as functioning before running tests.</td>
    </tr>
    <tr>
        <td>waitForPendingAjaxCallsVia</td>
        <td>none/angular/jquery (none)</td>
        <td>Indicates what mechanism to use to check for pending AJAX requests before proceeding with each step.</td>
    </tr>
</table>

### Supported Browsers

Each driver has its own supported browsers and the list below is subject to change if the driver changes. The list is provided as a basic reference of what is supported.

| Browser | Coded UI | Selenium    |
|---------|----------|-------------|
| IE      | Yes      | Yes         |
| Chrome  | Yes      | Yes         |
| FireFox | Yes      | Yes         |
| Safari  | No       | Yes         |
| Edge    | No       | Yes         |
| ChromeHeadless | No       | Yes **(New 2.0)** |
| Android | No       | No **(Removed 2.0)** |
| iPhone  | No       | No **(Removed 2.0)** |
| iPad    | No       | No **(Removed 2.0)** |


### Driver Settings

The setting section of the configuration can be used to pass configuration values to the visual drivers. The following sections describe any settings that can be passed in at this point.

#### Coded UI Settings

At this point there are no settings that need to be mapped from Coded UI. 

#### Selenium Settings

Selenium support the concept of a "remote" driver. This driver allows you to call a 3rd party service to perform a test. To allow Selenium to support this, you need to specify a setting called "RemoteUrl" and give the URL of the remote Selenium service. The system will then use the browser type on the Remote URL. Any other settings are passed into Selenium's DesiredCapabilities settings section.

##### Environment Variables

If you want to supply a setting value through environment variables, set the value of the setting to the format  `${<variable name>}`. So for instance if your environment variable is `BROWSER_KEY` then the value would look like `${BROWSER_KEY}`.

If you're interested in using BrowserStack with Selenium, the following settings will help you create a test.

| Name | Required | Value | Description |
|------|----------|-------|-------------|
| RemoteUrl | Yes | http://hub.browserstack.com:80/wd/hub/ | The BrowserStack connection |
| browserstack.user | Yes | {username} | Your username for BrowserStack |
| browserstack.key  | Yes | {access key} | Your access key |
| browserstack.debug | No | true/false     | Indicates if the test should run in debug mode |
| browserstack.tunnel | No | true/false    | Indicates if tunneling back to a local machine is supported.|
| os                | No  | Windows        | The OS name |
| os_version                | No  | 8        | The OS version |
| version | No | 10 | The browser version |

See this [page](http://www.browserstack.com/automate/c-sharp#setup "page") for other settings and latest details.
