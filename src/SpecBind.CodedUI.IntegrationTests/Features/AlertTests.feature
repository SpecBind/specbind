@mstest:DeploymentItem:TechTalk.SpecFlow.MSTest.SpecFlowPlugin.dll
@mstest:DeploymentItem:SpecBind.MsTest.Steps.dll
Feature: Test Alerts For Proper Response

Scenario: Display and dismiss an alert box
	Given I navigated to the Home page
	  And I chose New Information
	  And I waited for the Information page
	 When I choose Alert Box
	  And I see an alert box and select Ok
	 Then I see
		  | Field  | Rule   | Value         |
		  | Result | Equals | Alert Cleared |

Scenario: Press OK on a Confirm Dialog
	Given I navigated to the Home page
	  And I chose New Information
	  And I waited for the Information page
	 When I choose Confirm Box
	  And I see an alert box and select Ok
	 Then I see
		  | Field  | Rule   | Value           |
		  | Result | Equals | You pressed OK! |

Scenario: Press Cancel on a Confirm Dialog
	Given I navigated to the Home page
	  And I chose New Information
	  And I waited for the Information page
	 When I choose Confirm Box
	  And I see an alert box and select Cancel
	 Then I see
		  | Field  | Rule   | Value           |
		  | Result | Equals | You pressed Cancel! |

Scenario: Enter data on a dialog prompt
	Given I navigated to the Home page
	  And I chose New Information
	  And I waited for the Information page
	 When I choose Prompt Example
	  And I see an alert box, enter "I am HAL" and select Ok
	 Then I see
		  | Field  | Rule   | Value    |
		  | Result | Equals | I am HAL |

