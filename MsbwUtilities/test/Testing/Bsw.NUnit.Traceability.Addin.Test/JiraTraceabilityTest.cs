// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Core;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bsw.NUnit.Traceability.Addin.Test
{
    [TestFixture]
    public class JiraTraceabilityTest
    {
        private JiraTraceability _addin;
        private List<string> _written;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var writer = MockRepository.GenerateMock<IWriteCategoriesToOutput>();
            _addin = new JiraTraceability(writer);
            _written = new List<string>();
            writer.Stub(w => w.Write(null))
                  .IgnoreArguments()
                  .Do(new Action<string>(txt => _written.Add(txt)));
        }

        #endregion

        #region Utility Methods

        private static TestName TestNameBasedOnCurrentTestMethod
        {
            get
            {
// ReSharper disable PossibleNullReferenceException
                var frame = new StackTrace().GetFrames()[1];
                var method = frame.GetMethod();
                var theType = method.DeclaringType.FullName;
                // ReSharper restore PossibleNullReferenceException
                var methodName = method.Name;
                return new TestName
                       {
                           FullName = string.Format("{0}.{1}",
                                                    theType,
                                                    methodName)
                       };
            }
        }

        #endregion

        #region Tests

        [Test]
        [Category("some other stuff")]
        public void Non_jira_category()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            _addin.TestStarted(testName);

            // assert
            _written
                .ShouldBeEquivalentTo(new[] {"Test Category: some other stuff"});
        }

        [Test]
        [Category("some other stuff")]
        [Category("some other stuff 2")]
        public void Non_jira_categories()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            _addin.TestStarted(testName);

            // assert
            _written
                .ShouldBeEquivalentTo(new[] {"Test Category: some other stuff", "Test Category: some other stuff 2"});
        }

        [Test]
        public void No_categories()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            _addin.TestStarted(testName);

            // assert
            _written
                .Should()
                .BeEmpty();
        }

        [Test]
        [Category("JIRA_CRT_123")]
        [Category("JIRA_CRT_456")]
        public void Multiple_jira_categories()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            _addin.TestStarted(testName);

            // assert
            _written
                .ShouldBeEquivalentTo(new[] {"Related JIRA Issue: CRT-123", "Related JIRA Issue: CRT-456"});
        }

        [Test]
        [Category("JIRA_CRT_123")]
        public void Single_jira_category()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            _addin.TestStarted(testName);

            // assert
            _written
                .ShouldBeEquivalentTo(new[] { "Related JIRA Issue: CRT-123" });
        }

        #endregion
    }
}