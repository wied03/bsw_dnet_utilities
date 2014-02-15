// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using FluentAssertions.Execution;
using MsBw.MsBwUtility.Tasks;

namespace MsbwTest.CustomAssertions
{
    public class AsyncActionAssertions
    {
        readonly Func<Task> _asyncAction;

        public AsyncActionAssertions(Func<Task> asyncAction)
        {
            _asyncAction = asyncAction;
        }

        public async Task ShouldCompleteWithin(TimeSpan time,
                                               string reason = "",
                                               params object[] reasonArgs)
        {
            TimeoutException timeout = null;
            try
            {
                await _asyncAction.WithTimeout(time);
            }
            catch (TimeoutException t)
            {
                timeout = t;
            }
            catch (AggregateException aggregate)
            {
                ExceptionDispatchInfo.Capture(aggregate.InnerException).Throw();
            }

            Execute
                .Verification
                .ForCondition(timeout == null)
                .BecauseOf(reason,
                           reasonArgs)
                .FailWith("Expected task to complete within {0} milliseconds{reason}, but the task didn't complete",
                          time.TotalMilliseconds);
        }

        public async Task<TException> ShouldThrow<TException>(string reason = "",
                                                              params object[] reasonArgs) where TException : Exception
        {
            var exception = (Exception) null;
            try
            {
                await _asyncAction();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Execute.Verification.ForCondition(exception != null).BecauseOf(reason,
                                                                           reasonArgs)
                   .FailWith("Expected {0}{reason}, but no exception was thrown.",
                             new object[]
                             {
                                 typeof (TException)
                             });
            Execute.Verification.ForCondition(exception is TException).BecauseOf(reason,
                                                                                 reasonArgs)
                   .FailWith("Expected {0}{reason}, but found {1}.",
                             (object) typeof (TException),
                             (object) exception);
            return (TException) exception;
        }
    }
}