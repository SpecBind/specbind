Feature: Waiting Feature Tests

Scenario: Wait for the screen to be enabled
	Given I navigated to the Home page
	  And I chose About
	  And I waited for the About page
	 When I wait for the view to become active
	 Then I see
			| Field            | Rule       | Value |
			| Enrollment Table | exists     | true  |
			| Enrollment Table | is enabled | true  |