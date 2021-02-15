@mstest:DeploymentItem:TechTalk.SpecFlow.MSTest.SpecFlowPlugin.dll
@mstest:DeploymentItem:SpecBind.MsTest.Steps.dll
Feature: Test Data Entry For Controls

Scenario: Test Successful Combo Box Input
	 Given I navigated to the Home page
	  And I chose Courses
	  And I waited for the Courses page
	  And I chose Create New
	  And I waited for the Create a Course page
	 When I enter data
	      | Field      | Value   |
	      | Department | English |
	 Then I see
		  | Field      | Rule   | Value   |
		  | Department | Equals | English |

Scenario: Test Successful Text Input
	 Given I navigated to the Home page
	  And I chose Courses
	  And I waited for the Courses page
	  And I chose Create New
	  And I waited for the Create a Course page
	 When I enter data
	      | Field        | Value          |
	      | Number       | 12345          |
	 Then I see
		  | Field  | Rule   | Value |
		  | Number | Equals | 12345 |

Scenario: Test Combo Box Input For An Invalid Value Of Spanish
	Given I navigated to the Home page
	  And I chose Courses
	  And I waited for the Courses page
	  And I chose Create New
	  And I waited for the Create a Course page
	 When I enter invalid data
	      | Field      | Value   |
	      | Department | Spanish |
	 Then I see
		  | Field      | Rule   | Value   |
		  | Department | Equals |         |

Scenario: Test Successful Check Box Input
	 Given I navigated to the Home page
	   And I chose Courses
	   And I waited for the Courses page
	   And I chose Create New
	   And I waited for the Create a Course page
	  When I enter data
	       | Field   | Value |
	       | Popular | true  |
	  Then I see
		   | Field   | Rule   | Value |
		   | Popular | Equals | true  |

Scenario: Test Text Area Input
	 Given I navigated to the Home page
	   And I chose Courses
	   And I waited for the Courses page
	   And I chose Create New
	   And I waited for the Create a Course page
	  When I enter data
	       | Field   | Value |
	       | Description | This is a really long description of what's needed.  |
	  Then I see
		   | Field   | Rule   | Value |
		   | Description | Equals | This is a really long description of what's needed.  |

Scenario: Test Successful Password Input
	 Given I navigated to the Home page
	   And I chose Log On
	   And I waited for the Log On page
	  When I enter data
	       | Field    | Value    |
	       | Password | I'm Cool |

Scenario: Test Clearing Text Input
	 Given I navigated to the Home page
	  And I chose Courses
	  And I waited for the Courses page
	  And I chose Create New
	  And I waited for the Create a Course page
	  And I entered data
	      | Field        | Value     |
	      | Course Title | My Course |
	 When I clear data
	      | Field        |
	      | Course Title |
	 Then I see
	      | Field        | Rule   | Value |
	      | Course Title | Equals |       |

Scenario: Test Clearing Text Area Input
	 Given I navigated to the Home page
	   And I chose Courses
	   And I waited for the Courses page
	   And I chose Create New
	   And I waited for the Create a Course page
	   And I entered data
	       | Field       | Value                                               |
	       | Description | This is a really long description of what's needed. |
	  When I clear data
	       | Field       |
	       | Description |
	  Then I see
	       | Field       | Rule   | Value |
	       | Description | Equals |       |

Scenario: Enter special characters in input text field
	Given I navigated to the Home page
	And I chose Courses
	And I was on the Courses page
	And I chose Create New
	And I was on the Create a Course page
	And I entered data
		| Field        | Value     |
		| Course Title | {{SPACE}} |
	Then I see
		| Field        | Rule   | Value |
		| Course Title | Equals |       |