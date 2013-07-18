Feature: Search for a Student by Name
	In order to find a student quickly
	As a Contoso University Administrator
	I want to find a student by entering their name

Scenario: Find Peggy Justice by her first name
	Given I navigated to the home page
	  And I chose Students
	  And I was on the students search page
	 When I enter data
		  | Field        | Value |
		  | Find by name | Peggy |
	  And I choose Search
	 Then I see results grid list starts with
		  | Field           | Rule   | Value    |
		  | Last Name       | Equals | Justice  |
		  | First Name      | Equals | Peggy    |
		  | Enrollment Date | Equals | 9/1/2001 |

Scenario: Find Peggy Justice by part of her last name
	Given I navigated to the home page
	  And I chose Students
	  And I was on the students search page
	 When I enter data
		  | Field        | Value |		  | Find by name | Jus |
	  And I choose Search
	 Then I see results grid list starts with
		  | Field           | Rule   | Value    |
		  | Last Name       | Equals | Justice  |
		  | First Name      | Equals | Peggy    |
		  | Enrollment Date | Equals | 9/1/2001 |
