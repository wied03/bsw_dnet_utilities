// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace MsBw.MsBwUtility.Enum
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class StringValueAttribute : Attribute
    {
        public string Value { get; private set; }

        public StringValueAttribute(string stringValue)
        {
            Value = stringValue;
        }
    }
}