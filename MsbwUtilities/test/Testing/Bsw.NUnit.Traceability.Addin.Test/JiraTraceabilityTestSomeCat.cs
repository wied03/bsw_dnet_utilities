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
    public abstract class BaseJiraTest
    {
        protected JiraTraceability Addin { get; private set; }
        protected List<string> Written { get; private set; }

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var writer = MockRepository.GenerateMock<IWriteCategoriesToOutput>();
            Addin = new JiraTraceability(writer);
            Written = new List<string>();
            writer.Stub(w => w.Write(null))
                  .IgnoreArguments()
                  .Do(new Action<string>(txt => Written.Add(txt)));
        }

        #endregion

        #region Utility Methods

        protected static TestName TestNameBasedOnCurrentTestMethod
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
    }

    [TestFixture]
    public class JiraTraceabilityTestNoCat : BaseJiraTest
    {
        [Test]
        public void No_cat()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            Addin.TestStarted(testName);

            // assert
            Written
                .Should()
                .BeEmpty();
        }

        [Test]
        [Category("some other stuff")]
        public void Other_cat()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            Addin.TestStarted(testName);

            // assert
            Written
                .ShouldBeEquivalentTo(new[] { "Test Category: some other stuff" });
        }

        [Test]
        [Category("JIRA_CRT_123")]
        public void Jira_cat()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            Addin.TestStarted(testName);

            // assert
            Written
                .ShouldBeEquivalentTo(new[] { "Related JIRA Issue: CRT-123" });
        }
    }

    [TestFixture]
    [Category("JIRA_CRT_123")]
    public class JiraTraceabilityTestJiraCat : BaseJiraTest
    {
        [Test]
        public void No_cat()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            Addin.TestStarted(testName);

            // assert
            Written
                .ShouldBeEquivalentTo(new[] {"Related JIRA Issue: CRT-123"});
        }

        [Test]
        [Category("some other stuff")]
        public void Other_cat()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            Addin.TestStarted(testName);

            // assert
            Written
                .ShouldBeEquivalentTo(new[] {"Related JIRA Issue: CRT-123", "Test Category: some other stuff"});
        }

        [Test]
        [Category("JIRA_CRT_123")]
        public void Jira_cat()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            Addin.TestStarted(testName);

            // assert
            Written
                .ShouldBeEquivalentTo(new[] {"Related JIRA Issue: CRT-123", "Related JIRA Issue: CRT-123"});
        }

        [Test]
        public void Suite_leval_cat_written_with_each_test()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            Addin.TestStarted(testName);
            Addin.TestStarted(testName);

            // assert
            Written
               .ShouldBeEquivalentTo(new[] { "Related JIRA Issue: CRT-123", "Related JIRA Issue: CRT-123" });
        }
    }

    [TestFixture]
    [Category("fixture level cat")]
    public class JiraTraceabilityTestSomeCat : BaseJiraTest
    {
        #region Tests

        [Test]
        [Category("some other stuff")]
        public void Non_jira_category()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            Addin.TestStarted(testName);

            // assert
            Written
                .ShouldBeEquivalentTo(new[] { "Test Category: fixture level cat", "Test Category: some other stuff" });
        }

        [Test]
        [Category("some other stuff")]
        [Category("some other stuff 2")]
        public void Non_jira_categories()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            Addin.TestStarted(testName);

            // assert
            Written
                .ShouldBeEquivalentTo(new[]
                                      {
                                          "Test Category: fixture level cat", 
                                          "Test Category: some other stuff",
                                          "Test Category: some other stuff 2"
                                      });
        }

        [Test]
        public void No_categories()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            Addin.TestStarted(testName);

            // assert
            Written
                 .ShouldBeEquivalentTo(new[] { "Test Category: fixture level cat" });
        }

        [Test]
        [Category("JIRA_CRT_456")]
        [Category("JIRA_CRT_123")]
        public void Multiple_jira_categories()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            Addin.TestStarted(testName);

            // assert
            Written
                .ShouldBeEquivalentTo(new[]
                                      {
                                          "Test Category: fixture level cat",
                                          "Related JIRA Issue: CRT-456",
                                          "Related JIRA Issue: CRT-123"
                                      });
        }

        [Test]
        [Category("JIRA_CRT_123")]
        public void Single_jira_category()
        {
            // arrange
            var testName = TestNameBasedOnCurrentTestMethod;

            // act
            Addin.TestStarted(testName);

            // assert
            Written
                .ShouldBeEquivalentTo(new[] { "Test Category: fixture level cat", "Related JIRA Issue: CRT-123" });
        }

        #endregion
    }
}