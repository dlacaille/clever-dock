using CleverDock.Models;
using CleverDock.Patterns;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.ViewModels
{
    public class ThemeSettingsViewModel : ViewModelBase
    {
        public static ThemeSettingsViewModel DefaultThemeSettings = new ThemeSettingsViewModel
        {
            IconPaddingX = 15,
            IconPaddingY = 20
        };

        public ThemeSettingsViewModel() { }

        public ThemeSettingsViewModel(ThemeSettingsModel model)
        {
            ThemeName = model.ThemeName;
            Author = model.Author;
            Version = model.Version;
            Description = model.Description;
            IconPaddingX = model.IconPaddingX;
            IconPaddingY = model.IconPaddingY;
        }

        private string _themeName;
        /// <summary>
        /// Name of the Theme.
        /// </summary>
        public string ThemeName { get { return _themeName; } set { if (value != _themeName) { _themeName = value; OnPropertyChanged(); } } }

        private string _author;
        /// <summary>
        /// Author of the Theme.
        /// </summary>
        public string Author { get { return _author; } set { if (value != _author) { _author = value; OnPropertyChanged(); } } }

        private string _version;
        /// <summary>
        /// Version of the Theme.
        /// </summary>
        public string Version { get { return _version; } set { if (value != _version) { _version = value; OnPropertyChanged(); } } }

        private string _description;
        /// <summary>
        /// Description of the Theme.
        /// </summary>
        public string Description { get { return _description; } set { if (value != _description) { _description = value; OnPropertyChanged(); } } }

        private int _iconPaddingX;
        /// <summary>
        /// X icon padding.
        /// </summary>
        public int IconPaddingX { get { return _iconPaddingX; } set { if (value != _iconPaddingX) { _iconPaddingX = value; OnPropertyChanged(); } } }

        private int _iconPaddingY;
        /// <summary>
        /// Y icon padding
        /// </summary>
        public int IconPaddingY { get { return _iconPaddingY; } set { if (value != _iconPaddingY) { _iconPaddingY = value; OnPropertyChanged(); } } }
    }
}
