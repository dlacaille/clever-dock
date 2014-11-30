using CleverDock.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private static Theme DefaultTheme = new Theme
        {
            Name = "Default - Metal",
            Path = "/Cleverdock;component/Themes/Metal.xaml"
        };

        private Window window;
        private ResourceDictionary theme;

        public void ThemeWindow(Window window)
        {            
            this.window = window;
            LoadTheme(DefaultTheme);
        }

        public void LoadTheme(Theme theme)
        {
            if (theme == DefaultTheme)
                LoadComponentTheme(theme.Path);
            else
            {
                var xaml = XamlHelper.LoadXaml(theme.Path);
                LoadResourceDictionary(xaml);
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
                window.Resources.MergedDictionaries.Remove(theme);
            window.Resources.MergedDictionaries.Add(theme = resourceDict);
        }

        public Theme[] GetThemes()
        {
            var themes = Directory.GetFiles("Themes", "*.xaml").Select(f => new Theme()
                {
                    Name = Path.GetFileNameWithoutExtension(f),
                    Path = f
                }).ToList();
            themes.Insert(0, DefaultTheme);
            return themes.ToArray();
        }
    }
}
