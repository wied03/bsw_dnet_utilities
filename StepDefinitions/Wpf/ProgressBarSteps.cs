// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MsBw.MsBwUtility.JetBrains.Annotations;
using TechTalk.SpecFlow;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.Utility;

namespace Bsw.Coworking.Agent.Config.Sys.Test.StepDefinitions.Wpf
{
    [Binding]
    [UsedImplicitly]
    public class ProgressBarSteps : WpfBaseSteps
    {
        private Window Window
        {
            get { return Context.Window; }
        }

        [Given("Wait for progress bar to finish")]
        public void GivenWaitForProgressBarToFinish()
        {
            Wait();
        }

        public static string ActionWhenWaitForProgressBarToFinish()
        {
            return "Wait for progress bar to finish";
        }

        [When("Wait for progress bar to finish")]
        public void WhenWaitForProgressBarToFinish()
        {
            Wait();
        }

        private void Wait()
        {
            Console.WriteLine(@"Getting progress bar...");
            var progressBar = Window.Get<ProgressBar>(SearchCriteria.Indexed(0));
            Console.WriteLine(@"Waiting 10 seconds for progress bar to become hidden...");
            Retry.For(getMethod: () => !progressBar.Visible,
                      retryFor: 10.Seconds());
        }
    }
}