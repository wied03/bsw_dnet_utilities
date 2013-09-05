// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
﻿#region

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

        public static byte[] ToByteArrayFromHex(this String hex)
        {
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string ToHex(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-",
                                                        string.Empty);
        }

        public static int? ToNullableInt(this string str)
        {
            return string.IsNullOrEmpty(str)
                       ? (int?) null
                       : Convert.ToInt32(str);
        }
    }
}