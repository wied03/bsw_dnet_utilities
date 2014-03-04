// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security;

namespace MsbwTest
{
    public static class TestingSecureStringExtensions
    {
        public static string ToNonSecureString(this SecureString secureString)
        {
            var pointer = Marshal.SecureStringToGlobalAllocUnicode(secureString);
            try
            {
                return Marshal.PtrToStringAuto(pointer);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(pointer);
            }
        }

        public static SecureString ToSecureString(this string value)
        {
            var ss = new SecureString();
            value.ToList().ForEach(ss.AppendChar);
            return ss;
        }
    }
}