Feature: Browser Tests

Scenario Outline: Navigate to the Home page
    Given the browser factory configuration
      | Field        | Value          |
      | Provider     | <Provider>     |
      | Browser Type | <Browser Type> |
      | Settings     | <Settings>     |
    And I navigated to the Home page
    Then I am on the Home page

    Examples: 
    | Description       | Provider                                                 | Browser Type | Settings |
    | Internet Explorer | SpecBind.CodedUI.CodedUIBrowserFactory, SpecBind.CodedUI | IE           |          |
    | Chrome            | SpecBind.CodedUI.CodedUIBrowserFactory, SpecBind.CodedUI | Chrome       |          |