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

        private static void WaitFor(int seconds)
        {
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }

        [BeforeScenario]
        public static void RemoveStartingDateTime()
        {
            ContextStatic.NowSnapshot = null;
        }

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