// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceProcess;
using Bsw.Utilities.Windows.SystemTest.StepDefinitions.Util;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions
{
    [Binding]
    public class ServiceSteps : BaseSteps
    {
        [When(@"I start the '(.*)' service")]
        public void WhenIStartTheService(string serviceName)
        {
            var controller = new ServiceController(serviceName);
            Context.ServiceController = controller;
            controller.Status
                      .Should()
                      .Be(ServiceControllerStatus.Stopped,
                          "Can't start a service that isn't stopped");
            controller.Start();
        }

        [AfterScenario]
        public static void ShutdownOrphanedService()
        {
            var controller = ContextStatic.ServiceController;
            if (controller == null || controller.Status != ServiceControllerStatus.Running) return;
            Console.WriteLine("Service was orphaned in running state, stopping");
            controller.Stop();
        }

        [When(@"I stop the '(.*)' service")]
        public void WhenIStopTheService(string serviceName)
        {
            var existingController = Context.ServiceController;
            var controllerToUse = existingController != null && existingController.ServiceName == serviceName
                                      ? existingController
                                      : new ServiceController(serviceName);
            controllerToUse.Stop();
        }
    }
}