@mstest:DeploymentItem:TechTalk.SpecFlow.MSTest.SpecFlowPlugin.dll
@mstest:DeploymentItem:SpecBind.MsTest.Steps.dll
Feature: Regression tests to ensure navigation functions correctly
	
Scenario: Navigate to an initial screen
	Given I navigated to the Home page
	 Then I am on the Home page 

@Highlight
Scenario: Navigate to a student screen with a parameter
	Given I navigated to the Student Detail page with parameters
			| Id |
			| 1  |
	 Then I see
			| Field      | Rule   | Value     |
			| First Name | Equals | Carson    |
			| Last Name  | Equals | Alexander |
