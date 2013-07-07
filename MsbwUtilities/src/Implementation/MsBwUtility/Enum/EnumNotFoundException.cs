#region

using System;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace MsBw.MsBwUtility.Enum
{
    public class EnumNotFoundException : Exception
    {
        public EnumNotFoundException(string value,
                                     Type enumType) : base(string.Format("Unable to find a value in '{0}' for '{1}'",
                                                                         enumType.Name,
                                                                         value))
        {
        }
    }
}