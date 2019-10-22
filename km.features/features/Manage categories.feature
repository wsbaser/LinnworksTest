@login
@delete_categories
Feature: Manage categories

# Buisiness cases
Scenario: Can create category
	Given I am on the 'Categories' page
	When I click 'Create New link'
	And I type 'MY NEW CATEGORY' to Name textbox
	And I click Save button
	Then I am on the 'Categories' page
	And Categories list contains category 'MY NEW CATEGORY'

Scenario: Can edit category
	Given I created category 'CATEGORY TO EDIT'
	Given I am on the 'Categories' page
	When I click Edit link for category 'CATEGORY TO EDIT'
	And I type 'EDITED CATEGORY' to Name textbox
	And I click Save button
	Then I am on the 'Categories' page
	And Categories list contains category 'EDITED CATEGORY'
	And Categories list does not contain category 'CATEGORY TO EDIT'

Scenario: Can delete category
	Given I created category 'CATEGORY TO DELETE'
	And I am on the 'Categories' page
	When I click Delete link for category 'CATEGORY TO DELETE'
	And I click Ok in confirmation alert
	Then I am on the 'Categories' page
	And Categories list does not contain category 'CATEGORY TO DELETE'

# Edge cases
Scenario: Fill fields on Edit page with values from edited category
	Given I created category 'CATEGORY TO EDIT'
	Given I am on the 'Categories' page
	When I click Edit link for category 'CATEGORY TO EDIT'
	Then Name textbox value is 'CATEGORY TO EDIT' 

Scenario: Can cancel category creation
	Given I am on the 'Create Category' page
	When I click 'Cancel button'
	Then I am on the 'Categories' page

Scenario: Show error when category name is empty
	Given I am on the 'Categories' page
	When I click 'Create New link'
	And I click Save button
	Then Error message 'Name is required.' is displayed for Name field

Scenario: Can cancel category deletion in confirmation dialog
	Given I created category 'DELETE CATEGORY CANCELLATION'
	And I am on the 'Categories' page
	When I click Delete link for category 'DELETE CATEGORY CANCELLATION'
	And I click Cancel in confirmation alert
	Then Categories list contains category 'DELETE CATEGORY CANCELLATION'