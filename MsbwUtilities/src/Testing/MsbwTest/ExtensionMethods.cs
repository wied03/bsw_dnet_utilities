#region

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Collections;
using MsbwTest.CustomAssertions;

#endregion

namespace MsbwTest
{
    public static class ExtensionMethods
    {
        public static AndConstraint<GenericCollectionAssertions<T>> ContainEquivalent<T>(
            this GenericCollectionAssertions<T> assertions,
            IEnumerable<T> expectedItemsList)
        {
            return new CustomCollectionAssertions<T>(assertions).ContainEquivalent(expectedItemsList);
        }
    }
}