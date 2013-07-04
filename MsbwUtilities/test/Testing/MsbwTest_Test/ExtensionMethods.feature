Feature: ExtensionMethods
	
Scenario: ToStringWithCount
	Given character 'c' with count 5	
	When I call ToStringWithCount
	Then the result should be 'ccccc'
