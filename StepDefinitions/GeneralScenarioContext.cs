// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceProcess;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions
{
    public class GeneralScenarioContext
    {
        private const string CONTEXT_KEY_SERVICE_CONTROLLER = "servicecontroller";

        protected ScenarioContext Context
        {
            get { return ScenarioContext.Current; }
        }

        public ServiceController ServiceController
        {
            get
            {
                return Context.ContainsKey(CONTEXT_KEY_SERVICE_CONTROLLER)
                           ? Context.Get<ServiceController>(CONTEXT_KEY_SERVICE_CONTROLLER)
                           : null;
            }
            set { Context[CONTEXT_KEY_SERVICE_CONTROLLER] = value; }
        }
    }
}