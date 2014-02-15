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
        [StepArgumentTransformation(@"(\d+) minutes from '(.*)'")]
        public DateTimeOffset GetTime(int minutes,
                                      string baseDate)
        {
            if (baseDate != "now")
            {
                var error = string.Format("Don't understand from '{0}', only 'now' is currently understood",
                                          baseDate);
                throw new NotSupportedException(error);
            }
            var useThisForNow = ContextStatic.NowSnapshot ?? (ContextStatic.NowSnapshot = DateTimeOffset.Now);
            return useThisForNow.Value.AddMinutes(minutes);
        } 
    }
}