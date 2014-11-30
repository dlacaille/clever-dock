using CleverDock.Managers;
using System.Collections.Generic;

namespace CleverDock.Model
{
    public class DockSettings
    {
        public const string SETTINGS_VERSION = "1.0.0";

        public static DockSettings DefaultDockSettings = new DockSettings
        {
                IconSize = 48,
                IconPadding = 20,
                CollapseDuration = 0.3,
                SaveAutomatically = true,
                Icons = new List<SerializableIconInfo>(),
                Theme = ThemeManager.DefaultTheme
        };

        public double CollapseDuration;
        public int IconPadding;
        public int IconSize;
        public Theme Theme;
        public List<SerializableIconInfo> Icons;
        public bool SaveAutomatically;
        public string Version = SETTINGS_VERSION;

        public int OuterIconSize
        {
            get { return IconSize + IconPadding; }
        }
    }
}