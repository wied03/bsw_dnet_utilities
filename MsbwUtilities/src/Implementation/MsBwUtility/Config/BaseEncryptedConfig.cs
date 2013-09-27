// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace MsBw.MsBwUtility.Config
{
    public abstract class BaseEncryptedConfig
    {
        private readonly Configuration _configuration;

        protected BaseEncryptedConfig()
            : this(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None))
        {
        }

        protected BaseEncryptedConfig(string exePath)
        {
            var map = new ExeConfigurationFileMap
            {
                ExeConfigFilename = exePath
            };
            _configuration = ConfigurationManager.OpenMappedExeConfiguration(map,
                                                                             ConfigurationUserLevel.None);
        }

        protected BaseEncryptedConfig(Configuration configuration)
        {
            _configuration = configuration;
        }

        protected int? GetSettingInt(string setting)
        {
            var value = GetSetting(setting);
            if (value == null)
            {
                return null;
            }
            return Convert.ToInt32(value);
        }

        protected string GetSetting(string setting)
        {
            var element = _configuration.AppSettings.Settings[setting];
            return element == null
                       ? null
                       : element.Value;
        }

        protected void SaveSetting<T>(string setting,
                                      T value)
        {
            var settings = _configuration.AppSettings.Settings;
            settings.Remove(setting);
            settings.Add(setting,
                         value.ToString());
            _configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        protected void SaveEncryptedSetting(string setting,
                                            byte[] redBytes)
        {
            var blackBytes = Encrypt(redBytes);
            var blackBase64 = Convert.ToBase64String(blackBytes);
            SaveSetting(setting,
                        blackBase64);
        }

        protected string GetEncryptedSettingString(string setting)
        {
            var blackBase64 = GetSetting(setting);
            return blackBase64 == null
                       ? null
                       : Decrypt(blackBase64);
        }

        protected void SaveEncryptedSetting(string setting,
                                            string value)
        {
            var encrypted = Encrypt(value);
            SaveSetting(setting,
                        encrypted);
        }

        protected byte[] GetEncryptedSetting(string setting)
        {
            var blackBase64 = GetSetting(setting);
            if (blackBase64 == null)
            {
                return null;
            }
            var blackBytes = Convert.FromBase64String(blackBase64);
            var redHexBytes = Decrypt(blackBytes);
            return redHexBytes;
        }

        protected static byte[] Encrypt(byte[] redBytes)
        {
            return ProtectedData.Protect(redBytes,
                                         null, // entropy doesn't add much value (would just be another key)
                                         DataProtectionScope.CurrentUser);
        }

        protected static byte[] Decrypt(byte[] blackBytes)
        {
            return ProtectedData.Unprotect(blackBytes,
                                           null,
                                           DataProtectionScope.CurrentUser);
        }

        protected static string Encrypt(string text)
        {
            var redBytes = Encoding.Default.GetBytes(text);
            var blackBytes = Encrypt(redBytes);
            var blackBase64 = Convert.ToBase64String(blackBytes);
            return blackBase64;
        }

        protected static string Decrypt(string blackBase64)
        {
            var blackBytes = Convert.FromBase64String(blackBase64);
            var redBytes = Decrypt(blackBytes);
            var redText = Encoding.Default.GetString(redBytes);
            return redText;
        }
    }
}