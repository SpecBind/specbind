Feature: IntegrationTests for the list features of SpecBind

Scenario: Nested Lists
	Given I navigated to the Student Courses page
	And I was on list Student Courses item 2
	When I am on list Courses item 3
	Then I see
		| Field        | Rule   | Value       |
		| Course Title | Equals | Composition |
