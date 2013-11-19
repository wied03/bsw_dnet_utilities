// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using MsBw.MsBwUtility.Enum;

namespace MsBw.MsBwUtility.Config
{
    public abstract class BaseConfig<TSettingsEnum>
    {
        private readonly Configuration _configuration;
        private readonly string _sectionName;

        protected Configuration Config
        {
            get { return _configuration; }
        }

        protected BaseConfig(string sectionName,
                             Configuration configuration)
        {
            _sectionName = sectionName;
            _configuration = configuration;
        }

        protected KeyValueConfigurationCollection Settings
        {
            get
            {
                var section = (AppSettingsSection) _configuration.GetSection(_sectionName);
                return section.Settings;
            }
        }

        protected string GetSetting(TSettingsEnum setting)
        {
            var asEnum = setting as System.Enum;
            var theSetting = Settings[asEnum.StringValue()];
            return theSetting == null
                       ? null
                       : theSetting.Value;
        }

        protected void SetSetting(TSettingsEnum setting,
                                  string value,
                                  bool saveAndReloadConfigToo = false)
        {
            var asEnum = setting as System.Enum;
            var key = asEnum.StringValue();
            var settings = Settings;
            if (settings.AllKeys.Contains(key))
            {
                settings[key].Value = value;
                return;
            }
            settings.Add(key,
                         value);
            if (!saveAndReloadConfigToo) return;
            _configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(_sectionName);
        }
    }
}