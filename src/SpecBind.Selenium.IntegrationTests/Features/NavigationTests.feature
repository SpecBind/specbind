@mstest:DeploymentItem:TechTalk.SpecFlow.MSTest.SpecFlowPlugin.dll
Feature: Regression tests to ensure navigation functions correctly
	
Scenario: Navigate to an initial screen
	Given I navigated to the Home page
	 Then I am on the Home page 

Scenario: Navigate to a student screen with a parameter
	Given I navigated to the Student Detail page with parameters
			| Id |
			| 1  |
	 Then I see
			| Field      | Rule   | Value     |
			| First Name | Equals | Carson    |
			| Last Name  | Equals | Alexander |

Scenario: Navigate to Google
   Given I navigated to the Google Home page
   Then I see
         | Field      | Rule       | Value |
         | Search Box | Is Enabled | True  |

Scenario: Navigate to an initial screen with a post-navigate hook
   Given I navigated to the Home page
    Then I ensure token NavigatedPageSuccess matches rule equals with value HomePage
