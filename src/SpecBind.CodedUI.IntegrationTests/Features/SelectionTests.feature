Feature: Clicking items on the screen

Scenario: Select a Menu Hyperlink
	Given I navigated to the Home page
	 When I choose Students
	 Then I am on the Students Search page

Scenario: Select an item in the list by index and click the link
	Given I navigated to the Home page
	  And I chose Students
	  And I waited for the Students Search page
	  And I was on list results grid item 1
	 When I choose Details
	 Then I am on the Student Detail page
	  And I see
         | Field      | Rule   | Value     |
         | Last Name  | Equals | Alexander |
         | First Name | Equals | Carson    |

Scenario: Select an item in the list by criteria and click the link
	Given I navigated to the Home page
	  And I chose Students
	  And I waited for the Students Search page
	  And I was on results grid list item matching criteria
			| Field      | Rule   | Value     |
			| Last Name  | equals | Alexander |
			| First Name | Equals | Carson    |
	 When I choose Details
	 Then I am on the Student Detail page
	  And I see
         | Field      | Rule   | Value     |
         | Last Name  | Equals | Alexander |
         | First Name | Equals | Carson    |
