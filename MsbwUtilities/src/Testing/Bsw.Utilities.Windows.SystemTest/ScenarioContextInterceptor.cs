// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest
{
    public class ScenarioContextInterceptor : IInterceptor
    {
        static ScenarioContext Context
        {
            get { return ScenarioContext.Current; }
        }

        static string GetKey(MethodInfo method)
        {
            return method.Name.Substring(4);
        }

        static object GetValue(MethodInfo method)
        {
            var key = GetKey(method);
            return Context.ContainsKey(key)
                       ? Context[key]
                       : null;
        }

        static void SetValue(MethodInfo method,
                             object value)
        {
            var key = GetKey(method);
            if (value == null && Context.ContainsKey(key))
            {
                Context.Remove(key);
            }
            else
            {
                Context[key] = value;
            }
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.Contains("get"))
            {
                invocation.ReturnValue = GetValue(invocation.Method);
            }
            else
            {
                SetValue(invocation.Method,
                         invocation.Arguments[0]);
            }
        }
    }
}