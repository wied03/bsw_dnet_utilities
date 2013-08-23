﻿using System;
using FluentAssertions;
using MsbwTest;
using TechTalk.SpecFlow;

namespace MsbwTest_Test
{
    [Binding]
    public class ExtensionMethodsSteps
    {
        private char _character;
        private int _count;
        private string _result;

        [Given(@"character '(.*)' with count (.*)")]
        public void GivenCharacterWithCount(char character, int count)
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