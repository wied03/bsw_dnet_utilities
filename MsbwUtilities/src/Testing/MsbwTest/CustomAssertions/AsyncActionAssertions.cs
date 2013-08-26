#region

// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions.Execution;

#endregion

#endregion

namespace MsbwTest.CustomAssertions
{
    public class AsyncActionAssertions
    {
        private readonly Func<Task> _asyncAction;

        public AsyncActionAssertions(Func<Task> asyncAction)
        {
            _asyncAction = asyncAction;
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

            Execute.Assertion.ForCondition(exception != null).BecauseOf(reason,
                                                                        reasonArgs)
                   .FailWith("Expected {0}{reason}, but no exception was thrown.",
                             new object[]
                             {
                                 typeof (TException)
                             });
            Execute.Assertion.ForCondition(exception is TException).BecauseOf(reason,
                                                                              reasonArgs)
                   .FailWith("Expected {0}{reason}, but found {1}.",
                             (object) typeof (TException),
                             (object) exception);
            return (TException) exception;
        }
    }
}