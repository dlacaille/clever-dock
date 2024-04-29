using CleverDock.Patterns;

namespace CleverDock.Models;

public class SettingsModel : SerializableModelBase
{
    public const string SETTINGS_VERSION = "0.4.1";
    public const string SETTINGS_FILE = "config.json";

    public string Version = SETTINGS_VERSION;

    public SettingsModel()
    {
        IconSize = 40;
        CollapseDuration = 0.2;
        HotspotHeight = 20;
        DockEdgeSpacing = 20;
        DockHideDuration = 0.5;
        DockShowDuration = 0.3;
        DockHideDelay = 0;
        DockShowDelay = 0;
        AutoHide = true;
        SaveAutomatically = true;
        RemoveTaskbar = true;
        ReserveScreenSpace = false;
        ShowWidgets = true;
        Icons = new List<IconModel>();
        //Theme = ThemeModel.DefaultTheme;
        // Load properties from file.
        LoadFromFile(SETTINGS_FILE);
    }

    public double CollapseDuration { get; set; }
    public double DockHideDuration { get; set; }
    public double DockShowDuration { get; set; }
    public double DockHideDelay { get; set; }
    public double DockShowDelay { get; set; }
    public int HotspotHeight { get; set; }
    public int DockEdgeSpacing { get; set; }
    public int IconSize { get; set; }
    public ThemeModel Theme { get; set; }
    public List<IconModel> Icons { get; set; }
    public bool SaveAutomatically { get; set; }
    public bool ReserveScreenSpace { get; set; }
    public bool RemoveTaskbar { get; set; }
    public bool AutoHide { get; set; }
    public bool ShowWidgets { get; set; }
    public int OuterIconWidth { get; set; }
    public int OuterIconHeight { get; set; }
}