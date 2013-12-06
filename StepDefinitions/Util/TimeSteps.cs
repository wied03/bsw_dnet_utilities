using System;
using System.Threading;
using TechTalk.SpecFlow;

namespace Bsw.Coworking.Agent.Config.Sys.Test.StepDefinitions.Util
{
    [Binding]
    public class TimeSteps
    {
        [When(@"I wait for (.*) seconds")]
        public void WhenIWaitForSeconds(int seconds)
        {
            WaitFor(seconds);
        }

        private static void WaitFor(int seconds)
        {
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }
    }
}