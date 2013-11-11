Feature: Table List Validation Features
	Contains tests for table list validation features.

Scenario: Validate Table List "Start With" Validator
	 Given I navigated to the Home page
       And I chose Students
	   And I was on the Students Search page
	  When I choose Search
	  Then I see results grid list starts with
		   | Field      | Rule   | Value     |
		   | First Name | Equals | Carson    |
		   | Last Name  | Equals | Alexander |

Scenario: Validate Table List "Ends With" Validator
	 Given I navigated to the Home page
       And I chose Students
	   And I was on the Students Search page
	  When I choose Search
	  Then I see results grid list ends with
		   | Field      | Rule   | Value  |
		   | First Name | Equals | Arturo |
		   | Last Name  | Equals | Anand  |

Scenario: Validate Table List "Contains" Validator
	 Given I navigated to the Home page
       And I chose Students
	   And I was on the Students Search page
	  When I choose Search
	  Then I see results grid list contains
		   | Field      | Rule   | Value    |
		   | First Name | Equals | Meredith |
		   | Last Name  | Equals | Alonso   |

Scenario: Validate Table List "Equals" Validator
	 Given I navigated to the Home page
       And I chose Students
	   And I was on the Students Search page
	  When I enter data
		   | Field        | Value  |
		   | Find by name | Alonso |
	   And I choose Search
	  Then I see results grid list equals
		   | Field      | Rule   | Value    |
		   | First Name | Equals | Meredith |
		   | Last Name  | Equals | Alonso   |

Scenario: Validate Table List "Does Not Contain" Validator
	 Given I navigated to the Home page
       And I chose Students
	   And I was on the Students Search page
	  When I choose Search
	  Then I see results grid list does not contain
		   | Field      | Rule   | Value  |
		   | First Name | Equals | Bob    |
		   | Last Name  | Equals | Smitty |