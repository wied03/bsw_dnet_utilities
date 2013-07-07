#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public static string ToStringWithCount(this char character,
                                               int count)
        {
            var str = new string(Enumerable.Repeat(character,
                                                   count).ToArray());
            return str;
        }

        public static AsyncActionAssertions InvokingAsync<T>(this T subject,
                                                             Func<T, Task> asyncAction)
        {
            return new AsyncActionAssertions(() => asyncAction(subject));
        }

        public static async Task Exception<TException>(this Task<TException> task,
                                                       Action<TException> passPredicate)
        {
            var excep = await task;
            passPredicate(excep);
        }
    }
}