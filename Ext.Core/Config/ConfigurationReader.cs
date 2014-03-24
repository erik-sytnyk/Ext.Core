using System;
using System.Configuration;

namespace Ext.Core.Config
{
    public class ConfigurationReader : IConfigurationReader
    {
        public string CustomConfigurationPath { get; set; }

        public ConfigurationReader(string customConfigurationPath)
        {
            CustomConfigurationPath = customConfigurationPath;
        }

        public string ReadConfigurationValue(string key)
        {
            var result = String.Empty;

            if (!String.IsNullOrEmpty(CustomConfigurationPath))
            {
                var fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = CustomConfigurationPath;
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                if (config.AppSettings.Settings[key] != null)
                {
                    result = config.AppSettings.Settings[key].Value;
                }
            }

            if (String.IsNullOrEmpty(result))
            {
                result = ConfigurationManager.AppSettings[key];
            }

            return result;
        }

        public string ReadConnectionStringValue(string connectionStringName)
        {
            var result = String.Empty;

            if (!string.IsNullOrEmpty(CustomConfigurationPath))
            {
                var fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = CustomConfigurationPath;
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


                if (config.ConnectionStrings.ConnectionStrings[connectionStringName] != null)
                {
                    result = config.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString;
                }
            }

            if (String.IsNullOrEmpty(result))
            {
                result = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            }

            return result;
        }
    }
}