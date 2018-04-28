Feature: Browser Tests

Scenario Outline: Navigate to the Home page
    Given the browser factory configuration
      | Field        | Value          |
      | Browser Type | <Browser Type> |
      | Settings     | <Settings>     |
    And I navigated to the Home page
    Then I am on the Home page

    Examples: 
    | Description       | Browser Type | Settings |
    | Internet Explorer | IE           |          |
    | Chrome            | Chrome       |          |

Scenario: Get chrome browser logs
    Given the browser factory configuration
      | Field        | Value  |
      | Browser Type | Chrome |
    Then I can get the browser logs