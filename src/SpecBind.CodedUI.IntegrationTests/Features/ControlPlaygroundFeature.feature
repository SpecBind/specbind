Feature: A test playground for inspecting control's behaviors
	
Scenario: Transition to a search page date picker to another control.
	Given I navigated to the Echo Search page
	  And I was on the Echo Search page
	 When I enter data
			| Field      | Value                    |
			| Start Date | 10/25/2013               |
			| End Date   | 10/30/2013               |
			| Country    | United States of America |
