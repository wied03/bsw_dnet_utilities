#region

using System;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace MsBw.MsBwUtility.Enum
{
    [AttributeUsage(AttributeTargets.Field,AllowMultiple = false)]
    public class StringValueAttribute : Attribute
    {
        public string Value { get; private set; }

        public StringValueAttribute(string stringValue)
         {
             Value = stringValue;
         }
    }
}