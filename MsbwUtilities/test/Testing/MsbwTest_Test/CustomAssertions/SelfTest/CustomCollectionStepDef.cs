// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
#region

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MsBw.MsBwUtility.JetBrains.Annotations;
using MsbwTest;
using NUnit.Framework;
using TechTalk.SpecFlow;

#endregion

namespace MsbwTest_Test.CustomAssertions.SelfTest
{
    [Binding]
    public class CustomCollectionStepDef
    {
        private IEnumerable<object> _actualItems;
        private IEnumerable<object> _expectedItems;
        private Exception _exception;

        public class TestObject
        {
            public string SomeValue { get; set; }

            public TestObject(string someValue)
            {
                SomeValue = someValue;
            }

            public override string ToString()
            {
                return "TestObject " + SomeValue;
            }
        }

        public CustomCollectionStepDef()
        {
            _actualItems = null;
            _expectedItems = null;
            _exception = null;
        }

        [StepArgumentTransformation(@"\[(.*)\]")]
        [UsedImplicitly]
        public IEnumerable<TestObject> JsonObjectTransform(string csvItems)
        {
            return csvItems.Split(',')
                           .Select(str => new TestObject(str));
        }


        [Given(@"Actual collection having strings (.*)")]
        public void GivenActualCollectionHavingStrings(IEnumerable<String> items)
        {
            _actualItems = items;
        }

        [Given(@"Expected collection having strings (.*)")]
        public void GivenExpectedCollectionHavingStrings(IEnumerable<String> items)
        {
            _expectedItems = items;
        }

        [Given(@"Actual collection having objects (.*)")]
        public void GivenActualCollectionHavingObjects(IEnumerable<TestObject> items)
        {
            _actualItems = items;
        }

        [Given(@"Actual collection is null")]
        public void GivenActualCollectionIsNull()
        {
            _actualItems = null;
        }

        [Given(@"Expected collection is null")]
        public void GivenExpectedCollectionIsNull()
        {
            _expectedItems = null;
        }

        [Given(@"Expected collection having objects (.*)")]
        public void GivenExpectedCollectionHavingObjects(IEnumerable<TestObject> items)
        {
            _expectedItems = items;
        }

        [When(@"I test ContainEquivalent")]
        public void WhenITestContainEquivalent()
        {
            try
            {
                _actualItems
                    .Should()
                    .ContainEquivalent(_expectedItems);
            }
            catch (Exception e)
            {
                _exception = e;
            }
        }

        [Then(@"the test should pass")]
        public void ThenTheTestShouldPass()
        {
            var assertException = _exception as AssertionException;
            var failureMessage = assertException == null
                                     ? string.Empty
                                     : assertException.Message;
            Assert.That(assertException,
                        Is.Null,
                        "Expected the test to pass, but it failed with {0}",
                        failureMessage);
        }

        [Then(@"the test should fail with message (.*)")]
        public void ThenTheTestShouldFail(string expectedMessage)
        {
            var assertException = _exception as AssertionException;
            assertException
                .Should()
                .NotBeNull("Expected the test to fail");
            assertException
                .Message
                .Should()
                .Be(expectedMessage);
        }

        [Then(@"the test should throw (.*) : (.*)")]
        public void ThenTheTestShouldThrowSystem_NullReferenceExceptionCannotVerifyContainmentAgainstACollection(string type,string message)
        {
            var expectedExceptionType = Type.GetType(type);
            _exception
                .Should()
                .NotBeNull("Was expecting exception in test");

            var actualType = _exception.GetType();
            actualType
                .Should()
                .Be(expectedExceptionType);

            var actualMessage = _exception.Message;
            actualMessage
                .Should()
                .Be(message);
        }
    }
}