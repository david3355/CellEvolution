using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Text;

namespace Evolution
{
    public enum ConfigKeys { MaxLevel, HighScore, ShowTutorial, LastLevel, GameMode, FirstStart }

    public enum GameMode { Evolution, Survival }

    public class ConfigManager
    {
        private ConfigManager()
        {
            configFile = "config.conf";
            config = new Dictionary<ConfigKeys, string>();
            filemanager = new FileManager();
            LoadConfig();
        }

        private static ConfigManager self;

        public static ConfigManager GetInstance
        {
            get
            {
                if (self == null) self = new ConfigManager();
                return self;
            }
        }

        private Dictionary<ConfigKeys, String> config;
        private FileManager filemanager;
        private String configFile;

        private String GetDefaultValue(ConfigKeys Key)
        {
            switch (Key)
            {
                case ConfigKeys.MaxLevel: return "0";
                case ConfigKeys.HighScore: return "0";
                case ConfigKeys.ShowTutorial: return "true";
                case ConfigKeys.LastLevel: return "1";
                case ConfigKeys.GameMode: return GameMode.Evolution.ToString();
                case ConfigKeys.FirstStart: return "true";
                default: return "0";
            }
        }

        private void CreateConfig()
        {
            SetDefaultConfig(ConfigKeys.HighScore);
            SetDefaultConfig(ConfigKeys.MaxLevel);
            SetDefaultConfig(ConfigKeys.ShowTutorial);
            SetDefaultConfig(ConfigKeys.LastLevel);
            SetDefaultConfig(ConfigKeys.GameMode);
            SetDefaultConfig(ConfigKeys.FirstStart);
        }

        private void SetDefaultConfig(ConfigKeys Key)
        {
            WriteConfig(Key, GetDefaultValue(Key));
        }

        private void LoadConfig()
        {
            List<String> lines = filemanager.ReadAllLines(configFile);
            if(lines.Count == 0) CreateConfig();
            foreach (String line in lines)
            {
                if (line != null && line != String.Empty)
                {
                    string[] tags = line.Split('=');
                    ConfigKeys key = (ConfigKeys)Enum.Parse(typeof(ConfigKeys), tags[0], true);
                    WriteConfig(key, tags[1], true);
                }
            }
        }

        public void WriteConfig(ConfigKeys ConfigKey, String Value, bool load = false)
        {
            if (!config.ContainsKey(ConfigKey)) config.Add(ConfigKey, Value);
            else config[ConfigKey] = Value;
            if(!load) SaveConfig();
        }

        public String ReadConfig(ConfigKeys ConfigKey)
        {
            if (!config.ContainsKey(ConfigKey)) SetDefaultConfig(ConfigKey);
            return config[ConfigKey];
        }

        private void SaveConfig()
        {
            StringBuilder stb = new StringBuilder();
            foreach (KeyValuePair<ConfigKeys, String> configitem in config)
            {
                stb.Append(String.Format("{0}={1}\n", configitem.Key.ToString(), configitem.Value));
            }
            filemanager.WriteFile(configFile, stb.ToString());
        }

    }

}
