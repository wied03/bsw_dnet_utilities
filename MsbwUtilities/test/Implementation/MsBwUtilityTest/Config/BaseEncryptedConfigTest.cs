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
        [StringValue("bytes")] Bytes
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
            _config.AppSettings.Settings.Remove(Settings.Username.StringValue());
            _config.AppSettings.Settings.Remove(Settings.Password.StringValue());
            _config.AppSettings.Settings.Remove(Settings.Bytes.StringValue());
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

            // assert
            _config.AppSettings.Settings[Settings.Username.StringValue()].Value
                                                                         .Should()
                                                                         .Be("updated username");
            _storage.Password
                    .Should()
                    .Be("the password");
        }

        [Test]
        public void Null_set_ok()
        {
            // arrange
            _storage.Password = "foo";
            _storage.Key = "AB".ToByteArrayFromHex();

            // act
            _storage.Password = null;
            _storage.Key = null;

            // assert
            _storage.Password
                    .Should()
                    .BeNull();

            _storage.Key
                    .Should()
                    .BeNull();
        }

        #endregion
    }
}