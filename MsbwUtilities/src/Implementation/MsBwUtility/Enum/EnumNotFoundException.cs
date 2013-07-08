#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace MsBw.MsBwUtility.Enum
{
    public class EnumNotFoundException : Exception
    {
        public EnumNotFoundException(string value,
                                     Type enumType,
                                     Dictionary<string, FieldInfo> validValues)
            : base(string.Format("Unable to find a value in '{0}' for '{1}'.  Valid values are [{2}]",
                                 enumType.Name,
                                 value,
                                 validValues
                                     .Select(kv => "String value: " + kv.Key + " ENUM val: " + kv.Value.Name)
                                     .Aggregate((val1,
                                                 val2) => val1 + "," + val2)))
        {
        }
    }
}