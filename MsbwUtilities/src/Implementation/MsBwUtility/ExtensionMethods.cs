#region

using System;
using System.Collections.Generic;
using System.Linq;

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
                                 Action<int> action)
        {
            for (var i = 0; i < count; i++)
            {
                action(i);
            }
        }

        public static IEnumerable<int> SequentialArray(this int count)
        {
            var arr = new int[count];
            Times(count,
                  index => arr[index] = index);
            return arr;
        }
    }
}