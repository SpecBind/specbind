Feature: Integration tests for the list features of SpecBind

Scenario: Nested Lists By Index
	Given I navigated to the Student Courses page
	And I was on list Student Courses item 2
	When I am on list Courses item 3
	Then I see
		| Field        | Rule   | Value       |
		| Course Title | Equals | Composition |

Scenario: Long Lists By Index
	Given I navigated to the Student Courses page
	And I was on list Student Courses item 7
	When I am on list Courses item 1
	Then I see
		| Field        | Rule   | Value        |
		| Course Title | Equals | Trigonometry |

Scenario: Nested Lists By Criteria
	Given I navigated to the Student Courses page
	And I was on Student Courses list item matching criteria
		| Field             | Rule   | Value            |
		| Student Full Name | Equals | Barzdukas, Gytis |
	When I am on Courses list item matching criteria
		| Field        | Rule   | Value          |
		| Course Title | Equals | Microeconomics |
	Then I see
		| Field        | Rule   | Value          |
		| Course Title | Equals | Microeconomics |

Scenario: Long Lists By Criteria
	Given I navigated to the Student Courses page
	When I am on Student Courses list item matching criteria
		| Field             | Rule   | Value            |
		| Student Full Name | Equals | Olivetto, Nino   |
	Then I see Courses list contains exactly 0 items

Scenario: Verifying List Count
	Given I navigated to the Student Courses page
	Then I see Student Courses list contains 8 items

Scenario: Selecting Items By Index
	Given I navigated to the Student Courses page
	When I am on list Student Courses item 3
	Then I see
		| Field             | Rule   | Value         |
		| Student Full Name | Equals | Anand, Arturo |

Scenario: Selecting Items By Criteria
	Given I navigated to the Student Courses page
	When I am on Student Courses list item matching criteria
		| Field             | Rule   | Value   |
		| Student Full Name | Equals | Li, Yan |
	Then I see
		| Field             | Rule   | Value   |
		| Student Full Name | Equals | Li, Yan |
