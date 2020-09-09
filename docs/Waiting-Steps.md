In some scenarios, your page may have JavaScript animations or some other page load that the browser otherwise considers ready. In this case you may want to introduce an artificial delay to wait for that transition to complete. The follow step assists with this, and will wait for an active flag from the page.

| Verb | Action |
|------|--------|
| Given | I waited for the view to become active |
| When | I wait for the view to become active | 

While the above method introduces the most flexibility, a simpler set of steps also exist. These setups can check for an element to be displayed, not displayed, exist or not exist and optionally wait a given number of seconds for that to happen. This can be useful for scenarios where a wait dialog has appeared and you don't wish to perform further work until it has cleared. It also implicitly sets performance benchmarks. The commands are listed below with the (for \<seconds\> seconds) as optional.

At times you may need to wait for a page become the URL (i.e. Login, Redirect etc.) In this scenario you can use the following:

| Verb | Action |
|------|--------|
| Given | I waited for the \<page name\> page |
| When, Then | I wait for the \<page name\> page |

This will wait the default timeout for that page to become the active URL. You can also specify the timeout if you need more or less time:

| Verb | Action |
|------|--------|
| Given | I waited \<count\> seconds for the \<page name\> page |
| When, Then | I wait \<count\> seconds for the \<page name\> page |

**Check to see if an element is displayed**

| Verb | Action |
|------|--------|
| Given | I waited to see \<property name\> |
| When | I wait to see \<property name\> |
| Then | I wait to see \<property name\> | 

| Verb | Action |
|------|--------|
| Given | I waited for \<seconds\> seconds to see \<property name\> |
| When | I wait for \<seconds\> seconds to see \<property name\> |
| Then | I wait for \<seconds\> seconds to see \<property name\> | 

**Check to see if an element is not displayed**

| Verb | Action |
|------|--------|
| Given | I waited to not see \<property name\> |
| When | I wait to not see \<property name\> |
| Then | I wait to not see \<property name\> | 

| Verb | Action |
|------|--------|
| Given | I waited for \<seconds\> seconds to not see \<property name\> |
| When | I wait for \<seconds\> seconds to not see \<property name\> |
| Then | I wait for \<seconds\> seconds to not see \<property name\> | 

**Check to see if an element is enabled**

| Verb | Action |
|------|--------|
| Given | I waited for \<property name\> to become enabled |
| When | I wait for \<property name\> to become enabled |
| Then | I wait for \<property name\> to become enabled | 

| Verb | Action |
|------|--------|
| Given | I waited \<seconds\> seconds for \<property name\> to become enabled |
| When | I wait \<seconds\> seconds for \<property name\> to become enabled |
| Then | I wait \<seconds\> seconds for \<property name\> to become enabled | 

The example below shows waiting for Checkout to become enabled:

```Cucumber
Given I waited for Checkout to become enabled
``` 

**Check to see if an element is disabled**

| Verb | Action |
|------|--------|
| Given | I waited \<seconds\> seconds for \<property name\> to become disabled |
| When | I wait \<seconds\> seconds for \<property name\> to become disabled |
| Then | I wait \<seconds\> seconds for \<property name\> to become disabled | 

| Verb | Action |
|------|--------|
| Given | I waited for \<property name\> to become disabled |
| When | I wait for \<property name\> to become disabled |
| Then | I wait for \<property name\> to become disabled | 

**Check to see if a list element contains items**

| Verb | Action |
|------|--------|
| Given | I waited \<seconds\> seconds for \<property name\> to contain items |
| When | I wait \<seconds\> seconds for \<property name\> to contain items |
| Then | I wait \<seconds\> seconds for \<property name\> to contain items | 

| Verb | Action |
|------|--------|
| Given | I waited for \<property name\> to contain items |
| When | I wait for \<property name\> to contain items |
| Then | I wait for \<property name\> to contain items | 

**Check to see if ajax calls are completed**

| Verb | Action |
|------|--------|
| Given | I waited for angular ajax calls to complete |
| When |  I wait for angular ajax calls to complete |
| Then |  I wait for angular ajax calls to complete | 

| Verb | Action |
|------|--------|
| Given | I waited for jquery ajax calls to complete |
| When |  I wait for jquery ajax calls to complete |
| Then |  I wait for jquery ajax calls to complete | 
