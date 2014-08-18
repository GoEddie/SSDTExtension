
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EnvDTE;
using GoEddieUk.SqlServerTddHelper.Config.UI;
using GoEddieUk.SqlServerTddHelper.UI;

namespace GoEddieUk.SqlServerTddHelper.Config
{
    public class Configuration
    {
        public static Settings GetSettings(Project project)
        {
            var fileName = GetConfigFilename(project);
            
            if (File.Exists(fileName))
                return ReadSettings(project);
            
            return ShowConfig(project);
        }

        public static Settings ShowConfig(Project project)
        {

            ConfigWindow configWindow;// = new ConfigWindow(GetSettings(project));


            if (!File.Exists(GetConfigFilename(project)))
            {
                configWindow = new ConfigWindow(new Settings()
                {
                    ConnectionString = "Server=.;Integrated Security=SSPI;",
                    UseDefaultSqlCmdValues = true,
                    DeploymentFolder = Path.Combine(new FileInfo(project.FullName).DirectoryName, "Deploy")
                });
            }
            else
            {
                configWindow = new ConfigWindow(GetSettings(project));
            }

            configWindow.ShowDialog();

            if (configWindow.Cancelled)
            {
                return null;
            }

            var newSettings = configWindow.GetSettings();
            WriteSettings(GetConfigFilename(project), newSettings.ConnectionString, newSettings.UseDefaultSqlCmdValues.ToString(), newSettings.DeploymentFolder);
         
            return GetSettings(project);
        }

        private static string GetConfigFilename(Project project)
        {
            return Path.Combine(new FileInfo(project.FullName).DirectoryName, "SQLTddExtender.config");
        }

        private static Settings ReadSettings(Project project)
        {
            var settings = new Settings();
            var configFile = GetConfigFilename(project);

            
            var x = XElement.Load(configFile);
            settings.ConnectionString = (from element in x.Elements()
                                where element.Name == "connection_string"
                                select element.Value).FirstOrDefault();

            string first = (from element in x.Elements() where element.Name == "use_default_sql_cmd_vars" select element.Value).FirstOrDefault();
            settings.UseDefaultSqlCmdValues = bool.Parse(first);

            x = XElement.Load(configFile);
            settings.DeploymentFolder = (from element in x.Elements()
                                where element.Name == "deployment_folder"
                                select element.Value).FirstOrDefault();

            return settings;

        }

        private static void WriteSettings(string filename, string connection_string, string use_default_cmd_vars, string deployment_folder)
        {
            var sw = new StreamWriter(filename);
            sw.WriteLine(string.Format(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<settings>
  <connection_string>{0}</connection_string>
  <use_default_sql_cmd_vars>{1}</use_default_sql_cmd_vars>
  <deployment_folder>{2}</deployment_folder>
</settings>", connection_string, use_default_cmd_vars, deployment_folder));

            sw.Flush();
            sw.Close();
        }
        
    }

    public class Settings
    {
        public string ConnectionString;
        public bool UseDefaultSqlCmdValues;
        public string DeploymentFolder;
    }
}




