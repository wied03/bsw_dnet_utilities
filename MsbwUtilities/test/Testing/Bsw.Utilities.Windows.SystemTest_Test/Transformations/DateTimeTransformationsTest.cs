// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
        ITestRunner _runner;

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _runner = GetDummyTestRunner();
        }

        #endregion

        #region Utility Methods

        static ITestRunner GetDummyTestRunner()
        {
            var runner = TestRunnerManager.GetTestRunner();
            var featureInfo = new FeatureInfo(new System.Globalization.CultureInfo("en-US"),
                                              "foo",
                                              "foo");
            runner.OnFeatureStart(featureInfo);
            var scenarioInfo = new ScenarioInfo("foo",
                                                null);
            runner.OnScenarioStart(scenarioInfo);
            return runner;
        }

        #endregion

        #region Tests
       
        [Test]
        public void Get_time_from_date()
        {
            // arrange
            DummySteps.TransformedDate = null;

            // act
            _runner.When("I test 5 minutes from '2/14/2013 1:45 PM'");
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

        #endregion
    }
}