// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Collections;
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
            var type = typeof (TEnum);
            var key = String.Format("{0}_{1}",
                                    type.FullName,
                                    theStringVal);
            if (CachedEnumValues.ContainsKey(key))
            {
                return (TEnum) CachedEnumValues[key];
            }

            var stringVals =
                type.GetFields()
                    .ToDictionary(GetStringValForField,
                                  field => field);
            if (!stringVals.ContainsKey(theStringVal))
            {
                throw new EnumNotFoundException(theStringVal,
                                                type,
                                                stringVals);
            }
            var enumIndex = (int) stringVals[theStringVal].GetValue(type);
            var enumValue = (TEnum) System.Enum.ToObject(type,
                                                         enumIndex);
            CachedEnumValues[key] = enumValue;
            return enumValue;
        }

        private static string GetStringValForField(FieldInfo field)
        {
            var stringValueAttribute = field.StringValAttr();
            return stringValueAttribute != null
                       ? stringValueAttribute.Value
                       : field.Name;
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

        public static void ThrowException<TEnumType, TKeyValue>(this Dictionary<TKeyValue, TEnumType> mapping,
                                                                object value)
        {
            var dict = (IDictionary) mapping;
            throw new EnumNotFoundException(value,
                                            typeof (TEnumType),
                                            (dict));
        }

        public static TEnumType ToMappedEnumValue<TEnumType, TKeyValue>(this Dictionary<TKeyValue, TEnumType> mapping,
                                                                        TKeyValue value)
        {
            if (!mapping.ContainsKey(value))
            {
                ThrowException(mapping,
                               value);
            }
            return mapping[value];
        }

        private static readonly Dictionary<Type, Dictionary<object, object>> ReverseMapping =
            new Dictionary<Type, Dictionary<object, object>>();

        private static void AddReverseMapping<TEnumType, TKeyValue>(Dictionary<TKeyValue, TEnumType> mapping)
        {
            var reverse = new Dictionary<object, object>();
            ReverseMapping[typeof (TEnumType)] = reverse;
            foreach (var kv in mapping)
            {
                reverse[kv.Value] = kv.Key;
            }
        }

        public static TKeyValue FromMappedEnumValue<TEnumType, TKeyValue>(this Dictionary<TKeyValue, TEnumType> mapping,
                                                                          TEnumType value)
        {
            var type = typeof (TEnumType);
            if (!ReverseMapping.ContainsKey(type))
            {
                AddReverseMapping(mapping);
            }
            var reverse = ReverseMapping[type];
            var asEnum = (object) value;
            if (!reverse.ContainsKey(asEnum))
            {
                ThrowException(mapping,
                               value);
            }
            return (TKeyValue) reverse[asEnum];
        }
    }
}