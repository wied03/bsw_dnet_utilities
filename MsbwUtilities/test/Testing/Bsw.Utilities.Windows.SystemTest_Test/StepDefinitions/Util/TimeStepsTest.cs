// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using Bsw.Utilities.Windows.SystemTest.Transformations;
using NUnit.Framework;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest_Test.StepDefinitions.Util
{
    [TestFixture]
    public class TimeStepsTest : BaseTest
    {
        #region Tests

        [Test]
        public void Set_date_time_to_now()
        {
            // arrange

            // act
            Runner.When("I set the current date/time for this test to 'now'");
            var result = ScenarioContext.Current.Get<DateTimeOffset>(DateTimeTransformations.SCENARIO_CONTEXT_NOW_SNAPSHOT);

            // assert
            result
                .DateTime
                .Should()
                .BeCloseTo(nearbyTime: DateTime.Now,
                           precision: 500);
        }

        [Test]
        public void Set_date_time_to_specific_date()
        {
            // arrange

            // act
            Runner.When("I set the current date/time for this test to '2/14/2014 1:40 PM'");
            var result = ScenarioContext.Current.Get<DateTimeOffset>(DateTimeTransformations.SCENARIO_CONTEXT_NOW_SNAPSHOT);

            // assert
            result
                .Should()
                .Be(DateTimeOffset.Parse("2/14/2014 1:40 PM"));
        }

        [Test]
        public void Set_date_time_to_relative_date_now()
        {
            // arrange

            // act
            Runner.When("I set the current date/time for this test to 5 minutes from 'now'");
            var result = ScenarioContext.Current.Get<DateTimeOffset>(DateTimeTransformations.SCENARIO_CONTEXT_NOW_SNAPSHOT);

            // assert
            result
                .DateTime
                .Should()
                .BeCloseTo(nearbyTime: DateTime.Now.AddMinutes(5),
                           precision: 500);
        }

        [Test]
        public void Set_date_time_to_relative_date_specific()
        {
            // arrange

            // act
            Runner.When("I set the current date/time for this test to 5 minutes from '2/5/2013 1:05 PM'");
            var result = ScenarioContext.Current.Get<DateTimeOffset>(DateTimeTransformations.SCENARIO_CONTEXT_NOW_SNAPSHOT);

            // assert
            result
                .Should()
                .Be(DateTimeOffset.Parse("2/5/2013 1:10 PM"));
        }

        #endregion
    }
}