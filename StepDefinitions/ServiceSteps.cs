// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceProcess;
using Bsw.Utilities.Windows.SystemTest.StepDefinitions.Util;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions
{
    [Binding]
    public class ServiceSteps : BaseSteps<GeneralScenarioContext>
    {
        [BeforeScenario]
        public static void ClearServices()
        {
            ContextStatic.ServicesStarted = new List<string>();
        }

        [When(@"I start the '(.*)' service")]
        public void WhenIStartTheService(string serviceName)
        {
            var controller = new ServiceController(serviceName);
            controller.Status
                      .Should()
                      .Be(ServiceControllerStatus.Stopped,
                          "Can't start a service that isn't stopped");
            controller.Start();
            Context.ServicesStarted.Add(serviceName);
        }

        [AfterScenario]
        public static void ShutdownOrphanedService()
        {
            // ServiceController doesn't update Status once constructed, so need to reconstruct each time
            var runningServices = ContextStatic.ServicesStarted.Select(serviceName => new ServiceController(serviceName))
                                               .Where(controller => controller.Status == ServiceControllerStatus.Running);
            foreach (var controller in runningServices)
            {
                Console.WriteLine("Service was orphaned in running state, stopping");
                controller.Stop();
            }
        }

        [When(@"I stop the '(.*)' service")]
        public void WhenIStopTheService(string serviceName)
        {
            // ServiceController doesn't update Status once constructed, so need to reconstruct each time
            var controller = new ServiceController(serviceName);
            controller.Stop();
        }
    }
}