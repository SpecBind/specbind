Feature: Integration tests of the table driver for Selenium
	
Scenario: Validate Table "Start With" Validator
	 Given I navigated to the Home page
       And I chose Instructors
	   And I was on the Instructors Search page
	  Then I see results grid list starts with
		   | Field      | Rule   | Value       |
		   | First Name | Equals | Kim         |
		   | Last Name  | Equals | Abercrombie |
		   | Hire Date  | Equals | 3/11/1995   |
		   | Office     | Equals | Smith 17    |
