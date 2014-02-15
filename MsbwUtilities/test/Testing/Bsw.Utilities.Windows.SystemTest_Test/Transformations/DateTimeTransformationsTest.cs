﻿// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Bsw.Utilities.Windows.SystemTest.Transformations;
using NUnit.Framework;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest_Test.Transformations
{
    [Binding]
    public class DummySteps : Steps
    {
        public static DateTimeOffset? TransformedDate { get; set; }

        [When("I test (.*)")]
        public void TimeTest(DateTimeOffset date)
        {
            TransformedDate = date;
        }
    }

    [TestFixture]
    public class DateTimeTransformationsTest : BaseTest
    {
        private void SetNowShapShotTo(DateTimeOffset? value)
        {
            const string key = DateTimeTransformations.SCENARIO_CONTEXT_NOW_SNAPSHOT;
            var context = ScenarioContext.Current;
            if (!value.HasValue && context.ContainsKey(key))
            {
                context.Remove(key);
            }
            else
            {
                context[key] = value;
            }
        }

        #region Tests
       
        [Test]
        public void Get_time_from_date()
        {
            // arrange
            DummySteps.TransformedDate = null;

            // act
            Runner.When("I test 5 minutes from '2/14/2013 1:45 PM'");
            var result = DummySteps.TransformedDate;

            // assert
            result
                .Should()
                .NotBeNull();
            Debug.Assert(result != null,
                         "result != null");
            result.Value.DateTime
                  .Should()
                  .Be(DateTime.Parse("2/14/2013 1:50 PM"));
        }

        [Test]
        public void Get_time_from_now_no_baseline()
        {
            // arrange
            DummySteps.TransformedDate = null;
            SetNowShapShotTo(null);

            // act
            Runner.When("I test 5 minutes from 'now'");
            var result = DummySteps.TransformedDate;

            // assert
            result
                .Should()
                .NotBeNull();
            Debug.Assert(result != null,
                         "result != null");
            result.Value.DateTime
                 .Should()
                 .BeCloseTo(nearbyTime: DateTime.Now.AddMinutes(5),
                            precision: 500);
        }

        [Test]
        public void Get_time_from_now_baseline_already()
        {
            // arrange
            DummySteps.TransformedDate = null;
            SetNowShapShotTo(DateTimeOffset.Parse("1/4/2013 1:02 PM"));

            // act
            Runner.When("I test 5 minutes from 'now'");
            var result = DummySteps.TransformedDate;

            // assert
            result
                .Should()
                .NotBeNull();
            Debug.Assert(result != null,
                         "result != null");
            result
                .Should()
                .Be(DateTimeOffset.Parse("1/4/2013 1:07 PM"));
        }

        [Test]
        public void Exact_date()
        {
            // arrange
            DummySteps.TransformedDate = null;

            // act
            Runner.When("I test '2/14/2013 1:45 PM'");
            var result = DummySteps.TransformedDate;

            // assert
            result
                .Should()
                .NotBeNull();
            Debug.Assert(result != null,
                         "result != null");
            result.Value.DateTime
                  .Should()
                  .Be(DateTime.Parse("2/14/2013 1:45 PM"));
        }

        [Test]
        public void Exact_now()
        {
            // arrange
            DummySteps.TransformedDate = null;
            SetNowShapShotTo(null);

            // act
            Runner.When("I test 'now'");
            var result = DummySteps.TransformedDate;

            // assert
            result
                .Should()
                .NotBeNull();
            Debug.Assert(result != null,
                         "result != null");
            result.Value.DateTime
                  .Should()
                  .BeCloseTo(nearbyTime: DateTime.Now,
                             precision: 500);
        }

        #endregion
    }
}