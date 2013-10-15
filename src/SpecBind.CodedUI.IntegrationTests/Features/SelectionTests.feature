Feature: Clicking items on the screen

@mytag
Scenario: Select a Menu Hyperlink
	Given I navigated to the Home page
	 When I choose Students
	 Then I am on the Students Search page
