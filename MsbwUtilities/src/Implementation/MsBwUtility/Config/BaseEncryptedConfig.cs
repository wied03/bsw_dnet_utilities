﻿// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using MsBw.MsBwUtility.Enum;

namespace MsBw.MsBwUtility.Config
{
    public abstract class BaseEncryptedConfig<TSettingsClass>
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

        protected int? GetSettingInt(TSettingsClass setting)
        {
            var value = GetSetting(setting);
            return string.IsNullOrEmpty(value) ? (int?) null : Convert.ToInt32(value);
        }

        protected void SaveSetting(TSettingsClass enumSetting,
                                   int? value)
        {
            var setting = enumSetting as System.Enum;
            var settingStr = setting.StringValue();
            var settings = _configuration.AppSettings.Settings;
            settings.Remove(settingStr);
            var valueStr = value == null
                               ? null
                               : value.ToString();
            settings.Add(settingStr,
                         valueStr);
            _configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        protected string GetSetting(TSettingsClass enumSetting)
        {
            var setting = enumSetting as System.Enum;
            var element = _configuration.AppSettings.Settings[setting.StringValue()];
            return element == null
                       ? null
                       : element.Value;
        }

        protected void SaveSetting<T>(TSettingsClass enumSetting,
                                      T value) where T : class
        {
            var setting = enumSetting as System.Enum;
            var settingStr = setting.StringValue();
            var settings = _configuration.AppSettings.Settings;
            settings.Remove(settingStr);
            var valueStr = value == null
                               ? null
                               : value.ToString();
            settings.Add(settingStr,
                         valueStr);
            _configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        protected void SaveEncryptedSetting(TSettingsClass setting,
                                            byte[] redBytes)
        {
            string blackBase64 = null;
            if (redBytes != null)
            {
                var blackBytes = Encrypt(redBytes);
                blackBase64 = Convert.ToBase64String(blackBytes);
            }
            SaveSetting(setting,
                        blackBase64);
        }

        protected int? GetEncryptedSettingInt(TSettingsClass setting)
        {
            var str = GetEncryptedSettingString(setting);
            return str == null
                       ? (int?) null
                       : Convert.ToInt32(str);
        }

        protected void SaveEncryptedSetting(TSettingsClass setting,
                                            int? value)
        {
            SaveEncryptedSetting(setting,
                                 value.HasValue
                                     ? value.ToString()
                                     : null);
        }

        protected string GetEncryptedSettingString(TSettingsClass setting)
        {
            var blackBase64 = GetSetting(setting);
            return blackBase64 == null
                       ? null
                       : Decrypt(blackBase64);
        }

        protected void SaveEncryptedSetting(TSettingsClass setting,
                                            string value)
        {
            var encrypted = value == null
                                ? null
                                : Encrypt(value);
            SaveSetting(setting,
                        encrypted);
        }

        protected byte[] GetEncryptedSetting(TSettingsClass setting)
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