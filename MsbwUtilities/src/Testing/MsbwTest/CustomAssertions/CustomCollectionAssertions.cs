#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using NUnit.Framework;
using Newtonsoft.Json;

#endregion

namespace MsbwTest.CustomAssertions
{
    public class CustomCollectionAssertions<T>
    {
        private readonly GenericCollectionAssertions<T> _assertions;

        public CustomCollectionAssertions(GenericCollectionAssertions<T> assertions)
        {
            _assertions = assertions;
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

            var expectedJson = expectedList
                .Select(expObj => JsonConvert.SerializeObject(expObj));
            var actualJson = actual
                .Select(actObj => JsonConvert.SerializeObject(actObj));
            actualJson
                .Should()
                .Contain(expectedJson);
            
            return continuation;
        }
    }
}