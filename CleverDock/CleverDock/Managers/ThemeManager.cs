using CleverDock.Handlers;
using CleverDock.Model;
using CleverDock.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Window = System.Windows.Window;
using Theme = CleverDock.Model.Theme;

namespace CleverDock.Managers
{
    public class ThemeManager
    {
        private static ThemeManager _manager;
        public static ThemeManager Manager
        {
            get {
                if (_manager == null)
                    _manager = new ThemeManager();
                return _manager;
            }
        }

        public event EventHandler<ThemeEventArgs> ThemeChanged;
        public event EventHandler<EventArgs> ThemeListChanged;

        private const string ThemeFolder = "Themes";

        public static Theme DefaultTheme = new Theme
        {
            Name = "Default - Metal",
            Path = "/Cleverdock;component/Themes/Metal2/style.xaml"
        };

        private List<Window> windows;
        private ResourceDictionary theme;
        public ThemeSettings Settings { get; set; }

        public void ThemeWindow(List<Window> windows)
        {            
            this.windows = windows;
            LoadTheme(SettingsManager.Settings.Theme);
            WatchChanges(ThemeFolder);
        }

        public void LoadTheme(Theme theme)
        {
            try
            {
                if (theme == null)
                    theme = DefaultTheme;
                if (theme.Path.StartsWith("/Cleverdock;component/"))
                    LoadComponentTheme(theme.Path);
                else
                {
                    var xamlPath = Path.Combine(theme.Path, "style.xaml");
                    var xaml = XamlHelper.LoadXaml(xamlPath);
                    LoadResourceDictionary(xaml);
                    var settingsPath = Path.Combine(theme.Path, "theme.json");
                    LoadSettings(settingsPath);
                }
                SettingsManager.Settings.Theme = theme;
                if (ThemeChanged != null)
                    ThemeChanged(this, new ThemeEventArgs(theme));
            } 
            catch (Exception ex)
            {
                MessageBox.Show("Theme \"" + theme.Name + "\" failed to load. \n" + ex.Message);
                LoadTheme(DefaultTheme);
            }
        }

        public void LoadSettings(string path)
        {
            Settings = GetSettings(path);
        }

        public ThemeSettings GetSettings(string path)
        {
            if (!File.Exists(path))
                return ThemeSettings.DefaultThemeSettings;
            using (var stream = new StreamReader(path))
            {
                try
                {
                    return JsonConvert.DeserializeObject<ThemeSettings>(stream.ReadToEnd());
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                    Settings = ThemeSettings.DefaultThemeSettings;
                }
            }
            return ThemeSettings.DefaultThemeSettings;
        }

        public void LoadComponentTheme(string path)
        {
            var uri = new Uri(path, UriKind.Relative);
            var theme = Application.LoadComponent(uri) as ResourceDictionary;
            LoadResourceDictionary(theme);
        }

        public void LoadResourceDictionary(ResourceDictionary resourceDict)
        {
            foreach(var window in windows)
            {
                if (theme != null)
                    window.Resources.MergedDictionaries.Remove(theme);
                window.Resources.MergedDictionaries.Add(theme = resourceDict);
            }
        }

        public void WatchChanges(string path)
        {
            foreach(var filter in new string[] { "style.xaml", "theme.json" })
            {
                var watcher = new FileSystemWatcher();
                watcher.Path = ThemeFolder;
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                watcher.Filter = filter;
                watcher.IncludeSubdirectories = true;
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnChanged;
                watcher.EnableRaisingEvents = true;
            }
        }

        private void OnChanged(object source, EventArgs e)
        {
            MainWindow.Window.Dispatcher.Invoke(() =>
            {
                if (ThemeListChanged != null)
                    ThemeListChanged(this, new EventArgs());
                // Reload theme if it still exists
                if (GetThemes().Any(t => t.Path == SettingsManager.Settings.Theme.Path))
                    LoadTheme(SettingsManager.Settings.Theme);
            });
        }

        public Theme[] GetThemes()
        {
            var themes = Directory.GetFiles(ThemeFolder, "theme.json", SearchOption.AllDirectories).Select(f => new Theme()
                {
                    Name = GetSettings(f).ThemeName,
                    Path = new FileInfo(f).Directory.FullName
                }).ToList();
            themes.Insert(0, DefaultTheme);
            return themes.ToArray();
        }
    }
}
