#region

// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using Newtonsoft.Json;

#endregion

#endregion

namespace MsbwTest.CustomAssertions
{
    public class CustomCollectionAssertions<T>
    {
        readonly GenericCollectionAssertions<T> _assertions;

        public CustomCollectionAssertions(GenericCollectionAssertions<T> assertions)
        {
            _assertions = assertions;
        }

        static IEnumerable<string> RemoveExtraQuotes(IEnumerable<string> list)
        {
            return list.Select(i => i.Replace("\"",
                                              string.Empty));
        }

        public AndConstraint<GenericCollectionAssertions<T>> NotContainEquivalent(IEnumerable<T> expected)
        {
            var actual = _assertions.Subject;
            var reason = string.Empty;
            var reasonArgs = new object[0];
            if (expected == null)
            {
                throw new NullReferenceException("Cannot verify containment against a <null> collection");
            }
            var expectedList = expected as IList<T> ?? expected.ToList();
            if (!expectedList.Any())
            {
                throw new ArgumentException("Cannot verify containment against an empty collection");
            }
            var continuation = new AndConstraint<GenericCollectionAssertions<T>>(_assertions);
            if (ReferenceEquals(actual,
                                null))
            {
                // Null collection, so can never contain what we are asserting
                return continuation;
            }
            var expectedJsonList = expectedList
                .Select(expObj => JsonConvert.SerializeObject(expObj))
                .ToList()
                ;
            var actualJsonList = actual
                .Select(actObj => JsonConvert.SerializeObject(actObj))
                .ToList()
                ;
            foreach (var actualJson in actualJsonList)
            {
                foreach (var expectedJson in expectedJsonList)
                {
                    if (actualJson.Equals(expectedJson))
                    {
                        Execute.Verification.BecauseOf(reason,
                                                       reasonArgs)
                               .FailWith("Expected collection {0} to not contain {1}{reason}",
                                         new object[]
                                         {
                                             // reduce cluttered failure message
                                             RemoveExtraQuotes(actualJsonList),
                                             RemoveExtraQuotes(expectedJsonList)
                                         });
                    }
                }
            }
            return continuation;
        }

        public AndConstraint<GenericCollectionAssertions<T>> ContainEquivalent(IEnumerable<T> expected)
        {
            var actual = _assertions.Subject;
            var reason = string.Empty;
            var reasonArgs = new object[0];
            if (expected == null)
            {
                throw new NullReferenceException("Cannot verify containment against a <null> collection");
            }
            var expectedList = expected as IList<T> ?? expected.ToList();
            if (!expectedList.Any())
            {
                throw new ArgumentException("Cannot verify containment against an empty collection");
            }
            var continuation = new AndConstraint<GenericCollectionAssertions<T>>(_assertions);
            if (ReferenceEquals(actual,
                                null))
            {
                Execute.Verification.BecauseOf(reason,
                                               reasonArgs)
                       .FailWith("Expected {context:collection} to contain {0}{reason}, but found <null>.",
                                 new object[]
                                 {
                                     expected
                                 });
                // not needed but keep resharper happy
                return continuation;
            }

            var expectedJsonList = expectedList
                .Select(expObj => JsonConvert.SerializeObject(expObj))
                .ToList()
                ;
            var actualJsonList = actual
                .Select(actObj => JsonConvert.SerializeObject(actObj))
                .ToList()
                ;
            foreach (var actualJson in actualJsonList)
            {
                foreach (var expectedJson in expectedJsonList)
                {
                    if (actualJson.Equals(expectedJson))
                    {
                        return continuation;
                    }
                }
            }
            Execute.Verification.BecauseOf(reason,
                                           reasonArgs)
                   .FailWith("Expected collection {0} to contain {1}{reason}",
                             new object[]
                             {
                                 // reduce cluttered failure message
                                 RemoveExtraQuotes(actualJsonList),
                                 RemoveExtraQuotes(expectedJsonList)
                             });
            return continuation;
        }
    }
}