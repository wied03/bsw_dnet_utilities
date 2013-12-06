// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Util
{
    public abstract class BaseSteps : Steps
    {
        protected void ThenFormat(string format,
                                  params object[] args)
        {
            Then(string.Format(format,
                               args));
        }

        protected void WhenFormat(string format,
                                  params object[] args)
        {
            When(string.Format(format,
                               args));
        }
    }
}