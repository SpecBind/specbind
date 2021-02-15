@mstest:DeploymentItem:TechTalk.SpecFlow.MSTest.SpecFlowPlugin.dll
Feature: Integration tests of the table driver for Selenium
	
Scenario: Validate Table "Start With" Validator
	 Given I navigated to the Home page
       And I chose Instructors
	   And I was on the Instructor Search page
	   And I waited for results grid to contain items
	  Then I see results grid list starts with
		   | Field      | Rule   | Value       |
		   | First Name | Equals | Kim         |
		   | Last Name  | Equals | Abercrombie |
		   | Office     | Equals | Smith 17    |
