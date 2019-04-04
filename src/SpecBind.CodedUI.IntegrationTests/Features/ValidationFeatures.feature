﻿Feature: Field Validation Features

Scenario: Validate Field Equality For a Text Field
	 Given I navigated to the Home page
       And I chose Courses
	   And I waited for the Courses page
	   And I chose Create New
	   And I waited for the Create a Course page
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
	   And I waited for the Courses page
	   And I chose Create New
	   And I waited for the Create a Course page
	  Then I see
		   | Field        | Rule           | Value |
		   | Course Title | Exists         |       |
		   | Course Title | Enabled        |       |
		   | Foo          | Does Not Exist |       |
		  
