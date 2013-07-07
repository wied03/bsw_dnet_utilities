#region

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions.Execution;

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
            var exception = (Exception)null;
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
                             (object)typeof(TException),
                             (object)exception);
            return (TException)exception;
        }
    }
}