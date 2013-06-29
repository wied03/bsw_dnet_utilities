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

        private static readonly IDictionary<string,object> EnumValues = new Dictionary<string, object>();
        public static TEnum EnumValue<TEnum>(this string theStringVal)
        {
            var type = typeof (TEnum);
            var key = String.Format("{0}_{1}",
                                    type.Name,
                                    theStringVal);
            if (EnumValues.ContainsKey(key))
            {
                return (TEnum) EnumValues[key];
            }
            
            var stringVals =
                type.GetFields()
                .Where(f => f.StringValAttr() != null)
                    .ToDictionary(field => field.StringValAttr().Value,
                                  field => field);
            var enumIndex = (int) stringVals[theStringVal].GetValue(type);
            var enumValue = (TEnum) System.Enum.ToObject(type,
                                                         enumIndex);
            EnumValues[key] = enumValue;
            return enumValue;
        }

        private static StringValueAttribute StringValAttr(this FieldInfo field)
        {
            var attrs = field.GetCustomAttributes(typeof (StringValueAttribute),
                                                  true);
            return attrs
                       .Any()
                       ? (StringValueAttribute) attrs[0]
                       : null;
        }

        private static readonly IDictionary<System.Enum,string> StringValues = new Dictionary<System.Enum, string>();

        public static string StringValue(this System.Enum theEnum)

        {
            if (StringValues.ContainsKey(theEnum))
            {
                return StringValues[theEnum];
            }
            var field = theEnum.GetType().GetField(theEnum.ToString());
            var stringValue = field.StringValAttr().Value;
            StringValues[theEnum] = stringValue;
            return stringValue;
        }
    }
}