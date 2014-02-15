// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MsBw.MsBwUtility.Config;
using MsBw.MsBwUtility.Enum;
using NUnit.Framework;

namespace MsBwUtilityTest.Config
{
    public enum Set
    {
        [StringValue("set1")] Setting1,
        [StringValue("set2")] Setting2
    }

    public class TestConfig : BaseConfig<Set>
    {
        public const string SECTION_NAME = "theSection";

        public TestConfig(Configuration config) : base(SECTION_NAME,
                                                       config)
        {
        }

        public string Setting1
        {
            get { return GetValue(Set.Setting1); }
            set
            {
                SetValue(Set.Setting1,
                         value);
            }
        }

        public string Setting2
        {
            get { return GetValue(Set.Setting2); }
            set
            {
                SetValue(Set.Setting2,
                         value,
                         true);
            }
        }

        public void DoSave()
        {
            Save();
        }
    }

    [TestFixture]
    public class BaseConfigTest : BaseTest
    {
        Configuration _config;
        TestConfig _testConfig;
        KeyValueConfigurationCollection _settings;

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _config = GetConfig();
            var section = (AppSettingsSection) _config.GetSection(TestConfig.SECTION_NAME);
            _settings = section.Settings;
            _settings.AllKeys
                     .ToList()
                     .ForEach(s => _settings.Remove(s));
            _config.Save();
            _testConfig = new TestConfig(_config);
        }

        #endregion

        #region Utility Methods

        static Configuration GetConfig()
        {
            return ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
                                                                   {
                                                                       ExeConfigFilename = "the.exe.config",
                                                                       RoamingUserConfigFilename = "the.user.config"
                                                                   },
                                                                   ConfigurationUserLevel.PerUserRoaming);
        }

        #endregion

        #region Tests

        [Test]
        public void Get_not_there()
        {
            // arrange

            // act
            var result = _testConfig.Setting1;

            // assert
            result
                .Should()
                .BeNull();
        }

        [Test]
        public void Get_is_already_there()
        {
            // arrange
            _settings.Add(Set.Setting1.StringValue(),
                          "foo");

            // act
            var result = _testConfig.Setting1;

            // assert
            result
                .Should()
                .Be("foo");
        }

        [Test]
        public void Set_is_not_there()
        {
            // arrange
            _testConfig.Setting1
                       .Should()
                       .BeNull();

            // act
            _testConfig.Setting1 = "bar";

            // assert
            _settings[Set.Setting1.StringValue()].Value
                                                 .Should()
                                                 .Be("bar");
        }

        [Test]
        public void Set_is_already_there_auto_save()
        {
            // arrange
            _settings.Add(Set.Setting2.StringValue(),
                          "foo");
            _testConfig.DoSave();

            // act
            _testConfig.Setting2 = "bar";

            // assert
            _settings[Set.Setting2.StringValue()].Value
                                                 .Should()
                                                 .Be("bar");
            var readConfig = GetConfig();
            var section = (AppSettingsSection) readConfig.GetSection(TestConfig.SECTION_NAME);
            section.Settings[Set.Setting2.StringValue()].Value
                                                        .Should()
                                                        .Be("bar",
                                                            "in a newly read config file, we should already be updated");
        }

        [Test]
        public void Set_is_already_there_no_auto_save()
        {
            // arrange
            _settings.Add(Set.Setting1.StringValue(),
                          "foo");
            _testConfig.DoSave();

            // act
            _testConfig.Setting1 = "bar";
            _testConfig.DoSave();

            // assert
            _settings[Set.Setting1.StringValue()].Value
                                                 .Should()
                                                 .Be("bar");
            var readConfig = GetConfig();
            var section = (AppSettingsSection) readConfig.GetSection(TestConfig.SECTION_NAME);
            section.Settings[Set.Setting1.StringValue()].Value
                                                        .Should()
                                                        .Be("bar",
                                                            "in a newly read config file, we should already be updated");
        }

        #endregion
    }
}