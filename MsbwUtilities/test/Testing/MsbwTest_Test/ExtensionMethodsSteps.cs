// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using FluentAssertions;
using MsbwTest;
using TechTalk.SpecFlow;

namespace MsbwTest_Test
{
    [Binding]
    public class ExtensionMethodsSteps
    {
        char _character;
        int _count;
        string _result;

        [Given(@"character '(.*)' with count (.*)")]
        public void GivenCharacterWithCount(char character,
                                            int count)
        {
            _count = count;
            _character = character;
        }

        [When(@"I call ToStringWithCount")]
        public void WhenICallToStringWithCount()
        {
            _result = _character.ToStringWithCount(_count);
        }

        [Then(@"the result should be '(.*)'")]
        public void ThenTheResultShouldBe(string expected)
        {
            _result
                .Should()
                .Be(expected);
        }
    }
}