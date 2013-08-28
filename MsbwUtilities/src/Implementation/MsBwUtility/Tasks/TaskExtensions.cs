// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
﻿#region

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace MsBw.MsBwUtility.Tasks
{
    public enum Result
    {
        Normal,
        Timeout,
        Canceled
    }

    public static class TaskExtensions
    {
        public static async Task<TResult> WithTimeout<TResult, TObject>(this TObject obj,
                                                                        Func<TObject, Task<TResult>> action,
                                                                        TimeSpan timeout)
        {
            var task = action(obj);
            var timeoutTask = await Timeout(task,
                                            timeout);
            if (timeoutTask == Result.Normal)
            {
                return task.Result;
            }
            throw new TimeoutException(string.Format("Timed out at {0} milliseconds waiting for task to complete",
                                                     timeout.TotalMilliseconds));
        }

        public static async Task<Result> Timeout(this Task task,
                                                 TimeSpan timeout)
        {
            var timeoutTask = Task.Delay(timeout);
            var waitOnStuff = await Task.WhenAny(task,
                                                 timeoutTask);
            return waitOnStuff == task
                       ? Result.Normal
                       : Result.Timeout;
        }

        public static async Task<Result> TimeoutOrCancel(this Task task,
                                                         CancellationToken token,
                                                         TimeSpan timeout)
        {
            var timeoutOrCancelTask = Task.Delay(timeout,
                                                 token);
            var waitOnStuff = await Task.WhenAny(task,
                                                 timeoutOrCancelTask);
            if (waitOnStuff == task)
            {
                return Result.Normal;
            }
            if (waitOnStuff == timeoutOrCancelTask && waitOnStuff.IsCanceled)
            {
                return Result.Canceled;
            }
            return Result.Timeout;
        }
    }
}