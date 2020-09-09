### Overview

There are times when your tests won't run like they're supposed to, and SpecBind has some mechanisms to help with diagnostics. 

### Highlight Mode

If you're walking through your tests and think that SpecBind is having issues locating an element, you can enable hightlight mode. In this mode, SpecBind will draw a box around each element it tries to locate.

To enable this feature for all scenarios, simply add an application setting named `HighlightMode` with a value of `true`.

If you want to add it to a single scenario add the `@Highlight` attribute to the top of a scenario or feature and it will highlight.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="HighlightMode" value="true" />
  </appSettings>
</configuration>
```

### Logging

SpecBind logs output of the feature to the SpecFlow trace listener. This logs to the console by default, but you can create other `TechTalk.SpecFlow.Tracing.ITraceListener` implementations to log to other locations.