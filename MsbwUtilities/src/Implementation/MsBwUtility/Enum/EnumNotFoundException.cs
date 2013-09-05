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
    public class EnumNotFoundException : Exception
    {
        private const string ERROR_FORMAT = "Unable to find a value in '{0}' for '{1}'.  Valid values are [{2}]";

        public EnumNotFoundException(string value,
                                     Type enumType,
                                     Dictionary<string, FieldInfo> validValues)
            : base(string.Format(ERROR_FORMAT,
                                 enumType.Name,
                                 value,
                                 validValues
                                     .Select(kv => "String value: " + kv.Key + " ENUM val: " + kv.Value.Name)
                                     .Aggregate((val1,
                                                 val2) => val1 + "," + val2)))
        {
        }

        public EnumNotFoundException(object value,
                                     Type enumType,
                                     IDictionary mappings) : base(GenerateMapExceptionMessage(value,
                                                                                              enumType,
                                                                                              mappings))
        {
        }

        private static string GenerateMapExceptionMessage(object value,
                                                          Type enumType,
                                                          IDictionary mappings)
        {
            var valuesList = (from object mapping in mappings.Keys
                              select string.Format("Value: {0} ENUM val: {1}",
                                                   mapping,
                                                   mappings[mapping])).ToList();
            var values = valuesList
                .Aggregate((val1,
                            val2) => val1 + "," + val2);
            return string.Format(ERROR_FORMAT,
                                 enumType.Name,
                                 value,
                                 values);
        }
    }
}