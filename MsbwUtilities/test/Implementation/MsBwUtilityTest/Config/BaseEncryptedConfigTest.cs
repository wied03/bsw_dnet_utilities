// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MsBw.MsBwUtility;
using MsBw.MsBwUtility.Config;
using MsBw.MsBwUtility.Enum;
using NUnit.Framework;

namespace MsBwUtilityTest.Config
{
    public enum Settings
    {
        [StringValue("foo")] Username,
        [StringValue("pass")] Password,
        [StringValue("bytes")] Bytes,
        [StringValue("integer")] Number,
        [StringValue("integerenc")] NumberEncrypted
    }

    public class ConfigTest : BaseEncryptedConfig<Settings>
    {
        public ConfigTest(Configuration config) : base(config)
        {
        }

        public string Username
        {
            get { return GetSetting(Settings.Username); }
            set
            {
                SaveSetting(Settings.Username,
                            value);
            }
        }

        public string Password
        {
            get { return GetEncryptedSettingString(Settings.Password); }
            set
            {
                SaveEncryptedSetting(Settings.Password,
                                     value);
            }
        }

        public byte[] Key
        {
            get { return GetEncryptedSetting(Settings.Bytes); }
            set
            {
                SaveEncryptedSetting(Settings.Bytes,
                                     value);
            }
        }

        public int? Number
        {
            get { return GetSettingInt(Settings.Number); }
            set
            {
                SaveSetting(Settings.Number,
                            value);
            }
        }

        public int? NumberEnc
        {
            get { return GetEncryptedSettingInt(Settings.NumberEncrypted); }
            set
            {
                SaveEncryptedSetting(Settings.NumberEncrypted,
                                     value);
            }
        }
    }

    [TestFixture]
    public class BaseEncryptedConfigTest : BaseTest
    {
        private Configuration _config;
        private ConfigTest _storage;

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _storage = new ConfigTest(_config);
        }

        [TearDown]
        public override void TearDown()
        {
            Action<Settings> remove = settings => _config.AppSettings.Settings.Remove(settings.StringValue());
            remove(Settings.Username);
            remove(Settings.Password);
            remove(Settings.Bytes);
            remove(Settings.Number);
            remove(Settings.NumberEncrypted);
            _config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            base.TearDown();
        }

        #endregion

        #region Utility Methods

        #endregion

        #region Tests

        [Test]
        public void Save_get_setting()
        {
            // assert
            _storage.Username
                    .Should()
                    .BeNull();
            _storage.Password
                    .Should()
                    .BeNull();
            _storage.Number
                    .Should()
                    .NotHaveValue();
            _storage.NumberEnc
                    .Should()
                    .NotHaveValue();
            // arrange
            _config.AppSettings.Settings.Add(Settings.Username.StringValue(),
                                             "the username");

            // act + assert
            _storage.Username
                    .Should()
                    .Be("the username");

            // act
            _storage.Username = "updated username";
            _storage.Password = "the password";
            _storage.Number = 2;
            _storage.NumberEnc = 4;

            // assert
            _config.AppSettings.Settings[Settings.Username.StringValue()].Value
                                                                         .Should()
                                                                         .Be("updated username");
            _storage.Password
                    .Should()
                    .Be("the password");
            _storage.Number
                    .Should()
                    .Be(2);
            _storage.NumberEnc
                    .Should()
                    .Be(4);
        }

        [Test]
        public void Null_set_ok()
        {
            // arrange
            _storage.Password = "foo";
            _storage.Key = "AB".ToByteArrayFromHex();
            _storage.NumberEnc = 4;

            // act
            _storage.Password = null;
            _storage.Key = null;
            _storage.Number = null;
            _storage.NumberEnc = null;

            // assert
            _storage.Password
                    .Should()
                    .BeNull();
            _storage.Key
                    .Should()
                    .BeNull();
            _storage.NumberEnc
                    .Should()
                    .NotHaveValue();
        }

        #endregion
    }
}