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
        readonly Configuration _configuration;
        readonly string _sectionName;

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

        protected string GetValue(TSettingsEnum setting)
        {
            var asEnum = setting as System.Enum;
            var theSetting = Settings[asEnum.StringValue()];
            return theSetting == null
                       ? null
                       : theSetting.Value;
        }

        protected void SetValue(TSettingsEnum setting,
                                string value,
                                bool saveAndReloadConfigToo = false)
        {
            var asEnum = setting as System.Enum;
            var key = asEnum.StringValue();
            var settings = Settings;
            settings.Remove(key);
            settings.Add(key,
                         value);
            if (!saveAndReloadConfigToo) return;
            Save();
        }

        public virtual void Save()
        {
            _configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(_sectionName);
        }
    }
}