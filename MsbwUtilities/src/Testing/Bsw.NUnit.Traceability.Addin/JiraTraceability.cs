// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Core;
using NUnit.Core.Extensibility;
using NUnit.Framework;

namespace Bsw.NUnit.Traceability.Addin
{
    [NUnitAddin(Description = "JIRA Traceability addin")]
    public class JiraTraceability : IAddin,
                                    EventListener
    {
        private static readonly Regex JiraRegex = new Regex(@"JIRA_(\w+_\d+)");
        private readonly IDictionary<string, Type> _typeNameToReflectedTypeCache;
        private readonly IDictionary<Type, string> _alreadyWrittenFixtureLevelCategories;
        private readonly IWriteCategoriesToOutput _writer;

        public JiraTraceability() : this(new WriteCategoriesToOutput())
        {
        }

        internal JiraTraceability(IWriteCategoriesToOutput writer)
        {
            _writer = writer;
            _typeNameToReflectedTypeCache = new Dictionary<string, Type>();
            _alreadyWrittenFixtureLevelCategories = new Dictionary<Type, string>();
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
            var declaringType = method.DeclaringType;
            // only do fixture level stuff once since we can only intercept each test event
            if (!_alreadyWrittenFixtureLevelCategories.ContainsKey(declaringType))
            {
                Debug.Assert(declaringType != null,
                             "declaringType != null");
                var fixtureAttributes = declaringType.GetCustomAttributes(typeof (CategoryAttribute),
                                                                          true);
                WriteCategories(fixtureAttributes);
                _alreadyWrittenFixtureLevelCategories[declaringType] = string.Empty;
            }
            var attributes = method.GetCustomAttributes(typeof (CategoryAttribute),
                                                        true);
            WriteCategories(attributes);
        }

        private void WriteCategories(IEnumerable<object> categoryAttributes)
        {
            var categories = categoryAttributes
                .Cast<CategoryAttribute>()
                .Select(c => c.Name)
                .ToList()
                ;
            if (!categories.Any()) return;
            foreach (var category in categories)
            {
                var match = JiraRegex.Match(category);
                var text = match.Success
                               ? FormatNUnitCompatibileCategoryBackToJiraIssue(match)
                               : string.Format("Test Category: {0}",
                                               category);
                _writer.Write(text);
            }
        }

        private static string FormatNUnitCompatibileCategoryBackToJiraIssue(Match match)
        {
            return string.Format("Related JIRA Issue: {0}",
                                 match.Groups[1].Value.Replace('_',
                                                               '-'));
        }

        private MethodInfo GetMethodInfo(TestName testName)
        {
            var fullName = testName.FullName;
            var marker = fullName.LastIndexOf('.');
            var type = fullName.Substring(0,
                                          marker);
            var theType = _typeNameToReflectedTypeCache.ContainsKey(type)
                              ? _typeNameToReflectedTypeCache[type]
                              : (_typeNameToReflectedTypeCache[type] = GetTestType(type));
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