// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest.Transformations
{
    [Binding]
    public class DateTimeTransformations
    {
        [StepArgumentTransformation(@"'(.*)'")]
        public DateTimeOffset GetTimeFrom(string exactDate)
        {
            if (exactDate == "now")
            {
                var context = ScenarioContext.Current;
                var existingSnapshot = context.ContainsKey(SCENARIO_CONTEXT_NOW_SNAPSHOT)
                                           ? context[SCENARIO_CONTEXT_NOW_SNAPSHOT]
                                           : null;
                return (existingSnapshot as DateTimeOffset?) ?? DateTimeOffset.Now;
            }
            return DateTimeOffset.Parse(exactDate);
        }

        [StepArgumentTransformation(@"(\d+) months? from (.*)")]
        public DateTimeOffset GetMonthsFrom(int months,
                                            DateTimeOffset typed)
        {
            return typed.AddMonths(months);
        }

        [StepArgumentTransformation(@"(\d+) days? from (.*)")]
        public DateTimeOffset GetDaysFrom(int days,
                                          DateTimeOffset typed)
        {
            return typed.AddDays(days);
        }

        [StepArgumentTransformation(@"(\d+) minutes? from (.*)")]
        public DateTimeOffset GetMinutesFrom(int minutes,
                                             DateTimeOffset typed)
        {
            return typed.AddMinutes(minutes);
        }

        internal const string SCENARIO_CONTEXT_NOW_SNAPSHOT = "NowSnapshot";
    }
}