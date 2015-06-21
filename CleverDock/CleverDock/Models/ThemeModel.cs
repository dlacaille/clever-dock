using CleverDock.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Models
{
    public class ThemeModel
    {
        public ThemeModel()
        {
        }

        public ThemeModel(string path)
        {
            var fileInfo = new FileInfo(path);
            var settings = new ThemeSettingsModel(fileInfo.Directory.FullName);
            Name = settings.ThemeName;
            Path = fileInfo.Directory.FullName;
        }

        public static ThemeModel DefaultTheme = new ThemeModel
        {
            Name = "Default - Metal",
            Path = "/Cleverdock;component/Themes/Metal2/style.xaml"
        };

        /// <summary>
        /// Name of the theme.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Path of the theme.
        /// </summary>
        public string Path { get; set; }
    }
}
