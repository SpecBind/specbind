Feature: Application Configuration Tests

Scenario Outline: Multiple starting urls in a scenario outline
    Given the application configuration
    | Start Url   |
    | <Start Url> |
    When I navigate to the Home page
    Then I am at the url "<Full Home Page Url>"

    Examples: 
    | Description | Start Url                           | Full Home Page Url                  |
    | localhost   | http://localhost/ContosoUniversity/ | http://localhost/ContosoUniversity/ |
    | 127.0.0.1   | http://127.0.0.1/ContosoUniversity/ | http://127.0.0.1/ContosoUniversity/ |