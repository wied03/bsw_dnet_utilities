#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using MsBw.MsBwUtility.Enum;

#endregion

namespace MsBw.MsBwUtility
{
    public static class ExtensionMethods
    {
        public static void Times(this int count,
                                 Action action)
        {
            count.Times(i => action());
        }

        public static void Times(this int count,
                                 Func<Task> asyncAction)
        {
            count.Times(i =>
                {
                    var task = asyncAction.Invoke();
                    try
                    {
                        task.Wait();
                    }
                    catch (AggregateException ae)
                    {
                        var edi = ExceptionDispatchInfo.Capture(ae.GetBaseException());
                        edi.Throw();
                    }
                });
        }

        public static void Times(this int count,
                                 Action<int> action)
        {
            for (var i = 0; i < count; i++)
            {
                action(i);
            }
        }

        public static void Times(this int count,
                                 Func<int,Task> action)
        {
            for (var i = 0; i < count; i++)
            {
                var task = action(i);
                try
                {
                    task.Wait();
                }
                catch (AggregateException ae)
                {
                    var edi = ExceptionDispatchInfo.Capture(ae.GetBaseException());
                    edi.Throw();
                }
            }
        }

        public static IEnumerable<int> Times(this int count)
        {
            var arr = new int[count];
            Times(count,
                  index => arr[index] = index);
            return arr;
        }
    }
}