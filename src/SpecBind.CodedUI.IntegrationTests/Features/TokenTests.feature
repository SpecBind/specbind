@mstest:DeploymentItem:TechTalk.SpecFlow.MSTest.SpecFlowPlugin.dll
@mstest:DeploymentItem:SpecBind.MsTest.Steps.dll
Feature: Token Passing Tests
	
Scenario: Passing a token into an input field and using for validation
	Given I navigated to the Home page
	  And I chose Courses
	  And I waited for the Courses page
	  And I chose Create New
	  And I waited for the Create a Course page
	 When I enter data
	      | Field  | Value           |
	      | Number | {MyToken:12345} |
	 Then I see
		  | Field  | Rule   | Value     |
		  | Number | Equals | {MyToken} |

Scenario: Gathering a token via the property step and using it for validation
	Given I navigated to the Home page
	  And I chose Courses
	  And I waited for the Courses page
	  And I chose Create New
	  And I waited for the Create a Course page
	 When I enter data
	      | Field  | Value |
	      | Number | 12345 |
	  And I set token MyToken with the value of Number
	 Then I see
		  | Field  | Rule   | Value     |
		  | Number | Equals | {MyToken} |
