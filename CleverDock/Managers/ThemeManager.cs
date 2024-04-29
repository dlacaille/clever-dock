using System.IO;
using System.Windows;
using CleverDock.Handlers;
using CleverDock.Models;
using CleverDock.Patterns;
using CleverDock.Tools;
using CleverDock.ViewModels;
using Newtonsoft.Json;

namespace CleverDock.Managers;

public class ThemeManager
{
    private static ThemeManager _manager;

    public static ThemeModel DefaultTheme = new()
    {
        Name = "Default - Flat",
        Path = "/Cleverdock;component/Themes/Flat/style.xaml"
    };

    private ResourceDictionary theme;

    public static ThemeManager Manager
    {
        get
        {
            if (_manager == null)
                _manager = new ThemeManager();
            return _manager;
        }
    }

    public event EventHandler<ThemeEventArgs> ThemeChanged;
    public event EventHandler<EventArgs> ThemeListChanged;

    public void ThemeWindow()
    {
        LoadTheme(VMLocator.Main.Theme);
    }

    public ThemeSettingsViewModel LoadTheme(ThemeModel theme)
    {
        ThemeSettingsViewModel result = null;
        try
        {
            if (theme == null)
                theme = DefaultTheme;
            if (theme.Path.StartsWith("/Cleverdock;component/"))
            {
                LoadComponentTheme(theme.Path);
            }
            else
            {
                var xamlPath = Path.Combine(theme.Path, "style.xaml");
                var xaml = XamlHelper.LoadXaml(xamlPath);
                LoadResourceDictionary(xaml);
                var settingsPath = Path.Combine(theme.Path, "theme.json");
                result = GetSettings(settingsPath);
            }

            VMLocator.Main.Theme = theme;
            if (ThemeChanged != null)
                ThemeChanged(this, new ThemeEventArgs(theme));
        }
        catch (Exception ex)
        {
            MessageBox.Show("Theme \"" + theme.Name + "\" failed to load. \n" + ex.Message);
            LoadTheme(DefaultTheme);
        }

        return result;
    }

    public ThemeSettingsViewModel GetSettings(string path)
    {
        if (!File.Exists(path))
            return ThemeSettingsViewModel.DefaultThemeSettings;
        using (var stream = new StreamReader(path))
        {
            try
            {
                return JsonConvert.DeserializeObject<ThemeSettingsViewModel>(stream.ReadToEnd());
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return ThemeSettingsViewModel.DefaultThemeSettings;
            }
        }
    }

    public void LoadComponentTheme(string path)
    {
        var uri = new Uri(path, UriKind.Relative);
        var theme = Application.LoadComponent(uri) as ResourceDictionary;
        LoadResourceDictionary(theme);
    }

    public void LoadResourceDictionary(ResourceDictionary resourceDict)
    {
        if (theme != null)
            Application.Current.Resources.MergedDictionaries.Remove(theme);
        Application.Current.Resources.MergedDictionaries.Add(resourceDict);
    }
}