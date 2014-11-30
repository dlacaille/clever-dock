using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CleverDock.Model;
using Newtonsoft.Json;

namespace CleverDock.Managers
{
    internal class SettingsManager
    {
        public const string SettingsFile = "config.json";

        private static DockSettings settings;

        public static DockSettings Settings
        {
            // Singleton Pattern
            get
            {
                if (settings == null)
                {
                    if (CanDeserialize(SettingsFile))
                        LoadSettings(); // If can serialize, load settings TODO: Validation
                    else
                        settings = DockSettings.DefaultDockSettings; // If can't serialize, load default settings
                }
                return settings;
            }
        }

        public static void LoadSettings(string path = SettingsFile)
        {
            if (!File.Exists(path))
                return;
            using (var stream = new StreamReader(path))
            {
                try
                {
                    settings = JsonConvert.DeserializeObject<DockSettings>(stream.ReadToEnd());
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                    settings = DockSettings.DefaultDockSettings;
                }
            }
        }

        public static void SaveSettings(string path = SettingsFile)
        {
            var json = JsonConvert.SerializeObject(Settings, Newtonsoft.Json.Formatting.Indented);
            using (var stream = new StreamWriter(path, false))
                stream.Write(json);
        }

        public static bool CanDeserialize(string path)
        {
            if (!File.Exists(path))
                return false;
            return true; // TODO: Check if json is valid.
        }

        public static JsonSerializer GetSerializer()
        {
            return new JsonSerializer();
        }
    }
}