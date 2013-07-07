#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace MsBw.MsBwUtility.Enum
{
    public static class StringValueExtensions
    {
        private static readonly IDictionary<string, object> CachedEnumValues = new Dictionary<string, object>();
        public static TEnum EnumValue<TEnum>(this string theStringVal)
        {
            var type = typeof(TEnum);
            var key = String.Format("{0}_{1}",
                                    type.Name,
                                    theStringVal);
            if (CachedEnumValues.ContainsKey(key))
            {
                return (TEnum)CachedEnumValues[key];
            }

            var stringVals =
                type.GetFields()
                .Where(f => f.StringValAttr() != null)
                    .ToDictionary(field => field.StringValAttr().Value,
                                  field => field);
            if (!stringVals.ContainsKey(theStringVal))
            {
                // TODO: Check for a field name matching this, if we don't find one, THEN throw an exception
            }
            var enumIndex = (int)stringVals[theStringVal].GetValue(type);
            var enumValue = (TEnum)System.Enum.ToObject(type,
                                                         enumIndex);
            CachedEnumValues[key] = enumValue;
            return enumValue;
        }

        private static StringValueAttribute StringValAttr(this FieldInfo field)
        {
            var attrs = field.GetCustomAttributes(typeof(StringValueAttribute),
                                                  true);
            return attrs
                       .Any()
                       ? (StringValueAttribute)attrs[0]
                       : null;
        }

        private static readonly IDictionary<System.Enum, string> StringValues = new Dictionary<System.Enum, string>();

        public static string StringValue(this System.Enum theEnum)
        {
            if (StringValues.ContainsKey(theEnum))
            {
                return StringValues[theEnum];
            }
            var field = theEnum.GetType().GetField(theEnum.ToString());
            var attribute = field.StringValAttr();
            var stringValue = attribute == null
                                  ? field.Name
                                  : attribute.Value;
            StringValues[theEnum] = stringValue;
            return stringValue;
        }

    }
}