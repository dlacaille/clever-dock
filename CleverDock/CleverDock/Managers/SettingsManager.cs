using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CleverDock.Model;

namespace CleverDock.Managers
{
    internal class SettingsManager
    {
        public const string SettingsFile = "config.xml";

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
            using (var reader = File.OpenRead(path))
            {
                try
                {
                    settings = GetSerializer().Deserialize(reader) as DockSettings;
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
            new Action(() =>
            {
                using (var writer = File.Open(path, FileMode.Create, FileAccess.Write))
                    GetSerializer().Serialize(writer, Settings);
            }).BeginInvoke(null, null);
        }

        public static bool CanDeserialize(string path)
        {
            if (!File.Exists(path))
                return false;
            using (var reader = File.OpenRead(path))
            using (var xmlreader = XmlReader.Create(reader))
                return GetSerializer().CanDeserialize(xmlreader);
        }

        public static XmlSerializer GetSerializer()
        {
            return new XmlSerializer(typeof (DockSettings));
        }
    }
}