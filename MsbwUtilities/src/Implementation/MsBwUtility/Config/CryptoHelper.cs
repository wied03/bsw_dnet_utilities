// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace MsBw.MsBwUtility.Config
{
    public class CryptoHelper : ICryptoHelper
    {
        public string Encrypt(byte[] redBytes)
        {
            string blackBase64 = null;
            if (redBytes != null)
            {
                var blackBytes = DoEncrypt(redBytes);
                blackBase64 = Convert.ToBase64String(blackBytes);
            }
            return blackBase64;
        }

        public int? DecryptNullableInt(string base64)
        {
            var str = Decrypt(base64);
            return str == null
                       ? (int?) null
                       : Convert.ToInt32(str);
        }

        public string Encrypt(int? value)
        {
            var asStr = value.HasValue
                            ? value.ToString()
                            : null;

            return Encrypt(asStr);
        }

        public string Decrypt(string base64)
        {
            return base64 == null
                       ? null
                       : DoDecrypt(base64);
        }

        public string Encrypt(string value)
        {
            return value == null
                       ? null
                       : DoEncrypt(value);
        }

        public byte[] DecryptAsBytes(string blackBase64)
        {
            if (blackBase64 == null)
            {
                return null;
            }
            var blackBytes = Convert.FromBase64String(blackBase64);
            var redHexBytes = DoDecrypt(blackBytes);
            return redHexBytes;
        }

        static byte[] DoEncrypt(byte[] redBytes)
        {
            return ProtectedData.Protect(redBytes,
                                         null, // entropy doesn't add much value (would just be another key)
                                         DataProtectionScope.CurrentUser);
        }

        static byte[] DoDecrypt(byte[] blackBytes)
        {
            return ProtectedData.Unprotect(blackBytes,
                                           null,
                                           DataProtectionScope.CurrentUser);
        }

        static string DoEncrypt(string text)
        {
            var redBytes = Encoding.Default.GetBytes(text);
            var blackBytes = DoEncrypt(redBytes);
            var blackBase64 = Convert.ToBase64String(blackBytes);
            return blackBase64;
        }

        static string DoDecrypt(string blackBase64)
        {
            var blackBytes = Convert.FromBase64String(blackBase64);
            var redBytes = DoDecrypt(blackBytes);
            var redText = Encoding.Default.GetString(redBytes);
            return redText;
        }
    }
}