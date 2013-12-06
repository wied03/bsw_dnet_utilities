// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions
{
    [Binding]
    public class Reporting
    {
        private static readonly Regex CardRegex = new Regex(@"card_(\d+)");

        [BeforeFeature]
        public static void OutputFeatureWeAreTesting()
        {
            var info = FeatureContext.Current.FeatureInfo;
            var cards = info.Tags
                            .Where(t => t.Contains("card"))
                            .Select(t => CardRegex.Match(t).Groups[1].Value)
                            .Select(cardstr => Convert.ToInt32(cardstr))
                ;
            var anon = new {title = info.Title, cards};
            var json = JsonConvert.SerializeObject(anon);
            Console.WriteLine(@"BSW_Feature: {0} BEGIN",
                              json);
        }

        [AfterFeature]
        public static void CloseFeature()
        {
            Console.WriteLine(@"BSW_Feature END");
        }

        [BeforeScenario]
        public void OutputScenarioWeAreTesting()
        {
            Console.WriteLine(@"BSW Scenario: {0}",
                              ScenarioContext.Current.ScenarioInfo.Title);
        }
    }
}