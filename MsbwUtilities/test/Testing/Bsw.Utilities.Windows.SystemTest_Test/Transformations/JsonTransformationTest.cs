// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest_Test.Transformations
{
    [Binding]
    public class DummyStepsForJson : Steps
    {
        public static IDictionary<string, object> TransformedDictionary { get; set; }

        [When("I test (.*)")]
        public void TimeTest(IDictionary<string, object> jsonDict)
        {
            TransformedDictionary = jsonDict;
        }
    }

    [TestFixture]
    public class JsonTransformationTest : BaseTest
    {
        #region Tests

        [Test]
        public void Transform()
        {
            // arrange
            DummyStepsForJson.TransformedDictionary = null;
            const string json = "{stuff: \"hello\", number: 5.2}";

            // act
            Runner.When(string.Format("I test {0}",
                                      json));
            var result = DummyStepsForJson.TransformedDictionary;

            // assert
            result
                .Should()
                .NotBeNull();
            Debug.Assert(result != null,
                         "result != null");
            result
                .Should()
                .Contain("stuff",
                         "hello");
            result
                .Should()
                .Contain("number",
                         5.2);
        }

        #endregion
    }
}