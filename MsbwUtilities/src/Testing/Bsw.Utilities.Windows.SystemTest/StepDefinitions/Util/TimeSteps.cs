using System;
using System.Threading;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Util
{
    [Binding]
    public class TimeSteps : BaseSteps<GeneralScenarioContext>
    {
        [When(@"I wait for (.*) seconds")]
        public void WhenIWaitForSeconds(int seconds)
        {
            WaitFor(seconds);
        }

        static void WaitFor(int seconds)
        {
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }

        [BeforeScenario]
        public static void RemoveStartingDateTime()
        {
            ContextStatic.NowSnapshot = null;
        }

        [When(@"I set the current date/time for this test to (.*)")]
        public void SetNowToThisTime(DateTimeOffset date)
        {
            Context.NowSnapshot = date;
        }
    }
}