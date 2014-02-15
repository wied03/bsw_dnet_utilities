// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest_Test
{
    public abstract class BaseTest
    {
        protected static ITestRunner Runner { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            var scenarioInfo = new ScenarioInfo("foo",
                                                null);
            Runner.OnScenarioStart(scenarioInfo);
        }

        [TearDown]
        public virtual void TearDown()
        {
            Runner.OnScenarioEnd();
        }

        [TestFixtureSetUp]
        public static void FixtureSetup()
        {
            Runner = TestRunnerManager.GetTestRunner();
            var featureInfo = new FeatureInfo(new CultureInfo("en-US"),
                                              "foo",
                                              "foo");
            Runner.OnFeatureStart(featureInfo);
        }

        [TestFixtureTearDown]
        public static void FixtureTeardown()
        {
            Runner.OnFeatureEnd();
        }
    }
}