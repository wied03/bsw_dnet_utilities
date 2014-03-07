﻿Feature: Not Contain Equivalent	

Scenario: 2 empty collections of strings
	Given Actual collection having strings []
	And Expected collection having strings []
	When I test NotContainEquivalent
	Then the test should pass
	
Scenario: Expected string exists in larger actual collection
	Given Actual collection having strings ['foo','bar']
	And Expected collection having strings ['foo']
	When I test NotContainEquivalent
	Then the test should fail with message Expected collection {"\"foo\"", "\"bar\""} to not contain "\"foo2\"".

Scenario: Expected string does not exist in larger actual collection
	Given Actual collection having strings ['foo','bar']
	And Expected collection having strings ['foo2']
	When I test NotContainEquivalent
	Then the test should pass

Scenario: Expected object exists in larger actual collection
	Given Actual collection having objects [foo,bar]
	And Expected collection having objects [foo]
	When I test NotContainEquivalent
	Then the test should fail with message Expected collection {"{{\"SomeValue\":\"foo\"}}", "{{\"SomeValue\":\"bar\"}}"} to not contain "{{\"SomeValue\":\"foo2\"}}".

Scenario: Expected object does not exist in larger actual collection
	Given Actual collection having objects [foo,bar]
	And Expected collection having objects [foo2]
	When I test NotContainEquivalent
	Then the test should pass

Scenario: 2 empty collections of objects
	Given Actual collection having objects []
	And Expected collection having objects []
	When I test NotContainEquivalent
	Then the test should pass

Scenario: Null actual
	Given Actual collection is null
	And Expected collection having objects [hi2]
	When I test NotContainEquivalent
	Then the test should pass

Scenario: Null expected
	Given Actual collection having objects [hi,joe]
	And Expected collection is null
	When I test NotContainEquivalent
	Then the test should throw System.NullReferenceException : Cannot verify containment against a <null> collection
