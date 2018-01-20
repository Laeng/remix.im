using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace REMIX.Laeng
{
    class LocalEnvironmentFile
    {
        private MainWindow UI;
        private String Local;

        internal LocalEnvironmentFile(MainWindow Ul)
        {
            UI = Ul;

            Local = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "REMIX");
            Local = Path.Combine(Local, "REMIX.exe.config");
        }

        internal void Set(String k, String v)
        {
            try
            {
                Configuration config = Open(Local);
                if (config.AppSettings.Settings.AllKeys.Contains(k))
                {
                    config.AppSettings.Settings[k].Value = "";
                    config.AppSettings.Settings[k].Value = v;
                }
                else
                {
                    config.AppSettings.Settings.Add(k, v);
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
            }
            catch (Exception e)
            {
                UI.SendSingleMessage(e.Message);
            }
        }

        internal String Get(String k)
        {
            Configuration config = Open(Local);
            String v = String.Empty;

            if(config.AppSettings.Settings.AllKeys.Contains(k))
            {
                v = config.AppSettings.Settings[k].Value;
            }

            return v;
        }

        private Configuration Open(String local)
        {
            Configuration config = null;

            if (String.IsNullOrWhiteSpace(local))
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }

            else
            {
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = local
                };
                config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            }

            return config;
        }
    }
}
