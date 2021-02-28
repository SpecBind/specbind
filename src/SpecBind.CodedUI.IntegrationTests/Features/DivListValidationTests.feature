@mstest:DeploymentItem:TechTalk.SpecFlow.MSTest.SpecFlowPlugin.dll
@mstest:DeploymentItem:SpecBind.MsTest.Steps.dll
Feature: Validation Tests for an Ordered Div
	Contains tests for ordered div list validation features.

Background: 
	Given I navigated to the Home page
	  And I chose Departments
	  And I waited for the Department Search page

Scenario: Validate Div List "Start With" Validator
	  Then I see results grid list starts with
		   | Field           | Rule   | Value       |
		   | Department Name | Equals | Economics   |
		   | Budget          | Equals | $100,000.00 |

Scenario: Validate Div List "Ends With" Validator
	  Then I see results grid list ends with
		   | Field           | Rule   | Value       |
		   | Department Name | Equals | Mathematics |
		   | Start Date      | Equals | 9/1/2007    |

Scenario: Validate Div List "Contains" Validator
	  Then I see results grid list contains
		   | Field           | Rule   | Value   |
		   | Department Name | Equals | English |

Scenario: Validate Div List "Does Not Contain" Validator
	  Then I see results grid list does not contain
		   | Field           | Rule   | Value |
		   | Department Name | Equals | Bob   |
		   