// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Collections;
using MsbwTest.CustomAssertions;
using Nito.AsyncEx;

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

        public static AndConstraint<GenericCollectionAssertions<T>> NotContainEquivalent<T>(
            this GenericCollectionAssertions<T> assertions,
            IEnumerable<T> expectedItemsList)
        {
            return new CustomCollectionAssertions<T>(assertions).NotContainEquivalent(expectedItemsList);
        }

        public static string ToStringWithCount(this char character,
                                               int count)
        {
            var str = new string(Enumerable.Repeat(character,
                                                   count).ToArray());
            return str;
        }

        public static async Task<TResult> ShouldCompleteWithin<TResult>(this TaskCompletionSource<TResult> tcs,
                                                                        TimeSpan time,
                                                                        string reason = "",
                                                                        params object[] reasonArgs)
        {
            return await tcs.Task.ShouldCompleteWithin(time,
                                                       reason,
                                                       reasonArgs);
        }

        public static async Task ShouldCompleteWithin(this TaskCompletionSource tcs,
                                                      TimeSpan time,
                                                      string reason = "",
                                                      params object[] reasonArgs)
        {
            await tcs.Task.ShouldCompleteWithin(time,
                                                reason,
                                                reasonArgs);
        }

        public static async Task<TResult> ShouldCompleteWithin<TResult>(this Task<TResult> task,
                                                                        TimeSpan time,
                                                                        string reason = "",
                                                                        params object[] reasonArgs)
        {
            return await task.InvokingAsync(t => t)
                             .ShouldCompleteWithin(time,
                                                   reason,
                                                   reasonArgs);
        }

        public static async Task ShouldCompleteWithin(this Task task,
                                                      TimeSpan time,
                                                      string reason = "",
                                                      params object[] reasonArgs)
        {
            var assertions = new AsyncActionAssertions(() => task);
            await assertions.ShouldCompleteWithin(time,
                                                  reason,
                                                  reasonArgs);
        }

        public static AsyncActionAssertionsWithResult<TResult> InvokingAsync<T, TResult>(this T subject,
                                                                                         Func<T, Task<TResult>>
                                                                                             asyncAction)
        {
            return new AsyncActionAssertionsWithResult<TResult>(() => asyncAction(subject));
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