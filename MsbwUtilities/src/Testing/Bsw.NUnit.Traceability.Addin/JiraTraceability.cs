// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Core;
using NUnit.Core.Extensibility;
using NUnit.Framework;

namespace Bsw.NUnit.Traceability.Addin
{
    [NUnitAddin(Description = "JIRA Traceability addin")]
    public class JiraTraceability : IAddin,
                                    EventListener
    {
        private readonly IDictionary<string, Type> _testTypeMapping;

        public JiraTraceability()
        {
            _testTypeMapping = new Dictionary<string, Type>();
        }

        public bool Install(IExtensionHost host)
        {
            var ext = host.GetExtensionPoint("EventListeners");
            ext.Install(this);
            return true;
        }

        public void RunStarted(string name,
                               int testCount)
        {
        }

        public void RunFinished(TestResult result)
        {
        }

        public void RunFinished(Exception exception)
        {
        }

        public void TestStarted(TestName testName)
        {
            var method = GetMethodInfo(testName);
            var categories = method.GetCustomAttributes(typeof (CategoryAttribute),
                                                        true)
                                   .Cast<CategoryAttribute>()
                                   .Select(c => c.Name)
                                   .ToList()
                ;
            if (!categories.Any()) return;
            foreach (var category in categories)
            {
                Console.WriteLine("Test Category: {0}",
                                  category);
            }
        }

        private MethodInfo GetMethodInfo(TestName testName)
        {
            var fullName = testName.FullName;
            var marker = fullName.LastIndexOf('.');
            var type = fullName.Substring(0,
                                          marker);
            var theType = _testTypeMapping.ContainsKey(type)
                              ? _testTypeMapping[type]
                              : (_testTypeMapping[type] = GetTestType(type));
            var testMethod = fullName.Substring(marker + 1);
            var method = theType.GetMethod(testMethod);
            return method;
        }

        private static Type GetTestType(string type)
        {
            var testAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                          .Where(asm => asm.GetName().Name.Contains("Test"));
            return testAssemblies.First(asm => asm.GetType(type) != null)
                                 .GetType(type);
        }

        public void TestFinished(TestResult result)
        {
        }

        public void SuiteStarted(TestName testName)
        {
        }

        public void SuiteFinished(TestResult result)
        {
        }

        public void UnhandledException(Exception exception)
        {
        }

        public void TestOutput(TestOutput testOutput)
        {
        }
    }
}