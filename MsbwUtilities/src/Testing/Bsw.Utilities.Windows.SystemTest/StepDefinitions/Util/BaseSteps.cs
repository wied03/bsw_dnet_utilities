// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Nito.AsyncEx;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Util
{
    public abstract class BaseSteps<TContextType> : Steps where TContextType : GeneralScenarioContext, new()
    {
        static readonly ProxyGenerator Generator = new ProxyGenerator();
        protected static readonly TContextType ContextStatic = CreateProxiedContext(new TContextType());

        protected TContextType Context
        {
            get { return ContextStatic; }
        }

        static TContextType CreateProxiedContext(TContextType context)
        {
            return Generator.CreateClassProxyWithTarget(context,
                                                        new ScenarioContextInterceptor());
        }

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

        // until Specflow supports async tasks properly
        protected static void SpecFlowAsync(Func<Task> task)
        {
            AsyncContext.Run(task);
        }
    }
}