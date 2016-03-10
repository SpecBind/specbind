# How to contribute

One of the easiest ways to contribute is to participate in discussions and discuss issues. You can also contribute by submitting pull requests with code changes.

## General feedback, discussions, bugs, feature requests?
Please start a discussion or log a new issue on the [issue tracker](https://github.com/dpiessens/specbind/issues) page.

## Filing issues
When filing issues, please skim the current open issues to see if yours is already there.
Providing steps to reproduce the problem is ideal.
Here are questions you can answer before you file a bug to make sure you're not missing any important information.

1. Did you read the [documentation](https://github.com/dpiessens/specbind/wiki)?
2. Did you include the snippet of broken code in the issue?
3. What are the *EXACT* scenario steps to reproduce this problem?
4. What version of SpecBind are you using?
5. What operating system and browser (version) are you using to test?
6. What driver are you using (Selenium or CodedUI)?

GitHub supports [markdown](http://github.github.com/github-flavored-markdown/), so when filing bugs make sure you check the formatting before clicking submit.

## Contributing code and content
If you don't know what a pull request is read this article: https://help.github.com/articles/using-pull-requests.
You might also read these two blogs posts on contributing code: [Open Source Contribution Etiquette](http://tirania.org/blog/archive/2010/Dec-31.html) by Miguel de Icaza and [Don't "Push" Your Pull Requests](http://www.igvita.com/2011/12/19/dont-push-your-pull-requests/) by Ilya Grigorik.

## Development Tools
- Visual Studio 2015

## Before submitting a pull request

To avoid delays in pull requests please check the following before submitting:
- Does the project build?
- If you added a feature is there an implementation for Selenium and CodedUI?
- Do all the unit tests pass (Main, CodedUI and Selenium)?
- Did you look for StyleCop (SA) Warnings? Build failure on this is not currently enabled.
- Is there documentation for the feature? If so, include in the issue.

The build will be checked by AppVeyor so if it passes, and the request can be merged it will be accepted.
