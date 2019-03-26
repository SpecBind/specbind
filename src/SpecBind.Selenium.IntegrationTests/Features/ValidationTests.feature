﻿Feature: Validation Features

Scenario: Validate Field Equality For a Text Field
	 Given I navigated to the Home page
       And I chose Courses
	   And I was on the Courses page
	   And I chose Create New
	   And I was on the Create a Course page
	  When I enter data
	       | Field        | Value       |
	       | Course Title | Cool Course |
	  Then I see
		   | Field        | Rule             | Value       |
		   | Course Title | Equals           | Cool Course |
		   | Course Title | Does Not Equal   | Bad Course  |
		   | Course Title | Not Equal        | Bad Course  |
		   | Course Title | Not Equals       | Bad Course  |
		   | Course Title | Contains         | Course      |
		   | Course Title | Does Not Contain | Bad         |
		   | Course Title | Starts With      | C           |
		   | Course Title | Ends With        | se          |

Scenario: Validate Field Virtual Property
	 Given I navigated to the Home page
	  Then I see
		   | Field        | Rule      | Value                     |
		   | Courses Link | Ends With | /ContosoUniversity/Course |

Scenario: Validate Field Existence and Enabled
	 Given I navigated to the Home page
       And I chose Courses
	   And I was on the Courses page
	   And I chose Create New
	   And I was on the Create a Course page
	  Then I see
		   | Field        | Rule           | Value |
		   | Course Title | Exists         |       |
		   | Course Title | Enabled        |       |
		   | Foo          | Does Not Exist |       |

Scenario: Validate List "Start With" Validator
	 Given I navigated to the Home page
       And I chose Students
	   And I was on the Students Search page
	  When I choose Search
	  Then I see results grid list starts with
		   | Field      | Rule   | Value     |
		   | First Name | Equals | Carson    |
		   | Last Name  | Equals | Alexander |

Scenario: Validate List "Start With" Validator and String Validation Field
	 Given I navigated to the Home page
       And I chose Students
	   And I was on the Students Search page
	   And I was on list results grid item 1
	  When I choose Details
	  Then I am on the Student Detail page
	   And I see
		   | Field      | Rule   | Value     |
		   | Full Name | Equals | Carson Alexander |

Scenario: Validate List "Ends With" Validator
	 Given I navigated to the Home page
       And I chose Students
	   And I was on the Students Search page
	  When I choose Search
	  Then I see results grid list ends with
		   | Field      | Rule   | Value  |
		   | First Name | Equals | Arturo |
		   | Last Name  | Equals | Anand  |

Scenario: Validate List "Contains" Validator
	 Given I navigated to the Home page
       And I chose Students
	   And I was on the Students Search page
	  When I choose Search
	  Then I see results grid list contains
		   | Field      | Rule   | Value    |
		   | First Name | Equals | Meredith |
		   | Last Name  | Equals | Alonso   |

Scenario: Validate List "Equals" Validator
	 Given I navigated to the Home page
       And I chose Students
	   And I was on the Students Search page
	  When I enter data
		   | Field        | Value  |
		   | Find by name | Alonso |
	   And I choose Search
	  Then I see
		   | Field   | Rule   | Value             |
		   | Caption | Equals | Find your Friends |
	  Then I see results grid list equals
		   | Field      | Rule   | Value    |
		   | First Name | Equals | Meredith |
		   | Last Name  | Equals | Alonso   |

Scenario: Validate List "Does Not Contain" Validator
	 Given I navigated to the Home page
       And I chose Students
	   And I was on the Students Search page
	  When I choose Search
	  Then I see results grid list does not contain
		   | Field      | Rule   | Value  |
		   | First Name | Equals | Bob    |
		   | Last Name  | Equals | Smitty |
		  
Scenario: Validate List Count Step
	 Given I navigated to the Home page
       And I chose Students
	   And I was on the Students Search page
	  When I choose Search
	  Then I see results grid list contains exactly 3 items